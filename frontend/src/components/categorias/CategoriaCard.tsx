import type { Categoria } from '@/types';
import { formatearGraduacion } from './graduacion';

const SEXO_LABEL: Record<Categoria['sexo'], string> = {
  M: 'Masculino',
  F: 'Femenino',
};

// Colores de la pill de tipo: azul para Formas, ámbar para Combate
// (el rojo queda reservado a los CTAs según la guía de UX).
const TIPO_PILL: Record<Categoria['tipoCompetencia'], string> = {
  Formas: 'bg-secondary text-secondary-foreground',
  Combate: 'bg-amber-100 text-amber-900',
};

/** Texto de un rango numérico abierto (edad/peso): "Hasta X", "X en adelante", "A–B" o null. */
function textoRango(min: number | null | undefined, max: number | null | undefined, unidad: string): string | null {
  const tieneMin = min != null;
  const tieneMax = max != null;
  if (!tieneMin && !tieneMax) return null;
  if (!tieneMin) return `Hasta ${max} ${unidad}`;
  if (!tieneMax) return `${min} ${unidad} en adelante`;
  return `${min}–${max} ${unidad}`;
}

/** Rango de graduación: si mínimo y máximo coinciden, muestra una sola. */
function textoGraduacion(min: string, max: string): string {
  const desde = formatearGraduacion(min);
  const hasta = formatearGraduacion(max);
  return desde === hasta ? desde : `${desde} a ${hasta}`;
}

export function CategoriaCard({ categoria }: { categoria: Categoria }) {
  const edad = textoRango(categoria.rangoEdadMin, categoria.rangoEdadMax, 'años') ?? 'Todas las edades';
  const peso = textoRango(categoria.rangoPesoMin, categoria.rangoPesoMax, 'kg');

  return (
    <article className="flex flex-col gap-2 rounded-lg border bg-card p-4 shadow-sm">
      <div className="flex items-start justify-between gap-2">
        <h2 className="text-base leading-relaxed font-semibold text-left">{categoria.nombre}</h2>
        <span
          className={`rounded-full px-2 py-0.5 text-xs leading-relaxed font-medium ${TIPO_PILL[categoria.tipoCompetencia]}`}
        >
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
          <dd>{edad}</dd>
        </div>
        {peso && (
          <div className="flex gap-2">
            <dt className="font-medium text-foreground">Peso:</dt>
            <dd>{peso}</dd>
          </div>
        )}
        <div className="flex gap-2">
          <dt className="font-medium text-foreground">Graduación:</dt>
          <dd>{textoGraduacion(categoria.rangoGraduacionMin, categoria.rangoGraduacionMax)}</dd>
        </div>
      </dl>
    </article>
  );
}
