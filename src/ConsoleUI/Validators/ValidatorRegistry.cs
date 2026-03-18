using ConsoleUI.Models;
using FluentValidation;

namespace ConsoleUI.Validators;

/// <summary>
/// Registro central que mapea cada tipo de entidad a su validador correspondiente.
/// Actúa como punto de entrada único para obtener el <see cref="AbstractValidator{T}"/>
/// adecuado según el tipo de clase solicitado.
/// </summary>
/// <remarks>
/// Internamente delega la creación del validador a la fábrica correspondiente:
/// <list type="bullet">
///   <item><description><see cref="Persona"/> → <see cref="PersonaValidatorFactory.Create"/></description></item>
///   <item><description><see cref="Empleado"/> → <see cref="EmpleadoValidatorFactory.Create"/></description></item>
/// </list>
/// Si se solicita un tipo no registrado, se lanza una <see cref="InvalidOperationException"/>.
/// </remarks>
public static class ValidatorRegistry
{
    /// <summary>
    /// Obtiene el validador registrado para el tipo <typeparamref name="T"/> especificado.
    /// </summary>
    /// <typeparam name="T">Tipo de entidad para la cual se requiere el validador. Debe ser una clase.</typeparam>
    /// <returns>
    /// Una instancia de <see cref="AbstractValidator{T}"/> configurada con las reglas de validación
    /// correspondientes al tipo solicitado.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Se lanza cuando no existe un validador registrado para el tipo <typeparamref name="T"/>.
    /// </exception>
    public static AbstractValidator<T> GetValidator<T>() where T : class
    {
        return typeof(T).Name switch
        {
            nameof(Persona) => (AbstractValidator<T>)(object)PersonaValidatorFactory.Create(),
            nameof(Empleado) => (AbstractValidator<T>)(object)EmpleadoValidatorFactory.Create(),
            _ => throw new InvalidOperationException($"No validator registered for type {typeof(T).Name}")
        };
    }
}
