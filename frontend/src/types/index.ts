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

export type UpdateTorneoInput = CreateTorneoInput;

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
  llavesGeneradas: boolean;
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

export interface Competidor {
  id: string;
  torneoId: string;
  // La categoría es opcional: se asigna al armar las llaves (H0005).
  categoriaId?: string | null;
  categoriaNombre?: string | null;
  nombre: string;
  apellido: string;
  nombreCompleto: string;
  sexo: Sexo;
  edad: number;
  graduacion: string;
  peso: number;
  altura: number;
  escuela: string;
  responsable: string;
  telefono?: string;
}

export interface CreateCompetidorInput {
  nombre: string;
  apellido: string;
  sexo: Sexo;
  edad: number;
  graduacion: string;
  peso: number;
  altura: number;
  escuela: string;
  responsable: string;
  telefono?: string;
}

export type UpdateCompetidorInput = CreateCompetidorInput;

// ── Llaves / Bracket (H0005) ──────────────────────────────────────────────
export type EstadoLlave = 'Pendiente' | 'EnCurso' | 'Finalizado' | 'Bye';

/** Competidor tal como aparece dentro de un match (datos mínimos). */
export interface MatchCompetidor {
  id: string;
  nombreCompleto: string;
}

export interface Match {
  id: string;
  categoriaId: string;
  ronda: number;
  posicion: number;
  competidor1?: MatchCompetidor | null;
  competidor2?: MatchCompetidor | null;
  ganador?: MatchCompetidor | null;
  estado: EstadoLlave;
}

export interface Bracket {
  categoriaId: string;
  categoriaNombre: string;
  totalRondas: number;
  rounds: Match[][]; // [ronda][match]
}

export interface GenerarLlavesResultado {
  categoriaId: string;
  categoriaNombre: string;
  competidoresClasificados: number;
  llavesGeneradas: boolean;
  motivo?: string | null;
}

export interface GenerarLlavesResponse {
  competidoresSinClasificar: number;
  categorias: GenerarLlavesResultado[];
}
