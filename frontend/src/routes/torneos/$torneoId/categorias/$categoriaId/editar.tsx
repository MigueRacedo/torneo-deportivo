import { Link, useParams } from 'react-router-dom';
import { CategoriaForm } from '@/components/categorias/CategoriaForm';
import { useCategorias } from '@/hooks/useCategorias';
import { ApiError } from '@/lib/api';

/**
 * Página de edición de una categoría. Reutiliza el listado del torneo (cache de TanStack Query)
 * para obtener la categoría por id y precargar el formulario.
 */
export default function EditarCategoriaPage() {
  const { torneoId = '', categoriaId = '' } = useParams<{ torneoId: string; categoriaId: string }>();
  const { data: categorias, isLoading, error } = useCategorias(torneoId);
  const categoria = categorias?.find((c) => c.id === categoriaId);

  return (
    <section className="flex flex-col gap-6">
      <header className="flex flex-col gap-2">
        <Link
          to={`/torneos/${torneoId}/categorias`}
          className="text-sm leading-relaxed text-secondary underline-offset-4 hover:underline"
        >
          ← Volver a categorías
        </Link>
        <h1 className="text-2xl leading-relaxed font-bold text-left">Editar categoría</h1>
      </header>

      {isLoading && <div className="h-96 animate-pulse rounded-lg bg-muted" aria-hidden="true" />}

      {error && (
        <p
          role="alert"
          className="rounded-lg border border-destructive/40 bg-destructive/10 p-4 text-sm leading-relaxed text-destructive"
        >
          {error instanceof ApiError ? error.message : 'No se pudieron cargar las categorías. Intentá de nuevo.'}
        </p>
      )}

      {!isLoading && !error && !categoria && (
        <p className="text-sm leading-relaxed text-muted-foreground">
          No se encontró la categoría solicitada. Puede que haya sido eliminada.
        </p>
      )}

      {categoria && <CategoriaForm torneoId={torneoId} categoria={categoria} />}
    </section>
  );
}
