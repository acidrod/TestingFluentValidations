using ConsoleUI.Enums;
using ConsoleUI.Models;
using FluentValidation;

namespace ConsoleUI.Validators;

/// <summary>
/// Fábrica estática encargada de crear validadores para la entidad <see cref="Empleado"/>.
/// </summary>
/// <remarks>
/// <para>
/// Combina las reglas base heredadas de <see cref="PersonaValidatorFactory.GetPersonaBaseRules{T}"/>
/// con reglas específicas del dominio de un empleado, incluyendo validaciones sobre:
/// </para>
/// <list type="bullet">
///   <item><description>Edad mínima de 18 años.</description></item>
///   <item><description>Fecha de contratación requerida y no futura.</description></item>
///   <item><description>Sueldo mínimo configurable.</description></item>
///   <item><description>Porcentaje de retención acorde al rango salarial.</description></item>
///   <item><description>Incentivo correspondiente a la <see cref="Sucursal"/> asignada.</description></item>
///   <item><description>Horas diarias según si el empleado está afecto al Artículo 22.</description></item>
/// </list>
/// </remarks>
public static class EmpleadoValidatorFactory
{
    /// <summary>
    /// Crea una instancia de <see cref="DynamicValidator{T}"/> configurada con todas las reglas
    /// de validación aplicables a un <see cref="Empleado"/>, incluyendo las reglas base de
    /// <see cref="Persona"/> y las reglas específicas del empleado.
    /// </summary>
    /// <returns>
    /// Una nueva instancia de <see cref="DynamicValidator{T}"/> de tipo <see cref="Empleado"/>.
    /// </returns>
    public static DynamicValidator<Empleado> Create()
    {
        // Configuraciones basicas para un Empleado
        var sueldoMinimo = 1000m;

        const bool Enabled = true;
        const bool Disabled = false;

        // Obtener reglas base de Persona
        var rulesToApply = PersonaValidatorFactory.GetPersonaBaseRules<Empleado>();

        // Agregar regla específica: empleado debe ser mayor de 18
        rulesToApply.Add(
            v => v.RuleFor(e => e.Age)
                .GreaterThanOrEqualTo(18)
                .WithMessage("El empleado debe ser mayor de 18 años"),
            Enabled
        );

        // Reglas específicas de Empleado
        rulesToApply.Add(
            v => v.RuleFor(e => e.FechaContratacion)
                .NotEmpty()
                .WithMessage("La fecha de contratación es requerida"),
            Enabled
        );
        rulesToApply.Add(
            v => v.RuleFor(e => e.FechaContratacion)
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("La fecha no puede ser futura"),
            Enabled
        );
        rulesToApply.Add(
            v => v.RuleFor(e => e.Sueldo)
                .GreaterThanOrEqualTo(sueldoMinimo)
                .WithMessage($"El sueldo debe ser como mínimo de {sueldoMinimo}"),
            Enabled
        );
        rulesToApply.Add(
            v => v.RuleFor(e => e.PorcentajeRetencion)
                .InclusiveBetween(0, 100)
                .WithMessage("El porcentaje debe estar entre 0 y 100"),
            Enabled
        );
        rulesToApply.Add(
            v => v.RuleFor(e => e)
                .Must(TenerRetencionValidaSegunSueldo)
                .WithName("PorcentajeRetencion")
                .WithMessage("El porcentaje de retención no corresponde al sueldo del empleado"),
            Enabled
        );

        // Validacion de incentivos según la sucursal
        rulesToApply.Add(
            v => v.RuleFor(e => e.Incentivo)
                .Must((empleado, incentivo) => TenerIncentivoValidoSegunSucursal(empleado))
                .WithName("Incentivo")
                .WithMessage(e => $"El incentivo debe ser {ObtenerIncentivoEsperado(e.Sucursal)} para una sucursal en la zona {e.Sucursal}"),
            Enabled
        );

        // Empleados NO afectos al Art. 22: mínimo 8 horas, máximo 12
        rulesToApply.Add(
            v => v.RuleFor(e => e.HorasDiarias)
            .InclusiveBetween(8,12)
            .WithMessage("El empleado debe trabajar entre 8 y 12 horas diarias")
            .When(e => !e.AfectoArticulo22), // Solo aplica cuando AfectoArticulo22 es FALSE
            Enabled
        );

        // Empleados afectos al Art. 22: sin mínimo, pero máximo 24 (validación básica)
        rulesToApply.Add(
            v => v.RuleFor(e => e.HorasDiarias)
                .InclusiveBetween(0, 24)
                .WithMessage("Las horas deben estar entre 0 y 24")
                .When(e => e.AfectoArticulo22),
            Enabled
        );

        return new DynamicValidator<Empleado>(rulesToApply);
    }

    /// <summary>
    /// Determina si el porcentaje de retención del empleado corresponde al esperado según su sueldo.
    /// </summary>
    /// <param name="empleado">Instancia de <see cref="Empleado"/> a evaluar.</param>
    /// <returns>
    /// <see langword="true"/> si el porcentaje de retención coincide con el esperado;
    /// <see langword="false"/> en caso contrario.
    /// </returns>
    private static bool TenerRetencionValidaSegunSueldo(Empleado empleado)
    {
        var porcentajeEsperado = ObtenerPorcentajeRetencionEsperado(empleado.Sueldo);
        return empleado.PorcentajeRetencion == porcentajeEsperado;
    }

    /// <summary>
    /// Obtiene el porcentaje de retención esperado según el rango salarial.
    /// </summary>
    /// <remarks>
    /// Los rangos definidos son:
    /// <list type="bullet">
    ///   <item><description>$1.000 – $4.000: 5%</description></item>
    ///   <item><description>$4.001 – $6.000: 7%</description></item>
    ///   <item><description>Más de $6.000: 10%</description></item>
    ///   <item><description>Cualquier otro valor: 0%</description></item>
    /// </list>
    /// </remarks>
    /// <param name="sueldo">Sueldo bruto del empleado.</param>
    /// <returns>Porcentaje de retención esperado.</returns>
    private static decimal ObtenerPorcentajeRetencionEsperado(decimal sueldo)
    {
        return sueldo switch
        {
            >= 1000m and <= 4000m => 5m,
            >= 4001m and <= 6000m => 7m,
            > 6000m => 10m,
            _ => 0m
        };
    }

    /// <summary>
    /// Determina si el incentivo del empleado corresponde al esperado según su sucursal.
    /// </summary>
    /// <param name="empleado">Instancia de <see cref="Empleado"/> a evaluar.</param>
    /// <returns>
    /// <see langword="true"/> si el incentivo coincide con el esperado para la sucursal;
    /// <see langword="false"/> en caso contrario.
    /// </returns>
    private static bool TenerIncentivoValidoSegunSucursal(Empleado empleado)
    {
        var incentivoEsperado = ObtenerIncentivoEsperado(empleado.Sucursal);
        return empleado.Incentivo == incentivoEsperado;
    }

    /// <summary>
    /// Obtiene el monto de incentivo esperado según la sucursal asignada.
    /// </summary>
    /// <remarks>
    /// Los incentivos por sucursal son:
    /// <list type="bullet">
    ///   <item><description><see cref="Sucursal.Norte"/>: $500</description></item>
    ///   <item><description><see cref="Sucursal.Sur"/>: $700</description></item>
    ///   <item><description>Cualquier otra (<see cref="Sucursal.Central"/>): $200</description></item>
    /// </list>
    /// </remarks>
    /// <param name="sucursal">Sucursal del empleado.</param>
    /// <returns>Monto de incentivo esperado.</returns>
    private static decimal ObtenerIncentivoEsperado(Sucursal sucursal)
    {
        return sucursal switch
        {
            Sucursal.Norte => 500m,
            Sucursal.Sur => 700m,
            _ => 200m
        };
    }
}
