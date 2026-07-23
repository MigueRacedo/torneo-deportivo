import type { Graduacion } from '@/types';

/**
 * Convierte el nombre técnico de una graduación (e.g. "CinturonAmarilloPtaVerde")
 * en una etiqueta legible para el usuario (e.g. "Cinturón Amarillo Pta Verde").
 */
export function formatearGraduacion(graduacion: string): string {
  return graduacion
    .replace(/^Cinturon/, 'Cinturón ')
    .replace(/([a-z])([A-Z0-9])/g, '$1 $2')
    .replace(/Pta/g, 'Pta.')
    .trim();
}

export type { Graduacion };
