import { Link, useParams } from 'react-router-dom';
import { CompetidorForm } from '@/components/competidores/CompetidorForm';
import { CompetidorList } from '@/components/competidores/CompetidorList';
import { useTorneo } from '@/hooks/useTorneos';

/** Página de carga de competidores de un torneo (H0004): listado + alta (solo Coordinador). */
export default function CompetidoresPage() {
  const { torneoId = '' } = useParams<{ torneoId: string }>();
  const { data: torneo } = useTorneo(torneoId);

  return (
    <section className="flex flex-col gap-8">
      <header className="flex flex-col gap-2">
        <Link to="/torneos" className="text-sm leading-relaxed text-secondary underline-offset-4 hover:underline">
          ← Volver a torneos
        </Link>
        <h1 className="text-2xl leading-relaxed font-bold text-left">Competidores</h1>
        {torneo && (
          <p className="text-sm leading-relaxed text-muted-foreground text-left">{torneo.nombre}</p>
        )}
        <p className="text-sm leading-relaxed text-muted-foreground text-left">
          Cargá a todos los competidores del torneo. La categoría se les asigna más adelante, al armar las llaves.
        </p>
      </header>

      <CompetidorList torneoId={torneoId} />

      <section className="flex flex-col gap-6 border-t pt-8">
        <h2 className="text-xl leading-relaxed font-semibold text-left">Cargar competidor</h2>
        <CompetidorForm torneoId={torneoId} />
      </section>
    </section>
  );
}
