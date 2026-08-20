import { Handle, Position, type Node, type NodeProps } from '@xyflow/react';
import type { Match, MatchCompetidor } from '@/types';

export interface MatchNodeData {
  match: Match;
  /** Si el usuario (Coordinador) puede registrar el ganador haciendo clic en un competidor. */
  puedeRegistrar: boolean;
  onRegistrarGanador: (llaveId: string, ganadorId: string) => void;
  [key: string]: unknown;
}

export type MatchFlowNode = Node<MatchNodeData, 'match'>;

/** Nodo custom del bracket: muestra los dos competidores del match (o un bye) y resalta al ganador. */
export function MatchNode({ data }: NodeProps<MatchFlowNode>) {
  const { match, puedeRegistrar, onRegistrarGanador } = data;
  const esBye = match.estado === 'Bye';
  const finalizado = match.estado === 'Finalizado';

  return (
    <div className="w-[220px] overflow-hidden rounded-lg border bg-card text-card-foreground shadow-sm">
      <Handle type="target" position={Position.Left} className="!bg-muted-foreground" />
      <Handle type="source" position={Position.Right} className="!bg-muted-foreground" />

      {esBye ? (
        <p className="px-3 py-4 text-center text-sm leading-relaxed text-muted-foreground italic">
          {match.competidor1?.nombreCompleto ?? '—'} pasa directo
        </p>
      ) : (
        <ul className="divide-y">
          <CompetidorRow
            competidor={match.competidor1}
            esGanador={Boolean(match.ganador && match.ganador.id === match.competidor1?.id)}
            habilitado={puedeRegistrar && !finalizado && Boolean(match.competidor1 && match.competidor2)}
            onElegir={() => match.competidor1 && onRegistrarGanador(match.id, match.competidor1.id)}
          />
          <CompetidorRow
            competidor={match.competidor2}
            esGanador={Boolean(match.ganador && match.ganador.id === match.competidor2?.id)}
            habilitado={puedeRegistrar && !finalizado && Boolean(match.competidor1 && match.competidor2)}
            onElegir={() => match.competidor2 && onRegistrarGanador(match.id, match.competidor2.id)}
          />
        </ul>
      )}
    </div>
  );
}

function CompetidorRow({
  competidor,
  esGanador,
  habilitado,
  onElegir,
}: {
  competidor?: MatchCompetidor | null;
  esGanador: boolean;
  habilitado: boolean;
  onElegir: () => void;
}) {
  const contenido = (
    <span className="flex items-center justify-between gap-2">
      <span className="truncate">{competidor?.nombreCompleto ?? 'A definir'}</span>
      {esGanador && (
        <span className="shrink-0 font-semibold text-emerald-700" aria-label="Ganador">
          ✓ Ganó
        </span>
      )}
    </span>
  );

  const clasesBase = `block w-full px-3 py-2 text-left text-sm leading-relaxed ${
    esGanador ? 'bg-emerald-50 font-medium text-emerald-900' : 'text-foreground'
  }`;

  if (habilitado && competidor) {
    return (
      <li>
        <button
          type="button"
          onClick={onElegir}
          className={`${clasesBase} min-h-11 hover:bg-muted focus-visible:outline focus-visible:outline-2 focus-visible:-outline-offset-2 focus-visible:outline-secondary`}
        >
          {contenido}
        </button>
      </li>
    );
  }

  return <li className={clasesBase}>{contenido}</li>;
}
