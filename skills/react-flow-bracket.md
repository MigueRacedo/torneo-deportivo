# Skill: Bracket con React Flow

> Guía para implementar la visualización del bracket (H0005) usando React Flow.
> Leer junto con `skills/bracket-algorithm.md` (que explica la lógica de datos).

## Instalación
```bash
npm install @xyflow/react
```

## Concepto
React Flow renderiza grafos de nodos conectados. El bracket de eliminación directa
es un árbol que se lee de izquierda (primera ronda) a derecha (final):

```
Ronda 1        Ronda 2        Final
┌────────┐
│ Ana    │──┐
└────────┘  ├──┌────────┐
┌────────┐  │  │ Ana    │──┐
│ Bruno  │──┘  └────────┘  │
└────────┘                 ├──┌────────┐
┌────────┐                 │  │ Final  │
│ Carlos │──┐              │  └────────┘
└────────┘  ├──┌────────┐  │
┌────────┐  │  │ Diana  │──┘
│ Diana  │──┘  └────────┘
└────────┘
```

## Convertir Bracket → nodes + edges

```typescript
// components/bracket/bracketLayout.ts
import type { Node, Edge } from '@xyflow/react';
import type { Bracket, Match } from '@/types';

const COLUMN_WIDTH = 280;   // separación horizontal entre rondas
const NODE_HEIGHT = 90;     // alto de cada match
const BASE_GAP = 30;        // espacio vertical base en la primera ronda

export function bracketToFlow(bracket: Bracket): { nodes: Node[]; edges: Edge[] } {
  const nodes: Node[] = [];
  const edges: Edge[] = [];

  bracket.rounds.forEach((matchesEnRonda, rondaIdx) => {
    // El espaciado vertical se duplica en cada ronda (las llaves se "juntan")
    const espaciado = (NODE_HEIGHT + BASE_GAP) * Math.pow(2, rondaIdx);
    const offsetInicial = (espaciado - NODE_HEIGHT) / 2;

    matchesEnRonda.forEach((match, posIdx) => {
      const x = rondaIdx * COLUMN_WIDTH;
      const y = offsetInicial + posIdx * espaciado;

      nodes.push({
        id: match.id,
        type: 'match',          // usa el nodo custom MatchNode
        position: { x, y },
        data: { match },
      });

      // Conectar este match con el de la siguiente ronda
      if (rondaIdx < bracket.rounds.length - 1) {
        const siguientePos = Math.floor(posIdx / 2);
        const siguienteMatch = bracket.rounds[rondaIdx + 1][siguientePos];
        if (siguienteMatch) {
          edges.push({
            id: `${match.id}->${siguienteMatch.id}`,
            source: match.id,
            target: siguienteMatch.id,
            type: 'smoothstep',
          });
        }
      }
    });
  });

  return { nodes, edges };
}
```

## Componente Principal

```tsx
// components/bracket/BracketView.tsx
import { ReactFlow, Background, Controls, type NodeTypes } from '@xyflow/react';
import '@xyflow/react/dist/style.css';
import { useMemo } from 'react';
import { bracketToFlow } from './bracketLayout';
import { MatchNode } from './MatchNode';
import type { Bracket } from '@/types';

const nodeTypes: NodeTypes = { match: MatchNode };

export function BracketView({ bracket }: { bracket: Bracket }) {
  const { nodes, edges } = useMemo(() => bracketToFlow(bracket), [bracket]);

  return (
    <div className="h-[600px] w-full rounded-lg border bg-slate-50">
      <ReactFlow
        nodes={nodes}
        edges={edges}
        nodeTypes={nodeTypes}
        fitView
        nodesDraggable={false}
        nodesConnectable={false}
        elementsSelectable={false}
        proOptions={{ hideAttribution: true }}
      >
        <Background />
        <Controls showInteractive={false} />
      </ReactFlow>
    </div>
  );
}
```

## Nodo Custom (MatchNode)

```tsx
// components/bracket/MatchNode.tsx
import { Handle, Position, type NodeProps } from '@xyflow/react';
import type { Match } from '@/types';

export function MatchNode({ data }: NodeProps<{ match: Match }>) {
  const { match } = data;
  const esBye = match.estado === 'Bye';

  return (
    <div className="w-[220px] rounded-md border bg-white shadow-sm">
      {/* Handle de entrada (izquierda) y salida (derecha) */}
      <Handle type="target" position={Position.Left} className="!bg-slate-400" />
      <Handle type="source" position={Position.Right} className="!bg-slate-400" />

      {esBye ? (
        <div className="p-3 text-center text-sm text-slate-500 italic">
          {match.competidor1?.nombreCompleto ?? '—'} pasa directo
        </div>
      ) : (
        <div className="divide-y">
          <CompetidorRow competidor={match.competidor1} esGanador={match.ganador?.id === match.competidor1?.id} />
          <CompetidorRow competidor={match.competidor2} esGanador={match.ganador?.id === match.competidor2?.id} />
        </div>
      )}
    </div>
  );
}

function CompetidorRow({ competidor, esGanador }: { competidor?: Match['competidor1']; esGanador: boolean }) {
  return (
    <div className={`flex items-center justify-between px-3 py-2 text-sm
      ${esGanador ? 'bg-emerald-50 font-medium text-emerald-900' : 'text-slate-700'}`}>
      <span>{competidor?.nombreCompleto ?? 'A definir'}</span>
      {esGanador && <span className="text-emerald-600">✓</span>}
    </div>
  );
}
```

## Hook de Datos

```typescript
// hooks/useBracket.ts
import { useQuery } from '@tanstack/react-query';
import { api } from '@/lib/api';

export const bracketKeys = {
  detail: (torneoId: string, categoriaId: string) =>
    ['bracket', torneoId, categoriaId] as const,
};

export function useBracket(torneoId: string, categoriaId: string) {
  return useQuery({
    queryKey: bracketKeys.detail(torneoId, categoriaId),
    queryFn: () => api.llaves.obtener(torneoId, categoriaId),
  });
}
```

## Actualización en Vivo con SignalR

```tsx
// Dentro del componente que muestra el bracket:
import { useEffect } from 'react';
import { useQueryClient } from '@tanstack/react-query';
import { crearConexionBracket } from '@/lib/signalr';
import { bracketKeys } from '@/hooks/useBracket';

function BracketPage({ torneoId, categoriaId }: Props) {
  const qc = useQueryClient();
  const { data: bracket, isLoading } = useBracket(torneoId, categoriaId);

  useEffect(() => {
    const connection = crearConexionBracket(torneoId);
    connection.on('MatchActualizado', () => {
      // Cuando el coordinador registra un ganador, refrescar el bracket
      qc.invalidateQueries({ queryKey: bracketKeys.detail(torneoId, categoriaId) });
    });
    return () => { connection.stop(); };
  }, [torneoId, categoriaId, qc]);

  if (isLoading) return <BracketSkeleton />;
  return <BracketView bracket={bracket} />;
}
```

## Registrar Ganador (interacción del Coordinador)

```tsx
// El coordinador hace clic en un competidor del match para marcarlo ganador.
// Agregar al MatchNode un onClick (solo si rol === Coordinador):

const { mutate: registrarGanador } = useMutation({
  mutationFn: ({ matchId, ganadorId }: { matchId: string; ganadorId: string }) =>
    api.llaves.registrarGanador(torneoId, matchId, ganadorId),
  onSuccess: () => qc.invalidateQueries({ queryKey: bracketKeys.detail(torneoId, categoriaId) }),
});

// El backend, además de persistir, emite el evento SignalR a los demás clientes.
```

## Checklist de Implementación
```
✓ Instalar @xyflow/react y @microsoft/signalr
✓ bracketLayout.ts convierte Bracket → nodes + edges
✓ El espaciado vertical se duplica por ronda (Math.pow(2, rondaIdx))
✓ MatchNode muestra dos competidores con Handles izq/der
✓ Los BYEs se muestran con estilo "pasa directo"
✓ El ganador se resalta en verde con ✓
✓ fitView centra el bracket automáticamente
✓ Solo el Coordinador puede registrar ganadores (onClick condicional)
✓ SignalR invalida la query del bracket en MatchActualizado
✓ El bracket permite pan + zoom (Controls) en mobile
✓ hideAttribution en proOptions (requiere licencia para producción comercial,
  pero para proyecto personal/educativo está OK)
```

## Notas
- React Flow es gratis para uso open-source/personal. La marca de agua se puede
  ocultar con `proOptions={{ hideAttribution: true }}`.
- Para brackets muy grandes (>32 competidores), considerar virtualización o
  paginar por ronda. Para el piloto (escuela de Taekwondo) no será necesario.
- El layout de izquierda a derecha es el estándar en torneos. Si preferís de
  arriba hacia abajo, intercambiar x↔y en bracketLayout.ts y usar Position.Top/Bottom.
