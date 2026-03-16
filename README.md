# TestingFluentValidations

> ⚠️ **AVISO IMPORTANTE:** Este proyecto es **únicamente una prueba de concepto (proof of concept)**. No es una aplicación completa ni está diseñada para producción. El objetivo es explorar validación dinámica con FluentValidation en .NET.
> ⚠️ **IMPORTANT NOTICE:** This project is **only a proof of concept**. It is not a complete application and is not designed for production use. Its purpose is to explore dynamic validation with FluentValidation in .NET.

---

## Documentacion en Espanol

### Resumen

Este repositorio contiene una app de consola en C# que implementa una arquitectura de validación dinámica y reutilizable con [FluentValidation](https://docs.fluentvalidation.net/). Las reglas se definen por tipo de modelo y pueden habilitarse o deshabilitarse con un diccionario de `bool`.

---

### Tecnologias utilizadas

- **.NET 9.0**
- **C# 12**
- **FluentValidation 12.1.1**
- **xUnit**
- **GitHub Actions**

---

### Estructura del proyecto

```text
TestingFluentValidations/
├── README.md
├── .github/
│   └── workflows/
│       └── ci.yml              # CI que compila y ejecuta pruebas unitarias
├── src/
│   └── ConsoleUI/
│       ├── .gitignore          # Ignorados clásicos de VS Code/.NET para este proyecto
│       ├── Program.cs          # Punto de entrada, demuestra el uso de los validadores
│       ├── Persona.cs          # Modelo base (Name, Age)
│       ├── Empleado.cs         # Modelo derivado (FechaContratacion, Sueldo, PorcentajeRetencion)
│       ├── DynamicValidator.cs # Validador genérico con reglas activables/desactivables
│       └── ValidatorRegistry.cs # Registro central de reglas por tipo + reglas de negocio
├── tests/
│   └── ConsoleUI.Tests/
│       └── ValidatorTests/
│           └── ValidatorRegistryTests.cs  # Pruebas unitarias con xUnit
└── TestingFluentValidations.slnx
```

---

### Conceptos demostrados

#### 1. Validador Dinamico (`DynamicValidator<T>`)

`DynamicValidator<T>` recibe un diccionario de reglas con la forma `Dictionary<Action<AbstractValidator<T>>, bool>`. Solo ejecuta las reglas marcadas en `true`.

```csharp
// Ejemplo: la regla solo se aplica si el valor es true.
new Dictionary<Action<AbstractValidator<T>>, bool>
{
    { regla1, true },
    { regla2, false },
}
```

#### 2. Registro de validadores (`ValidatorRegistry`)

`ValidatorRegistry` actúa como punto central. Según el tipo (`Persona`, `Empleado`, etc.), arma el conjunto de reglas y devuelve el validador adecuado.

#### 3. Regla de negocio compuesta (Sueldo + Retencion)

Además de reglas simples por campo, el validador de `Empleado` incluye una regla compuesta que valida el `% de retención` según el sueldo:

- `1500` a `4000` -> `5%`
- `4001` a `6000` -> `7%`
- Mayor a `6000` -> `10%`

Esta validación se aplica sobre el objeto completo (`RuleFor(e => e).Must(...)`) porque depende de más de una propiedad.

#### 4. Herencia de modelos

- `Empleado` hereda de `Persona`.
- Comparte validaciones base (por ejemplo `Name`, `Age`) y agrega reglas específicas.

#### 5. Patrones aplicados

- **Registry/Factory**: resolución de validador por tipo.
- **Programación genérica**: un único `DynamicValidator<T>` para cualquier entidad.
- **Reglas activables**: encendido/apagado por bandera booleana para POC y pruebas.

---

### ¿Que NO incluye este proyecto?

Este proyecto **no incluye** ni pretende incluir:

- Interfaz de usuario real (web, escritorio o móvil)
- Persistencia de datos (base de datos, archivos, etc.)
- Autenticación o autorización
- API REST o cualquier tipo de servicio expuesto
- Manejo de errores para producción

---

### ¿Como ejecutarlo?

```bash
dotnet build
dotnet run --project src/ConsoleUI/ConsoleUI.csproj
```

La app valida instancias de `Persona` y `Empleado` e imprime en consola si son válidas.

### ¿Como ejecutar las pruebas?

```bash
dotnet test TestingFluentValidations.slnx
```

### Integracion continua

El repositorio incluye una workflow de GitHub Actions en `.github/workflows/ci.yml` que:

- Restaura dependencias
- Compila la solución completa
- Ejecuta las pruebas unitarias
- Publica los resultados de prueba como artifact `.trx`

### Estado actual de la POC

- Validación dinámica por tipo funcionando.
- `DynamicValidator<T>` consolidado (sin duplicar validadores por entidad).
- Regla de retención por tramos de sueldo implementada en `Empleado`.
- Estructura reorganizada en carpetas `src/` y `tests/`.
- Pruebas unitarias con xUnit para `ValidatorRegistry` y `Program`.
- Solución y pruebas alineadas en `.NET 9.0`.
- CI configurada para compilar y probar automáticamente en GitHub Actions.

---

### Objetivo

Servir como base de exploración para diseñar validaciones flexibles en escenarios con múltiples modelos de negocio (por ejemplo, dominios contables con reglas distintas por entidad o región).

---

## English Documentation

### Summary

This repository contains a C# console application that implements a reusable dynamic validation architecture with [FluentValidation](https://docs.fluentvalidation.net/). Rules are defined per model type and can be enabled or disabled with a `bool` dictionary.

---

### Technologies used

- **.NET 9.0**
- **C# 12**
- **FluentValidation 12.1.1**
- **xUnit**
- **GitHub Actions**

---

### Project structure

```text
TestingFluentValidations/
├── README.md
├── .github/
│   └── workflows/
│       └── ci.yml              # CI that builds the solution and runs unit tests
├── src/
│   └── ConsoleUI/
│       ├── .gitignore          # Standard VS Code/.NET ignores for this project
│       ├── Program.cs          # Entry point showing validator usage
│       ├── Persona.cs          # Base model (Name, Age)
│       ├── Empleado.cs         # Derived model (HireDate, Salary, WithholdingRate)
│       ├── DynamicValidator.cs # Generic validator with enable/disable rule flags
│       └── ValidatorRegistry.cs # Central registry of rules per type + business rules
├── tests/
│   └── ConsoleUI.Tests/
│       └── ValidatorTests/
│           └── ValidatorRegistryTests.cs  # xUnit unit tests
└── TestingFluentValidations.slnx
```

---

### Concepts demonstrated

#### 1. Dynamic validator (`DynamicValidator<T>`)

`DynamicValidator<T>` receives a dictionary of rules in the form `Dictionary<Action<AbstractValidator<T>>, bool>`. Only rules marked as `true` are applied.

```csharp
// Example: the rule is applied only if the flag is true.
new Dictionary<Action<AbstractValidator<T>>, bool>
{
    { rule1, true },
    { rule2, false },
}
```

#### 2. Validator registry (`ValidatorRegistry`)

`ValidatorRegistry` acts as the central point for validator resolution. Based on the type (`Persona`, `Empleado`, and so on), it builds the rule set and returns the proper validator.

#### 3. Composite business rule (salary + withholding)

In addition to simple property rules, the `Empleado` validator includes a composite rule that validates the withholding percentage based on salary:

- `1500` to `4000` -> `5%`
- `4001` to `6000` -> `7%`
- Greater than `6000` -> `10%`

This validation is applied to the entire object (`RuleFor(e => e).Must(...)`) because it depends on more than one property.

#### 4. Model inheritance

- `Empleado` inherits from `Persona`.
- It reuses base validations such as `Name` and `Age`, then adds employee-specific rules.

#### 5. Applied patterns

- **Registry/Factory**: validator resolution by type.
- **Generic programming**: one `DynamicValidator<T>` for multiple entities.
- **Toggleable rules**: rules can be turned on or off for experimentation and testing.

---

### What this project does NOT include

This project does **not** include and does not aim to include:

- A real user interface (web, desktop, or mobile)
- Data persistence (database, files, and so on)
- Authentication or authorization
- A REST API or any exposed service layer
- Production-grade error handling

---

### How to run it

```bash
dotnet build
dotnet run --project src/ConsoleUI/ConsoleUI.csproj
```

The console app validates `Persona` and `Empleado` instances and prints whether they are valid.

### How to run the tests

```bash
dotnet test TestingFluentValidations.slnx
```

### Continuous integration

The repository includes a GitHub Actions workflow in `.github/workflows/ci.yml` that:

- Restores dependencies
- Builds the full solution
- Runs unit tests
- Publishes test results as `.trx` artifacts

### Current POC status

- Dynamic validation by type is working.
- `DynamicValidator<T>` is consolidated without duplicating validators per entity.
- The salary-band withholding rule is implemented for `Empleado`.
- The repository structure has been reorganized into `src/` and `tests/` folders.
- xUnit unit tests cover `ValidatorRegistry` and `Program`.
- The solution and tests are aligned on `.NET 9.0`.
- CI is configured to build and test automatically in GitHub Actions.

---

### Goal

To serve as an exploration baseline for designing flexible validation strategies in scenarios with multiple business models, such as accounting or domain-heavy systems with different rules per entity or region.
