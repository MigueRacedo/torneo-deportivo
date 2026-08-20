import { useMemo } from 'react';
import { Link, useParams, useSearchParams } from 'react-router-dom';
import { BracketPanel } from '@/components/bracket/BracketPanel';
import { CategoriaLlaveNav } from '@/components/bracket/CategoriaLlaveNav';
import { CompetidoresSinCategoria } from '@/components/bracket/CompetidoresSinCategoria';
import { useBracketLiveUpdates } from '@/hooks/useBracket';
import { useCategorias } from '@/hooks/useCategorias';
import { useCompetidores } from '@/hooks/useCompetidores';
import { useTorneo } from '@/hooks/useTorneos';
import { ApiError } from '@/lib/api';

/** Explica por qué una categoría no llegó a tener bracket, según cuántos competidores le tocaron. */
function motivoSinLlaves(total: number): string {
  if (total === 1) return 'Un solo competidor: es campeón directo, no hay bracket que armar.';
  return 'Ningún competidor encajó en sus rangos.';
}

/**
 * Vista consolidada de las llaves del torneo (H0005): navegación entre las categorías con bracket
 * (se ve una por vez), las categorías que quedaron sin llaves con su motivo, y los competidores que
 * ninguna categoría absorbió. Es la pantalla a la que se llega después de generar.
 */
export default function LlavesTorneoPage() {
  const { torneoId = '' } = useParams<{ torneoId: string }>();
  const [searchParams] = useSearchParams();

  const { data: torneo } = useTorneo(torneoId);
  const { data: categorias, isLoading: cargandoCategorias, error: errorCategorias } = useCategorias(torneoId);
  const { data: competidores, isLoading: cargandoCompetidores, error: errorCompetidores } =
    useCompetidores(torneoId);

  // Una sola conexión SignalR para toda la página, no una por categoría.
  useBracketLiveUpdates(torneoId);

  const competidoresPorCategoria = useMemo(() => {
    const conteo = new Map<string, number>();
    for (const competidor of competidores ?? []) {
      if (competidor.categoriaId) {
        conteo.set(competidor.categoriaId, (conteo.get(competidor.categoriaId) ?? 0) + 1);
      }
    }
    return conteo;
  }, [competidores]);

  const conLlaves = useMemo(() => (categorias ?? []).filter((c) => c.llavesGeneradas), [categorias]);
  const sinLlaves = useMemo(() => (categorias ?? []).filter((c) => !c.llavesGeneradas), [categorias]);
  const sinCategoria = useMemo(
    () => (competidores ?? []).filter((c) => c.categoriaId == null),
    [competidores],
  );

  // La categoría del query param manda; si no es válida (o no hay), se cae a la primera con llaves.
  const categoriaParam = searchParams.get('categoria');
  const seleccionada = conLlaves.find((c) => c.id === categoriaParam) ?? conLlaves[0];

  const cargando = cargandoCategorias || cargandoCompetidores;
  const error = errorCategorias ?? errorCompetidores;

  return (
    <section className="flex flex-col gap-8">
      <header className="flex flex-col gap-2">
        <Link
          to={`/torneos/${torneoId}/categorias`}
          className="text-sm leading-relaxed text-secondary underline-offset-4 hover:underline"
        >
          ← Volver a categorías
        </Link>
        <h1 className="text-2xl leading-relaxed font-bold text-left">Llaves del torneo</h1>
        {torneo && (
          <p className="text-sm leading-relaxed text-muted-foreground text-left">{torneo.nombre}</p>
        )}
      </header>

      {cargando && <div className="h-[600px] animate-pulse rounded-lg bg-muted" aria-hidden="true" />}

      {error && (
        <p
          role="alert"
          className="rounded-lg border border-destructive/40 bg-destructive/10 p-4 text-sm leading-relaxed text-destructive"
        >
          {error instanceof ApiError
            ? error.message
            : 'No se pudieron cargar las llaves del torneo. Intentá de nuevo.'}
        </p>
      )}

      {!cargando && !error && (
        <>
          {conLlaves.length === 0 ? (
            <p className="rounded-lg border bg-card p-4 text-sm leading-relaxed text-muted-foreground">
              Ninguna categoría de este torneo tiene llaves generadas todavía. Volvé a categorías y usá
              “Generar llaves”.
            </p>
          ) : (
            <div className="grid grid-cols-1 gap-8 lg:grid-cols-12">
              <aside className="flex flex-col gap-4 lg:col-span-4">
                <h2 className="text-base leading-relaxed font-semibold text-left">
                  Categorías con llaves ({conLlaves.length})
                </h2>
                <CategoriaLlaveNav
                  torneoId={torneoId}
                  categorias={conLlaves}
                  seleccionadaId={seleccionada?.id}
                  competidoresPorCategoria={competidoresPorCategoria}
                />
              </aside>

              <div className="flex flex-col gap-4 lg:col-span-8">
                {seleccionada && (
                  <>
                    <div className="flex flex-wrap items-baseline justify-between gap-2">
                      <h2 className="text-xl leading-relaxed font-semibold text-left">
                        {seleccionada.nombre}
                      </h2>
                      <Link
                        to={`/torneos/${torneoId}/categorias/${seleccionada.id}/llaves`}
                        className="text-sm leading-relaxed font-medium text-secondary underline-offset-4 hover:underline"
                      >
                        Ver solo esta categoría →
                      </Link>
                    </div>
                    <BracketPanel torneoId={torneoId} categoriaId={seleccionada.id} />
                  </>
                )}
              </div>
            </div>
          )}

          {sinLlaves.length > 0 && (
            <section className="flex flex-col gap-4 border-t pt-8">
              <h2 className="text-xl leading-relaxed font-semibold text-left">
                Categorías sin llaves ({sinLlaves.length})
              </h2>
              <ul className="flex flex-col gap-2">
                {sinLlaves.map((categoria) => (
                  <li
                    key={categoria.id}
                    className="flex flex-col gap-0.5 rounded-lg border bg-card px-4 py-2"
                  >
                    <span className="text-sm leading-relaxed font-medium text-left">
                      {categoria.nombre}
                    </span>
                    <span className="text-sm leading-relaxed text-muted-foreground text-left">
                      {motivoSinLlaves(competidoresPorCategoria.get(categoria.id) ?? 0)}
                    </span>
                  </li>
                ))}
              </ul>
            </section>
          )}

          <section className="flex flex-col gap-4 border-t pt-8">
            <h2 className="text-xl leading-relaxed font-semibold text-left">
              Competidores sin categoría ({sinCategoria.length})
            </h2>
            <CompetidoresSinCategoria torneoId={torneoId} competidores={sinCategoria} />
          </section>
        </>
      )}
    </section>
  );
}
