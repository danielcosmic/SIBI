-- 1. Creación de la Base de Datos
CREATE DATABASE SIBI;
GO

USE SIBI;
GO

-- 2. Tabla Placa
CREATE TABLE Placa (
	Numero VARCHAR(8) NOT NULL,
	Tipo VARCHAR(15) NOT NULL,

	CONSTRAINT PK_Placa PRIMARY KEY (Numero),

	CONSTRAINT CHK_Placa_Tipo CHECK (Tipo IN ('Institucional','Interno'))
);

-- 3. Tabla Usuario
CREATE TABLE Usuario (
	Nombre VARCHAR(50) NOT NULL,
	Correo VARCHAR(50) NOT NULL,
	Contrasena VARCHAR(255) NOT NULL,
	Permisos VARCHAR(20) NOT NULL,

	CONSTRAINT PK_Usuario PRIMARY KEY (Correo)
);

-- 4. Tabla Encargado
CREATE TABLE Encargado (
	Id uniqueidentifier NOT NULL CONSTRAINT DF_Encargado_Id DEFAULT NEWID(),
	Nombre VARCHAR(50) NOT NULL,
	Rol VARCHAR(30) NOT NULL,

	CONSTRAINT PK_Encargado PRIMARY KEY (Id)
);

-- 5. Tabla Ubicacion
CREATE TABLE Ubicacion (
	Id BIGINT IDENTITY(1,1) NOT NULL,
	Actual VARCHAR(50) NOT NULL,
	Anterior VARCHAR(50) NOT NULL,
	EncargadoActual uniqueidentifier NOT NULL,
	EncargadoAnterior uniqueidentifier NOT NULL,

	CONSTRAINT PK_Ubicacion PRIMARY KEY (Id),
	
	CONSTRAINT FK_Ubicacion_EncargadoActual 
		FOREIGN KEY (EncargadoActual) REFERENCES Encargado(Id),
		
	CONSTRAINT FK_Ubicacion_EncargadoAnterior 
		FOREIGN KEY (EncargadoAnterior) REFERENCES Encargado(Id)
);

-- 6. Tabla Activo
CREATE TABLE Activo (
	Placa VARCHAR(8) NOT NULL,
	Marca VARCHAR(30) NOT NULL,
	Modelo VARCHAR(30) NOT NULL,
	NumSerial VARCHAR(50) NOT NULL,
	Articulo VARCHAR(30) NOT NULl,
	Categoria VARCHAR(30) NOT NULL,
	Observaciones VARCHAR(255) NULL,
	Ubicacion BIGINT NOT NULL,

	CONSTRAINT PK_Activo PRIMARY KEY (Placa),

	CONSTRAINT FK_Activo_Placa 
        FOREIGN KEY (Placa) REFERENCES Placa(Numero),

	CONSTRAINT FK_Activo_Ubicacion 
        FOREIGN KEY (Ubicacion) REFERENCES Ubicacion(Id)
);