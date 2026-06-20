# Skill: Algoritmo de Generación de Llaves (Bracket)

> ⭐ Esta es la funcionalidad core del proyecto (H0005, 13 story points).
> Leer completo antes de escribir o modificar cualquier código relacionado.

## Descripción del Problema
Dado un conjunto de competidores en una categoría, generar un bracket de
**eliminación directa (single-elimination)** donde:
- El ganador de cada match avanza a la siguiente ronda
- Si la cantidad de competidores no es potencia de 2, se asignan **BYEs**
  (el competidor pasa automáticamente sin rival)
- Los BYEs se colocan estratégicamente para que los cabezas de serie más
  fuertes no se enfrenten en rondas tempranas

## Reglas de Negocio del Dominio
1. Mínimo 2 competidores por categoría para generar llaves
2. Si hay un solo competidor, ese es campeón automático (caso especial)
3. Los competidores se asignan aleatoriamente a las posiciones (sin seeding)
4. Los BYEs siempre van en la primera ronda
5. El número de rondas = ⌈log₂(n)⌉ donde n = cantidad de competidores
6. Una vez generadas las llaves, **no se pueden regenerar** (solo editar ganadores)

## Algoritmo — Paso a Paso

### Paso 1: Calcular tamaño del bracket
```csharp
int competidores = categoria.Competidores.Count; // e.g. 6
int tamañoBracket = (int)Math.Pow(2, Math.Ceiling(Math.Log2(competidores))); // → 8
int totalRondas = (int)Math.Log2(tamañoBracket); // → 3
int totalByes = tamañoBracket - competidores; // → 2 BYEs
```

### Paso 2: Mezclar competidores aleatoriamente
```csharp
var competidoresOrdenados = categoria.Competidores
    .OrderBy(_ => Random.Shared.Next())  // shuffle aleatorio
    .ToList();
```

### Paso 3: Construir la primera ronda con BYEs
```csharp
// Crear slots: llenar con competidores y agregar nulls (BYE) al final
var slots = new List<Competidor?>(tamañoBracket);
slots.AddRange(competidoresOrdenados);
while (slots.Count < tamañoBracket) slots.Add(null); // agregar BYEs

// Crear matches de la primera ronda (pares de slots)
var matchesPrimerRonda = new List<Match>();
for (int i = 0; i < tamañoBracket; i += 2)
{
    var match = new Match
    {
        CategoriaId = categoria.Id,
        Ronda = 1,
        Posicion = i / 2,
        Competidor1 = slots[i],
        Competidor2 = slots[i + 1]
    };

    // Si uno de los dos es BYE, el otro pasa automáticamente
    if (match.Competidor2 == null)
    {
        match.Ganador = match.Competidor1;
        match.Estado = EstadoMatch.Bye;
    }
    else
    {
        match.Estado = EstadoMatch.Pendiente;
    }

    matchesPrimerRonda.Add(match);
}
```

### Paso 4: Crear matches vacíos de rondas siguientes
```csharp
var todosLosMatches = new List<Match>(matchesPrimerRonda);

int matchesPorRonda = tamañoBracket / 2;
for (int ronda = 2; ronda <= totalRondas; ronda++)
{
    matchesPorRonda /= 2;
    for (int pos = 0; pos < matchesPorRonda; pos++)
    {
        todosLosMatches.Add(new Match
        {
            CategoriaId = categoria.Id,
            Ronda = ronda,
            Posicion = pos,
            Estado = EstadoMatch.Pendiente
        });
    }
}
```

### Paso 5: Propagar BYEs automáticamente
Después de crear todos los matches, propagar los ganadores de BYEs a la siguiente ronda:
```csharp
void PropagateGanador(Match match, List<Match> todos)
{
    if (match.Ganador == null) return;

    int siguienteRonda = match.Ronda + 1;
    int posicionEnSiguienteRonda = match.Posicion / 2;
    bool esCompetidor1 = match.Posicion % 2 == 0;

    var siguienteMatch = todos.FirstOrDefault(
        m => m.Ronda == siguienteRonda && m.Posicion == posicionEnSiguienteRonda);

    if (siguienteMatch == null) return;

    if (esCompetidor1) siguienteMatch.Competidor1 = match.Ganador;
    else siguienteMatch.Competidor2 = match.Ganador;
}
```

## Ejemplo Concreto: 6 Competidores

```
Competidores: [Ana, Bruno, Carlos, Diana, Elena, Fede]
Bracket size: 8 (siguiente potencia de 2)
BYEs: 2

Primera ronda:
  [0] Ana     vs Bruno   → pendiente
  [1] Carlos  vs Diana   → pendiente
  [2] Elena   vs Fede    → pendiente
  [3] BYE     vs BYE     → ningún ganador (ronda final del bye)

Espera... con 6 y bracket de 8, hay 2 byes.
Los byes se distribuyen en posiciones 6 y 7:

  [0] Ana     vs Bruno   → pendiente
  [1] Carlos  vs Diana   → pendiente
  [2] Elena   vs Fede    → pendiente
  [3] BYE (null) vs BYE (null)  → NO, incorrecto

CORRECTO — distribuir BYEs entre competidores:
slots = [Ana, Bruno, Carlos, Diana, Elena, Fede, null, null]

Ronda 1 (4 matches):
  pos[0]: Ana     vs Bruno   → ganador: ???
  pos[1]: Carlos  vs Diana   → ganador: ???
  pos[2]: Elena   vs Fede    → ganador: ???
  pos[3]: null(BYE) vs null(BYE) → sin ganador, match ignorado

Ronda 2 (2 matches):
  pos[0]: Ganador[0] vs Ganador[1]
  pos[1]: Ganador[2] vs Ganador[3]  ← Ganador[3] nunca existe (doble BYE)

PROBLEMA: doble BYE crea un hueco. Solución: distribuir BYEs entre competidores reales.

SOLUCIÓN CORRECTA para 6 competidores:
slots = [Ana, null, Bruno, null, Carlos, Diana, Elena, Fede]

Ronda 1 (4 matches):
  pos[0]: Ana   vs null(BYE) → Ganador: Ana (bye automático)
  pos[1]: Bruno vs null(BYE) → Ganador: Bruno (bye automático)
  pos[2]: Carlos vs Diana    → ganador: ???
  pos[3]: Elena  vs Fede     → ganador: ???

Ronda 2 (2 matches):
  pos[0]: Ana vs Bruno     ← ambos con bye, se enfrentan ahora
  pos[1]: Ganador[2] vs Ganador[3]

Ronda 3 (final):
  pos[0]: Ganador[0] vs Ganador[1]
```

## Algoritmo de Distribución de BYEs
Para distribuir BYEs de forma que los BYEs queden separados entre sí:
```csharp
static List<Competidor?> DistribuirByes(List<Competidor> competidores, int tamañoBracket)
{
    int n = competidores.Count;
    int byes = tamañoBracket - n;

    // Intercalar BYEs con competidores
    // Técnica: colocar BYEs en posiciones pares al principio
    var slots = new List<Competidor?>(tamañoBracket);
    int byesColocados = 0;
    int competidoresColocados = 0;

    for (int i = 0; i < tamañoBracket; i++)
    {
        bool colocarBye = (byesColocados < byes) &&
                          (competidoresColocados >= n ||
                           (double)byesColocados / byes <= (double)i / tamañoBracket);

        if (colocarBye)
        {
            slots.Add(null);
            byesColocados++;
        }
        else
        {
            slots.Add(competidores[competidoresColocados]);
            competidoresColocados++;
        }
    }

    return slots;
}
```

## Registrar Ganador de un Match
Cuando se registra el ganador de un match, propagar al siguiente match:
```csharp
public async Task RegistrarGanador(Guid matchId, Guid ganadorId)
{
    var match = await _repo.GetMatchById(matchId);
    match.GanadorId = ganadorId;
    match.Estado = EstadoMatch.Finalizado;

    // Propagar al siguiente match
    var siguienteMatch = await _repo.GetMatchEnSiguienteRonda(
        match.CategoriaId, match.Ronda + 1, match.Posicion / 2);

    if (siguienteMatch != null)
    {
        if (match.Posicion % 2 == 0) siguienteMatch.Competidor1Id = ganadorId;
        else siguienteMatch.Competidor2Id = ganadorId;

        await _repo.UpdateMatch(siguienteMatch);
    }
}
```

## Visualización en Frontend
- El bracket se renderiza de **izquierda a derecha** (Ronda 1 → Final)
- Cada ronda ocupa una columna
- Los matches se alinean verticalmente con espacio exponencial
- Las líneas conectoras muestran qué match alimenta al siguiente
- Los BYEs se muestran con fondo diferente y texto "Pasa directo"

## Tests Obligatorios
```
✓ Con 2 competidores: 1 ronda, 1 match, 0 byes
✓ Con 4 competidores: 2 rondas, 3 matches, 0 byes
✓ Con 3 competidores: 2 rondas, 3 matches, 1 bye
✓ Con 6 competidores: 3 rondas, 7 matches (4+2+1), 2 byes
✓ Con 8 competidores: 3 rondas, 7 matches, 0 byes
✓ Con 1 competidor: campeón directo, 0 matches
✓ Los BYEs no quedan juntos (distribución uniforme)
✓ Registrar ganador propaga correctamente a siguiente ronda
✓ No se pueden generar llaves si ya existen para esa categoría
```
