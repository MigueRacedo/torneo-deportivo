import type { Edge, Node } from '@xyflow/react';
import type { Bracket } from '@/types';
import type { MatchNodeData } from './MatchNode';

const COLUMN_WIDTH = 280; // separación horizontal entre rondas
const NODE_HEIGHT = 96; // alto aproximado de cada match
const BASE_GAP = 32; // espacio vertical base en la primera ronda

/**
 * Convierte un bracket en los nodos y aristas de React Flow. Cada match es un nodo custom (MatchNode);
 * el espaciado vertical se duplica en cada ronda para que las llaves converjan hacia la final.
 */
export function bracketToFlow(
  bracket: Bracket,
  extra: { puedeRegistrar: boolean; onRegistrarGanador: (llaveId: string, ganadorId: string) => void },
): { nodes: Node<MatchNodeData>[]; edges: Edge[] } {
  const nodes: Node<MatchNodeData>[] = [];
  const edges: Edge[] = [];

  bracket.rounds.forEach((matchesEnRonda, rondaIdx) => {
    const espaciado = (NODE_HEIGHT + BASE_GAP) * 2 ** rondaIdx;
    const offsetInicial = (espaciado - NODE_HEIGHT) / 2;

    matchesEnRonda.forEach((match, posIdx) => {
      nodes.push({
        id: match.id,
        type: 'match',
        position: { x: rondaIdx * COLUMN_WIDTH, y: offsetInicial + posIdx * espaciado },
        data: { match, ...extra },
      });

      if (rondaIdx < bracket.rounds.length - 1) {
        const siguiente = bracket.rounds[rondaIdx + 1]?.[Math.floor(posIdx / 2)];
        if (siguiente) {
          edges.push({
            id: `${match.id}->${siguiente.id}`,
            source: match.id,
            target: siguiente.id,
            type: 'smoothstep',
          });
        }
      }
    });
  });

  return { nodes, edges };
}
