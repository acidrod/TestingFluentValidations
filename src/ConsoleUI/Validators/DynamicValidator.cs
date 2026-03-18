using FluentValidation;

namespace ConsoleUI.Validators;

/// <summary>
/// Validador genérico dinámico basado en <see cref="AbstractValidator{T}"/> de FluentValidation
/// que permite construir reglas de validación en tiempo de ejecución para cualquier tipo de entidad.
/// </summary>
/// <remarks>
/// <para>
/// Recibe un diccionario donde cada clave es una <see cref="Action{T}"/> de tipo
/// <c>Action&lt;AbstractValidator&lt;T&gt;&gt;</c> que define una regla de validación,
/// y el valor <see cref="bool"/> indica si la regla debe aplicarse (<see langword="true"/>)
/// u omitirse (<see langword="false"/>).
/// </para>
/// <para>
/// Está configurado con <see cref="CascadeMode.Continue"/> tanto a nivel de regla como de clase,
/// lo que garantiza que todas las reglas se evalúen incluso si alguna falla previamente.
/// </para>
/// </remarks>
/// <typeparam name="T">Tipo de entidad a validar. Debe ser una clase (<see langword="class"/>).</typeparam>
/// <example>
/// <code>
/// var rules = new Dictionary&lt;Action&lt;AbstractValidator&lt;Persona&gt;&gt;, bool&gt;
/// {
///     { v => v.RuleFor(p => p.Name).NotEmpty(), true },
///     { v => v.RuleFor(p => p.Age).GreaterThan(0), true }
/// };
/// var validator = new DynamicValidator&lt;Persona&gt;(rules);
/// </code>
/// </example>
public sealed class DynamicValidator<T> : AbstractValidator<T> where T : class
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="DynamicValidator{T}"/> aplicando únicamente
    /// las reglas marcadas como activas (<see langword="true"/>) en el diccionario proporcionado.
    /// </summary>
    /// <param name="rulesToApply">
    /// Diccionario de reglas de validación donde la clave es la acción que registra la regla
    /// y el valor indica si debe habilitarse (<see langword="true"/>) o ignorarse (<see langword="false"/>).
    /// </param>
    public DynamicValidator(Dictionary<Action<AbstractValidator<T>>, bool> rulesToApply)
    {
        // Continuar validando todas las reglas aunque alguna falle
        RuleLevelCascadeMode = CascadeMode.Continue;
        ClassLevelCascadeMode = CascadeMode.Continue;

        foreach (var rule in rulesToApply.Where(r => r.Value))
        {
            rule.Key.Invoke(this);
        }
    }
}