import { toast } from 'sonner';
import { ConfirmDialog } from '@/components/ui/confirm-dialog';
import { useCambiarEstadoTorneo } from '@/hooks/useTorneos';
import { ApiError } from '@/lib/api';
import { useAuthStore } from '@/store/authStore';
import type { Torneo, TorneoEstado } from '@/types';

/** Qué puede hacer el Coordinador según la etapa en la que está el torneo. */
const SIGUIENTE: Record<TorneoEstado, { destino: TorneoEstado; label: string; descripcion: string } | null> = {
  Borrador: {
    destino: 'Activo',
    label: 'Iniciar torneo',
    descripcion:
      'El torneo pasa a competencia: vas a poder registrar ganadores, pero se bloquean las categorías, ' +
      'los competidores y las llaves. Necesitás al menos una categoría con llaves generadas.',
  },
  Activo: {
    destino: 'Finalizado',
    label: 'Finalizar torneo',
    descripcion:
      'El torneo se cierra y queda solo para consulta y reportes. No se podrá registrar ningún resultado ' +
      'más. Requiere que todas las categorías con llaves estén terminadas.',
  },
  Finalizado: null,
};

/**
 * Botones de transición del ciclo de vida del torneo (H0009), solo para el Coordinador.
 *
 * Cada transición es irreversible en la práctica (a Borrador solo se vuelve si no se compitió), así que
 * todas pasan por `ConfirmDialog` con una descripción de qué se habilita y qué se bloquea.
 */
export function TorneoEstadoActions({ torneo }: { torneo: Torneo }) {
  const esCoordinador = useAuthStore((state) => state.usuario?.rol === 'Coordinador');
  const { mutate, isPending } = useCambiarEstadoTorneo(torneo.id);

  if (!esCoordinador) return null;

  const avance = SIGUIENTE[torneo.estado];
  const puedeVolverABorrador = torneo.estado === 'Activo';

  const cambiar = (estado: TorneoEstado, forzar = false) =>
    mutate(
      { estado, forzar },
      {
        onSuccess: (t) => toast.success(`El torneo pasó a ${t.estado}.`),
        onError: (err) =>
          toast.error(
            err instanceof ApiError ? err.message : 'No se pudo cambiar el estado del torneo.',
          ),
      },
    );

  if (!avance && !puedeVolverABorrador) {
    return (
      <p className="text-sm leading-relaxed text-muted-foreground text-left">
        El torneo está finalizado: queda disponible solo para consulta y reportes.
      </p>
    );
  }

  return (
    <div className="flex flex-wrap items-center gap-4">
      {avance && (
        <ConfirmDialog
          trigger={isPending ? 'Cambiando…' : avance.label}
          triggerClassName="inline-flex min-h-11 items-center rounded-md bg-primary px-6 text-sm font-medium text-primary-foreground hover:bg-primary/90 focus-visible:outline focus-visible:outline-3 focus-visible:outline-primary disabled:opacity-60"
          title={avance.label}
          description={avance.descripcion}
          onConfirm={() => cambiar(avance.destino)}
        />
      )}

      {/* Solo desde Activo y solo si no se registró ningún ganador; si hay resultados el backend lo rechaza
          con un mensaje explicando por qué, que se muestra en el toast. */}
      {puedeVolverABorrador && (
        <ConfirmDialog
          trigger="Volver a planificación"
          triggerClassName="inline-flex min-h-11 items-center text-sm leading-relaxed font-medium text-muted-foreground underline-offset-4 hover:text-secondary hover:underline focus-visible:text-secondary focus-visible:outline focus-visible:outline-3 focus-visible:outline-secondary"
          title="Volver a planificación"
          description="El torneo vuelve a Borrador y se desbloquean categorías, competidores y llaves. Solo es posible si todavía no se registró ningún ganador."
          onConfirm={() => cambiar('Borrador')}
        />
      )}

      {torneo.estado === 'Activo' && (
        <ConfirmDialog
          trigger="Finalizar de todos modos"
          triggerClassName="inline-flex min-h-11 items-center text-sm leading-relaxed font-medium text-muted-foreground underline-offset-4 hover:text-destructive hover:underline focus-visible:text-destructive focus-visible:outline focus-visible:outline-3 focus-visible:outline-destructive"
          title="Finalizar con categorías sin terminar"
          description="Cierra el torneo aunque queden categorías sin resolver. Se usa cuando una categoría no llegó a disputarse. Esta acción no se puede deshacer."
          onConfirm={() => cambiar('Finalizado', true)}
        />
      )}
    </div>
  );
}
