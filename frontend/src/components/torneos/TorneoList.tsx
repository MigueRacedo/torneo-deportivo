import { Link } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { useTorneos } from '@/hooks/useTorneos';
import { ApiError } from '@/lib/api';
import { useAuthStore } from '@/store/authStore';
import { TorneoCard } from './TorneoCard';

function TorneoListSkeleton() {
  return (
    <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3" aria-hidden="true">
      {[1, 2, 3].map((i) => (
        <div key={i} className="h-24 animate-pulse rounded-lg bg-muted" />
      ))}
    </div>
  );
}

export function TorneoList() {
  const { data: torneos, isLoading, error } = useTorneos();
  const esCoordinador = useAuthStore((state) => state.usuario?.rol === 'Coordinador');

  return (
    <section className="flex flex-col gap-6">
      <header className="flex items-center justify-between gap-4">
        <h1 className="text-2xl leading-relaxed font-bold text-left">Torneos</h1>
        {esCoordinador && (
          <Button render={<Link to="/torneos/nuevo" />} className="min-h-11 px-6">
            Nuevo torneo
          </Button>
        )}
      </header>

      {isLoading && <TorneoListSkeleton />}

      {error && (
        <p role="alert" className="rounded-lg border border-destructive/40 bg-destructive/10 p-4 text-sm text-destructive">
          {error instanceof ApiError ? error.message : 'No se pudieron cargar los torneos. Intentá de nuevo.'}
        </p>
      )}

      {!isLoading && !error && torneos?.length === 0 && (
        <p className="text-sm leading-relaxed text-muted-foreground">
          {esCoordinador
            ? 'Todavía no hay torneos creados. Hacé clic en "Nuevo torneo" para crear el primero.'
            : 'Todavía no hay torneos creados.'}
        </p>
      )}

      {!isLoading && !error && torneos && torneos.length > 0 && (
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
          {torneos.map((torneo) => (
            <TorneoCard key={torneo.id} torneo={torneo} />
          ))}
        </div>
      )}
    </section>
  );
}
