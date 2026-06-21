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
