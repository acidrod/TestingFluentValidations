using ConsoleUI.Validators;

namespace ConsoleUI.Tests.ValidatorTests;

/// <summary>
/// Tests para el ValidatorRegistry (registro central de validadores).
/// </summary>
public class ValidatorRegistryTests
{
    [Fact]
    public void GetValidator_TipoNoRegistrado_DebeLanzarExcepcion()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => ValidatorRegistry.GetValidator<TipoNoRegistrado>());

        Assert.Contains("No validator registered for type TipoNoRegistrado", ex.Message);
    }

    /// <summary>
    /// Tipo de prueba usado para verificar que el registro lanza excepción para tipos no registrados.
    /// </summary>
    private class TipoNoRegistrado
    {
        public string Id { get; } = string.Empty;
    }
}
