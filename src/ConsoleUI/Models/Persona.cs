namespace ConsoleUI.Models;

/// <summary>
/// Representa una persona con información básica de identificación.
/// </summary>
public class Persona
{
    /// <summary>
    /// Obtiene o establece el nombre de la persona.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Obtiene o establece la edad de la persona en años.
    /// </summary>
    /// <value>El valor predeterminado es 0.</value>
    public int Age { get; set; } = 0;
}
