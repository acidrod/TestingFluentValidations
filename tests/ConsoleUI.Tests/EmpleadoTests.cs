using ConsoleUI.Models;

namespace ConsoleUI.Tests;

/// <summary>
/// Pruebas unitarias para la lógica de negocio de la entidad <see cref="Empleado"/>.
/// Verifica el cálculo de <see cref="Empleado.SueldoNeto"/> y los valores por defecto de las propiedades.
/// </summary>
public class EmpleadoTests
{
    /// <summary>
    /// Verifica que sin retención ni incentivo, el sueldo neto sea igual al sueldo bruto.
    /// </summary>
    [Fact]
    public void SueldoNeto_SinRetencionNiIncentivo_DebeSerIgualAlSueldo()
    {
        var empleado = CrearEmpleado(sueldo: 5000m, retencion: 0m, incentivo: 0m);

        Assert.Equal(5000m, empleado.SueldoNeto);
    }

    /// <summary>
    /// Verifica que con retención y sin incentivo, se descuente correctamente.
    /// Fórmula: Sueldo - (Sueldo * Retención / 100).
    /// </summary>
    [Fact]
    public void SueldoNeto_ConRetencionSinIncentivo_DebeDescontarRetencion()
    {
        var empleado = CrearEmpleado(sueldo: 5000m, retencion: 10m, incentivo: 0m);

        // 5000 - (5000 * 10 / 100) + 0 = 5000 - 500 = 4500
        Assert.Equal(4500m, empleado.SueldoNeto);
    }

    /// <summary>
    /// Verifica que sin retención y con incentivo, se sume correctamente.
    /// Fórmula: Sueldo + Incentivo.
    /// </summary>
    [Fact]
    public void SueldoNeto_SinRetencionConIncentivo_DebeSumarIncentivo()
    {
        var empleado = CrearEmpleado(sueldo: 5000m, retencion: 0m, incentivo: 500m);

        // 5000 - 0 + 500 = 5500
        Assert.Equal(5500m, empleado.SueldoNeto);
    }

    /// <summary>
    /// Verifica que con retención e incentivo, el cálculo completo sea correcto.
    /// Fórmula: Sueldo - (Sueldo * Retención / 100) + Incentivo.
    /// </summary>
    [Fact]
    public void SueldoNeto_ConRetencionYIncentivo_DebeCalcularCorrectamente()
    {
        var empleado = CrearEmpleado(sueldo: 5000m, retencion: 7m, incentivo: 700m);

        // 5000 - (5000 * 7 / 100) + 700 = 5000 - 350 + 700 = 5350
        Assert.Equal(5350m, empleado.SueldoNeto);
    }

    /// <summary>
    /// Verifica el cálculo de sueldo neto con múltiples combinaciones de sueldo, retención e incentivo.
    /// </summary>
    [Theory]
    [InlineData(1000, 5, 200, 1150)]   // 1000 - 50 + 200 = 1150
    [InlineData(4000, 5, 500, 4300)]   // 4000 - 200 + 500 = 4300
    [InlineData(5000, 7, 700, 5350)]   // 5000 - 350 + 700 = 5350
    [InlineData(7000, 10, 200, 6500)]  // 7000 - 700 + 200 = 6500
    public void SueldoNeto_VariosEscenarios_DebeCalcularCorrectamente(
        decimal sueldo, 
        decimal retencion, 
        decimal incentivo, 
        decimal sueldoNetoEsperado)
    {
        var empleado = CrearEmpleado(sueldo, retencion, incentivo);

        Assert.Equal(sueldoNetoEsperado, empleado.SueldoNeto);
    }

    /// <summary>
    /// Verifica que el valor por defecto de Incentivo sea 200m al crear un nuevo Empleado.
    /// </summary>
    [Fact]
    public void Incentivo_ValorPorDefecto_DebeSerDoscientos()
    {
        var empleado = new Empleado { Name = "Test" };

        Assert.Equal(200m, empleado.Incentivo);
    }

    /// <summary>
    /// Crea un empleado con los valores de sueldo, retención e incentivo proporcionados para pruebas.
    /// </summary>
    private static Empleado CrearEmpleado(decimal sueldo, decimal retencion, decimal incentivo)
    {
        return new Empleado
        {
            Name = "Test",
            Sueldo = sueldo,
            PorcentajeRetencion = retencion,
            Incentivo = incentivo
        };
    }
}
