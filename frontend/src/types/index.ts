export type TorneoEstado = 'Borrador' | 'Activo' | 'Finalizado';

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
