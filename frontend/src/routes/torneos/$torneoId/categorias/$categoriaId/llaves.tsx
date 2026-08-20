import { useCallback, useEffect } from 'react';
import { Link, useParams } from 'react-router-dom';
import { useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { BracketView } from '@/components/bracket/BracketView';
import { bracketKeys, useBracket, useRegistrarGanador } from '@/hooks/useBracket';
import { ApiError } from '@/lib/api';
import { crearConexionBracket } from '@/lib/signalr';
import { useAuthStore } from '@/store/authStore';

/** Página del bracket de una categoría (H0005): visualización React Flow + registrar ganador (Coordinador). */
export default function LlavesPage() {
  const { torneoId = '', categoriaId = '' } = useParams<{ torneoId: string; categoriaId: string }>();
  const qc = useQueryClient();
  const esCoordinador = useAuthStore((state) => state.usuario?.rol === 'Coordinador');

  const { data: bracket, isLoading, error } = useBracket(torneoId, categoriaId);
  const registrar = useRegistrarGanador(torneoId, categoriaId);

  // Actualización en vivo: cuando otro cliente registra un ganador, refrescamos el bracket.
  useEffect(() => {
    if (!torneoId) return;
    const connection = crearConexionBracket(torneoId);
    connection.on('MatchActualizado', () => {
      qc.invalidateQueries({ queryKey: bracketKeys.detail(torneoId, categoriaId) });
    });
    return () => {
      connection.off('MatchActualizado');
      void connection.stop();
    };
  }, [torneoId, categoriaId, qc]);

  const onRegistrarGanador = useCallback(
    (llaveId: string, ganadorId: string) => {
      registrar.mutate(
        { llaveId, ganadorId },
        {
          onSuccess: (match) =>
            toast.success(`Ganador registrado: ${match.ganador?.nombreCompleto ?? ''}.`),
          onError: (err) =>
            toast.error(err instanceof ApiError ? err.message : 'No se pudo registrar el ganador.'),
        },
      );
    },
    [registrar],
  );

  const sinLlaves = bracket && bracket.rounds.length === 0;

  return (
    <section className="flex flex-col gap-6">
      <header className="flex flex-col gap-2">
        <Link
          to={`/torneos/${torneoId}/categorias`}
          className="text-sm leading-relaxed text-secondary underline-offset-4 hover:underline"
        >
          ← Volver a categorías
        </Link>
        <h1 className="text-2xl leading-relaxed font-bold text-left">Llaves</h1>
        {bracket && (
          <p className="text-sm leading-relaxed text-muted-foreground text-left">{bracket.categoriaNombre}</p>
        )}
        {esCoordinador && bracket && !sinLlaves && (
          <p className="text-sm leading-relaxed text-muted-foreground text-left">
            Hacé clic en un competidor de un match para marcarlo ganador; avanza a la ronda siguiente.
          </p>
        )}
      </header>

      {isLoading && <div className="h-[600px] animate-pulse rounded-lg bg-muted" aria-hidden="true" />}

      {error && (
        <p
          role="alert"
          className="rounded-lg border border-destructive/40 bg-destructive/10 p-4 text-sm leading-relaxed text-destructive"
        >
          {error instanceof ApiError ? error.message : 'No se pudo cargar el bracket. Intentá de nuevo.'}
        </p>
      )}

      {sinLlaves && (
        <p className="text-sm leading-relaxed text-muted-foreground">
          Esta categoría todavía no tiene llaves generadas.
        </p>
      )}

      {bracket && !sinLlaves && (
        <BracketView bracket={bracket} puedeRegistrar={esCoordinador} onRegistrarGanador={onRegistrarGanador} />
      )}
    </section>
  );
}
