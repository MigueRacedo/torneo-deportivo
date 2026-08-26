using NSubstitute;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Application.Features.Competidores.Queries.GetMisCompetidores;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;
using Xunit;

namespace TorneoDeportivo.Application.Tests.Features.Competidores;

public class GetMisCompetidoresQueryHandlerTests
{
    private const string EscuelaPropia = "Escuela Central";
    private const string EscuelaAjena = "Escuela Rival";

    private readonly Guid _torneoId = Guid.NewGuid();
    private readonly Guid _usuarioId = Guid.NewGuid();

    private static Competidor Competidor(Guid torneoId, string escuela, string apellido) => new()
    {
        Id = Guid.NewGuid(),
        TorneoId = torneoId,
        Nombre = "Alumno",
        Apellido = apellido,
        Sexo = Sexo.M,
        Edad = 13,
        Graduacion = "CinturonAmarillo",
        Peso = 40m,
        Altura = 1.5m,
        Escuela = escuela,
        Responsable = "Responsable"
    };

    private (ITorneoRepository tor, IUsuarioRepository usr, ICompetidorRepository comp) Repos(
        string? escuelaDelUsuario, List<Competidor>? alumnos = null, bool torneoExiste = true)
    {
        var tor = Substitute.For<ITorneoRepository>();
        tor.GetByIdAsync(_torneoId, Arg.Any<CancellationToken>())
            .Returns(torneoExiste ? new Torneo { Id = _torneoId } : null);

        var usr = Substitute.For<IUsuarioRepository>();
        usr.GetByIdAsync(_usuarioId, Arg.Any<CancellationToken>())
            .Returns(new Usuario { Id = _usuarioId, Rol = RolUsuario.Profesor, Escuela = escuelaDelUsuario });

        var comp = Substitute.For<ICompetidorRepository>();
        comp.GetByEscuelaAsync(_torneoId, Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(alumnos ?? []);

        return (tor, usr, comp);
    }

    [Fact]
    public async Task Handle_ProfesorConEscuela_DevuelveSusAlumnos()
    {
        var alumnos = new List<Competidor>
        {
            Competidor(_torneoId, EscuelaPropia, "Gómez"),
            Competidor(_torneoId, EscuelaPropia, "Díaz"),
        };
        var (tor, usr, comp) = Repos(EscuelaPropia, alumnos);
        var handler = new GetMisCompetidoresQueryHandler(tor, usr, comp);

        var result = await handler.Handle(new GetMisCompetidoresQuery(_torneoId, _usuarioId), CancellationToken.None);

        Assert.Equal(EscuelaPropia, result.Escuela);
        Assert.Equal(2, result.Competidores.Count);
        Assert.Contains(result.Competidores, c => c.NombreCompleto == "Alumno Gómez");
        // El DTO de esta vista no expone datos de contacto: no tiene Responsable ni Telefono.
        Assert.DoesNotContain("Responsable", typeof(MiAlumnoResponse).GetProperties().Select(p => p.Name));
        Assert.DoesNotContain("Telefono", typeof(MiAlumnoResponse).GetProperties().Select(p => p.Name));
    }

    [Fact]
    public async Task Handle_ConsultaSiempreConLaEscuelaDelUsuarioAutenticado()
    {
        // El corazón de la historia: la escuela sale del usuario, nunca de un parámetro del cliente.
        // Si esto se rompiera, un Profesor podría leer los datos de contacto de alumnos ajenos.
        var (tor, usr, comp) = Repos(EscuelaPropia);
        var handler = new GetMisCompetidoresQueryHandler(tor, usr, comp);

        await handler.Handle(new GetMisCompetidoresQuery(_torneoId, _usuarioId), CancellationToken.None);

        await comp.Received(1).GetByEscuelaAsync(_torneoId, EscuelaPropia, Arg.Any<CancellationToken>());
        await comp.DidNotReceive().GetByEscuelaAsync(Arg.Any<Guid>(), EscuelaAjena, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ProfesorSinEscuelaAsignada_DevuelveVacioSinConsultarCompetidores()
    {
        // Perfil incompleto, no error del sistema: el cliente muestra un mensaje accionable.
        var (tor, usr, comp) = Repos(escuelaDelUsuario: null);
        var handler = new GetMisCompetidoresQueryHandler(tor, usr, comp);

        var result = await handler.Handle(new GetMisCompetidoresQuery(_torneoId, _usuarioId), CancellationToken.None);

        Assert.Null(result.Escuela);
        Assert.Empty(result.Competidores);
        await comp.DidNotReceive().GetByEscuelaAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_EscuelaEnBlanco_SeTrataComoSinAsignar()
    {
        var (tor, usr, comp) = Repos("   ");
        var handler = new GetMisCompetidoresQueryHandler(tor, usr, comp);

        var result = await handler.Handle(new GetMisCompetidoresQuery(_torneoId, _usuarioId), CancellationToken.None);

        Assert.Null(result.Escuela);
        await comp.DidNotReceive().GetByEscuelaAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TorneoInexistente_LanzaNotFoundException()
    {
        var (tor, usr, comp) = Repos(EscuelaPropia, torneoExiste: false);
        var handler = new GetMisCompetidoresQueryHandler(tor, usr, comp);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GetMisCompetidoresQuery(_torneoId, _usuarioId), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_UsuarioInexistente_LanzaNotFoundException()
    {
        var (tor, _, comp) = Repos(EscuelaPropia);
        var usr = Substitute.For<IUsuarioRepository>();
        usr.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Usuario?)null);
        var handler = new GetMisCompetidoresQueryHandler(tor, usr, comp);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GetMisCompetidoresQuery(_torneoId, _usuarioId), CancellationToken.None));
    }
}
