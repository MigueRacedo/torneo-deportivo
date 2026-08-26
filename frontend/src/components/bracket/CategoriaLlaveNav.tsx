import { Link } from 'react-router-dom';
import type { Categoria } from '@/types';

/**
 * Navegación lateral entre las categorías que ya tienen llaves: se ve una por vez.
 *
 * Son `<Link>` y no botones a propósito: cada categoría es una URL propia
 * (`?categoria=<id>`), así el enlace se puede compartir, abrir en otra pestaña y el
 * botón "atrás" del navegador vuelve a la categoría anterior.
 *
 * El total de competidores sale de `categoria.totalCompetidores`, que ya calcula el backend en el
 * listado (H0006). Antes se recibía un mapa armado recorriendo todos los competidores del torneo,
 * lo cual obligaba a consultar un endpoint exclusivo del Coordinador.
 */
export function CategoriaLlaveNav({
  torneoId,
  categorias,
  seleccionadaId,
}: {
  torneoId: string;
  categorias: Categoria[];
  seleccionadaId?: string;
}) {
  return (
    <nav aria-label="Categorías con llaves generadas">
      <ul className="flex flex-col gap-2">
        {categorias.map((categoria) => {
          const activa = categoria.id === seleccionadaId;
          const total = categoria.totalCompetidores ?? 0;

          return (
            <li key={categoria.id}>
              <Link
                to={`/torneos/${torneoId}/llaves?categoria=${categoria.id}`}
                aria-current={activa ? 'page' : undefined}
                // El estado activo no se comunica solo con color: además del fondo lleva
                // borde izquierdo grueso y negrita (regla de accesibilidad del proyecto).
                className={`flex min-h-11 flex-col justify-center gap-0.5 rounded-lg border-l-4 px-4 py-2 text-left text-sm leading-relaxed focus-visible:outline focus-visible:outline-3 focus-visible:outline-secondary ${
                  activa
                    ? 'border-l-secondary bg-secondary/10 font-semibold text-foreground'
                    : 'border-l-transparent text-muted-foreground hover:bg-muted'
                }`}
              >
                <span className="truncate">{categoria.nombre}</span>
                <span className="text-xs leading-relaxed text-muted-foreground">
                  {categoria.tipoCompetencia} · {total} competidor{total === 1 ? '' : 'es'}
                </span>
              </Link>
            </li>
          );
        })}
      </ul>
    </nav>
  );
}
