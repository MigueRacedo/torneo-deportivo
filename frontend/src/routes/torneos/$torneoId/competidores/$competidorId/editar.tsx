import { Link, useParams } from 'react-router-dom';
import { CompetidorForm } from '@/components/competidores/CompetidorForm';
import { useCompetidores } from '@/hooks/useCompetidores';
import { ApiError } from '@/lib/api';

/**
 * Página de edición de un competidor. Reutiliza el listado del torneo (cache de TanStack Query)
 * para obtener el competidor por id y precargar el formulario.
 */
export default function EditarCompetidorPage() {
  const { torneoId = '', competidorId = '' } = useParams<{ torneoId: string; competidorId: string }>();
  const { data: competidores, isLoading, error } = useCompetidores(torneoId);
  const competidor = competidores?.find((c) => c.id === competidorId);

  return (
    <section className="flex flex-col gap-6">
      <header className="flex flex-col gap-2">
        <Link
          to={`/torneos/${torneoId}/competidores`}
          className="text-sm leading-relaxed text-secondary underline-offset-4 hover:underline"
        >
          ← Volver a competidores
        </Link>
        <h1 className="text-2xl leading-relaxed font-bold text-left">Editar competidor</h1>
      </header>

      {isLoading && <div className="h-96 animate-pulse rounded-lg bg-muted" aria-hidden="true" />}

      {error && (
        <p
          role="alert"
          className="rounded-lg border border-destructive/40 bg-destructive/10 p-4 text-sm leading-relaxed text-destructive"
        >
          {error instanceof ApiError ? error.message : 'No se pudieron cargar los competidores. Intentá de nuevo.'}
        </p>
      )}

      {!isLoading && !error && !competidor && (
        <p className="text-sm leading-relaxed text-muted-foreground">
          No se encontró el competidor solicitado. Puede que haya sido eliminado.
        </p>
      )}

      {competidor && <CompetidorForm torneoId={torneoId} competidor={competidor} />}
    </section>
  );
}
