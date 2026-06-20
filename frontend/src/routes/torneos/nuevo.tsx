import { Link } from 'react-router-dom';
import { TorneoForm } from '@/components/torneos/TorneoForm';

export default function NuevoTorneoPage() {
  return (
    <section className="flex flex-col gap-6">
      <header className="flex flex-col gap-2">
        <Link to="/torneos" className="text-sm leading-relaxed text-secondary underline-offset-4 hover:underline">
          ← Volver a torneos
        </Link>
        <h1 className="text-2xl leading-relaxed font-bold text-left">Nuevo torneo</h1>
      </header>
      <TorneoForm />
    </section>
  );
}
