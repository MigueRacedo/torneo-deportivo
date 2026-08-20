import * as signalR from '@microsoft/signalr';

const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5000';

/**
 * Crea (y arranca) una conexión SignalR al hub del bracket y se une al grupo del torneo para recibir
 * las actualizaciones en vivo de sus matches.
 */
export function crearConexionBracket(torneoId: string) {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${BASE_URL}/hubs/bracket`)
    .withAutomaticReconnect()
    .build();

  connection
    .start()
    .then(() => connection.invoke('JoinTorneo', torneoId))
    .catch(() => {
      // La actualización en vivo es un extra; si falla, la vista sigue funcionando con refetch manual.
    });

  return connection;
}
