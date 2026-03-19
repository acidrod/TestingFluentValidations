using ConsoleUI.Models;
using ConsoleUI.Validators;

namespace ConsoleUI.Tests.ValidatorTests;

/// <summary>
/// Pruebas unitarias para las reglas de validación de la entidad <see cref="Persona"/>.
/// Verifica las reglas base de nombre y edad definidas en <see cref="PersonaValidatorFactory"/>.
/// </summary>
public class PersonaValidatorTests
{
    /// <summary>
    /// Verifica que una persona con nombre y edad válidos pase todas las validaciones.
    /// </summary>
    [Fact]
    public void Persona_Valida_DebeSerValida()
    {
        var validator = ValidatorRegistry.GetValidator<Persona>();
        var persona = new Persona { Name = "Alice", Age = 30 };

        var result = validator.Validate(persona);

        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifica que un nombre vacío produzca el error "El nombre es requerido".
    /// </summary>
    [Fact]
    public void Persona_NombreVacio_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Persona>();
        var persona = new Persona { Name = string.Empty, Age = 30 };

        var result = validator.Validate(persona);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "El nombre es requerido");
    }

    /// <summary>
    /// Verifica que un nombre con menos de 3 caracteres produzca error de longitud mínima.
    /// </summary>
    [Fact]
    public void Persona_NombreCorto_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Persona>();
        var persona = new Persona { Name = "Al", Age = 30 };

        var result = validator.Validate(persona);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "El nombre debe tener al menos 3 caracteres");
    }

    /// <summary>
    /// Verifica que una edad igual a 0 produzca error ya que debe ser mayor a 0.
    /// </summary>
    [Fact]
    public void Persona_EdadNoPositiva_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Persona>();
        var persona = new Persona { Name = "Alice", Age = 0 };

        var result = validator.Validate(persona);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "La edad debe ser mayor a 0");
    }

    /// <summary>
    /// Verifica que una edad mayor o igual a 110 produzca error de límite superior.
    /// </summary>
    [Fact]
    public void Persona_EdadMayorA110_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Persona>();
        var persona = new Persona { Name = "Alice", Age = 200 };

        var result = validator.Validate(persona);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "La edad debe ser menor a 110");
    }

    #region Casos límite (Boundary Testing)

    /// <summary>
    /// Verifica que una edad negativa produzca error de edad no positiva.
    /// </summary>
    [Fact]
    public void Persona_EdadNegativa_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Persona>();
        var persona = new Persona { Name = "Alice", Age = -5 };

        var result = validator.Validate(persona);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "La edad debe ser mayor a 0");
    }

    /// <summary>
    /// Verifica que la edad exactamente 110 sea inválida (regla: menor estricto a 110).
    /// </summary>
    [Fact]
    public void Persona_EdadExactamente110_DebeFallar()
    {
        var validator = ValidatorRegistry.GetValidator<Persona>();
        var persona = new Persona { Name = "Alice", Age = 110 };

        var result = validator.Validate(persona);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "La edad debe ser menor a 110");
    }

    /// <summary>
    /// Verifica que la edad 109 sea el máximo válido (límite superior - 1).
    /// </summary>
    [Fact]
    public void Persona_EdadExactamente109_DebeSerValida()
    {
        var validator = ValidatorRegistry.GetValidator<Persona>();
        var persona = new Persona { Name = "Alice", Age = 109 };

        var result = validator.Validate(persona);

        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifica que la edad 1 sea el mínimo válido (límite inferior + 1).
    /// </summary>
    [Fact]
    public void Persona_EdadExactamente1_DebeSerValida()
    {
        var validator = ValidatorRegistry.GetValidator<Persona>();
        var persona = new Persona { Name = "Alice", Age = 1 };

        var result = validator.Validate(persona);

        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifica que un nombre con exactamente 3 caracteres sea válido (límite mínimo de longitud).
    /// </summary>
    [Fact]
    public void Persona_NombreExactamente3Caracteres_DebeSerValido()
    {
        var validator = ValidatorRegistry.GetValidator<Persona>();
        var persona = new Persona { Name = "Ana", Age = 30 };

        var result = validator.Validate(persona);

        Assert.True(result.IsValid);
    }

    #endregion

    #region Múltiples errores simultáneos

    /// <summary>
    /// Verifica que al tener nombre vacío y edad negativa se retornen todos los errores correspondientes.
    /// </summary>
    [Fact]
    public void Persona_MultiplesErrores_DebeRetornarTodosLosErrores()
    {
        var validator = ValidatorRegistry.GetValidator<Persona>();
        var persona = new Persona { Name = "", Age = -10 };

        var result = validator.Validate(persona);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 2, "Debería tener al menos 2 errores");
        Assert.Contains(result.Errors, e => e.ErrorMessage == "El nombre es requerido");
        Assert.Contains(result.Errors, e => e.ErrorMessage == "La edad debe ser mayor a 0");
    }

    #endregion
}
