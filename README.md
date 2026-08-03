# MoonBrew Coffee

Sistema web para la administración y atención al cliente de MoonBrew Coffee, desarrollado con ASP.NET Core MVC y una arquitectura por capas.

## Estructura

- `MoonBrewCoffee.Web`: interfaz web, controladores, vistas y recursos estáticos.
- `MoonBrewCoffee.Application`: servicios, DTO y reglas de aplicación.
- `MoonBrewCoffee.Infrastructure`: acceso a datos, entidades y repositorios.
- `Database`: scripts necesarios para preparar la base de datos.

## Requisitos

- .NET 8 SDK
- SQL Server
- Visual Studio 2022 o una herramienta compatible con proyectos .NET 8

## Inicio rápido

1. Clona el repositorio.
2. Ejecuta el script incluido en `Database` sobre SQL Server.
3. Verifica la conexión `DefaultConnection` de `MoonBrewCoffee.Web/appsettings.json`.
4. Abre `MoonBrewCoffee.sln`.
5. Establece `MoonBrewCoffee.Web` como proyecto de inicio y ejecuta la aplicación.

## Autoras

- Fiorella Solórzano
- Yendry Cisneros
