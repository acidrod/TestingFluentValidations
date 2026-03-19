using ConsoleUI.Enums;
using ConsoleUI.Models;
using ConsoleUI.Validators;

namespace ConsoleUI.Tests.ValidatorTests;

/// <summary>
/// Pruebas unitarias para las reglas de validación de la entidad <see cref="Empleado"/>.
/// Incluye validaciones heredadas de <see cref="Persona"/>, reglas específicas del empleado,
/// validaciones condicionales (Artículo 22) y pruebas de múltiples errores simultáneos.
/// </summary>
public class EmpleadoValidatorTests
{
    /// <summary>
    /// Verifica que un empleado con todos los campos válidos pase todas las validaciones.
    /// </summary>
    [Fact]
    public void Empleado_Valido_DebeSerValido()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();

        var result = validator.Validate(empleado);

        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifica que un empleado con nombre vacío falle con la regla heredada de Persona.
    /// </summary>
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

    /// <summary>
    /// Verifica que un empleado con nombre menor a 3 caracteres falle con la regla heredada de Persona.
    /// </summary>
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

    /// <summary>
    /// Verifica que un empleado con edad exactamente 18 sea válido (GreaterThanOrEqualTo).
    /// </summary>
    [Fact]
    public void Empleado_EdadIgual18_DebeSerValido()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.Age = 18;

        var result = validator.Validate(empleado);

        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifica que un empleado menor de 18 años falle con la regla específica de empleado.
    /// </summary>
    [Fact]
    public void Empleado_EdadMenorA18_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.Age = 17;

        var result = validator.Validate(empleado);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "El empleado debe ser mayor de 18 años");
    }

    /// <summary>
    /// Verifica que una fecha de contratación vacía (default) produzca error de campo requerido.
    /// </summary>
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

    /// <summary>
    /// Verifica que una fecha de contratación futura produzca error.
    /// </summary>
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

    /// <summary>
    /// Verifica que un sueldo menor al mínimo (1000) produzca error.
    /// </summary>
    [Fact]
    public void Empleado_SueldoMenorAlMinimo_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.Sueldo = 500;
        empleado.PorcentajeRetencion = 0;

        var result = validator.Validate(empleado);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "El sueldo debe ser como mínimo de 1000");
    }

    /// <summary>
    /// Verifica que un porcentaje de retención negativo falle por estar fuera del rango 0-100.
    /// </summary>
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

    /// <summary>
    /// Verifica que un porcentaje de retención mayor a 100 falle por estar fuera del rango 0-100.
    /// </summary>
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

    /// <summary>
    /// Verifica que un porcentaje de retención que no corresponde al tramo del sueldo produzca error.
    /// </summary>
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

    /// <summary>
    /// Verifica que retenciones incorrectas para cada tramo de sueldo produzcan error.
    /// Tramos: 1000-4000 ? 5%, 4001-6000 ? 7%, >6000 ? 10%.
    /// </summary>
    [Theory]
    [InlineData(1000, 0)]   // esperado: 5
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

    /// <summary>
    /// Verifica que las retenciones correctas para cada tramo de sueldo sean válidas.
    /// Tramos: 1000-4000 ? 5%, 4001-6000 ? 7%, >6000 ? 10%.
    /// </summary>
    [Theory]
    [InlineData(1000, 5)]
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

    /// <summary>
    /// Verifica que incentivos incorrectos según la sucursal produzcan error.
    /// Norte ? 500, Sur ? 700, Central ? 200.
    /// </summary>
    [Theory]
    [InlineData(Sucursal.Norte, 200)]   // esperado: 500
    [InlineData(Sucursal.Norte, 700)]   // esperado: 500
    [InlineData(Sucursal.Sur, 200)]     // esperado: 700
    [InlineData(Sucursal.Sur, 500)]     // esperado: 700
    [InlineData(Sucursal.Central, 500)] // esperado: 200
    [InlineData(Sucursal.Central, 700)] // esperado: 200
    public void Empleado_IncentivoInvalidoSegunSucursal_DebeFallar(Sucursal sucursal, decimal incentivoIncorrecto)
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.Sucursal = sucursal;
        empleado.Incentivo = incentivoIncorrecto;

        var result = validator.Validate(empleado);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Incentivo");
    }

    /// <summary>
    /// Verifica que los incentivos correctos según la sucursal sean válidos.
    /// Norte ? 500, Sur ? 700, Central ? 200.
    /// </summary>
    [Theory]
    [InlineData(Sucursal.Norte, 500)]
    [InlineData(Sucursal.Sur, 700)]
    [InlineData(Sucursal.Central, 200)]
    public void Empleado_IncentivoValidoSegunSucursal_DebePasar(Sucursal sucursal, decimal incentivo)
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.Sucursal = sucursal;
        empleado.Incentivo = incentivo;

        var result = validator.Validate(empleado);

        Assert.True(result.IsValid);
    }

    #region Casos límite (Boundary Testing)

    /// <summary>
    /// Verifica que el sueldo exactamente 1000 (límite inferior) sea válido.
    /// </summary>
    [Fact]
    public void Empleado_SueldoExactamente1000_DebeSerValido()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.Sueldo = 1000;
        empleado.PorcentajeRetencion = 5; // Retención correcta para sueldo 1000

        var result = validator.Validate(empleado);

        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifica que un sueldo de 999 (justo por debajo del mínimo) falle.
    /// </summary>
    [Fact]
    public void Empleado_SueldoExactamente999_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.Sueldo = 999;
        empleado.PorcentajeRetencion = 0;

        var result = validator.Validate(empleado);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "El sueldo debe ser como mínimo de 1000");
    }

    /// <summary>
    /// Verifica que retención 0 sea válida en rango (0-100) aunque falle por sueldo mínimo.
    /// </summary>
    [Fact]
    public void Empleado_RetencionExactamente0_DebeSerValido()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.Sueldo = 500;
        empleado.PorcentajeRetencion = 0;

        var result = validator.Validate(empleado);

        // Falla por sueldo mínimo, pero retención 0 es válida en rango
        Assert.Contains(result.Errors, e => e.ErrorMessage == "El sueldo debe ser como mínimo de 1000");
        Assert.DoesNotContain(result.Errors, e => e.ErrorMessage == "El porcentaje debe estar entre 0 y 100");
    }

    /// <summary>
    /// Verifica que retención 100 sea válida en rango (0-100) aunque falle por regla de negocio.
    /// </summary>
    [Fact]
    public void Empleado_RetencionExactamente100_DebeSerValidoEnRango()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.PorcentajeRetencion = 100;

        var result = validator.Validate(empleado);

        // Falla por retención según sueldo, pero 100 está dentro del rango 0-100
        Assert.DoesNotContain(result.Errors, e => e.ErrorMessage == "El porcentaje debe estar entre 0 y 100");
    }

    #endregion

    #region Horas diarias - No afecto al Artículo 22

    /// <summary>
    /// Verifica que un empleado no afecto al Art. 22 con 8 horas sea válido.
    /// </summary>
    [Fact]
    public void Empleado_NoAfectoArt22_HorasDentroDeRango_DebeSerValido()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.AfectoArticulo22 = false;
        empleado.HorasDiarias = 8;

        var result = validator.Validate(empleado);

        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifica que las horas dentro del rango 8-12 sean válidas para empleados no afectos al Art. 22.
    /// </summary>
    [Theory]
    [InlineData(8)]
    [InlineData(10)]
    [InlineData(12)]
    public void Empleado_NoAfectoArt22_HorasValidas_DebePasar(int horas)
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.AfectoArticulo22 = false;
        empleado.HorasDiarias = horas;

        var result = validator.Validate(empleado);

        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifica que horas fuera del rango 8-12 fallen para empleados no afectos al Art. 22.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(7)]
    [InlineData(13)]
    [InlineData(16)]
    [InlineData(24)]
    public void Empleado_NoAfectoArt22_HorasFueraDeRango_DebeFallar(int horas)
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.AfectoArticulo22 = false;
        empleado.HorasDiarias = horas;

        var result = validator.Validate(empleado);
        var erroresHoras = result.Errors.Where(e => e.PropertyName == nameof(Empleado.HorasDiarias)).ToList();

        Assert.False(result.IsValid);
        Assert.Single(erroresHoras);
        Assert.Equal("El empleado debe trabajar entre 8 y 12 horas diarias", erroresHoras[0].ErrorMessage);
    }

    #endregion

    #region Horas diarias - Afecto al Artículo 22

    /// <summary>
    /// Verifica que las horas entre 0 y 24 sean válidas para empleados afectos al Art. 22 (sin jornada mínima).
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    [InlineData(12)]
    [InlineData(24)]
    public void Empleado_AfectoArt22_HorasEntre0Y24_DebePasar(int horas)
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.AfectoArticulo22 = true;
        empleado.HorasDiarias = horas;

        var result = validator.Validate(empleado);

        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifica que más de 24 horas falle incluso para empleados afectos al Art. 22.
    /// </summary>
    [Fact]
    public void Empleado_AfectoArt22_HorasMayorA24_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.AfectoArticulo22 = true;
        empleado.HorasDiarias = 25;

        var result = validator.Validate(empleado);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Las horas deben estar entre 0 y 24");
    }

    /// <summary>
    /// Verifica que horas negativas fallen para empleados afectos al Art. 22.
    /// </summary>
    [Fact]
    public void Empleado_AfectoArt22_HorasNegativas_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.AfectoArticulo22 = true;
        empleado.HorasDiarias = -1;

        var result = validator.Validate(empleado);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Las horas deben estar entre 0 y 24");
    }

    /// <summary>
    /// Verifica que 0 horas sea válido para empleados afectos al Art. 22 (sin jornada mínima obligatoria).
    /// </summary>
    [Fact]
    public void Empleado_AfectoArt22_0Horas_DebeSerValido()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.AfectoArticulo22 = true;
        empleado.HorasDiarias = 0;

        var result = validator.Validate(empleado);

        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifica que 0 horas falle para empleados no afectos al Art. 22 (requiere mínimo 8 horas).
    /// </summary>
    [Fact]
    public void Empleado_NoAfectoArt22_0Horas_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = CrearEmpleadoValido();
        empleado.AfectoArticulo22 = false;
        empleado.HorasDiarias = 0;

        var result = validator.Validate(empleado);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "El empleado debe trabajar entre 8 y 12 horas diarias");
    }

    #endregion

    #region Múltiples errores simultáneos

    /// <summary>
    /// Verifica que múltiples errores básicos (nombre corto, menor de edad, fecha futura, sueldo bajo)
    /// se retornen todos simultáneamente.
    /// </summary>
    [Fact]
    public void Empleado_MultiplesErroresBasicos_DebeRetornarTodosLosErrores()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = new Empleado
        {
            Name = "A",                              // Error: nombre muy corto
            Age = 15,                                // Error: menor de 18
            FechaContratacion = DateTime.Now.AddDays(5), // Error: fecha futura
            Sueldo = 500,                            // Error: sueldo menor al mínimo
            PorcentajeRetencion = 0,
            Sucursal = Sucursal.Central,
            Incentivo = 200,
            AfectoArticulo22 = false,
            HorasDiarias = 8
        };

        var result = validator.Validate(empleado);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 4, $"Debería tener al menos 4 errores, pero tiene {result.Errors.Count}");
        Assert.Contains(result.Errors, e => e.ErrorMessage == "El nombre debe tener al menos 3 caracteres");
        Assert.Contains(result.Errors, e => e.ErrorMessage == "El empleado debe ser mayor de 18 años");
        Assert.Contains(result.Errors, e => e.ErrorMessage == "La fecha no puede ser futura");
        Assert.Contains(result.Errors, e => e.ErrorMessage == "El sueldo debe ser como mínimo de 1000");
    }

    /// <summary>
    /// Verifica que errores en reglas de negocio (retención incorrecta + incentivo incorrecto)
    /// se retornen exactamente 2 errores.
    /// </summary>
    [Fact]
    public void Empleado_ErroresEnReglasDeNegocio_DebeRetornarTodosLosErrores()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = new Empleado
        {
            Name = "Roberto",
            Age = 25,
            FechaContratacion = DateTime.Now.AddDays(-1),
            Sueldo = 5000,
            PorcentajeRetencion = 5,    // Error: debería ser 7 para sueldo 5000
            Sucursal = Sucursal.Norte,
            Incentivo = 200,             // Error: debería ser 500 para Norte
            AfectoArticulo22 = false,
            HorasDiarias = 8
        };

        var result = validator.Validate(empleado);

        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "El porcentaje de retención no corresponde al sueldo del empleado");
        Assert.Contains(result.Errors, e => e.PropertyName == "Incentivo");
    }

    /// <summary>
    /// Verifica que cuando todos los campos son inválidos se retornen al menos 6 errores.
    /// Incluye: nombre vacío, edad 0, fecha default, sueldo 0, retención fuera de rango,
    /// incentivo incorrecto y horas insuficientes.
    /// </summary>
    [Fact]
    public void Empleado_TodosLosCamposInvalidos_DebeRetornarTodosLosErrores()
    {
        var validator = ValidatorRegistry.GetValidator<Empleado>();
        var empleado = new Empleado
        {
            Name = "",                               // Error 1: nombre vacío
            Age = 0,                                 // Error 2: edad no positiva, Error 3: menor de 18
            FechaContratacion = default,             // Error 4: fecha vacía
            Sueldo = 0,                              // Error 5: sueldo menor al mínimo
            PorcentajeRetencion = 150,               // Error 6: fuera de rango 0-100
            Sucursal = Sucursal.Sur,
            Incentivo = 100,                         // Error 7: incentivo incorrecto para Sur
            AfectoArticulo22 = false,
            HorasDiarias = 2                         // Error 8: menos de 8 horas (no afecto Art. 22)
        };

        var result = validator.Validate(empleado);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 6, $"Debería tener al menos 6 errores, pero tiene {result.Errors.Count}");
    }

    #endregion

    /// <summary>
    /// Crea un empleado con todos los campos válidos para usar como base en los tests.
    /// Sucursal Central, 8 horas diarias, no afecto al Art. 22.
    /// </summary>
    private static Empleado CrearEmpleadoValido()
    {
        return new Empleado
        {
            Name = "Roberto",
            Age = 25,
            FechaContratacion = DateTime.Now.AddDays(-1),
            Sueldo = 5000,
            PorcentajeRetencion = 7,
            Sucursal = Sucursal.Central,
            Incentivo = 200,
            AfectoArticulo22 = false,
            HorasDiarias = 8
        };
    }
}
