# Manual Técnico — SIBI (Sistema de Inventario de Bienes Institucionales)

**Escuela de Ingeniería Civil (EIC) — Universidad de Costa Rica**

Este documento existe para que cualquier desarrollador que no conozca el proyecto pueda leerlo, entender por qué está construido como está, y retomar el trabajo sin tener que hacer arqueología de código. Cubre arquitectura, backend, frontend, base de datos, seguridad, despliegue y flujos de negocio.

> Convención del proyecto: nombres de clases, propiedades, rutas y mensajes están en español, porque el dominio (bienes institucionales de una universidad costarricense) y sus usuarios son hispanohablantes. Este manual mantiene esa convención al referirse a entidades del código (`Activo`, `Encargado`, etc.) pero está escrito en español para que sea consistente con el resto del repositorio.

---

## Tabla de contenido

1. [Qué es SIBI](#1-qué-es-sibi)
2. [Arquitectura general](#2-arquitectura-general)
3. [Stack tecnológico](#3-stack-tecnológico)
4. [Estructura de carpetas](#4-estructura-de-carpetas)
5. [Modelo de datos y base de datos](#5-modelo-de-datos-y-base-de-datos)
6. [Backend (.NET)](#6-backend-net)
7. [Frontend (Vue 3)](#7-frontend-vue-3)
8. [Autenticación, autorización y seguridad](#8-autenticación-autorización-y-seguridad)
9. [Notificaciones en tiempo real (SignalR)](#9-notificaciones-en-tiempo-real-signalr)
10. [Despliegue con Docker Compose](#10-despliegue-con-docker-compose)
11. [Despliegue alternativo (frontend/backend separados, ej. Vercel)](#11-despliegue-alternativo-frontendbackend-separados-ej-vercel)
12. [Cómo levantar el proyecto en desarrollo local (sin Docker)](#12-cómo-levantar-el-proyecto-en-desarrollo-local-sin-docker)
13. [Flujos de negocio clave](#13-flujos-de-negocio-clave)
14. [Matriz de permisos por rol](#14-matriz-de-permisos-por-rol)
15. [Decisiones de diseño y por qué existen](#15-decisiones-de-diseño-y-por-qué-existen)
16. [Deuda técnica y limitaciones conocidas](#16-deuda-técnica-y-limitaciones-conocidas)
17. [Troubleshooting](#17-troubleshooting)
18. [Dónde tocar el código para tareas comunes](#18-dónde-tocar-el-código-para-tareas-comunes)

---

## 1. Qué es SIBI

SIBI es el sistema interno de la EIC-UCR para llevar el inventario de bienes institucionales (computadoras, mobiliario, equipo de laboratorio, etc.), identificados por **placa**. Para cada bien se registra marca, modelo, número de serie, categoría, ubicación física, encargado responsable, estado (Activo / Mantenimiento / Desecho) y un historial completo de cambios (auditoría).

El sistema tiene cuatro roles con distinto nivel de permiso (ver [sección 14](#14-matriz-de-permisos-por-rol)):

- **Administradora**: control total (usuarios, categorías, eliminación definitiva de activos, aprobación de solicitudes).
- **GTI** (Gestión de Tecnologías de Información): gestión operativa del inventario, encargados, aprobación/rechazo de solicitudes de cambio.
- **JefaAdministrativa**: no edita activos directamente; **propone** cambios mediante "solicitudes de cambio" que GTI/Administradora deben aprobar.
- **Invitado**: solo lectura. Puede ver el inventario, una vista rápida de cada activo (modal, al hacer clic en la tabla), categorías y la lista de Desecho (sin botones de acción), pero no la página de detalle completo de un activo ni el historial/auditoría (ver `requiresNoInvitado` en el router y el `v-if="!auth.esInvitado"` sobre el botón "ver detalle completo" en `ActivoModal.vue`).

Módulos funcionales (cada uno = una vista Vue + un controller en el backend):

| Módulo | Vista frontend | Controller backend |
|---|---|---|
| Login / recuperación / cambio de contraseña | `LoginView`, `RecuperarContrasenaView`, `CambiarContrasenaView` | `AuthController` |
| Dashboard con estadísticas | `DashboardView` | `ActivoController` (`/stats`, `/recientes`) |
| Inventario (listar/crear/editar/buscar activos) | `InventarioView`, `ActivoDetalleView`, `ActivoModal`, `ImportarActivosModal` | `ActivoController` |
| Categorías de activos | `CategoriasView` | `CategoriaController` |
| Encargados (personas responsables de bienes) | `EncargadosView` | `EncargadoController` |
| Desecho (baja de activos) | `DesechoView` | `DesechoController` |
| Historial / auditoría | `HistorialView` | `HistorialController` |
| Solicitudes de cambio (flujo de aprobación) | `SolicitudesView`, `MisSolicitudesView` | `SolicitudCambioController` |
| Usuarios del sistema | `UsuariosView` | `UsuarioController` |
| Notificaciones push en tiempo real | `stores/notificaciones.js` | `Hubs/NotificacionHub.cs` |

---

## 2. Arquitectura general

Aplicación de 3 capas clásica, cada una en su propio contenedor Docker:

```mermaid
flowchart LR
    subgraph Cliente
        B["Navegador (SPA Vue 3)"]
    end

    subgraph "Contenedor frontend (nginx:stable-alpine)"
        F["Archivos estáticos (dist/)\n+ nginx como reverse proxy"]
    end

    subgraph "Contenedor backend (ASP.NET Core 10)"
        API["API REST\n(Controllers)"]
        HUB["SignalR Hub\n/hubs/notificaciones"]
    end

    subgraph "Contenedor db (SQL Server 2022)"
        SQL[("Base de datos SIBI")]
    end

    B -- HTTPS --> F
    F -- "/api/*  →  proxy_pass" --> API
    F -- "/hubs/*  →  proxy_pass (WebSocket)" --> HUB
    API -- EF Core / TCP 1433 --> SQL
    HUB -.-> API
```

Puntos clave de esta arquitectura:

- El **frontend nunca habla directo con el backend en producción**: nginx dentro del contenedor `frontend` actúa de reverse proxy para `/api/` y `/hubs/`, hacia el contenedor `backend` por la red interna de Docker (`sibi-net`). Esto evita problemas de CORS en producción y expone un solo puerto público.
- En **desarrollo**, el proxy lo hace `vue-cli-service` (ver `frontend/vue.config.js`, `devServer.proxy`) hacia `http://localhost:5025`.
- El backend expone Swagger solo en `Development` (`Program.cs`, `if (app.Environment.IsDevelopment())`).
- Hay un **servidor nginx del host** (fuera de Docker) descrito en `nginx/eic.sibi.ucr.ac.cr.conf`, que es el que finalmente recibe tráfico del dominio `eic.sibi.ucr.ac.cr` y lo reenvía al contenedor `frontend` (publicado en el puerto `8081` del host). Es decir, hay **dos niveles de nginx**: uno en el host (dominio, TLS terminado ahí presumiblemente por un proxy superior) y uno dentro del contenedor `frontend` (SPA + proxy interno hacia backend).

---

## 3. Stack tecnológico

### Backend
- **.NET 10** / ASP.NET Core Web API (`backend/backend.csproj`, `TargetFramework=net10.0`)
- **Entity Framework Core 10** con proveedor **SQL Server** (`Microsoft.EntityFrameworkCore.SqlServer`)
- **JWT Bearer** para autenticación (`Microsoft.AspNetCore.Authentication.JwtBearer`)
- **BCrypt.Net-Next** para hash de contraseñas
- **SignalR** para notificaciones push en tiempo real
- **Swashbuckle** (Swagger/OpenAPI) solo en desarrollo

### Frontend
- **Vue 3** (Options/Composition API mixtas) con **Vue CLI 5** (no Vite)
- **Pinia** para estado global (auth, notificaciones)
- **Vue Router 4** con guardas de navegación basadas en roles
- **Axios** para HTTP, con interceptores de token y logout automático en 401
- **@microsoft/signalr** cliente para el hub de notificaciones
- **Tailwind CSS 3** para estilos (utility-first, sin librería de componentes)
- **xlsx** (SheetJS) para importar/exportar Excel
- **jsPDF + jspdf-autotable** para exportar reportes en PDF

### Base de datos
- **SQL Server 2022** (imagen oficial `mcr.microsoft.com/mssql/server:2022-latest`, edición Express vía `MSSQL_PID`)

### Infraestructura
- **Docker Compose** orquesta 3 servicios: `db`, `backend`, `frontend`
- **nginx** como servidor web del frontend y como reverse proxy del host

---

## 4. Estructura de carpetas

```
SIBI/
├── backend/                     # API .NET
│   ├── Controllers/              # 7 controllers REST (uno por recurso)
│   ├── DTOs/                     # Records de entrada/salida (nunca se exponen los Models directo)
│   ├── Data/SibiDbContext.cs     # Configuración de EF Core (Fluent API, no hay migraciones EF)
│   ├── Hubs/NotificacionHub.cs   # Hub SignalR
│   ├── Models/                   # Entidades EF (mapeadas 1:1 a tablas SQL existentes)
│   ├── Services/                 # AuthService, HistorialService, NotificacionService
│   ├── Program.cs                # Composition root: DI, JWT, CORS, SignalR, pipeline HTTP
│   ├── appsettings.json          # Config base (producción)
│   └── appsettings.Development.json
├── database/
│   ├── SIBI.sql                  # DDL completo (CREATE DATABASE + tablas + constraints)
│   ├── seed.sql                  # Inserta usuario administrador inicial (idempotente)
│   ├── migrate_categoria_nvarchar.sql  # Migración puntual (VARCHAR→NVARCHAR en Categoria)
│   ├── init.sh                   # Entrypoint del contenedor db: aplica SQL solo la 1ª vez
│   └── Dockerfile
├── frontend/                     # SPA Vue 3
│   ├── src/
│   │   ├── components/           # Componentes compartidos (modales, layout, selects)
│   │   ├── composables/          # useDialog.js (diálogo de confirmación global)
│   │   ├── router/index.js       # Rutas + guardas de navegación
│   │   ├── services/             # 1 archivo por recurso, wrapper delgado sobre axios
│   │   ├── stores/                # Pinia: auth.js, notificaciones.js
│   │   ├── utils/emojiGrupos.js  # Paleta de emojis para íconos de categoría
│   │   └── views/                # 1 vista por ruta/módulo
│   ├── nginx.conf                # Config de nginx DENTRO del contenedor frontend
│   └── Dockerfile
├── nginx/eic.sibi.ucr.ac.cr.conf # Config de referencia para el nginx del SERVIDOR (host), no se usa en Docker
├── tools/HashGenerator/           # Consola .NET para generar hashes BCrypt manualmente (setup inicial)
├── docker-compose.yml             # Orquesta db + backend + frontend
└── .env.example                   # Plantilla de variables de entorno para docker compose
```

---

## 5. Modelo de datos y base de datos

### 5.1 Diagrama entidad-relación

```mermaid
erDiagram
    Placa ||--|| Activo : "1 placa = 1 activo (PK compartida)"
    Categoria ||--o{ Activo : clasifica
    Ubicacion ||--o{ Activo : "ubica (histórico: cada cambio crea nueva fila)"
    Encargado ||--o{ Ubicacion : "EncargadoActual"
    Encargado ||--o{ Ubicacion : "EncargadoAnterior"
    Activo ||--o{ Historial : audita
    Usuario ||--o{ Historial : "genera (UsuarioCorreo)"
    Activo ||--o{ SolicitudCambio : "es objeto de"
    Usuario ||--o{ SolicitudCambio : "Solicitante"
    Usuario ||--o{ SolicitudCambio : "Revisor (nullable)"

    Placa {
        varchar Numero PK
        varchar Tipo "Institucional | Interno"
    }
    Usuario {
        varchar Correo PK
        varchar Nombre
        varchar Contrasena "hash BCrypt"
        varchar Permisos "Administradora|GTI|JefaAdministrativa|Invitado"
        bit EsContrasenaTemporal
        int IntentosFallidos
        bit Activo
    }
    Encargado {
        uniqueidentifier Id PK
        varchar Nombre
        varchar Rol
    }
    Categoria {
        int Id PK
        nvarchar Nombre UK
        nvarchar Icono "emoji"
    }
    Ubicacion {
        bigint Id PK
        varchar Actual
        varchar Anterior
        uniqueidentifier EncargadoActual FK
        uniqueidentifier EncargadoAnterior FK
    }
    Activo {
        varchar Placa PK "PK propia; también es FK a Placa.Numero"
        varchar Marca
        varchar Modelo
        varchar NumSerial
        varchar Articulo
        int CategoriaId FK
        varchar Observaciones
        bigint Ubicacion FK
        varchar Estado "Activo|Mantenimiento|Desecho"
        date FechaDesecho "NULL salvo Estado=Desecho"
    }
    Historial {
        bigint Id PK
        varchar ActivoPlaca FK
        varchar UsuarioCorreo FK
        varchar TipoAccion
        varchar Descripcion
        datetime FechaHora
    }
    SolicitudCambio {
        int Id PK
        varchar ActivoPlaca FK
        varchar SolicitanteCorreo FK
        datetime FechaSolicitud
        nvarchar DatosNuevos "JSON serializado"
        varchar Estado "Pendiente|Aprobada|Rechazada"
        varchar RevisorCorreo FK "nullable"
        datetime FechaResolucion
        nvarchar Comentario
    }
```

### 5.2 Particularidades importantes del esquema

1. **`Activo.Placa` es simultáneamente la PK de `Activo` y una FK a `Placa.Numero`.** Es decir, cada bien tiene exactamente una fila en `Placa` (que solo guarda el tipo: Institucional/Interno) y una fila en `Activo` (todo lo demás). Esto permite **cambiar la placa de un activo** sin perder historial: `ActivoController.CambiarPlaca` crea una fila `Placa` + `Activo` nuevas, re-apunta `Historial` y `SolicitudCambio` a la nueva placa, y borra las filas viejas — todo dentro de una transacción (ver `backend/Controllers/ActivoController.cs`, método `CambiarPlaca`).

2. **`Ubicacion` es un histórico append-only, no se edita in-place.** Cada vez que cambia la ubicación física o el encargado de un activo, se inserta una **fila nueva** en `Ubicacion` (con `Anterior` apuntando a los valores previos) y el `Activo.Ubicacion` se re-apunta a esa fila nueva. La fila vieja **no se borra** (queda huérfana de `Activo` pero conserva su historia). Esto es lo que permite mostrar "ubicación anterior / encargado anterior" en el detalle de un activo sin necesidad de una tabla de auditoría aparte para movimientos.

3. **`Historial` es la tabla de auditoría central.** Toda mutación relevante de un `Activo` pasa por `HistorialService.RegistrarAsync(...)`, que inserta una fila con un `TipoAccion` restringido por CHECK constraint a un enum fijo: `Creacion, CambioUbicacion, CambioEncargado, CambioEstado, CambioPlaca, Eliminacion, Aprobacion, Rechazo, SolicitudCambio, SolicitudAprobada, SolicitudRechazada`. **No hay triggers de SQL**: la auditoría es 100% responsabilidad del backend (ver comentario en `SIBI.sql`: "gestionado desde el backend, no con triggers"). Si se agrega un endpoint nuevo que muta un `Activo`, hay que acordarse de llamar `HistorialService` explícitamente y, si el `TipoAccion` es nuevo, **agregar el valor al CHECK constraint** (`CHK_Historial_TipoAccion`) en la base de datos.

4. **`SolicitudCambio.DatosNuevos` es JSON libre**, deserializado como `SolicitudDatosDto` (ver `backend/DTOs/SolicitudCambioDto.cs`). Guarda tanto los valores propuestos como un **snapshot de los valores originales** (`MarcaOriginal`, `ModeloOriginal`, etc.) tomado en el momento de crear la solicitud, para que la UI pueda mostrar un diff "antes/después" aunque el activo haya cambiado entre tanto.

5. **Reglas de negocio impuestas por CHECK constraints** (documentadas en `database/SIBI.sql`):
   - `CHK_Usuario_Correo`: solo se permiten correos que terminen en `@ucr.ac.cr`.
   - `CHK_Usuario_Permisos`: el rol debe ser uno de los 4 válidos.
   - `CHK_Placa_Tipo`: `Institucional` o `Interno`.
   - `CHK_Activo_Estado`: `Activo`, `Mantenimiento` o `Desecho`.
   - `CHK_Activo_FechaDesecho`: `FechaDesecho` **debe** tener valor si y solo si `Estado = 'Desecho'` (constraint compuesta, no dos columnas independientes).

6. **No se usan EF Core Migrations.** El esquema vive en scripts SQL planos (`SIBI.sql` + `migrate_categoria_nvarchar.sql`), aplicados una sola vez por `database/init.sh` al arrancar el contenedor `db` (revisa si la base `SIBI` ya existe antes de correrlos). Si se necesita un cambio de esquema futuro, el patrón a seguir es: **crear un nuevo script `migrate_*.sql`, agregarlo a `database/Dockerfile` y a la secuencia de `init.sh`**, no usar `dotnet ef migrations add`.

### 5.3 Orden de arranque de la base de datos (`database/init.sh`)

1. Levanta `sqlservr` en background y espera a que responda `sqlcmd -Q "SELECT 1"` (hasta 30 reintentos).
2. Si la base `SIBI` **no existe**: ejecuta en orden `SIBI.sql` → `migrate.sql` (renombrado desde `migrate_categoria_nvarchar.sql`) → `seed.sql`.
3. Si ya existe, no hace nada (los datos persisten en el volumen Docker `mssql-data`).

`seed.sql` inserta el usuario administrador inicial (`soporte.eic@ucr.ac.cr`) **solo si no existe**, con el hash de contraseña como placeholder `REEMPLAZAR_CON_EL_HASH`. **Ese hash hay que generarlo manualmente antes del primer despliegue** con la herramienta de consola en `tools/HashGenerator`:

```bash
cd tools/HashGenerator
dotnet run "MiContrasenaTemporal123!"
# Copiar el hash $2a$11$... resultante y pegarlo en database/seed.sql
```

---

## 6. Backend (.NET)

### 6.1 Composition root (`backend/Program.cs`)

Es un backend minimal-hosting (top-level statements, sin `Startup.cs`). En orden:

1. Registra controllers, Swagger (con soporte de Bearer token en la UI), y `SibiDbContext` apuntando a `ConnectionStrings:SIBI`.
2. Lee `Jwt:Key` de configuración; **si está vacía, lanza excepción al arrancar** (`InvalidOperationException`) — esto es intencional, para no correr nunca con un JWT sin firmar correctamente.
3. Configura JWT Bearer: valida issuer, lifetime y firma; **no valida audience**. Tiene un hook especial (`OnMessageReceived`) para leer el token desde el **query string** (`?access_token=...`) cuando la petición es a `/hubs/*`, porque las conexiones WebSocket de SignalR no siempre pueden mandar el header `Authorization`.
4. CORS: política `FrontendVue`. En `Development` permite **cualquier origen** (`SetIsOriginAllowed(_ => true)`); en producción, solo `http://eic.sibi.ucr.ac.cr`. **Nota**: es `http://`, no `https://` — revisar si el dominio real sirve por HTTPS y ajustar si hace falta (ver [sección 16](#16-deuda-técnica-y-limitaciones-conocidas)).
5. Registra SignalR y los tres servicios de aplicación (`AuthService`, `HistorialService`, `NotificacionService`) como `Scoped`.
6. Pipeline HTTP: Swagger (solo dev) → `UseForwardedHeaders` (para que el backend vea la IP/proto real detrás de los proxies nginx) → CORS → Auth → Controllers → Hub.

### 6.2 Controllers

Todos los controllers heredan de `ControllerBase`, usan `[ApiController]` + `[Route("api/[controller]")]`, y devuelven DTOs (nunca entidades EF directamente, para no filtrar campos internos ni crear ciclos de serialización por las navigation properties).

| Controller | Ruta base | Autorización por defecto | Notas |
|---|---|---|---|
| `AuthController` | `/api/auth` | Anónimo (excepto `cambiar-contrasena`) | Login, recuperación, cambio de contraseña |
| `ActivoController` | `/api/activo` | `[Authorize]` en la clase, roles específicos por acción | CRUD de activos, importación masiva, cambio de placa |
| `CategoriaController` | `/api/categoria` | Lectura para cualquier autenticado, escritura solo `Administradora` | |
| `EncargadoController` | `/api/encargado` | `[Authorize]` en la clase | Incluye reasignación masiva de activos |
| `DesechoController` | `/api/desecho` | `[Authorize]` en la clase, aprobar/rechazar solo `Administradora` | |
| `HistorialController` | `/api/historial` | `Roles = "GTI,Administradora,JefaAdministrativa"` (Invitado no ve historial) | Filtrable por usuario, placa, tipo de acción, rango de fechas |
| `SolicitudCambioController` | `/api/solicitudcambio` | Depende del endpoint | Flujo de aprobación (ver [13.3](#133-flujo-de-solicitudes-de-cambio)) |
| `UsuarioController` | `/api/usuario` | `Roles = "Administradora"` en toda la clase | Gestión de cuentas |

Detalles de endpoints no obvios en `ActivoController` (el controller más grande y con más lógica transaccional):

- `GET /api/activo`: paginado (`pagina`, `tamano`), con filtros por texto libre (`busqueda`, sobre Placa/Marca/Modelo/NumSerial/Articulo), `categoriaIds[]` y `estado`. La fecha de creación de cada activo **no está en la tabla `Activo`**; se resuelve consultando el `Historial` por la primera fila `TipoAccion = 'Creacion'` de cada placa (batch, no N+1).
- `GET /api/activo/stats`: cuenta activos por estado y por categoría, usado por el Dashboard.
- `POST /api/activo`: transacción que crea `Placa` (si no existe) + `Ubicacion` + `Activo`, y registra `Historial` tipo `Creacion`. Roles: `GTI, Administradora, JefaAdministrativa`.
- `POST /api/activo/importar`: importación masiva desde Excel (el parseo del `.xlsx` ocurre en el **frontend** con SheetJS; el backend solo recibe un array de filas ya parseadas). Procesa fila por fila, **cada fila en su propia transacción** (si una fila falla no revierte las demás), y traduce mensajes de error de SQL Server (constraint violations) a mensajes en español legibles para el usuario (ver el bloque `catch` largo con `detalle.Contains(...)`).
- `PUT /api/activo/{placa}`: si cambia ubicación o encargado, crea una nueva fila `Ubicacion` (nunca edita la existente) y registra `Historial` separado para cada tipo de cambio (ubicación, encargado, estado) — puede generar hasta 3 filas de historial en una sola edición.
- `PATCH /api/activo/{placa}/cambiar-placa`: ver [5.2](#52-particularidades-importantes-del-esquema) punto 1.
- `DELETE /api/activo/{placa}/reciente`: permite borrar un activo **recién creado** (ventana de 10 minutos desde su `Historial` tipo `Creacion`), pensado como "deshacer" rápido de un error de captura. Pasado ese tiempo, retorna `400`.
- `DELETE /api/activo/{placa}`: eliminación **definitiva**, solo `Administradora`, y solo si el activo lleva **al menos 365 días** en estado `Desecho` (`hoy.DayNumber - activo.FechaDesecho.Value.DayNumber < 365` → rechazo). Borra en cascada manual: `Historial`, `SolicitudCambio`, `Activo`, `Placa`, `Ubicacion` (en ese orden, porque EF no tiene `ON DELETE CASCADE` configurado — todas las FK son `DeleteBehavior.Restrict`).
- `GET /api/activo/recientes`: activos creados más recientemente **dentro de una categoría**, usado en el Dashboard.

### 6.3 DTOs

Están en `backend/DTOs/`, son `record`s inmutables de C#. Un patrón consistente: por cada recurso hay un DTO de **salida** (ej. `ActivoDto`) y uno o más de **entrada** (`CrearActivoRequest`, `EditarActivoRequest`). Esto desacopla el contrato HTTP de las entidades EF, así que renombrar una columna de base de datos no rompe el frontend mientras se mantenga el DTO.

### 6.4 Servicios

- **`AuthService`**: login (con bloqueo tras 3 intentos fallidos y notificación a admins cuando eso ocurre), recuperación de contraseña (genera una temporal aleatoria — ver limitación en [16](#16-deuda-técnica-y-limitaciones-conocidas)), cambio de contraseña (valida fuerza: mínimo 6 caracteres, mayúscula, minúscula, número), y generación del JWT.
- **`HistorialService`**: un único método `RegistrarAsync`, usado por todos los controllers que mutan un `Activo`.
- **`NotificacionService`**: envía eventos SignalR a grupos de conexión (`GTIAdmin`, `Admin`) — ver [sección 9](#9-notificaciones-en-tiempo-real-signalr).

### 6.5 Generación de contraseñas temporales

Tanto `AuthService.GenerarContrasenaAleatoria` como `UsuarioController.GenerarContrasenaTemp` (código duplicado, ver [16](#16-deuda-técnica-y-limitaciones-conocidas)) generan una contraseña de 8 caracteres garantizando al menos una mayúscula, una minúscula y un dígito, usando `System.Random` (no criptográficamente seguro, pero aceptable para una contraseña de un solo uso que se descarta al primer login).

---

## 7. Frontend (Vue 3)

### 7.1 Arranque y configuración

`src/main.js` crea la app, instala Pinia y el router, e importa `assets/tailwind.css`. No hay SSR ni Vite: el build lo hace `@vue/cli-service` (`vue.config.js`). En desarrollo, `devServer.proxy` reenvía `/api` y `/hubs` (con soporte WebSocket, `ws: true`) hacia `http://localhost:5025`, el puerto por defecto del backend en local (ver `backend/Properties/launchSettings.json`).

Alias `@` → `src/` configurado en `jsconfig.json` y usado en todos los imports.

### 7.2 Router (`src/router/index.js`)

Rutas planas para Login/Recuperar/Cambiar contraseña, y un grupo anidado bajo `AppLayout.vue` (el sidebar + topbar) para todo lo que requiere sesión. El guard global `router.beforeEach` implementa, en este orden:

1. Si la ruta requiere auth y no hay sesión → redirige a `Login`.
2. Si el usuario tiene `esContrasenaTemporal = true` → lo fuerza a `CambiarContrasena` sin importar a dónde iba (excepto si ya iba ahí). Esto es lo que obliga a cambiar la contraseña temporal en el primer login.
3. Si ya está autenticado (y no tiene contraseña temporal) e intenta ir a Login/Recuperar → lo manda al Dashboard.
4. Chequeos de rol vía `meta`: `requiresAdmin`, `requiresGTIorAdmin`, `requiresNoInvitado`.

Estos `meta` flags son la **única** barrera de UI para ocultar rutas por rol; el backend vuelve a validar todo con `[Authorize(Roles = ...)]`, así que el frontend nunca es la única línea de defensa (correcto: nunca confiar solo en el cliente).

### 7.3 Estado global (Pinia)

- **`stores/auth.js`**: guarda `token` y `usuario` (ambos persistidos en `localStorage` bajo las claves `sibi_token` / `sibi_usuario`). Expone getters de conveniencia por rol (`esAdministradora`, `esGTI` — que incluye Administradora, `esJefaAdministrativa`, `esInvitado`) usados tanto por el router como por las vistas para mostrar/ocultar UI.
- **`stores/notificaciones.js`**: mantiene la conexión SignalR y la lista de notificaciones recibidas en memoria (no persiste en `localStorage`; si se recarga la página se pierden las notificaciones ya mostradas, solo llegan las nuevas mientras la conexión esté viva).

### 7.4 Capa de servicios (`src/services/`)

Cada archivo es un wrapper 1:1 sobre un controller del backend (mismo nombre de recurso). `api.js` centraliza:
- La `baseURL` (usa `VUE_APP_API_URL` si está definida — deployment separado — o `/api` relativo — mismo origen, proxeado por nginx).
- Un serializador de query params custom que **omite valores vacíos/null** y expande arrays repitiendo la clave (`categoriaIds=1&categoriaIds=2`), porque el binding de ASP.NET Core para `int[]?` espera ese formato.
- Interceptor de request: agrega `Authorization: Bearer <token>` desde `localStorage`.
- Interceptor de response: en un `401` (salvo que la petición sea a `/auth/*`, para no interferir con el propio login fallido) limpia la sesión y fuerza `window.location.href = '/'` — logout duro, no solo redirect de Vue Router, para asegurar que se limpie todo el estado de la SPA.

### 7.5 Vistas y componentes

| Archivo | Responsabilidad |
|---|---|
| `AppLayout.vue` | Sidebar de navegación (ítems condicionados por rol), topbar con buscador global de activos (autocompletado con debounce) y badge de notificaciones no leídas |
| `ActivoModal.vue` | Modal grande (~900 líneas) para crear/editar un activo; incluye creación rápida de categoría/encargado sin salir del modal, y muestra si hay una solicitud de cambio pendiente sobre ese activo |
| `ImportarActivosModal.vue` | Flujo de importación masiva: descarga plantilla `.xlsx`, parsea el archivo subido con SheetJS en el navegador, valida localmente, y manda las filas ya parseadas a `POST /api/activo/importar` |
| `SearchableSelect.vue` | Combobox genérico con búsqueda, reusado para seleccionar categoría/encargado en varios formularios |
| `AppDialog.vue` + `composables/useDialog.js` | Reemplazo de `window.confirm`/`alert` nativo: un diálogo modal único (estado singleton reactivo) invocado como `useDialog().confirm({...})` que retorna una `Promise<boolean>` |
| `DashboardView.vue` | KPIs (tarjetas clicables que navegan a otras vistas) + gráficos por categoría; el contenido varía según rol (GTI ve tarjeta de Encargados, JefaAdministrativa ve tarjeta de Cambios Solicitados) |
| `InventarioView.vue` | Tabla paginada/filtrable de activos, punto de entrada a `ActivoModal` |
| `SolicitudesView.vue` / `MisSolicitudesView.vue` | Bandeja de aprobación (GTI/Administradora) vs. bandeja de solicitudes propias (JefaAdministrativa) |

`utils/emojiGrupos.js` es simplemente un catálogo de emojis agrupados por temática, usado como selector de ícono al crear una categoría (el campo `Categoria.Icono` guarda el emoji tal cual, como texto Unicode).

---

## 8. Autenticación, autorización y seguridad

- **JWT firmado con HMAC-SHA256**, clave simétrica en `Jwt:Key` (variable de entorno `Jwt__Key` en producción). Claims: `NameIdentifier` = correo, `Name` = nombre, `Role` = permisos. Expira según `Jwt:ExpireHours` (8 horas por defecto).
- **Contraseñas**: hasheadas con BCrypt (`work factor` por defecto de la librería; `tools/HashGenerator` usa explícitamente `workFactor: 11`). Nunca se guardan ni se loguean en texto plano — excepto la contraseña temporal generada en recuperación/creación de usuario, que se **devuelve una vez en la respuesta HTTP** para que un administrador la comunique manualmente (no hay envío de correo implementado, ver [16](#16-deuda-técnica-y-limitaciones-conocidas)).
- **Bloqueo de cuenta**: 3 intentos fallidos de login consecutivos bloquean la cuenta (`Usuario.IntentosFallidos >= 3`); se resetea a 0 en login exitoso. Un administrador debe llamar `POST /api/usuario/{correo}/desbloquear` para levantar el bloqueo (genera una nueva contraseña temporal de paso).
- **Autorización por rol**: `[Authorize(Roles = "...")]` a nivel de acción o de clase en cada controller — es la fuente de verdad; el frontend solo oculta UI, no reemplaza esta validación.
- **CORS**: abierto en desarrollo, restringido a un origen fijo en producción (ver [6.1](#61-composition-root-backendprogramcs)).
- **Reglas de dominio de correo**: solo `@ucr.ac.cr` puede tener cuenta (`CHK_Usuario_Correo` en SQL + validación explícita en `UsuarioController.Crear`).
- **Cuenta de soporte protegida**: `soporte.eic@ucr.ac.cr` está hardcodeada en `UsuarioController` como cuenta que no se puede eliminar ni degradar de rol `Administradora` — es la cuenta de "no quedarse sin admin".

---

## 9. Notificaciones en tiempo real (SignalR)

- Hub: `backend/Hubs/NotificacionHub.cs`, mapeado en `/hubs/notificaciones`, protegido con `[Authorize]`.
- Al conectar (`OnConnectedAsync`), el hub lee el claim de rol del usuario y lo agrega a grupos de SignalR: `GTIAdmin` (roles `GTI` y `Administradora`) y/o `Admin` (solo `Administradora`). No hay lógica de negocio en el hub más allá de esta agrupación — es puramente un canal de push.
- `NotificacionService.NotificarGTIAdminAsync` / `NotificarAdminAsync` son llamados desde los controllers cuando ocurre algo que ese grupo debe saber:
  - Nueva solicitud de cambio → notifica a `GTIAdmin` (`SolicitudCambioController.Crear`).
  - Cuenta bloqueada por intentos fallidos → notifica a `Admin` (`AuthService.LoginAsync`).
- El cliente (`stores/notificaciones.js`) se conecta pasando el JWT como `accessTokenFactory`; como ya se explicó en [6.1](#61-composition-root-backendprogramcs), el backend intercepta ese token desde el query string porque el handshake de SignalR sobre WebSocket no siempre permite headers custom.
- Reconexión automática habilitada (`withAutomaticReconnect()`).

---

## 10. Despliegue con Docker Compose

`docker-compose.yml` define 3 servicios sobre una red bridge interna `sibi-net`:

| Servicio | Build context | Puertos publicados | Depende de |
|---|---|---|---|
| `db` | `./database` | ninguno (solo interno) | — |
| `backend` | `./backend` | ninguno (solo interno, puerto 8080) | `db` (con `condition: service_healthy`) |
| `frontend` | `./frontend` | `8081:80` | `backend` |

Es decir: **el único puerto expuesto al host es 8081** (el frontend). El backend y la base de datos solo son alcanzables entre contenedores. El healthcheck de `db` corre `sqlcmd -Q "SELECT 1"` cada 10s (hasta 15 reintentos, con 30s de gracia inicial) — el backend no arranca hasta que ese healthcheck pase.

### 10.1 Variables de entorno requeridas

Copiar `.env.example` a `.env` (nunca commitear `.env`) y completar:

```
MSSQL_SA_PASSWORD=   # mín. 8 chars, mayúscula, minúscula, número y símbolo especial (requisito de SQL Server)
JWT_KEY=              # mín. 32 caracteres aleatorios — generar con: openssl rand -base64 48
```

Estas variables alimentan `ConnectionStrings__SIBI` y `Jwt__Key` del contenedor `backend` (la doble guion bajo `__` es la convención de ASP.NET Core para mapear variables de entorno a claves anidadas de configuración, equivalente a `Jwt:Key`).

### 10.2 Construcción de cada imagen

- **`backend/Dockerfile`**: build multi-stage — SDK de .NET 10 compila y publica (`dotnet publish -c Release`), la imagen final usa solo el runtime ASP.NET (más liviana, sin SDK). Expone 8080, arranca con `dotnet backend.dll`.
- **`frontend/Dockerfile`**: build multi-stage — Node 20 corre `npm ci && npm run build` (genera `dist/`), la imagen final es `nginx:stable-alpine` sirviendo esos estáticos con la config de `nginx.conf`.
- **`database/Dockerfile`**: parte de la imagen oficial de SQL Server, copia los scripts SQL y `init.sh` a `/docker-init/`, y usa `init.sh` como `ENTRYPOINT` (reemplaza el arranque por defecto para poder aplicar el esquema la primera vez).

### 10.3 Comandos operativos

```bash
# Primer arranque (o después de borrar el volumen)
docker compose up -d --build

# Ver logs de un servicio
docker compose logs -f backend

# Reconstruir solo el frontend tras un cambio de código
docker compose up -d --build frontend

# Bajar todo conservando datos (el volumen mssql-data persiste)
docker compose down

# Bajar todo Y borrar los datos de la base (irreversible)
docker compose down -v
```

### 10.4 nginx del servidor host

`nginx/eic.sibi.ucr.ac.cr.conf` **no se usa dentro de Docker**; es la config a instalar en el nginx del servidor físico/VM donde corre Docker, para que el dominio público `eic.sibi.ucr.ac.cr` llegue al contenedor `frontend` (puerto `8081` publicado en el host). El archivo trae en comentarios las instrucciones de instalación (`sites-available` / `sites-enabled`) y un `map` necesario para que el upgrade de conexión a WebSocket (usado por SignalR) funcione a través de este proxy también.

---

## 11. Despliegue alternativo (frontend/backend separados, ej. Vercel)

El historial de commits (`test for vercel`, `changes for vercel deployment`) muestra que el frontend se probó desplegado por separado (p. ej. en Vercel) apuntando a un backend alojado en otro lugar. Esto es posible gracias a que:

- `frontend/.env.development` y potencialmente un `.env` de build en Vercel definen `VUE_APP_API_URL` con la URL absoluta del backend; `api.js` y `stores/notificaciones.js` la usan como prefijo en vez de asumir mismo origen.
- `frontend/.env.production` la deja **vacía a propósito**, para el caso "todo en Docker" donde nginx proxea `/api` internamente.

Si se retoma este modo de despliegue: recordar que el CORS del backend en producción solo permite `http://eic.sibi.ucr.ac.cr` como origen (`Program.cs`) — habría que agregar el dominio de Vercel a la política de CORS, o el navegador bloqueará las peticiones.

---

## 12. Cómo levantar el proyecto en desarrollo local (sin Docker)

### 12.1 Requisitos
- .NET SDK 10
- Node.js 20+ y npm
- SQL Server LocalDB (viene con Visual Studio) o cualquier instancia SQL Server accesible

### 12.2 Base de datos

```bash
# Con sqlcmd apuntando a tu instancia local:
sqlcmd -S "(localdb)\MSSQLLocalDB" -i database/SIBI.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -d SIBI -i database/migrate_categoria_nvarchar.sql
```

Generar el hash del usuario admin y pegarlo en `database/seed.sql` antes de ejecutarlo (ver [5.3](#53-orden-de-arranque-de-la-base-de-datos-databaseinitsh)):

```bash
cd tools/HashGenerator && dotnet run "TuContrasenaTemporal1!"
sqlcmd -S "(localdb)\MSSQLLocalDB" -d SIBI -i database/seed.sql
```

`backend/appsettings.Development.json` ya apunta por defecto a `(localdb)\MSSQLLocalDB` con `Trusted_Connection=True`, así que no se necesita variable de entorno para la cadena de conexión en local. La clave JWT de desarrollo también viene hardcodeada ahí (no usarla en producción).

### 12.3 Backend

```bash
cd backend
dotnet run
# Escucha en http://0.0.0.0:5025 (ver Properties/launchSettings.json)
# Swagger disponible en http://localhost:5025/swagger
```

### 12.4 Frontend

```bash
cd frontend
npm install
npm run serve
# vue-cli-service sirve en http://localhost:8080 (por defecto) y proxea
# /api y /hubs hacia http://localhost:5025 según vue.config.js
```

Con ambos corriendo, entrar a `http://localhost:8080`, iniciar sesión con el usuario admin sembrado, y el sistema forzará el cambio de contraseña temporal en el primer login.

---

## 13. Flujos de negocio clave

### 13.1 Ciclo de vida de un activo

```mermaid
stateDiagram-v2
    [*] --> Activo: Crear (POST /api/activo)
    Activo --> Mantenimiento: Editar estado
    Mantenimiento --> Activo: Editar estado
    Activo --> Desecho: Editar estado (fija FechaDesecho = hoy)
    Mantenimiento --> Desecho: Editar estado
    Desecho --> Activo: Rechazar baja (Administradora)\nlimpia FechaDesecho
    Desecho --> [*]: Eliminar definitivo\n(solo Administradora, ≥365 días en Desecho)
    Activo --> [*]: Eliminar reciente\n(cualquier rol autorizado, ventana de 10 min tras creación)
```

### 13.2 Cambio de ubicación/encargado (histórico inmutable)

Cuando se edita un activo y cambia su ubicación o su encargado, **no se actualiza la fila `Ubicacion` existente**: se inserta una fila nueva con `Anterior` = valor viejo, `Actual` = valor nuevo, y el `Activo` se re-apunta a esa fila nueva. La fila vieja queda en la tabla como registro histórico (ya no referenciada por ningún `Activo`, pero consultable). Cada tipo de cambio (ubicación vs. encargado) genera su propia entrada de `Historial`.

### 13.3 Flujo de solicitudes de cambio

Este es el flujo de aprobación más importante del sistema, pensado para que la `JefaAdministrativa` pueda **proponer** cambios sin tener permiso de edición directa:

```mermaid
sequenceDiagram
    participant JA as JefaAdministrativa
    participant API as SolicitudCambioController
    participant DB as Base de datos
    participant Hub as SignalR (grupo GTIAdmin)
    participant GTI as GTI / Administradora

    JA->>API: POST /api/solicitudcambio (datos propuestos)
    API->>DB: Guarda SolicitudCambio (Estado=Pendiente, snapshot original + propuesto en JSON)
    API->>DB: Historial: SolicitudCambio
    API->>Hub: NotificarGTIAdminAsync("solicitud_cambio")
    Hub-->>GTI: Notificación push en tiempo real
    GTI->>API: POST /{id}/aprobar  o  POST /{id}/rechazar
    alt Aprobar
        API->>DB: Aplica los DatosNuevos al Activo real
        API->>DB: Si cambió ubicación/encargado, crea nueva fila Ubicacion
        API->>DB: SolicitudCambio.Estado = Aprobada
        API->>DB: Historial: SolicitudAprobada
    else Rechazar
        API->>DB: SolicitudCambio.Estado = Rechazada (+ Comentario)
        API->>DB: Historial: SolicitudRechazada
    end
```

Una `JefaAdministrativa` solo puede tener el efecto de sus cambios reflejado si GTI/Administradora aprueba; mientras tanto el activo real **no cambia**. `GET /api/solicitudcambio/activo/{placa}/pendiente` permite a la UI (`ActivoModal`) avisar si ya hay una solicitud pendiente sobre ese activo, para no permitir duplicados confusos.

### 13.4 Flujo de baja/desecho

1. Alguien con permiso edita el `Activo.Estado` a `Desecho` → se fija `FechaDesecho = hoy`.
2. El activo aparece en `DesechoView` (`GET /api/desecho`), que calcula `diasEnDesecho` y `puedeEliminar` (≥365 días) en el propio DTO de respuesta.
3. Una `Administradora` puede:
   - **Rechazar** (`POST /api/desecho/{placa}/rechazar`): vuelve el activo a `Estado = Activo`, limpia `FechaDesecho`.
   - **Aprobar** (`POST /api/desecho/{placa}/aprobar`, solo si ≥365 días): registra `Historial: Aprobacion` y **borra el activo y todas sus dependencias** de forma permanente.

### 13.5 Recuperación e importación masiva desde Excel

1. El usuario descarga la plantilla `.xlsx` (generada en el navegador con SheetJS, columnas fijas: Placa, TipoPlaca, Articulo, Marca, Modelo, NumSerial, CategoriaNombre, UbicacionActual, EncargadoNombre, Observaciones).
2. Sube el archivo lleno; `ImportarActivosModal.vue` lo parsea **en el cliente** con SheetJS y hace una validación local básica (columnas presentes, filas no vacías).
3. Envía el array de filas parseadas a `POST /api/activo/importar`.
4. El backend procesa **fila por fila, cada una en su propia transacción**: resuelve `CategoriaNombre` → `CategoriaId` y `EncargadoNombre` → `EncargadoId` (fallando si el nombre no existe o es ambiguo entre varios encargados), y reporta éxito/error por fila con mensajes en español traducidos desde los errores crudos de SQL Server.

---

## 14. Matriz de permisos por rol

Basada en los atributos `[Authorize(Roles = "...")]` de cada controller (fuente de verdad: el backend).

| Acción | Administradora | GTI | JefaAdministrativa | Invitado |
|---|:---:|:---:|:---:|:---:|
| Ver inventario, categorías, dashboard, lista de Desecho | ✅ | ✅ | ✅ | ✅ |
| Ver vista rápida (modal) de un activo | ✅ | ✅ | ✅ | ✅ |
| Ver página de detalle completo de un activo | ✅ | ✅ | ✅ | ❌ |
| Ver historial / auditoría | ✅ | ✅ | ✅ | ❌ |
| Crear / importar activos | ✅ | ✅ | ✅ | ❌ |
| Editar activo directamente | ✅ | ✅ | ❌ (debe usar Solicitud de Cambio) | ❌ |
| Cambiar placa de un activo | ✅ | ✅ | ❌ | ❌ |
| Eliminar activo reciente (<10 min) | ✅ | ✅ | ✅ | ❌ |
| Eliminar activo definitivo (Desecho ≥1 año) | ✅ | ❌ | ❌ | ❌ |
| Crear solicitud de cambio | ❌ (edita directo) | ❌ (edita directo) | ✅ | ❌ |
| Aprobar / rechazar solicitud de cambio | ✅ | ✅ | ❌ | ❌ |
| Aprobar / rechazar baja (desecho) | ✅ | ❌ | ❌ | ❌ |
| Gestionar categorías (crear/editar/borrar) | ✅ | ❌ | ❌ | ❌ |
| Crear encargados | ✅ | ✅ | ✅ | ❌ |
| Editar encargados | ✅ | ✅ | ❌ | ❌ |
| Eliminar encargados / reasignar activos | ✅ (eliminar) / ✅ (reasignar, junto con GTI) | ✅ (reasignar) | ❌ | ❌ |
| Gestionar usuarios del sistema | ✅ | ❌ | ❌ | ❌ |

---

## 15. Decisiones de diseño y por qué existen

- **Sin EF Core Migrations, DDL plano**: el equipo prefirió controlar el SQL exacto que corre en SQL Server (constraints, checks) en vez de dejarlo en manos del generador de migraciones de EF. Costo: cualquier cambio de esquema requiere tocar `SIBI.sql`/un script de migración **y** las clases `Models`/`SibiDbContext` a mano, manteniéndolos sincronizados manualmente.
- **Ubicación histórica append-only en vez de una tabla `MovimientoUbicacion` separada**: reutiliza la misma tabla como snapshot "actual + anterior", evitando un join extra para el caso común (mostrar dónde está y dónde estuvo un activo), a costa de que un historial de más de 2 movimientos requiere reconstruirlo desde `Historial.Descripcion` (texto libre), no desde `Ubicacion`.
- **Placa como PK con soporte de "renombrar"**: en vez de tener un ID interno separado de la placa física, se aceptó la complejidad de `CambiarPlaca` (recrear fila) para que la placa (que en el mundo real puede reasignarse/corregirse) siga siendo la clave natural y visible del sistema.
- **Solicitudes de cambio con JSON en vez de tabla estructurada**: permite que el "shape" de los datos propuestos evolucione sin migrar la tabla `SolicitudCambio`, a costa de perder validación a nivel de base de datos sobre esos campos.
- **Frontend sin librería de componentes (solo Tailwind)**: todo el UI (modales, selects, tablas) está hecho a mano. Da control total de estilo pero significa que patrones como el `SearchableSelect` o `AppDialog` son "hechos en casa" y hay que mantenerlos en vez de actualizarlos vía dependencia.
- **Importación de Excel parseada en el cliente**: evita subir archivos binarios al backend y mantiene la lógica de parseo cerca de la UI que también genera la plantilla, pero implica que el backend confía en la forma del JSON que le llega (por eso valida cada campo igual server-side).

---

## 16. Deuda técnica y limitaciones conocidas

Documentado explícitamente para que quien retome el proyecto no las redescubra por sorpresa:

1. **Envío de correo no implementado.** `AuthController.Recuperar` y `UsuarioController.Crear/Desbloquear/ResetContrasena` generan una contraseña temporal y la **devuelven en el cuerpo de la respuesta HTTP** en vez de enviarla por correo institucional. Hay configuración SMTP en `appsettings.json` (`Email:SmtpHost`, etc.) que **no se usa en ningún lado del código actual** — es un placeholder para una integración futura (probablemente Gmail SMTP con app password, a juzgar por `appsettings.Development.json`). Mientras tanto, quien opera el sistema debe comunicar la contraseña temporal manualmente por otro canal, lo cual es un riesgo de seguridad si no se hace con cuidado.
2. **Endpoint de debug removido en el working tree actual**: había un `GET /api/auth/hash-temp` en `AuthController` que devolvía un hash BCrypt de una contraseña hardcodeada (`"CambiarEsto123!"`) — un endpoint público de generación de hash, sin autenticación. Fue eliminado en cambios locales sin commitear todavía; si se reintroduce algo similar para debug, debe protegerse o eliminarse antes de producción.
3. **CORS de producción fija `http://` (no `https://`)** para `eic.sibi.ucr.ac.cr` en `Program.cs`. Si el sitio se sirve por HTTPS (recomendado, y necesario para que el navegador no marque el login como inseguro), este origen no calzará y el navegador bloqueará las llamadas a la API. Verificar y corregir a `https://` si aplica.
4. **Generación de contraseñas temporales duplicada**: la misma función (8 caracteres, 1 mayúscula/1 minúscula/1 dígito garantizados, resto aleatorio) está copiada en `AuthService.GenerarContrasenaAleatoria` y `UsuarioController.GenerarContrasenaTemp`. Candidata a extraer a un servicio compartido si se vuelve a tocar.
5. **`System.Random` para contraseñas temporales**: no es criptográficamente seguro. Aceptable dado que son contraseñas de un solo uso, forzadas a cambiar en el primer login, pero si se quiere endurecer, migrar a `RandomNumberGenerator` (`System.Security.Cryptography`).
6. **Sin tests automatizados** en el repositorio (no se encontró carpeta de tests ni proyecto `*.Tests`). Cualquier cambio debe verificarse manualmente contra Swagger/la UI.
7. **Sin CI configurado** (no hay workflows en `.github/workflows`, la carpeta existe pero está vacía). Los checks de build/lint son responsabilidad manual del desarrollador antes de hacer push.
8. **`backend/backend/` es un directorio anidado accidental** (contiene `Controllers/`, `Properties/`, `bin/`, `obj/` duplicados) visible en el listado del repo — probablemente resultado de un `dotnet new`/copy mal ejecutado en algún momento. No forma parte del proyecto real (el `.csproj` activo es `backend/backend.csproj`); vale la pena confirmarlo y limpiarlo si efectivamente es basura, revisando antes que no esté referenciado por la solución (`backend.sln`).
9. **Notificaciones no persistentes**: solo llegan mientras la pestaña del navegador tiene la conexión SignalR activa; no hay tabla de notificaciones en base de datos ni "marcar como leída" persistente entre sesiones.

---

## 17. Troubleshooting

| Síntoma | Causa probable | Dónde mirar |
|---|---|---|
| Backend no arranca, excepción `Jwt:Key no está configurado` | Falta `Jwt__Key` (Docker) o `Jwt:Key` (appsettings) | `Program.cs`, `.env`, `appsettings.Development.json` |
| `docker compose up` se queda esperando el backend indefinidamente | El healthcheck de `db` no pasa (contraseña `sa` no cumple la política de complejidad de SQL Server) | Revisar `MSSQL_SA_PASSWORD` en `.env`, logs de `docker compose logs db` |
| Login funciona pero SignalR nunca conecta / no llegan notificaciones | Token no llega al hub, o CORS bloqueando el handshake | Verificar que la ruta pase por `/hubs/` en el proxy (nginx o dev-server), y el hook `OnMessageReceived` en `Program.cs` |
| Frontend en producción no puede llamar a la API (bloqueado por CORS) | Origen no coincide (`http` vs `https`) — ver limitación [16.3](#16-deuda-técnica-y-limitaciones-conocidas) | `Program.cs`, política `FrontendVue` |
| Importación de Excel falla toda con "categoría no encontrada" | Nombres de categoría en el Excel no coinciden exactamente (case-insensitive, pero sin tildes/espacios extra) con `Categoria.Nombre` en base de datos | `ActivoController.Importar`, tabla `Categoria` |
| No se puede eliminar un activo en Desecho | Aún no cumple los 365 días desde `FechaDesecho`, o el rol no es `Administradora` | `ActivoController.Eliminar` / `DesechoController.AprobarEliminacion` |
| Usuario bloqueado tras 3 intentos fallidos | Comportamiento esperado | `Administradora` debe llamar `POST /api/usuario/{correo}/desbloquear` |
| `dotnet run` local no conecta a la base | LocalDB no instalado/iniciado, o cadena de conexión apunta a otro servidor | `appsettings.Development.json`, `sqllocaldb info` |

---

## 18. Dónde tocar el código para tareas comunes

- **Agregar un campo nuevo a `Activo`**: 1) columna en `SIBI.sql` (o script `migrate_*.sql` nuevo si la base ya existe en producción), 2) propiedad en `Models/Activo.cs`, 3) mapeo si hace falta en `SibiDbContext`, 4) agregar el campo a `ActivoDto`, `CrearActivoRequest`, `EditarActivoRequest` en `DTOs/`, 5) actualizar `ActivoController.MapToDto` y los métodos `Crear`/`Editar`, 6) reflejar en `ActivoModal.vue` (formulario) e `InventarioView.vue`/`ActivoDetalleView.vue` (si se muestra en tabla/detalle).
- **Agregar un nuevo tipo de acción auditable**: agregar el valor al `CHECK CHK_Historial_TipoAccion` en SQL, y llamar `HistorialService.RegistrarAsync(..., tipoAccion: "TuNuevoTipo", ...)` desde el controller correspondiente.
- **Agregar un rol nuevo**: 1) agregarlo al `CHECK CHK_Usuario_Permisos` en SQL, 2) decidir sus permisos y reflejarlos en los atributos `[Authorize(Roles = "...")]` de cada controller/acción, 3) agregar getters en `stores/auth.js` (ej. `esNuevoRol`), 4) usarlos en `router/index.js` (nuevos `meta` flags si aplica) y en las vistas para condicionar UI.
- **Agregar un endpoint que dispara una notificación**: inyectar `NotificacionService` en el controller y llamar `NotificarGTIAdminAsync`/`NotificarAdminAsync` (o agregar un grupo nuevo en `NotificacionHub.OnConnectedAsync` si el destinatario no es GTI/Admin).
- **Cambiar la plantilla de importación de Excel**: las columnas están definidas en `ImportarActivosModal.vue` (función de descarga de plantilla) y deben mantenerse en sincronía con lo que `ActivoController.Importar` espera en `ImportarActivoFilaRequest`.
- **Implementar envío real de correo**: ya existe la sección `Email` en `appsettings.json`/`appsettings.Development.json` lista para usar (SMTP host/puerto/usuario/contraseña); falta el servicio que la consuma y los puntos donde reemplazar el "devolver contraseña en la respuesta HTTP" por "enviarla por correo" (`AuthService.RecuperarContrasenaAsync`, `UsuarioController.Crear/Desbloquear/ResetContrasena`).
