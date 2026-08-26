import { Link, useParams } from 'react-router-dom';
import { GenerarLlavesButton } from '@/components/bracket/GenerarLlavesButton';
import { TorneoEstadoActions } from '@/components/torneos/TorneoEstadoActions';
import { CategoriaForm } from '@/components/categorias/CategoriaForm';
import { CategoriaList } from '@/components/categorias/CategoriaList';
import { useCategorias } from '@/hooks/useCategorias';
import { useTorneo } from '@/hooks/useTorneos';
import { useAuthStore } from '@/store/authStore';
import type { TorneoEstado } from '@/types';


// Qué significa cada etapa, en términos de lo que el Coordinador puede hacer ahora.
const ETAPA_DESCRIPCION: Record<TorneoEstado, string> = {
  Borrador:
    'Estás planificando: podés crear y editar categorías y competidores, y generar las llaves. ' +
    'Todavía no se pueden registrar ganadores.',
  Activo:
    'El torneo está en competencia: se registran ganadores y avanzan las rondas. ' +
    'Las categorías, los competidores y las llaves quedan bloqueados.',
  Finalizado: 'El torneo está cerrado. Queda disponible solo para consulta y reportes.',
};
/** Página de gestión de categorías de un torneo (H0002): lista + alta (solo Coordinador). */
export default function CategoriasPage() {
  const { torneoId = '' } = useParams<{ torneoId: string }>();
  const { data: torneo } = useTorneo(torneoId);
  const esCoordinador = useAuthStore((state) => state.usuario?.rol === 'Coordinador');

  // Misma query key que usa CategoriaList: TanStack Query la deduplica, no hay request extra.
  const { data: categorias } = useCategorias(torneoId);
  const hayLlaves = (categorias ?? []).some((c) => c.llavesGeneradas);
  // En Activo y Finalizado el backend rechaza tocar categorías, competidores y llaves (H0009):
  // se ocultan esas secciones en vez de mostrar botones que van a fallar.
  const enPlanificacion = torneo?.estado === 'Borrador';

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
        {/* Solo cuando hay al menos un bracket: si no, la vista consolidada estaría vacía. */}
        {hayLlaves && (
          <Link
            to={`/torneos/${torneoId}/llaves`}
            className="text-sm leading-relaxed font-medium text-secondary underline-offset-4 hover:underline"
          >
            Ver todas las llaves del torneo →
          </Link>
        )}
      </header>

      {/* La página es de gestión para el Coordinador y de consulta para Profesor y Director (H0006):
          sin esta introducción, un rol de solo lectura ve una pantalla sin acciones y sin explicación. */}
      {!esCoordinador && (
        <p className="text-sm leading-relaxed text-muted-foreground text-left">
          Categorías definidas para este torneo, con sus rangos y cuántos competidores quedaron inscriptos
          en cada una. Las que ya tienen llaves generadas muestran el enlace a su cuadro.
        </p>
      )}

      {/* Ciclo de vida del torneo (H0009): define qué se puede hacer en el resto de la pantalla. */}
      {torneo && (
        <section className="flex flex-col gap-2 rounded-lg border bg-card p-4">
          <h2 className="text-base leading-relaxed font-semibold text-left">
            Estado del torneo: {torneo.estado}
          </h2>
          <p className="text-sm leading-relaxed text-muted-foreground text-left">
            {ETAPA_DESCRIPCION[torneo.estado]}
          </p>
          <TorneoEstadoActions torneo={torneo} />
        </section>
      )}

      {esCoordinador && enPlanificacion && (
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

      {esCoordinador && enPlanificacion && (
        <section className="flex flex-col gap-6 border-t pt-8">
          <h2 className="text-xl leading-relaxed font-semibold text-left">Nueva categoría</h2>
          <CategoriaForm torneoId={torneoId} />
        </section>
      )}
    </section>
  );
}
