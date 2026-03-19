using ConsoleUI.Enums;
using ConsoleUI.Models;
using ConsoleUI.Validators;
using System.Security.Cryptography;
using System.Text;

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
            Age = 20,
            Sucursal = Sucursal.Sur,
            FechaContratacion = DateTime.Now, 
            Sueldo = 1000, 
            PorcentajeRetencion = 5,
            Incentivo = 700,
            AfectoArticulo22 = true,
            HorasDiarias = 3
        };

        // Obtener el validador específico para Empleado desde el registro central
        var employeeValidator = ValidatorRegistry.GetValidator<Empleado>();
        
        // Validar la instancia de Empleado usando el validador obtenido
        var resultEmpleado = employeeValidator.Validate(employee);

        if(!resultEmpleado.IsValid)
        {
            Console.WriteLine("Lista de errores:");
            foreach (var error in resultEmpleado.Errors)
            {
                Console.WriteLine($"\t Error en {error.PropertyName}: {error.ErrorMessage}");
            }
        }

        Console.WriteLine($"Empleado válido: {resultEmpleado.IsValid}");

        // Intencionalmente inseguro para validar que el analyzer reporte hallazgos.
        _ = InsecureHashForDemo("demo");
    }

    private static byte[] InsecureHashForDemo(string input)
    {
        using var sha1 = SHA1.Create();
        return sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
    }
}
