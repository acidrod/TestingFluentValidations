# TestingFluentValidations

> ⚠️ **AVISO IMPORTANTE:** Este proyecto es **ÚNICAMENTE una prueba de concepto (proof of concept)**. No es una aplicación completa, ni está diseñada para ser usada en producción. Su propósito es explorar y demostrar patrones de validación dinámica con FluentValidation en .NET.

---

## ¿De qué trata este proyecto?

Este repositorio contiene una aplicación de consola en C# que explora cómo implementar **validaciones dinámicas y reutilizables** utilizando la librería [FluentValidation](https://docs.fluentvalidation.net/). El foco principal es demostrar una arquitectura flexible de validación donde las reglas pueden activarse o desactivarse en tiempo de ejecución.

---

## Tecnologías utilizadas

- **.NET 9.0**
- **C# 12**
- **FluentValidation 12.1.1**

---

## Estructura del proyecto

```
TestingFluentValidations/
├── ConsoleUI/
│   ├── Program.cs              # Punto de entrada, demuestra el uso de los validadores
│   ├── Persona.cs              # Modelo base con propiedades Name y Age
│   ├── Empleado.cs             # Modelo derivado con propiedades adicionales (FechaContratacion, Sueldo, etc.)
│   ├── DynamicValidator.cs     # Validador genérico con reglas activables/desactivables
│   └── ValidatorRegistry.cs   # Registro central de validadores por tipo
└── TestingFluentValidations.slnx
```

---

## Conceptos demostrados

### 1. Validador Dinámico (`DynamicValidator<T>`)
Un validador genérico que recibe un diccionario de reglas, donde cada regla está asociada a un valor booleano que indica si debe aplicarse o no. Esto permite habilitar/deshabilitar reglas de validación en tiempo de ejecución sin modificar el código del validador.

```csharp
// Ejemplo: la regla solo se aplica si el valor es true
new Dictionary<Action<AbstractValidator<T>>, bool>
{
    { regla1, true },   // se aplica
    { regla2, false },  // ignorada
}
```

### 2. Registro de Validadores (`ValidatorRegistry`)
Una clase estática que actúa como fábrica centralizada. Dado un tipo genérico `T`, devuelve el validador adecuado para ese tipo. Esto desacopla la creación y configuración de los validadores del resto de la aplicación.

### 3. Herencia de modelos y validadores
- `Empleado` extiende `Persona`, heredando sus propiedades base (`Name`, `Age`).
- El validador de `Empleado` incluye todas las reglas de `Persona` más reglas adicionales específicas del empleado (edad mínima de 18, salario positivo, fecha de contratación válida, etc.).

### 4. Patrones de diseño aplicados
- **Strategy** – Selección dinámica de reglas mediante flags booleanos.
- **Registry / Factory** – Creación centralizada de validadores por tipo.
- **Programación genérica** – Un único `DynamicValidator<T>` funciona para cualquier clase.

---

## ¿Qué NO incluye este proyecto?

Este proyecto **no incluye** ni pretende incluir:

- Interfaz de usuario real (web, escritorio o móvil)
- Persistencia de datos (base de datos, archivos, etc.)
- Autenticación o autorización
- API REST o cualquier tipo de servicio expuesto
- Manejo de errores para producción
- Cobertura de pruebas unitarias o de integración

---

## ¿Cómo ejecutarlo?

```bash
cd ConsoleUI
dotnet run
```

La aplicación validará instancias de `Persona` y `Empleado` e imprimirá los resultados en consola.

---

## Objetivo

Este código nació como un espacio de exploración personal para entender cómo construir sistemas de validación flexibles y mantenibles usando FluentValidation. Sirve como referencia o punto de partida para quien quiera implementar algo similar en un proyecto real.
