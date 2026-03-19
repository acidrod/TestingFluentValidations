using ConsoleUI.Models;
using FluentValidation;

namespace ConsoleUI.Validators;

/// <summary>
/// Fábrica estática encargada de crear validadores para la entidad <see cref="Persona"/>
/// y de exponer las reglas base reutilizables que pueden ser heredadas por entidades derivadas.
/// </summary>
/// <remarks>
/// Las reglas base incluyen validaciones sobre las propiedades <see cref="Persona.Name"/> y
/// <see cref="Persona.Age"/>, y están diseñadas para ser compartidas mediante el método genérico
/// <see cref="GetPersonaBaseRules{T}"/>.
/// </remarks>
public static class PersonaValidatorFactory
{
    /// <summary>
    /// Obtiene un diccionario con las reglas de validación base aplicables a cualquier entidad
    /// que herede de <see cref="Persona"/>.
    /// </summary>
    /// <remarks>
    /// Las reglas incluidas son:
    /// <list type="bullet">
    ///   <item><description><see cref="Persona.Name"/>: no puede estar vacío.</description></item>
    ///   <item><description><see cref="Persona.Name"/>: debe tener al menos 3 caracteres.</description></item>
    ///   <item><description><see cref="Persona.Age"/>: debe ser mayor a 0.</description></item>
    ///   <item><description><see cref="Persona.Age"/>: debe ser menor a 110.</description></item>
    /// </list>
    /// </remarks>
    /// <typeparam name="T">Tipo que hereda de <see cref="Persona"/>.</typeparam>
    /// <returns>
    /// Diccionario de reglas de validación con su estado de activación (<see langword="true"/> = activa).
    /// </returns>
    public static Dictionary<Action<AbstractValidator<T>>, bool> GetPersonaBaseRules<T>() where T : Persona
    {
        return new Dictionary<Action<AbstractValidator<T>>, bool>
        {
            { v => v.RuleFor(p => p.Name).NotEmpty().WithMessage("El nombre es requerido"), true },
            { v => v.RuleFor(p => p.Name).MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres"), true },
            { v => v.RuleFor(p => p.Age).GreaterThan(0).WithMessage("La edad debe ser mayor a 0"), true },
            { v => v.RuleFor(p => p.Age).LessThan(110).WithMessage("La edad debe ser menor a 110"), true }
        };
    }

    /// <summary>
    /// Crea una instancia de <see cref="DynamicValidator{T}"/> configurada con las reglas base
    /// de validación para <see cref="Persona"/>.
    /// </summary>
    /// <returns>
    /// Una nueva instancia de <see cref="DynamicValidator{T}"/> de tipo <see cref="Persona"/>.
    /// </returns>
    public static DynamicValidator<Persona> Create()
    {
        return new DynamicValidator<Persona>(GetPersonaBaseRules<Persona>());
    }
}
