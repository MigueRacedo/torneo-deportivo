import { Link } from 'react-router-dom';
import { formatearGraduacion } from '@/components/categorias/graduacion';
import { useCompetidores } from '@/hooks/useCompetidores';
import { ApiError } from '@/lib/api';

function CompetidorListSkeleton() {
  return (
    <div className="flex flex-col gap-2" aria-hidden="true">
      {[1, 2, 3].map((i) => (
        <div key={i} className="h-12 animate-pulse rounded-lg bg-muted" />
      ))}
    </div>
  );
}

/** Tabla de competidores del torneo; maneja estados de carga, error y lista vacía. */
export function CompetidorList({ torneoId }: { torneoId: string }) {
  const { data: competidores, isLoading, error } = useCompetidores(torneoId);

  if (isLoading) return <CompetidorListSkeleton />;

  if (error) {
    return (
      <p
        role="alert"
        className="rounded-lg border border-destructive/40 bg-destructive/10 p-4 text-sm leading-relaxed text-destructive"
      >
        {error instanceof ApiError ? error.message : 'No se pudieron cargar los competidores. Intentá de nuevo.'}
      </p>
    );
  }

  if (!competidores || competidores.length === 0) {
    return (
      <p className="text-sm leading-relaxed text-muted-foreground">
        Todavía no hay competidores cargados en este torneo. Cargá el primero con el formulario de abajo.
      </p>
    );
  }

  return (
    <div className="overflow-x-auto rounded-lg border">
      <table className="w-full border-collapse text-left text-sm leading-relaxed">
        <thead className="bg-secondary text-secondary-foreground">
          <tr>
            <th scope="col" className="px-4 py-2 font-semibold">Competidor</th>
            <th scope="col" className="px-4 py-2 font-semibold">Sexo</th>
            <th scope="col" className="px-4 py-2 font-semibold">Escuela</th>
            <th scope="col" className="px-4 py-2 font-semibold">Edad</th>
            <th scope="col" className="px-4 py-2 font-semibold">Graduación</th>
            <th scope="col" className="px-4 py-2 font-semibold">Peso / Altura</th>
            <th scope="col" className="px-4 py-2 font-semibold">Responsable</th>
            <th scope="col" className="px-4 py-2 font-semibold">Acciones</th>
          </tr>
        </thead>
        <tbody>
          {competidores.map((c) => (
            <tr key={c.id} className="border-t">
              <td className="px-4 py-2 font-medium">
                {c.apellido}, {c.nombre}
              </td>
              <td className="px-4 py-2">{c.sexo === 'F' ? 'Femenino' : 'Masculino'}</td>
              <td className="px-4 py-2">{c.escuela}</td>
              <td className="px-4 py-2">{c.edad}</td>
              <td className="px-4 py-2">{formatearGraduacion(c.graduacion)}</td>
              <td className="px-4 py-2 whitespace-nowrap">
                {c.peso} kg · {c.altura} m
              </td>
              <td className="px-4 py-2">
                {c.responsable}
                {c.telefono ? ` · ${c.telefono}` : ''}
              </td>
              <td className="px-4 py-2">
                <Link
                  to={`/torneos/${c.torneoId}/competidores/${c.id}/editar`}
                  className="inline-flex min-h-11 items-center font-medium text-secondary underline-offset-4 hover:underline focus-visible:outline focus-visible:outline-3 focus-visible:outline-secondary"
                >
                  Editar
                </Link>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
