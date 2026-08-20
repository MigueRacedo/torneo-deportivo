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

  const unirseAlGrupo = () =>
    connection.invoke('JoinTorneo', torneoId).catch(() => {
      // La actualización en vivo es un extra; si falla, la vista sigue funcionando con refetch manual.
    });

  connection.start().then(unirseAlGrupo).catch(() => {});

  // Al reconectar, SignalR asigna un ConnectionId nuevo y la membresía al grupo `torneo-{id}` del hub
  // se pierde (los grupos se indexan por ConnectionId). Sin volver a unirse, las actualizaciones en vivo
  // dejarían de llegar en silencio y la vista quedaría desactualizada sin ningún síntoma visible.
  connection.onreconnected(unirseAlGrupo);

  return connection;
}
