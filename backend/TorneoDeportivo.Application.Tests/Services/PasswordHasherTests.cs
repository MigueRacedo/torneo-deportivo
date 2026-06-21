using TorneoDeportivo.Infrastructure.Services;
using Xunit;

namespace TorneoDeportivo.Application.Tests.Services;

public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void Verify_PasswordCorrecta_ReturnsTrue()
    {
        var hash = _hasher.Hash("Coordinador123!");

        Assert.True(_hasher.Verify("Coordinador123!", hash));
    }

    [Fact]
    public void Verify_PasswordIncorrecta_ReturnsFalse()
    {
        var hash = _hasher.Hash("Coordinador123!");

        Assert.False(_hasher.Verify("otra-password", hash));
    }

    [Fact]
    public void Hash_MismaPassword_GeneraHashesDistintos()
    {
        var hash1 = _hasher.Hash("Coordinador123!");
        var hash2 = _hasher.Hash("Coordinador123!");

        Assert.NotEqual(hash1, hash2);
    }
}
