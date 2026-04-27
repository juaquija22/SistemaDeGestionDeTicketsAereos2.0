# Sistema de Gestión de Tiquetes Aéreos

## Descripción del Proyecto

Aplicación de **consola en C# (.NET)** para la **venta, reserva, consulta y administración de tiquetes aéreos**. El sistema permite registrar y gestionar aerolíneas, aeropuertos, rutas, vuelos, clientes, reservas, tiquetes y pagos, con la información persistida en **MySQL**. La interacción es 100 % por menús en consola (sin interfaz web ni móvil).

Enfoque **formativo y profesional**: modelado del dominio, **Entity Framework Core** como acceso a datos, **LINQ** para consultas y reportes, validaciones y reglas de negocio. Los pagos y ciertos estados se manejan de forma **simulada** (no hay pasarela de pago real ni integración con sistemas externos de aerolíneas).

---

## Características Destacadas

- Menús de consola con navegación por rol (**administrador** vs **cliente**).
- Persistencia en **MySQL** con **EF Core** (entidades, migraciones, repositorios).
- Módulos funcionales: aerolíneas, aeropuertos y destinos, vuelos y tarifas, clientes, reservas, tiquetes y check-in, pagos, tripulación, reportes.
- Control de **disponibilidad de asientos** por vuelo al crear o cancelar reservas.
- **Emisión de tiquetes** a partir de reservas en estado adecuado (reglas de negocio en casos de uso).
- **Reportes operativos** con LINQ (ocupación, disponibilidad, ingresos por aerolínea, reservas por estado, tiquetes por fechas, clientes con más reservas, destinos más solicitados, entre otros).
- Trazabilidad básica de estados (reservas, tiquetes, vuelos) mediante historiales donde aplica.
- Autenticación contra base de datos y **control de acceso por rol** (RBAC).

---

## Objetivo

**Objetivo general:** desarrollar una aplicación de consola en C# conectada a **MySQL** que permita administrar vuelos, clientes, reservas y emisión de tiquetes, usando **LINQ** para consultas, filtros, agrupaciones y procesamiento de datos.

**Objetivos específicos (resumen):**

- Modelar datos de aerolíneas, vuelos, rutas, clientes, reservas, tiquetes y pagos.
- Implementar consola con menús y submenús; conexión app–MySQL para guardar y consultar información.
- Registrar y administrar clientes, vuelos, destinos y reservas; emitir tiquetes desde reservas válidas.
- Controlar disponibilidad de asientos por vuelo; generar reportes con LINQ.
- Organizar el código por capas/módulos (presentación, aplicación, dominio, infraestructura), con validaciones y persistencia coherente del estado.

**Alcance (procesos cubiertos):** registro de aerolíneas; aeropuertos y destinos; creación de vuelos (origen, destino, fecha, horarios, capacidad, disponibilidad, estado); clientes; reservas (crear, confirmar/cancelar, cupos); emisión de tiquetes; consultas desde menú; reportes LINQ. **Fuera de alcance:** interfaz web/móvil, pasarelas de pago reales y APIs externas de aerolíneas.

---

## Tecnologías Utilizadas

| Tecnología | Uso en el proyecto |
|------------|-------------------|
| **C# / .NET** | Aplicación de consola ejecutable. |
| **MySQL 8** | Motor relacional (recomendado documentación oficial con EF Core / conector compatible). |
| **Entity Framework Core** | O/RM: `DbContext`, entidades, migraciones, repositorios. |
| **LINQ** | Consultas en casos de uso y sobre todo en el módulo de reportes (filtros, ordenamiento, agrupación, agregaciones). |
| **Pomelo.EntityFrameworkCore.MySql** | Proveedor EF Core para MySQL. |
| **Spectre.Console** | Menús, tablas y prompts en consola. |
| **Git** | Control de versiones del repositorio. |

La cadena de conexión se configura en `appsettings.json` (`ConnectionStrings:MySqlDB`). Cada entorno debe definir su propio servidor, base de datos y credenciales.

---

## Estructura del Sistema

```
SistemaDeGestionDeTicketsAereos/
├── Program.cs                    # Punto de entrada
├── appsettings.json              # Cadena de conexión MySQL
├── Migrations/                   # Migraciones EF Core
└── src/
    ├── shared/
    │   ├── context/              # AppDbContext, fábrica en diseño
    │   ├── helpers/            # DbContextFactory, utilidades
    │   └── ui/menus/           # Login, orquestador de menús principal
    └── modules/                 # Un módulo por dominio funcional
        └── <módulo>/
            ├── UI/             # Menús y pantallas de consola (*Menu.cs)
            ├── Application/    # Casos de uso (*UseCase.cs), servicios
            ├── Domain/         # Agregados, value objects, interfaces de repositorio
            └── Infrastructure/ # Entidades EF, configuraciones, repositorios
```

Cada **módulo** sigue la separación **UI → Application → Domain ← Infrastructure**, de modo que la lógica de negocio y el dominio no dependan directamente de la consola ni de detalles de SQL.

---

## Qué Hace Cada Archivo (principales)

| Archivo / carpeta | Función |
|-------------------|---------|
| `Program.cs` | Crea el `DbContext`, inicia `ConsoleMenuOrchestrator.StartAsync()` y captura errores críticos en consola. |
| `appsettings.json` | Contiene `ConnectionStrings:MySqlDB` para conectar a MySQL. |
| `src/shared/helpers/DbContextFactory.cs` | Construye `AppDbContext` leyendo configuración (uso desde menús y casos de uso). |
| `src/shared/context/AppDbContext.cs` | Contexto EF: `DbSet` de entidades y aplicación de configuraciones por módulo. |
| `src/shared/ui/menus/LoginMenu.cs` | Solicita usuario y contraseña, ejecuta login y deja rol e id en `AppState`. |
| `src/shared/ui/menus/ConsoleMenuOrchestrator.cs` | Bucle principal: según rol muestra menú de administrador o de cliente y despacha a cada `*Menu`. |
| `src/shared/ui/menus/AppState.cs` | Estado de sesión (autenticado, id de usuario, rol) compartido por la UI. |
| `src/modules/user/Application/UseCases/LoginUseCase.cs` | Valida credenciales contra el repositorio de usuarios. |
| `src/modules/user/Infrastructure/Repositories/UserRepository.cs` | Acceso a tabla `User` con EF Core. |
| `src/modules/report/UI/ReportsMenu.cs` | Reportes operativos implementados con LINQ sobre datos cargados desde el contexto. |
| `src/modules/*/UI/*Menu.cs` | Punto de entrada de consola de cada módulo (vuelos, reservas, clientes, pagos, etc.). |
| `src/modules/*/Application/UseCases/*.cs` | Orquestación de un caso de uso (crear reserva, emitir tiquete, etc.). |
| `src/modules/*/Infrastructure/Repositories/*.cs` | Implementación concreta de acceso a datos del módulo. |
| `Migrations/*.cs` | Evolución del esquema y datos iniciales donde aplica. |

El resto de archivos del repositorio extiende este patrón por entidad (configuración EF, entidades, agregados de dominio).

---

## Credenciales del Administrador

Usuario sembrado en base de datos (configuración `UserEntityConfiguration`, tabla `User`):

| Campo | Valor |
|--------|--------|
| **Usuario** | `admin` |
| **Contraseña** | `12345678` |
| **Rol** | Administrador (`IdUserRole = 1`) |

Los clientes finales se gestionan con rol distinto (por ejemplo registro desde el flujo de login según implementación actual). Cambiar contraseñas en entorno real debe hacerse por menú de usuarios o actualizando datos de forma segura; no uses estas credenciales en producción.

---

## Autores


- kevin sierra , juan pablo quijano , valentina mancilla

---

## Examen  — Reprogramación de Vuelo y Lista de Espera

Esta sección documenta las **5 funcionalidades nuevas** añadidas al sistema en el Examen 4, junto con todos los archivos creados o modificados para implementarlas.

---

### Funcionalidades Implementadas

#### 1. Reprogramación de Reservas
Un cliente puede solicitar mover su reserva (confirmada o pagada) a otro vuelo de la **misma ruta**. El sistema valida la compatibilidad de ruta, ajusta los cupos del vuelo anterior y del nuevo, y registra el cambio en el historial.

**Caso de uso:** `RescheduleBookingUseCase`
**Acceso:** Menú cliente → opción `4. Reprogramar mi reserva`

#### 2. Validación de Compatibilidad de Vuelo
Antes de reprogramar, el sistema verifica que el vuelo nuevo tenga el **mismo origen y destino** (mismo `IdRoute`) que el vuelo actual de la reserva. Si la ruta es diferente, se lanza una excepción con el mensaje *"Flights are incompatible: the new flight must have the same origin and destination."*

**Implementado dentro de:** `RescheduleBookingUseCase`

#### 3. Lista de Espera
Si el vuelo solicitado no tiene cupos suficientes, la reserva **no se rechaza**; en cambio, se agrega automáticamente a la lista de espera con una posición calculada (cantidad de entradas pendientes + 1). No se permiten duplicados: una misma reserva no puede estar dos veces en espera para el mismo vuelo.

**Caso de uso:** `RescheduleBookingUseCase` (rama sin cupo) + `CreateBookingWaitListUseCase`
**Acceso:** Menú cliente → opción `4. Reprogramar mi reserva` (flujo automático si el vuelo está lleno)
**Acceso:** Menú cliente → opción `5. Mi lista de espera` (consulta)
**Acceso:** Menú admin → opción `8. Lista de espera por vuelo` (vista admin)

#### 4. Liberación Automática de Cupos
Cuando se cancela una reserva, el sistema intenta **promover automáticamente** la primera reserva en espera para ese vuelo (ordenada por posición). Si hay alguien esperando, su reserva es reprogramada al vuelo liberado, los cupos se ajustan en ambos vuelos y la entrada en lista de espera queda marcada como *"Promovida"*.

**Caso de uso:** `PromoteFromWaitListUseCase`
**Disparador:** `CancelAsync` en `BookingMenu` (se ejecuta automáticamente al final de cada cancelación)

#### 5. Historial de Cambios de Vuelo
Cada vez que una reserva cambia de vuelo (reprogramación manual o promoción automática desde lista de espera), se registra un `BookingFlightChange` con: fecha del cambio, motivo opcional, vuelo anterior, vuelo nuevo y usuario que ejecutó la operación.

**Acceso:** Menú admin → opción `9. Historial de cambios de vuelo`

---

### Nuevos Módulos

#### `bookingFlightChange`
Registra el historial inmutable de cada reprogramación de vuelo.

```
src/modules/bookingFlightChange/
├── Domain/
│   ├── aggregate/
│   │   └── BookingFlightChange.cs           # Agregado: Id, ChangeDate, Reason, IdBooking, IdOldFlight, IdNewFlight, IdUser
│   ├── valueObject/
│   │   ├── BookingFlightChangeId.cs         # int >= 0
│   │   ├── BookingFlightChangeDate.cs       # DateTime, no futura
│   │   └── BookingFlightChangeReason.cs     # string?, máx 500 chars
│   └── Repositories/
│       └── IBookingFlightChangeRepository.cs  # Sin UpdateAsync (historial inmutable)
├── Infrastructure/
│   ├── Entity/
│   │   ├── BookingFlightChangeEntity.cs
│   │   └── BookingFlightChangeEntityConfiguration.cs  # Dos FKs a Flight con WithMany explícito
│   └── Repositories/
│       └── BookingFlightChangeRepository.cs
└── Application/
    ├── UseCases/
    │   ├── CreateBookingFlightChangeUseCase.cs
    │   ├── GetAllBookingFlightChangesUseCase.cs
    │   ├── GetBookingFlightChangeByIdUseCase.cs
    │   └── GetBookingFlightChangesByBookingUseCase.cs
    ├── Interfaces/
    │   └── IBookingFlightChangeService.cs
    └── Services/
        └── BookingFlightChangeService.cs
```

#### `bookingWaitList`
Gestiona la cola de espera de reservas para vuelos sin cupos.

```
src/modules/bookingWaitList/
├── Domain/
│   ├── aggregate/
│   │   └── BookingWaitList.cs               # Agregado: Id, Position, RequestedAt, IdBooking, IdFlight, IdStatus
│   ├── valueObject/
│   │   ├── BookingWaitListId.cs             # int >= 0
│   │   ├── BookingWaitListPosition.cs       # int >= 1
│   │   └── BookingWaitListRequestedAt.cs    # DateTime, no futura
│   └── Repositories/
│       └── IBookingWaitListRepository.cs    # Incluye GetNextPendingByFlightAsync, CountPendingByFlightAsync, ExistsAsync
├── Infrastructure/
│   ├── Entity/
│   │   ├── BookingWaitListEntity.cs
│   │   └── BookingWaitListEntityConfiguration.cs
│   └── Repositories/
│       └── BookingWaitListRepository.cs
└── Application/
    ├── UseCases/
    │   ├── CreateBookingWaitListUseCase.cs   # Verifica duplicado y calcula posición
    │   ├── GetAllBookingWaitListsUseCase.cs
    │   ├── GetBookingWaitListByIdUseCase.cs
    │   ├── GetBookingWaitListsByFlightUseCase.cs
    │   └── DeleteBookingWaitListUseCase.cs
    ├── Interfaces/
    │   └── IBookingWaitListService.cs
    └── Services/
        └── BookingWaitListService.cs
```

---

### Nuevos Casos de Uso en el Módulo `booking`

| Archivo | Descripción |
|---------|-------------|
| `src/modules/booking/Application/UseCases/RescheduleBookingUseCase.cs` | Orquesta la reprogramación: valida estado, verifica compatibilidad de ruta, asigna al vuelo o agrega a lista de espera. Retorna `RescheduleResult` (tipado, no bool). |
| `src/modules/booking/Application/UseCases/PromoteFromWaitListUseCase.cs` | Toma la primera entrada pendiente de una lista de espera, reprograma la reserva al vuelo liberado, ajusta cupos en ambos vuelos y marca la entrada como *Promovida*. |

---

### Archivos Modificados

| Archivo | Cambio |
|---------|--------|
| `src/modules/booking/Domain/Repositories/IBookingRepository.cs` | + `ListByFlightAsync(int idFlight)` |
| `src/modules/booking/Infrastructure/Repositories/BookingRepository.cs` | Implementación de `ListByFlightAsync` |
| `src/modules/flight/Domain/Repositories/IFlightRepository.cs` | + `ListByRouteAsync(int idRoute)` |
| `src/modules/flight/Infrastructure/Repositories/FlightRepository.cs` | Implementación de `ListByRouteAsync` |
| `src/modules/flight/Infrastructure/Entity/FlightEntity.cs` | + colecciones `OldFlightChanges`, `NewFlightChanges`, `WaitListEntries` |
| `src/modules/booking/Infrastructure/Entity/BookingEntity.cs` | + colecciones `BookingFlightChanges`, `BookingWaitListEntries` |
| `src/modules/systemStatus/Infrastructure/Entity/SystemStatusEntity.cs` | + colección `BookingWaitListEntries` |
| `src/modules/systemStatus/Infrastructure/Entity/SystemStatusEntityConfiguration.cs` | + seeds: IdStatus 21 (*En Espera*), 22 (*Promovida*), 23 (*Expirada*) con EntityType `WaitList` |
| `src/modules/booking/UI/BookingMenu.cs` | + menú admin opciones 8 y 9; + menú cliente opciones 4 y 5; + trigger de promoción automática al cancelar |

---

### Migración de Base de Datos

**`20260424185536_AddBookingFlightChangeAndWaitList`**

Crea las tablas:

**`BookingFlightChange`**

| Columna | Tipo | Descripción |
|---------|------|-------------|
| `IdChange` | int (PK, auto) | Identificador del registro |
| `IdBooking` | int (FK) | Reserva reprogramada |
| `IdOldFlight` | int (FK → Flight) | Vuelo de origen |
| `IdNewFlight` | int (FK → Flight) | Vuelo de destino |
| `ChangeDate` | datetime | Fecha y hora del cambio |
| `Reason` | varchar(500), nullable | Motivo opcional |
| `IdUser` | int (FK → User) | Usuario que ejecutó el cambio |

**`BookingWaitList`**

| Columna | Tipo | Descripción |
|---------|------|-------------|
| `IdWaitList` | int (PK, auto) | Identificador de la entrada |
| `IdBooking` | int (FK) | Reserva en espera |
| `IdFlight` | int (FK) | Vuelo deseado |
| `Position` | int | Posición en la cola (empieza en 1) |
| `RequestedAt` | datetime | Timestamp de ingreso a la cola |
| `IdStatus` | int (FK → SystemStatus) | Estado: En Espera / Promovida / Expirada |

Inserta los seeds de estado:

| IdStatus | StatusName | EntityType |
|----------|-----------|------------|
| 21 | En Espera | WaitList |
| 22 | Promovida | WaitList |
| 23 | Expirada | WaitList |

---

### Nuevas Opciones de Menú

#### Administrador (`BookingMenu`)

| Opción | Función |
|--------|---------|
| `8. Lista de espera por vuelo` | Selecciona un vuelo y muestra todas sus entradas en espera ordenadas por posición, con estado. |
| `9. Historial de cambios de vuelo` | Selecciona una reserva y muestra el historial completo de reprogramaciones con fecha, motivo, vuelo anterior, vuelo nuevo y usuario. |

#### Cliente (`BookingMenu`)

| Opción | Función |
|--------|---------|
| `4. Reprogramar mi reserva` | Lista las reservas propias confirmadas/pagadas, permite elegir un nuevo vuelo (mismo origen/destino). Si hay cupo: reprograma inmediatamente. Si no hay cupo: ingresa a lista de espera e informa la posición obtenida. |
| `5. Mi lista de espera` | Muestra todas las entradas en lista de espera asociadas a las reservas del cliente, con posición, vuelo deseado y estado actual. |

---

### Flujo Completo: Reprogramación con Lista de Espera

```
Cliente solicita reprogramar reserva R al vuelo V
        │
        ▼
¿Vuelo V tiene la misma ruta que el vuelo actual?
        │ No → Error: "Vuelos incompatibles"
        │ Sí
        ▼
¿Vuelo V tiene cupos suficientes?
        │ Sí → Reasignar reserva a V
        │       Liberar cupos en vuelo anterior
        │       Ocupar cupos en V
        │       Registrar BookingFlightChange
        │       → "Reserva reprogramada exitosamente"
        │
        │ No → ¿Reserva ya está en espera para V?
                │ Sí → Error: "Ya está en lista de espera"
                │ No → Calcular posición (entradas pendientes + 1)
                │       Crear BookingWaitList con estado "En Espera"
                │       → "Quedaste en lista de espera en posición X"
```

```
Admin/Cliente cancela reserva R (vuelo V)
        │
        ▼
Registrar BookingCancellation
Marcar reserva como "Cancelada"
Restaurar cupos en vuelo V
        │
        ▼
¿Hay entradas "En Espera" para el vuelo V?
        │ No → Fin
        │ Sí → Tomar la de menor posición (primera en cola)
                ¿La reserva asociada sigue activa (confirmada/pagada)?
                │ No → Fin (sin promoción)
                │ Sí → Reasignar reserva al vuelo V
                │       Liberar cupos en el vuelo original de esa reserva
                │       Ocupar cupos en V
                │       Marcar entrada como "Promovida"
                │       Registrar BookingFlightChange (motivo automático)
                │       → "Se promovió automáticamente una reserva desde la lista de espera"
```

---

