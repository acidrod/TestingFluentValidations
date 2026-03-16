namespace ConsoleUI;

public static class Program
{
    public static void Main(string[] args)
    { 
        // Validacion de ejemplo con Persona
        var person = new Persona { Name = "Alice", Age = 30 };

        // Obtener el validador específico para Persona desde el registro central
        var personValidator = ValidatorRegistry.GetValidator<Persona>();

        // Validar la instancia de Persona usando el validador obtenido
        var resultPersona = personValidator.Validate(person);
        Console.WriteLine($"Persona válida: {resultPersona.IsValid}");

        // Validacion de ejemplo con Empleado
        var employee = new Empleado 
        { 
            Name = "Bob", 
            Age = 25, 
            FechaContratacion = DateTime.Now, 
            Sueldo = 5000, 
            PorcentajeRetencion = 5 
        };

        // Obtener el validador específico para Empleado desde el registro central
        var employeeValidator = ValidatorRegistry.GetValidator<Empleado>();
        
        // Validar la instancia de Empleado usando el validador obtenido
        var resultEmpleado = employeeValidator.Validate(employee);
        Console.WriteLine($"Empleado válido: {resultEmpleado.IsValid}");
    }
}
