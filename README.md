
# FunEvents - Sistema de Reserva de Entradas para Eventos

Implementación de solución de reservas de boletas para espectáculos desarrollada bajo arquitectura de microservicios / aplicaciones distribuidas modernas en **.NET 8**, implementando los patrones de diseño **Clean Architecture**, **Domain-Driven Design (DDD)**, **Repository Pattern** y **Unit of Work**, orquestado mediante **.NET Aspire** y respaldado por una base de datos relacional **PostgreSQL**.

---

## 🏛️ Arquitectura del Sistema

El proyecto sigue una estricta separación de responsabilidades en capas (*Clean Architecture*) y principios de diseño guiado por el dominio (*DDD*):

```
FunEvents/
├── src/
│   ├── Core/
│   │   ├── FunEvents.Domain/         # Entidades del Dominio, Enums, Reglas de Negocio
│   │   └── FunEvents.Application/    # Casos de Uso, DTOs, Interfaces, DTOs de Validaciones
│   ├── Infrastructure/
│   │   └── FunEvents.Infrastructure/ # DbContext, Migraciones EF Core, Repositorios, UnitOfWork
│   ├── Presentation/
│   │   ├── FunEvents.API/            # Minimal APIs, Middlewares, Handlers Excepciones
│   │   └── FunEvents.ConsoleApp/     # Cliente HTTP interactivo para pruebas de alta concurrencia
│   └── Orchestration/
│       ├── FunEvents.AppHost/        # Orquestador .NET Aspire (PostgreSQL + Containers + Apps)
│       └── FunEvents.ServiceDefaults/# Configuración de Telemetría (OpenTelemetry, HealthChecks)
```

---

## 🚀 Requisitos Previos

Asegúrate de contar con las siguientes herramientas instaladas en tu entorno de desarrollo:

1. **.NET 8 SDK** (v8.0.x o superior).
2. **Docker Desktop** (Debe estar abierto y ejecutándose para levantar los contenedores de PostgreSQL y PgAdmin gestionados por Aspire).
3. **Visual Studio 2022** (v17.8+ con la carga de trabajo de *.NET Aspire*) o **VS Code / JetBrains Rider** con la CLI de .NET.

---

## 🛠️ Instrucciones de Ejecución

### Opción 1: Visual Studio 2022 (Recomendado)

1. Abre el proyecto **`Docker Desktop`** en tu sistema.
2. Abre la solución `FunEvents.sln` en Visual Studio 2022.
3. Establece el proyecto **`FunEvents.AppHost`** como proyecto de inicio (*Startup Project*).
4. Presiona **`F5`** (o el botón **Iniciar**).
5. Se abrirá automáticamente el **Dashboard de .NET Aspire** (`https://localhost:17125`) desde donde podrás monitorear logs, trazas de OpenTelemetry, métricas y los endpoints expuestos.

### Opción 2: Línea de Comandos (CLI de .NET)

1. Asegúrate de tener Docker Desktop corriendo.
2. Abre una terminal en la raíz de la solución.
3. Ejecuta el orquestador Aspire:
   ```bash
   dotnet run --project src/Orchestration/FunEvents.AppHost/FunEvents.AppHost.csproj
   ```
4. Accede al enlace del Dashboard de Aspire indicado en la consola.

---

## 📌 Sembrado de Datos de Prueba (Seed)

Para garantizar la consistencia en las pruebas de reserva de entradas, la API incluye un endpoint dedicado para sembrar el evento de prueba con cupo predefinido (`100` entradas).

- **Endpoint HTTP:** `POST /api/eventos/seed`
- **ID del Evento de Prueba:** `11111111-1111-1111-1111-111111111111`

> 💡 **Nota:** La aplicación cliente de consola `FunEvents.ConsoleApp` ejecuta automáticamente una llamada al endpoint de Seed al iniciar antes de desplegar el menú de opciones o ejecutar las pruebas concurrentes.

---

## 🧪 Ejecución de Pruebas Concurrentes

El proyecto incluye un cliente de consola (`FunEvents.ConsoleApp`) diseñado para simular escenarios de alta concurrencia y validar la consistencia transaccional y el manejo de cupos en la base de datos PostgreSQL.

### Pasos para probar concurrencia:
1. Con `FunEvents.AppHost` en ejecución, abre la consola interactiva asignada al recurso **`consoleclient`** en el Dashboard de Aspire (o ejecuta directamente el proyecto de consola).
2. Selecciona la **Opción 2: Prueba de Alta Concurrencia (Simulación de estrés)**.
3. Especifica la cantidad de hilos simultáneos (ejemplo: `50` o `100` peticiones en paralelo).
4. El cliente ejecutará las peticiones concurrentes contra `FunEvents.API` y presentará un resumen con:
   - Reservas exitosas.
   - Reservas rechazadas por agotamiento de cupo o validación.
   - Estado final del stock en la base de datos.

---

## 🗄️ Monitoreo e Inspección de la Base de Datos

La base de datos PostgreSQL es gestionada dinámicamente en un contenedor de Docker a través de .NET Aspire:

- **PgAdmin Integrado:** En el Dashboard de Aspire, localiza el recurso `pgadmin` y haz clic en su enlace de la columna **Endpoints** para acceder a la interfaz web visual de la base de datos.
- **DBeaver / Clientes Externos:** Revisa el puerto asignado dinámicamente al recurso `postgres` en Aspire y conéctate usando:
  - **Host:** `localhost`
  - **Port:** *(Puerto expuesto en el Dashboard de Aspire)*
  - **Database:** `funeventsdb`
  - **User:** `postgres`
