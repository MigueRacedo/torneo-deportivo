import { useCategorias } from '@/hooks/useCategorias';
import { ApiError } from '@/lib/api';
import { CategoriaCard } from './CategoriaCard';

function CategoriaListSkeleton() {
  return (
    <div className="grid grid-cols-1 gap-4 md:grid-cols-2" aria-hidden="true">
      {[1, 2].map((i) => (
        <div key={i} className="h-32 animate-pulse rounded-lg bg-muted" />
      ))}
    </div>
  );
}

/** Lista las categorías del torneo; maneja estados de carga, error y lista vacía. */
export function CategoriaList({ torneoId }: { torneoId: string }) {
  const { data: categorias, isLoading, error } = useCategorias(torneoId);

  if (isLoading) return <CategoriaListSkeleton />;

  if (error) {
    return (
      <p
        role="alert"
        className="rounded-lg border border-destructive/40 bg-destructive/10 p-4 text-sm leading-relaxed text-destructive"
      >
        {error instanceof ApiError ? error.message : 'No se pudieron cargar las categorías. Intentá de nuevo.'}
      </p>
    );
  }

  if (!categorias || categorias.length === 0) {
    return (
      <p className="text-sm leading-relaxed text-muted-foreground">
        Todavía no hay categorías en este torneo. Cargá la primera con el formulario de abajo.
      </p>
    );
  }

  return (
    <ul className="grid grid-cols-1 gap-4 md:grid-cols-2">
      {categorias.map((categoria) => (
        <li key={categoria.id}>
          <CategoriaCard categoria={categoria} />
        </li>
      ))}
    </ul>
  );
}
