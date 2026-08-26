import { Link, useParams } from 'react-router-dom';
import { MisAlumnosList } from '@/components/competidores/MisAlumnosList';
import { useMisCompetidores } from '@/hooks/useCompetidores';
import { useTorneo } from '@/hooks/useTorneos';
import { ApiError } from '@/lib/api';

/**
 * Vista del Profesor con sus alumnos inscriptos en un torneo (H0007).
 *
 * La escuela nunca viaja desde el cliente: el backend la resuelve a partir del usuario del token, así
 * que desde acá no hay forma de consultar los alumnos de otra escuela.
 */
export default function MisAlumnosPage() {
  const { torneoId = '' } = useParams<{ torneoId: string }>();
  const { data: torneo } = useTorneo(torneoId);
  const { data, isLoading, error } = useMisCompetidores(torneoId);

  const sinEscuelaAsignada = !isLoading && !error && data?.escuela == null;

  return (
    <section className="flex flex-col gap-6">
      <header className="flex flex-col gap-2">
        <Link to="/torneos" className="text-sm leading-relaxed text-secondary underline-offset-4 hover:underline">
          ← Volver a torneos
        </Link>
        <h1 className="text-2xl leading-relaxed font-bold text-left">Mis alumnos</h1>
        {torneo && (
          <p className="text-sm leading-relaxed text-muted-foreground text-left">{torneo.nombre}</p>
        )}
        {data?.escuela && (
          <p className="text-sm leading-relaxed text-muted-foreground text-left">
            Escuela: <span className="font-medium text-foreground">{data.escuela}</span> ·{' '}
            {data.competidores.length} inscripto{data.competidores.length === 1 ? '' : 's'}
          </p>
        )}
      </header>

      {error && (
        <p
          role="alert"
          className="rounded-lg border border-destructive/40 bg-destructive/10 p-4 text-sm leading-relaxed text-destructive"
        >
          {error instanceof ApiError
            ? error.message
            : 'No se pudieron cargar tus alumnos. Intentá de nuevo.'}
        </p>
      )}

      {/* Perfil incompleto, no falla del sistema: el mensaje dice qué pasó y a quién recurrir. */}
      {sinEscuelaAsignada && (
        <p
          role="alert"
          className="rounded-lg border border-amber-300 bg-amber-50 p-4 text-sm leading-relaxed text-amber-900"
        >
          Tu usuario todavía no tiene una escuela asignada, así que no podemos saber cuáles son tus
          alumnos. Pedile al Coordinador del torneo que la configure en tu perfil.
        </p>
      )}

      {!error && !sinEscuelaAsignada && (
        <MisAlumnosList competidores={data?.competidores ?? []} isLoading={isLoading} />
      )}
    </section>
  );
}
