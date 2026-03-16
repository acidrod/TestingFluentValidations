# TestingFluentValidations

> ⚠️ **AVISO IMPORTANTE:** Este proyecto es **únicamente una prueba de concepto (proof of concept)**. No es una aplicación completa ni está diseñada para producción. El objetivo es explorar validación dinámica con FluentValidation en .NET.

---

## Resumen

Este repositorio contiene una app de consola en C# que implementa una arquitectura de validación dinámica y reutilizable con [FluentValidation](https://docs.fluentvalidation.net/). Las reglas se definen por tipo de modelo y pueden habilitarse o deshabilitarse con un diccionario de `bool`.

---

## Tecnologías utilizadas

- **.NET 9.0**
- **C# 12**
- **FluentValidation 12.1.1**

---

## Estructura del proyecto

```
TestingFluentValidations/
├── README.md
├── ConsoleUI/
│   ├── .gitignore              # Ignorados clásicos de VS Code/.NET para este proyecto
│   ├── Program.cs              # Punto de entrada, demuestra el uso de los validadores
│   ├── Persona.cs              # Modelo base (Name, Age)
│   ├── Empleado.cs             # Modelo derivado (FechaContratacion, Sueldo, PorcentajeRetencion)
│   ├── DynamicValidator.cs     # Validador genérico con reglas activables/desactivables
│   └── ValidatorRegistry.cs    # Registro central de reglas por tipo + reglas de negocio
└── TestingFluentValidations.slnx
```

---

## Conceptos demostrados

### 1. Validador Dinámico (`DynamicValidator<T>`)
`DynamicValidator<T>` recibe un diccionario de reglas con la forma `Dictionary<Action<AbstractValidator<T>>, bool>`. Solo ejecuta las reglas marcadas en `true`.

```csharp
// Ejemplo: la regla solo se aplica si el valor es true.
new Dictionary<Action<AbstractValidator<T>>, bool>
{
    { regla1, true },
    { regla2, false },
}
```

### 2. Registro de Validadores (`ValidatorRegistry`)
`ValidatorRegistry` actúa como punto central. Según el tipo (`Persona`, `Empleado`, etc.), arma el conjunto de reglas y devuelve el validador adecuado.

### 3. Regla de negocio compuesta (Sueldo + Retención)
Además de reglas simples por campo, el validador de `Empleado` incluye una regla compuesta que valida el `% de retención` según el sueldo:

- `1500` a `4000` -> `5%`
- `4001` a `6000` -> `7%`
- Mayor a `6000` -> `10%`

Esta validación se aplica sobre el objeto completo (`RuleFor(e => e).Must(...)`) porque depende de más de una propiedad.

### 4. Herencia de modelos
- `Empleado` hereda de `Persona`.
- Comparte validaciones base (por ejemplo `Name`, `Age`) y agrega reglas específicas.

### 5. Patrones aplicados
- **Registry/Factory**: resolución de validador por tipo.
- **Programación genérica**: un único `DynamicValidator<T>` para cualquier entidad.
- **Reglas activables**: encendido/apagado por bandera booleana para POC y pruebas.

---

## ¿Qué NO incluye este proyecto?

Este proyecto **no incluye** ni pretende incluir:

- Interfaz de usuario real (web, escritorio o móvil)
- Persistencia de datos (base de datos, archivos, etc.)
- Autenticación o autorización
- API REST o cualquier tipo de servicio expuesto
- Manejo de errores para producción
- Pruebas unitarias o de integración

---

## ¿Cómo ejecutarlo?

```bash
dotnet build
dotnet run --project ConsoleUI/ConsoleUI.csproj
```

La app valida instancias de `Persona` y `Empleado` e imprime en consola si son válidas.

## Estado actual de la POC

- Validación dinámica por tipo funcionando.
- `DynamicValidator<T>` consolidado (sin duplicar validadores por entidad).
- Regla de retención por tramos de sueldo implementada en `Empleado`.
- `.gitignore` agregado en `ConsoleUI/` con ignores clásicos de VS Code/.NET.

---

## Objetivo

Servir como base de exploración para diseñar validaciones flexibles en escenarios con múltiples modelos de negocio (por ejemplo, dominios contables con reglas distintas por entidad o región).
