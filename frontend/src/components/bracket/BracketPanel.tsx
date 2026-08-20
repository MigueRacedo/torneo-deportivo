import { useCallback } from 'react';
import { toast } from 'sonner';
import { BracketView } from '@/components/bracket/BracketView';
import { useBracket, useRegistrarGanador } from '@/hooks/useBracket';
import { ApiError } from '@/lib/api';
import { useAuthStore } from '@/store/authStore';

/**
 * Panel con el bracket de una categoría: resuelve carga, error y vacío, y permite al Coordinador registrar
 * el ganador de un match. No abre la conexión SignalR — de eso se encarga `useBracketLiveUpdates` en la página,
 * para no reabrir el socket cada vez que se cambia de categoría.
 */
export function BracketPanel({ torneoId, categoriaId }: { torneoId: string; categoriaId: string }) {
  const esCoordinador = useAuthStore((state) => state.usuario?.rol === 'Coordinador');
  const { data: bracket, isLoading, error } = useBracket(torneoId, categoriaId);
  const { mutate } = useRegistrarGanador(torneoId, categoriaId);

  // `mutate` es estable entre renders, así que el callback no cambia de identidad y el
  // useMemo del layout de React Flow no se recalcula en cada render.
  const onRegistrarGanador = useCallback(
    (llaveId: string, ganadorId: string) => {
      mutate(
        { llaveId, ganadorId },
        {
          onSuccess: (match) =>
            toast.success(`Ganador registrado: ${match.ganador?.nombreCompleto ?? ''}.`),
          onError: (err) =>
            toast.error(err instanceof ApiError ? err.message : 'No se pudo registrar el ganador.'),
        },
      );
    },
    [mutate],
  );

  if (isLoading) {
    return <div className="h-[600px] animate-pulse rounded-lg bg-muted" aria-hidden="true" />;
  }

  if (error) {
    return (
      <p
        role="alert"
        className="rounded-lg border border-destructive/40 bg-destructive/10 p-4 text-sm leading-relaxed text-destructive"
      >
        {error instanceof ApiError ? error.message : 'No se pudo cargar el bracket. Intentá de nuevo.'}
      </p>
    );
  }

  if (!bracket || bracket.rounds.length === 0) {
    return (
      <p className="rounded-lg border bg-card p-4 text-sm leading-relaxed text-muted-foreground">
        Esta categoría todavía no tiene llaves generadas.
      </p>
    );
  }

  return (
    <div className="flex flex-col gap-2">
      {esCoordinador && (
        <p className="text-sm leading-relaxed text-muted-foreground text-left">
          Hacé clic en un competidor de un match para marcarlo ganador; avanza a la ronda siguiente.
        </p>
      )}
      <BracketView
        bracket={bracket}
        puedeRegistrar={esCoordinador}
        onRegistrarGanador={onRegistrarGanador}
      />
    </div>
  );
}
