using TorneoDeportivo.Application.Features.Llaves;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using Xunit;

namespace TorneoDeportivo.Application.Tests.Features.Llaves;

public class BracketGeneratorServiceTests
{
    private static List<Competidor> Competidores(int n) =>
        Enumerable.Range(0, n).Select(i => new Competidor { Id = Guid.NewGuid(), Nombre = $"C{i}" }).ToList();

    private static int Byes(List<LlaveCompetencia> l) => l.Count(x => x.Estado == EstadoLlave.Bye);
    private static int Rondas(List<LlaveCompetencia> l) => l.Count == 0 ? 0 : l.Max(x => x.Ronda);

    [Fact]
    public void Generar_DosCompetidores_UnaRondaUnMatchSinByes()
    {
        var llaves = BracketGeneratorService.Generar(Guid.NewGuid(), Competidores(2));
        Assert.Single(llaves);
        Assert.Equal(1, Rondas(llaves));
        Assert.Equal(0, Byes(llaves));
    }

    [Fact]
    public void Generar_CuatroCompetidores_DosRondasTresMatchesSinByes()
    {
        var llaves = BracketGeneratorService.Generar(Guid.NewGuid(), Competidores(4));
        Assert.Equal(3, llaves.Count);
        Assert.Equal(2, Rondas(llaves));
        Assert.Equal(0, Byes(llaves));
    }

    [Fact]
    public void Generar_TresCompetidores_DosRondasTresMatchesUnBye()
    {
        var llaves = BracketGeneratorService.Generar(Guid.NewGuid(), Competidores(3));
        Assert.Equal(3, llaves.Count);
        Assert.Equal(2, Rondas(llaves));
        Assert.Equal(1, Byes(llaves));
    }

    [Fact]
    public void Generar_SeisCompetidores_TresRondasSieteMatchesDosByes()
    {
        var llaves = BracketGeneratorService.Generar(Guid.NewGuid(), Competidores(6));
        Assert.Equal(7, llaves.Count);
        Assert.Equal(3, Rondas(llaves));
        Assert.Equal(2, Byes(llaves));
    }

    [Fact]
    public void Generar_OchoCompetidores_TresRondasSieteMatchesSinByes()
    {
        var llaves = BracketGeneratorService.Generar(Guid.NewGuid(), Competidores(8));
        Assert.Equal(7, llaves.Count);
        Assert.Equal(3, Rondas(llaves));
        Assert.Equal(0, Byes(llaves));
    }

    [Fact]
    public void Generar_UnCompetidor_CampeonDirectoCeroMatches()
    {
        var llaves = BracketGeneratorService.Generar(Guid.NewGuid(), Competidores(1));
        Assert.Empty(llaves);
    }

    [Fact]
    public void Generar_CeroCompetidores_Lanza()
    {
        Assert.Throws<InvalidOperationException>(() => BracketGeneratorService.Generar(Guid.NewGuid(), []));
    }

    [Fact]
    public void Generar_SeisCompetidores_LosByesNoQuedanJuntos()
    {
        var llaves = BracketGeneratorService.Generar(Guid.NewGuid(), Competidores(6));
        var posicionesBye = llaves.Where(l => l.Estado == EstadoLlave.Bye).Select(l => l.Posicion).OrderBy(p => p).ToList();

        // Ningún par de BYEs comparte el mismo match de la ronda siguiente (posicion/2 distinto).
        var padres = posicionesBye.Select(p => p / 2).ToList();
        Assert.Equal(padres.Count, padres.Distinct().Count());
    }

    [Fact]
    public void Generar_TodosLosCompetidoresQuedanUbicadosEnRonda1()
    {
        var competidores = Competidores(6);
        var ids = competidores.Select(c => c.Id).ToHashSet();
        var llaves = BracketGeneratorService.Generar(Guid.NewGuid(), competidores);

        var ubicados = llaves.Where(l => l.Ronda == 1)
            .SelectMany(l => new[] { l.Competidor1Id, l.Competidor2Id })
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToHashSet();

        Assert.True(ids.SetEquals(ubicados));
    }

    [Fact]
    public void Generar_ByeAvanzaAlGanadorALaSiguienteRonda()
    {
        var llaves = BracketGeneratorService.Generar(Guid.NewGuid(), Competidores(3));
        var bye = llaves.Single(l => l.Estado == EstadoLlave.Bye);
        var siguiente = llaves.Single(l => l.Ronda == 2 && l.Posicion == bye.Posicion / 2);

        var slot = bye.Posicion % 2 == 0 ? siguiente.Competidor1Id : siguiente.Competidor2Id;
        Assert.Equal(bye.GanadorId, slot);
    }
}
