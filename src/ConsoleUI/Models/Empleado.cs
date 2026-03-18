using ConsoleUI.Enums;

namespace ConsoleUI.Models;

/// <summary>
/// Representa un empleado de la organización, extendiendo la información básica de <see cref="Persona"/>.
/// </summary>
public class Empleado : Persona
{
    /// <summary>
    /// Obtiene o establece la sucursal donde trabaja el empleado.
    /// </summary>
    public Sucursal Sucursal { get; set; }

    /// <summary>
    /// Obtiene o establece el incentivo adicional del empleado.
    /// </summary>
    /// <value>El valor predeterminado es 200 para incentivar a los empleados a trabajar en sucursales menos populares.</value>
    public decimal Incentivo { get; set; } = 200m;

    /// <summary>
    /// Obtiene o establece la fecha en que el empleado fue contratado.
    /// </summary>
    public DateTime FechaContratacion { get; set; }

    /// <summary>
    /// Obtiene o establece el sueldo bruto del empleado.
    /// </summary>
    public decimal Sueldo { get; set; }

    /// <summary>
    /// Obtiene o establece el porcentaje de retención aplicado al sueldo del empleado.
    /// </summary>
    public decimal PorcentajeRetencion { get; set; }

    /// <summary>
    /// Obtiene el sueldo neto del empleado después de aplicar las retenciones y sumar el incentivo.
    /// </summary>
    /// <remarks>
    /// Se calcula como: Sueldo - (Sueldo * PorcentajeRetencion / 100) + Incentivo
    /// </remarks>
    public decimal SueldoNeto => Sueldo - Sueldo * PorcentajeRetencion / 100 + Incentivo;

    /// <summary>
    /// Indica si el empleado está afecto al Artículo 22 (sin límite de jornada laboral).
    /// </summary>
    public bool AfectoArticulo22 { get; set; }

    /// <summary>
    /// Horas diarias trabajadas por el empleado.
    /// </summary>
    public int HorasDiarias { get; set; }
}