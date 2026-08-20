import { Background, Controls, ReactFlow, type NodeTypes } from '@xyflow/react';
import '@xyflow/react/dist/style.css';
import { useMemo } from 'react';
import type { Bracket } from '@/types';
import { bracketToFlow } from './bracketLayout';
import { MatchNode } from './MatchNode';

const nodeTypes: NodeTypes = { match: MatchNode };

/**
 * Visualiza el bracket con React Flow (izquierda → derecha, con pan + zoom). Si el usuario es Coordinador,
 * puede registrar el ganador de un match haciendo clic en un competidor.
 */
export function BracketView({
  bracket,
  puedeRegistrar,
  onRegistrarGanador,
}: {
  bracket: Bracket;
  puedeRegistrar: boolean;
  onRegistrarGanador: (llaveId: string, ganadorId: string) => void;
}) {
  const { nodes, edges } = useMemo(
    () => bracketToFlow(bracket, { puedeRegistrar, onRegistrarGanador }),
    [bracket, puedeRegistrar, onRegistrarGanador],
  );

  return (
    <div className="h-[600px] w-full rounded-lg border bg-muted/30">
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
