-- Script de inicialización de SQL Server
-- Este script crea la base de datos si no existe

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'FinalProjectDb')
BEGIN
    CREATE DATABASE FinalProjectDb;
    PRINT 'Database FinalProjectDb created successfully.';
END
ELSE
BEGIN
    PRINT 'Database FinalProjectDb already exists.';
END
GO
