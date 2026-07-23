import type { Categoria } from '@/types';
import { formatearGraduacion } from './graduacion';

const SEXO_LABEL: Record<Categoria['sexo'], string> = {
  M: 'Masculino',
  F: 'Femenino',
};

/** Tarjeta de resumen de una categoría con sus rangos de edad, peso y graduación. */
export function CategoriaCard({ categoria }: { categoria: Categoria }) {
  return (
    <article className="flex flex-col gap-2 rounded-lg border bg-card p-4 shadow-sm">
      <div className="flex items-start justify-between gap-2">
        <h2 className="text-base leading-relaxed font-semibold text-left">{categoria.nombre}</h2>
        <span className="rounded-full bg-secondary px-2 py-0.5 text-xs leading-relaxed font-medium text-secondary-foreground">
          {categoria.tipoCompetencia}
        </span>
      </div>

      <dl className="grid grid-cols-1 gap-1 text-sm leading-relaxed text-muted-foreground sm:grid-cols-2">
        <div className="flex gap-2">
          <dt className="font-medium text-foreground">Sexo:</dt>
          <dd>{SEXO_LABEL[categoria.sexo]}</dd>
        </div>
        <div className="flex gap-2">
          <dt className="font-medium text-foreground">Edad:</dt>
          <dd>
            {categoria.rangoEdadMin}–{categoria.rangoEdadMax} años
          </dd>
        </div>
        <div className="flex gap-2">
          <dt className="font-medium text-foreground">Peso:</dt>
          <dd>
            {categoria.rangoPesoMin}–{categoria.rangoPesoMax} kg
          </dd>
        </div>
        <div className="flex gap-2">
          <dt className="font-medium text-foreground">Graduación:</dt>
          <dd>
            {formatearGraduacion(categoria.rangoGraduacionMin)} a{' '}
            {formatearGraduacion(categoria.rangoGraduacionMax)}
          </dd>
        </div>
      </dl>
    </article>
  );
}
