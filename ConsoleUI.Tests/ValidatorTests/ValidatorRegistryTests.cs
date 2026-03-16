using ConsoleUI;

namespace ConsoleUI.Tests.ValidatorTests;

public class ValidatorRegistryTests
{
    [Fact]
    public void Persona_Valida_DebeSerValida()
    {
        var validator = ValidatorRegistry.GetValidator<Persona>();
        var persona = new Persona { Name = "Alice", Age = 30 };

        var result = validator.Validate(persona);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Persona_NombreVacio_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Persona>();
        var persona = new Persona { Name = string.Empty, Age = 30 };

        var result = validator.Validate(persona);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "El nombre es requerido");
    }

    [Fact]
    public void Persona_NombreCorto_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Persona>();
        var persona = new Persona { Name = "Al", Age = 30 };

        var result = validator.Validate(persona);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "El nombre debe tener al menos 3 caracteres");
    }

    [Fact]
    public void Persona_EdadNoPositiva_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Persona>();
        var persona = new Persona { Name = "Alice", Age = 0 };

        var result = validator.Validate(persona);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "La edad debe ser mayor a 0");
    }

    [Fact]
    public void Persona_EdadMayorA150_SigueSiendoValida_PorqueReglaEstaDesactivada()
    {
        var validator = ValidatorRegistry.GetValidator<Persona>();
        var persona = new Persona { Name = "Alice", Age = 200 };

        var result = validator.Validate(persona);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Empleado_Valido_DebeSerValido()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();

        var result = validator.Validate(empleado);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Empleado_NombreVacio_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.Name = string.Empty;

        var result = validator.Validate(empleado);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "El nombre es requerido");
    }

    [Fact]
    public void Empleado_NombreCorto_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.Name = "Bo";

        var result = validator.Validate(empleado);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "El nombre debe tener al menos 3 caracteres");
    }

    [Fact]
    public void Empleado_EdadIgual18_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.Age = 18;

        var result = validator.Validate(empleado);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "El empleado debe ser mayor de 18 años");
    }

    [Fact]
    public void Empleado_FechaContratacionVacia_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.FechaContratacion = default;

        var result = validator.Validate(empleado);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "La fecha de contratación es requerida");
    }

    [Fact]
    public void Empleado_FechaFutura_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.FechaContratacion = DateTime.Now.AddDays(1);

        var result = validator.Validate(empleado);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "La fecha no puede ser futura");
    }

    [Fact]
    public void Empleado_SueldoNoPositivo_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.Sueldo = 0;
        empleado.PorcentajeRetencion = 0;

        var result = validator.Validate(empleado);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "El sueldo debe ser mayor a 0");
    }

    [Fact]
    public void Empleado_RetencionNegativa_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.PorcentajeRetencion = -1;

        var result = validator.Validate(empleado);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "El porcentaje debe estar entre 0 y 100");
    }

    [Fact]
    public void Empleado_RetencionFueraDeRango_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.PorcentajeRetencion = 120;

        var result = validator.Validate(empleado);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "El porcentaje debe estar entre 0 y 100");
    }

    [Fact]
    public void Empleado_RetencionInvalidaSegunSueldo_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.Sueldo = 5000;
        empleado.PorcentajeRetencion = 5;

        var result = validator.Validate(empleado);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "El porcentaje de retención no corresponde al sueldo del empleado");
    }

    [Theory]
    [InlineData(1499, 5)]   // esperado: 0
    [InlineData(1500, 7)]   // esperado: 5
    [InlineData(4000, 10)]  // esperado: 5
    [InlineData(4001, 5)]   // esperado: 7
    [InlineData(6000, 0)]   // esperado: 7
    [InlineData(6001, 5)]   // esperado: 10
    public void Empleado_RetencionInvalidaSegunTramo_DebeFallar(decimal sueldo, decimal retencionIncorrecta)
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.Sueldo = sueldo;
        empleado.PorcentajeRetencion = retencionIncorrecta;

        var result = validator.Validate(empleado);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "El porcentaje de retención no corresponde al sueldo del empleado");
    }

    [Theory]
    [InlineData(1499, 0)]
    [InlineData(1500, 5)]
    [InlineData(4000, 5)]
    [InlineData(4001, 7)]
    [InlineData(6000, 7)]
    [InlineData(6001, 10)]
    public void Empleado_RetencionValidaSegunTramo_DebePasar(decimal sueldo, decimal retencion)
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.Sueldo = sueldo;
        empleado.PorcentajeRetencion = retencion;

        var result = validator.Validate(empleado);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void GetValidator_TipoNoRegistrado_DebeLanzarExcepcion()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => ValidatorRegistry.GetValidator<TipoNoRegistrado>());

        Assert.Contains("No validator registered for type TipoNoRegistrado", ex.Message);
    }

    [Fact]
    public void Program_Main_NoDebeLanzarExcepcion()
    {
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            Program.Main(Array.Empty<string>());
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = writer.ToString();
        Assert.Contains("Persona válida: True", output);
        Assert.Contains("Empleado válido: True", output);
    }

    private static Empleado CrearEmpleadoValido()
    {
        return new Empleado
        {
            Name = "Roberto",
            Age = 25,
            FechaContratacion = DateTime.Now.AddDays(-1),
            Sueldo = 5000,
            PorcentajeRetencion = 7
        };
    }

    private class TipoNoRegistrado
    {
    }
}
