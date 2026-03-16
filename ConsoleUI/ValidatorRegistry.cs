using FluentValidation;

namespace ConsoleUI;

/// <summary>
/// Registro central que mapea cada tipo de entidad a sus reglas de validación específicas.
/// Actúa como un diccionario de "orden mayor" que decide qué reglas usar según el tipo de clase.
/// </summary>
public static class ValidatorRegistry
{
    public static AbstractValidator<T> GetValidator<T>() where T : class
    {
        return typeof(T).Name switch
        {
            nameof(Persona) => (AbstractValidator<T>)(object)CreatePersonaValidator(),
            nameof(Empleado) => (AbstractValidator<T>)(object)CreateEmpleadoValidator(),
            _ => throw new InvalidOperationException($"No validator registered for type {typeof(T).Name}")
        };
    }

    private static DynamicValidator<Persona> CreatePersonaValidator()
    {
        var rulesToApply = new Dictionary<Action<AbstractValidator<Persona>>, bool>
        {
            { v => v.RuleFor(p => p.Name).NotEmpty().WithMessage("El nombre es requerido"), true },
            { v => v.RuleFor(p => p.Name).MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres"), true },
            { v => v.RuleFor(p => p.Age).GreaterThan(0).WithMessage("La edad debe ser mayor a 0"), true },
            { v => v.RuleFor(p => p.Age).LessThan(150).WithMessage("La edad debe ser menor a 150"), false }
        };
        return new DynamicValidator<Persona>(rulesToApply);
    }

    private static DynamicValidator<Empleado> CreateEmpleadoValidator()
    {
        var rulesToApply = new Dictionary<Action<AbstractValidator<Empleado>>, bool>
        {
            // Reglas compartidas con Persona
            { v => v.RuleFor(e => e.Name).NotEmpty().WithMessage("El nombre es requerido"), true },
            { v => v.RuleFor(e => e.Name).MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres"), true },
            { v => v.RuleFor(e => e.Age).GreaterThan(18).WithMessage("El empleado debe ser mayor de 18 años"), true },
            
            // Reglas específicas de Empleado
            { v => v.RuleFor(e => e.FechaContratacion).NotEmpty().WithMessage("La fecha de contratación es requerida"), true },
            { v => v.RuleFor(e => e.FechaContratacion).LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha no puede ser futura"), true },
            { v => v.RuleFor(e => e.Sueldo).GreaterThan(0).WithMessage("El sueldo debe ser mayor a 0"), true },
            { v => v.RuleFor(e => e.PorcentajeRetencion).InclusiveBetween(0, 100).WithMessage("El porcentaje debe estar entre 0 y 100"), true }
        };
        return new DynamicValidator<Empleado>(rulesToApply);
    }
}
