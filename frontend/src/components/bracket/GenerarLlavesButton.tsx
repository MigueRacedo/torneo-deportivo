import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import { useGenerarLlaves } from '@/hooks/useBracket';
import { ApiError } from '@/lib/api';

/**
 * Botón (CTA) que dispara la generación de llaves del torneo: clasifica a los competidores y arma los brackets.
 * Muestra un resumen del resultado en un toast.
 */
export function GenerarLlavesButton({ torneoId }: { torneoId: string }) {
  const { mutate, isPending } = useGenerarLlaves(torneoId);

  const onGenerar = () =>
    mutate(undefined, {
      onSuccess: (res) => {
        const generadas = res.categorias.filter((c) => c.llavesGeneradas).length;
        const sinClasificar =
          res.competidoresSinClasificar > 0
            ? ` ${res.competidoresSinClasificar} competidor(es) sin clasificar.`
            : '';
        if (generadas === 0) {
          toast.info(`No se generaron llaves (ninguna categoría con 2+ competidores).${sinClasificar}`);
        } else {
          toast.success(`Llaves generadas en ${generadas} categoría(s).${sinClasificar}`);
        }
      },
      onError: (err) =>
        toast.error(err instanceof ApiError ? err.message : 'No se pudieron generar las llaves.'),
    });

  return (
    <Button onClick={onGenerar} disabled={isPending} className="min-h-11 self-start px-6">
      {isPending ? 'Generando llaves…' : 'Generar llaves'}
    </Button>
  );
}
