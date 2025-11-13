-- Script de inicialización de base de datos para Docker
-- Este script se ejecutará automáticamente al crear la base de datos

USE master;
GO

-- Crear la base de datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'NPSApplicationDB')
BEGIN
    CREATE DATABASE NPSApplicationDB;
    PRINT 'Base de datos NPSApplicationDB creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Base de datos NPSApplicationDB ya existe';
END
GO

USE NPSApplicationDB;
GO

-- Verificar que estamos en la base de datos correcta
SELECT DB_NAME() AS CurrentDatabase;
GO

PRINT 'Base de datos lista para usar';
GO
