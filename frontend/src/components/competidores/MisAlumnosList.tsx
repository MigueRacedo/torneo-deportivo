import { formatearGraduacion } from '@/components/categorias/graduacion';
import type { MiAlumno } from '@/types';

const SEXO_LABEL: Record<MiAlumno['sexo'], string> = { M: 'Masculino', F: 'Femenino' };

function MisAlumnosSkeleton() {
  return (
    <div className="flex flex-col gap-2" aria-hidden="true">
      {[1, 2, 3].map((i) => (
        <div key={i} className="h-12 animate-pulse rounded-lg bg-muted" />
      ))}
    </div>
  );
}

/**
 * Tabla de solo lectura con los alumnos del Profesor en un torneo (H0007).
 *
 * No incluye responsable ni teléfono: el Profesor ya los conoce, y mostrarlos multiplicaría las
 * pantallas donde viven datos de contacto de menores sin agregar nada a la consulta. Tampoco tiene
 * acciones de edición o borrado — la carga y corrección de competidores es del Coordinador.
 *
 * ⚠️ H0013 unifica esta tabla, `CompetidorList` y `CompetidoresSinCategoria` en un `DataTable`
 * filtrable/ordenable/paginado. Al hacerlo, esta debe migrar al componente compartido.
 */
export function MisAlumnosList({
  competidores,
  isLoading,
}: {
  competidores: MiAlumno[];
  isLoading: boolean;
}) {
  if (isLoading) return <MisAlumnosSkeleton />;

  if (competidores.length === 0) {
    return (
      <p className="rounded-lg border bg-card p-4 text-sm leading-relaxed text-muted-foreground">
        Todavía no hay alumnos de tu escuela inscriptos en este torneo. Cuando el Coordinador los cargue,
        vas a verlos acá.
      </p>
    );
  }

  return (
    <div className="overflow-x-auto rounded-lg border">
      <table className="w-full border-collapse text-left text-sm leading-relaxed">
        <thead className="bg-secondary text-secondary-foreground">
          <tr>
            <th scope="col" className="px-4 py-2 font-semibold">Alumno</th>
            <th scope="col" className="px-4 py-2 font-semibold">Sexo</th>
            <th scope="col" className="px-4 py-2 font-semibold">Edad</th>
            <th scope="col" className="px-4 py-2 font-semibold">Peso</th>
            <th scope="col" className="px-4 py-2 font-semibold">Graduación</th>
            <th scope="col" className="px-4 py-2 font-semibold">Categoría</th>
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
              {/* Sin categoría todavía no es un error: se asigna al generar las llaves (H0005). */}
              <td className="px-4 py-2">
                {competidor.categoriaNombre ?? (
                  <span className="text-muted-foreground">Sin asignar todavía</span>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
