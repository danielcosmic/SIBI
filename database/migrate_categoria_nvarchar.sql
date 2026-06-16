USE SIBI;
GO

-- 1. Soltar el constraint UNIQUE que depende de Nombre
ALTER TABLE Categoria DROP CONSTRAINT UQ_Categoria_Nombre;
GO

-- 2. Cambiar columnas a NVARCHAR para soportar emoji y Unicode
ALTER TABLE Categoria ALTER COLUMN Nombre NVARCHAR(30) NOT NULL;
ALTER TABLE Categoria ALTER COLUMN Icono  NVARCHAR(30) NULL;
GO

-- 3. Recrear el constraint UNIQUE en Nombre
ALTER TABLE Categoria ADD CONSTRAINT UQ_Categoria_Nombre UNIQUE (Nombre);
GO
