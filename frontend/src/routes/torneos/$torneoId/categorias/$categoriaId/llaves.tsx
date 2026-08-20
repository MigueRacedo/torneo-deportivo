import { Link, useParams } from 'react-router-dom';
import { BracketPanel } from '@/components/bracket/BracketPanel';
import { useBracket, useBracketLiveUpdates } from '@/hooks/useBracket';

/**
 * Vista unitaria del bracket de una sola categoría (H0005): sin navegación lateral ni secciones extra,
 * para concentrarse en una llave. El panorama completo del torneo vive en `/torneos/:torneoId/llaves`.
 */
export default function LlavesCategoriaPage() {
  const { torneoId = '', categoriaId = '' } = useParams<{ torneoId: string; categoriaId: string }>();

  // Misma query key que usa BracketPanel: TanStack Query la deduplica, no hay request extra.
  const { data: bracket } = useBracket(torneoId, categoriaId);

  useBracketLiveUpdates(torneoId);

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
          <p className="text-sm leading-relaxed text-muted-foreground text-left">
            {bracket.categoriaNombre}
          </p>
        )}
        <Link
          to={`/torneos/${torneoId}/llaves?categoria=${categoriaId}`}
          className="text-sm leading-relaxed font-medium text-secondary underline-offset-4 hover:underline"
        >
          Ver todas las llaves del torneo →
        </Link>
      </header>

      <BracketPanel torneoId={torneoId} categoriaId={categoriaId} />
    </section>
  );
}
