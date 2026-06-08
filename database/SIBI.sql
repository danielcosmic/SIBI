-- ============================================================
-- SIBI - Sistema de Inventario de Bienes Institucionales
-- Universidad de Costa Rica - Escuela de Ingeniería Civil
-- ============================================================

-- 1. Creación de la Base de Datos
CREATE DATABASE SIBI;
GO

USE SIBI;
GO

-- 2. Tabla Placa
--    Registra el número de placa y su tipo (institucional o interno).
CREATE TABLE Placa (
    Numero  VARCHAR(8)  NOT NULL,
    Tipo    VARCHAR(15) NOT NULL,

    CONSTRAINT PK_Placa      PRIMARY KEY (Numero),
    CONSTRAINT CHK_Placa_Tipo CHECK (Tipo IN ('Institucional', 'Interno'))
);

-- 3. Tabla Usuario
--    Cuentas de acceso al sistema. Solo se permiten correos @ucr.ac.cr.
CREATE TABLE Usuario (
    Nombre                VARCHAR(50)  NOT NULL,
    Correo                VARCHAR(50)  NOT NULL,
    Contrasena            VARCHAR(255) NOT NULL,
    Permisos              VARCHAR(20)  NOT NULL,
    EsContrasenaTemporal  BIT          NOT NULL DEFAULT 0,
    IntentosFallidos      INT          NOT NULL DEFAULT 0,
    Activo                BIT          NOT NULL DEFAULT 1,

    CONSTRAINT PK_Usuario          PRIMARY KEY (Correo),
    CONSTRAINT CHK_Usuario_Permisos CHECK (Permisos IN ('Administradora', 'GTI', 'JefaAdministrativa', 'Invitado')),
    CONSTRAINT CHK_Usuario_Correo   CHECK (Correo LIKE '%@ucr.ac.cr')
);

-- 4. Tabla Encargado
--    Personas responsables de activos. Pueden existir sin cuenta de sistema.
CREATE TABLE Encargado (
    Id     UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Encargado_Id DEFAULT NEWID(),
    Nombre VARCHAR(50)      NOT NULL,
    Rol    VARCHAR(30)      NOT NULL,

    CONSTRAINT PK_Encargado PRIMARY KEY (Id)
);

-- 5. Tabla Categoria
--    Categorías de activos administradas por la Administradora.
CREATE TABLE Categoria (
    Id     INT          IDENTITY(1,1) NOT NULL,
    Nombre VARCHAR(30)  NOT NULL,
    Icono  VARCHAR(30)  NULL,

    CONSTRAINT PK_Categoria      PRIMARY KEY (Id),
    CONSTRAINT UQ_Categoria_Nombre UNIQUE (Nombre)
);

-- 6. Tabla Ubicacion
--    Almacena la ubicación actual y la anterior de un activo,
--    junto con los encargados correspondientes a cada una.
CREATE TABLE Ubicacion (
    Id                  BIGINT           IDENTITY(1,1) NOT NULL,
    Actual              VARCHAR(30)      NOT NULL,
    Anterior            VARCHAR(30)      NOT NULL,
    EncargadoActual     UNIQUEIDENTIFIER NOT NULL,
    EncargadoAnterior   UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT PK_Ubicacion PRIMARY KEY (Id),

    CONSTRAINT FK_Ubicacion_EncargadoActual
        FOREIGN KEY (EncargadoActual)   REFERENCES Encargado(Id),

    CONSTRAINT FK_Ubicacion_EncargadoAnterior
        FOREIGN KEY (EncargadoAnterior) REFERENCES Encargado(Id)
);

-- 7. Tabla Activo
--    Tabla central del sistema. Registra cada bien institucional.
--    Longitudes de campo ajustadas a las reglas de negocio del mockup.
CREATE TABLE Activo (
    Placa          VARCHAR(8)   NOT NULL,
    Marca          VARCHAR(30)  NOT NULL,
    Modelo         VARCHAR(20)  NOT NULL,
    NumSerial      VARCHAR(30)  NOT NULL,
    Articulo       VARCHAR(20)  NOT NULL,
    CategoriaId    INT          NOT NULL,
    Observaciones  VARCHAR(40)  NULL,
    Ubicacion      BIGINT       NOT NULL,
    Estado         VARCHAR(15)  NOT NULL DEFAULT 'Activo',
    FechaDesecho   DATE         NULL,

    CONSTRAINT PK_Activo PRIMARY KEY (Placa),

    CONSTRAINT FK_Activo_Placa
        FOREIGN KEY (Placa)       REFERENCES Placa(Numero),

    CONSTRAINT FK_Activo_Categoria
        FOREIGN KEY (CategoriaId) REFERENCES Categoria(Id),

    CONSTRAINT FK_Activo_Ubicacion
        FOREIGN KEY (Ubicacion)   REFERENCES Ubicacion(Id),

    CONSTRAINT CHK_Activo_Estado
        CHECK (Estado IN ('Activo', 'Mantenimiento', 'Desecho')),

    -- FechaDesecho solo tiene valor cuando el estado es Desecho
    CONSTRAINT CHK_Activo_FechaDesecho
        CHECK (Estado = 'Desecho' AND FechaDesecho IS NOT NULL
            OR Estado <> 'Desecho' AND FechaDesecho IS NULL)
);

-- 8. Tabla Historial
--    Registro de auditoría. Cada cambio relevante sobre un activo
--    genera una fila aquí (gestionado desde el backend, no con triggers).
CREATE TABLE Historial (
    Id            BIGINT       IDENTITY(1,1) NOT NULL,
    ActivoPlaca   VARCHAR(8)   NOT NULL,
    UsuarioCorreo VARCHAR(50)  NOT NULL,
    TipoAccion    VARCHAR(20)  NOT NULL,
    Descripcion   VARCHAR(255) NULL,
    FechaHora     DATETIME     NOT NULL DEFAULT GETDATE(),

    CONSTRAINT PK_Historial PRIMARY KEY (Id),

    CONSTRAINT FK_Historial_Activo
        FOREIGN KEY (ActivoPlaca)   REFERENCES Activo(Placa),

    CONSTRAINT FK_Historial_Usuario
        FOREIGN KEY (UsuarioCorreo) REFERENCES Usuario(Correo),

    CONSTRAINT CHK_Historial_TipoAccion
        CHECK (TipoAccion IN (
            'Creacion',
            'CambioUbicacion',
            'CambioEncargado',
            'CambioEstado',
            'CambioPlaca',
            'Eliminacion',
            'Aprobacion',
            'Rechazo'
        ))
);

-- ============================================================
-- DATOS INICIALES
-- ============================================================

-- Categorías base
INSERT INTO Categoria (Nombre, Icono) VALUES
    ('Computadoras', 'Computer'),
    ('Impresoras',   'Printer'),
    ('Muebles',      'Armchair'),
    ('Proyectores',  'Projector'),
    ('Redes',        'Network');

-- Usuario administrador inicial con contraseña temporal.
-- El hash debe reemplazarse por uno generado con BCrypt antes de primer uso.
INSERT INTO Usuario (Nombre, Correo, Contrasena, Permisos, EsContrasenaTemporal) VALUES
    ('Administrador SIBI', 'admin@ucr.ac.cr', 'HASH_PENDIENTE', 'Administradora', 1);
