import { Link } from 'react-router-dom';
import { useAuthStore } from '@/store/authStore';
import type { Torneo, TorneoEstado } from '@/types';

const ESTADO_LABEL: Record<TorneoEstado, string> = {
  Borrador: 'Borrador',
  Activo: 'Activo',
  Finalizado: 'Finalizado',
};

const ESTADO_CLASES: Record<TorneoEstado, string> = {
  Borrador: 'bg-muted text-muted-foreground',
  Activo: 'bg-secondary text-secondary-foreground',
  Finalizado: 'bg-accent text-accent-foreground',
};

export function TorneoCard({ torneo }: { torneo: Torneo }) {
  const esCoordinador = useAuthStore((state) => state.usuario?.rol === 'Coordinador');
  const puedeEditar = esCoordinador && torneo.estado !== 'Finalizado';
  const fecha = new Date(`${torneo.fecha}T00:00:00`).toLocaleDateString('es-AR', {
    day: '2-digit',
    month: 'long',
    year: 'numeric',
  });

  return (
    <article className="flex flex-col gap-2 rounded-lg border bg-card p-4 shadow-sm">
      <div className="flex items-start justify-between gap-2">
        <h2 className="text-base leading-relaxed font-semibold text-left">{torneo.nombre}</h2>
        <span
          className={`rounded-full px-2 py-0.5 text-xs leading-relaxed font-medium ${ESTADO_CLASES[torneo.estado]}`}
        >
          {ESTADO_LABEL[torneo.estado]}
        </span>
      </div>
      <p className="text-sm leading-relaxed text-muted-foreground text-left">{fecha}</p>
      <p className="text-sm leading-relaxed text-muted-foreground text-left">{torneo.lugar}</p>
      <div className="mt-2 flex flex-wrap items-center gap-4">
        <Link
          to={`/torneos/${torneo.id}/categorias`}
          className="inline-flex min-h-11 items-center text-sm leading-relaxed font-medium text-secondary underline-offset-4 hover:underline focus-visible:outline focus-visible:outline-3 focus-visible:outline-secondary"
        >
          Ver categorías →
        </Link>
        {esCoordinador && (
          <Link
            to={`/torneos/${torneo.id}/competidores`}
            className="inline-flex min-h-11 items-center text-sm leading-relaxed font-medium text-secondary underline-offset-4 hover:underline focus-visible:outline focus-visible:outline-3 focus-visible:outline-secondary"
          >
            Competidores →
          </Link>
        )}
        {puedeEditar && (
          <Link
            to={`/torneos/${torneo.id}/editar`}
            className="inline-flex min-h-11 items-center text-sm leading-relaxed font-medium text-secondary underline-offset-4 hover:underline focus-visible:outline focus-visible:outline-3 focus-visible:outline-secondary"
          >
            Editar
          </Link>
        )}
      </div>
    </article>
  );
}
