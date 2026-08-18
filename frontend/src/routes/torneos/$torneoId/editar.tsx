import { Link, useParams } from 'react-router-dom';
import { TorneoForm } from '@/components/torneos/TorneoForm';
import { useTorneo } from '@/hooks/useTorneos';
import { ApiError } from '@/lib/api';

/** Página de edición de un torneo (H0003): precarga los datos y reutiliza el TorneoForm. */
export default function EditarTorneoPage() {
  const { torneoId = '' } = useParams<{ torneoId: string }>();
  const { data: torneo, isLoading, error } = useTorneo(torneoId);

  return (
    <section className="flex flex-col gap-6">
      <header className="flex flex-col gap-2">
        <Link to="/torneos" className="text-sm leading-relaxed text-secondary underline-offset-4 hover:underline">
          ← Volver a torneos
        </Link>
        <h1 className="text-2xl leading-relaxed font-bold text-left">Editar torneo</h1>
      </header>

      {isLoading && <div className="h-64 animate-pulse rounded-lg bg-muted" aria-hidden="true" />}

      {error && (
        <p
          role="alert"
          className="rounded-lg border border-destructive/40 bg-destructive/10 p-4 text-sm leading-relaxed text-destructive"
        >
          {error instanceof ApiError ? error.message : 'No se pudo cargar el torneo. Intentá de nuevo.'}
        </p>
      )}

      {!isLoading && !error && torneo && <TorneoForm torneo={torneo} />}
    </section>
  );
}
