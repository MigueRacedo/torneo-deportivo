export type TorneoEstado = 'Borrador' | 'Activo' | 'Finalizado';
export type RolUsuario = 'Coordinador' | 'Profesor' | 'Director';

export interface Usuario {
  nombre: string;
  email: string;
  rol: RolUsuario;
}

export interface LoginInput {
  email: string;
  password: string;
}

export interface LoginResult {
  token: string;
  nombre: string;
  email: string;
  rol: RolUsuario;
}

export interface Torneo {
  id: string;
  nombre: string;
  fecha: string;
  lugar: string;
  imagenFlyer?: string;
  estado: TorneoEstado;
}

export interface CreateTorneoInput {
  nombre: string;
  fecha: string;
  lugar: string;
  imagenFlyer?: string;
}

export type TipoCompetencia = 'Combate' | 'Formas';
export type Sexo = 'M' | 'F';

/** Graduaciones de Taekwondo (de menor a mayor), usadas en los rangos de categoría. */
export const GRADUACIONES = [
  'CinturonBlanco',
  'CinturonBlancoPtaAmarilla',
  'CinturonAmarillo',
  'CinturonAmarilloPtaVerde',
  'CinturonVerde',
  'CinturonVerdePtaAzul',
  'CinturonAzul',
  'CinturonAzulPtaRoja',
  'CinturonRojo',
  'CinturonRojoPtaNegra',
  'CinturonNegro1Dan',
  'CinturonNegro2Dan',
  'CinturonNegro3Dan',
  'CinturonNegro4Dan',
  'CinturonNegro5Dan',
  'CinturonNegro6Dan',
] as const;

export type Graduacion = (typeof GRADUACIONES)[number];

export interface Categoria {
  id: string;
  torneoId: string;
  nombre: string;
  tipoCompetencia: TipoCompetencia;
  sexo: Sexo;
  // Rangos abiertos: null/undefined = sin ese extremo ("hasta X" o "X en adelante").
  // El peso queda null cuando la categoría es de Formas.
  rangoEdadMin?: number | null;
  rangoEdadMax?: number | null;
  rangoPesoMin?: number | null;
  rangoPesoMax?: number | null;
  rangoGraduacionMin: string;
  rangoGraduacionMax: string;
}

export interface CreateCategoriaInput {
  nombre: string;
  tipoCompetencia: TipoCompetencia;
  sexo: Sexo;
  rangoEdadMin?: number;
  rangoEdadMax?: number;
  rangoPesoMin?: number;
  rangoPesoMax?: number;
  rangoGraduacionMin: string;
  rangoGraduacionMax: string;
}
