import { Link } from 'react-router-dom';
import { formatearGraduacion } from '@/components/categorias/graduacion';
import { useAuthStore } from '@/store/authStore';
import type { Competidor } from '@/types';

const SEXO_LABEL: Record<Competidor['sexo'], string> = { M: 'Masculino', F: 'Femenino' };

/**
 * Competidores que quedaron sin categoría tras la generación de llaves.
 *
 * Muestra los cuatro atributos que usa el clasificador (sexo, edad, peso y graduación) porque son
 * exactamente los que hay que comparar contra los rangos de las categorías para entender por qué
 * ninguno encajó — el dato accionable, no solo el nombre.
 */
export function CompetidoresSinCategoria({
  torneoId,
  competidores,
}: {
  torneoId: string;
  competidores: Competidor[];
}) {
  const esCoordinador = useAuthStore((state) => state.usuario?.rol === 'Coordinador');

  if (competidores.length === 0) {
    return (
      <p className="rounded-lg border border-emerald-200 bg-emerald-50 p-4 text-sm leading-relaxed text-emerald-900">
        ✓ Todos los competidores quedaron clasificados en alguna categoría.
      </p>
    );
  }

  return (
    <div className="flex flex-col gap-4">
      <p className="text-sm leading-relaxed text-muted-foreground text-left">
        Estos competidores no encajaron en los rangos de ninguna categoría, así que no entraron en ninguna
        llave. Revisá sus datos o ajustá los rangos de las categorías que todavía no tienen llaves generadas.
      </p>

      <div className="overflow-x-auto rounded-lg border">
        <table className="w-full border-collapse text-left text-sm leading-relaxed">
          <thead className="bg-secondary text-secondary-foreground">
            <tr>
              <th scope="col" className="px-4 py-2 font-semibold">Competidor</th>
              <th scope="col" className="px-4 py-2 font-semibold">Sexo</th>
              <th scope="col" className="px-4 py-2 font-semibold">Edad</th>
              <th scope="col" className="px-4 py-2 font-semibold">Peso</th>
              <th scope="col" className="px-4 py-2 font-semibold">Graduación</th>
              <th scope="col" className="px-4 py-2 font-semibold">Escuela</th>
              {esCoordinador && (
                <th scope="col" className="px-4 py-2 font-semibold">Acciones</th>
              )}
            </tr>
          </thead>
          <tbody className="divide-y">
            {competidores.map((competidor) => (
              <tr key={competidor.id}>
                <td className="px-4 py-2 font-medium">{competidor.nombreCompleto}</td>
                <td className="px-4 py-2">{SEXO_LABEL[competidor.sexo]}</td>
                <td className="px-4 py-2">{competidor.edad} años</td>
                <td className="px-4 py-2">{competidor.peso} kg</td>
                <td className="px-4 py-2">{formatearGraduacion(competidor.graduacion)}</td>
                <td className="px-4 py-2">{competidor.escuela}</td>
                {esCoordinador && (
                  <td className="px-4 py-2">
                    <Link
                      to={`/torneos/${torneoId}/competidores/${competidor.id}/editar`}
                      className="inline-flex min-h-11 items-center font-medium text-secondary underline-offset-4 hover:underline focus-visible:outline focus-visible:outline-3 focus-visible:outline-secondary"
                    >
                      Editar
                    </Link>
                  </td>
                )}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
