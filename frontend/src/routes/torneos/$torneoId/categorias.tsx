import { Link, useParams } from 'react-router-dom';
import { GenerarLlavesButton } from '@/components/bracket/GenerarLlavesButton';
import { CategoriaForm } from '@/components/categorias/CategoriaForm';
import { CategoriaList } from '@/components/categorias/CategoriaList';
import { useTorneo } from '@/hooks/useTorneos';
import { useAuthStore } from '@/store/authStore';

/** Página de gestión de categorías de un torneo (H0002): lista + alta (solo Coordinador). */
export default function CategoriasPage() {
  const { torneoId = '' } = useParams<{ torneoId: string }>();
  const { data: torneo } = useTorneo(torneoId);
  const esCoordinador = useAuthStore((state) => state.usuario?.rol === 'Coordinador');

  return (
    <section className="flex flex-col gap-8">
      <header className="flex flex-col gap-2">
        <Link to="/torneos" className="text-sm leading-relaxed text-secondary underline-offset-4 hover:underline">
          ← Volver a torneos
        </Link>
        <h1 className="text-2xl leading-relaxed font-bold text-left">Categorías</h1>
        {torneo && (
          <p className="text-sm leading-relaxed text-muted-foreground text-left">{torneo.nombre}</p>
        )}
      </header>

      {esCoordinador && (
        <section className="flex flex-col gap-2 rounded-lg border bg-card p-4">
          <h2 className="text-base leading-relaxed font-semibold text-left">Armar las llaves</h2>
          <p className="text-sm leading-relaxed text-muted-foreground text-left">
            Clasifica a los competidores en las categorías que encajen y genera el bracket de cada una (2+
            competidores). Una categoría con llaves generadas queda bloqueada para edición.
          </p>
          <GenerarLlavesButton torneoId={torneoId} />
        </section>
      )}

      <CategoriaList torneoId={torneoId} />

      {esCoordinador && (
        <section className="flex flex-col gap-6 border-t pt-8">
          <h2 className="text-xl leading-relaxed font-semibold text-left">Nueva categoría</h2>
          <CategoriaForm torneoId={torneoId} />
        </section>
      )}
    </section>
  );
}
