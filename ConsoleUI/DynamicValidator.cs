using FluentValidation;

namespace ConsoleUI;

/// <summary>
/// Validador genérico dinámico que funciona para cualquier tipo de entidad.
/// Recibe un diccionario donde cada Action<AbstractValidator<T>> es una regla de validación
/// y el boolean indica si debe aplicarse (true) u omitirse (false).
/// </summary>
sealed class DynamicValidator<T> : AbstractValidator<T> where T : class
{
    public DynamicValidator(Dictionary<Action<AbstractValidator<T>>, bool> rulesToApply)
    {
        foreach (var rule in rulesToApply.Where(r => r.Value))
        {
            rule.Key.Invoke(this);
        }
    }
}