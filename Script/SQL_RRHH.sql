----------------------------------------------------------
-- SCRIPT: SQL_RRHH.sql
-- Prpósito:	Crea la Base de datos de Recursos Humanos
--				Crea la tabla Employees
-- Fecha: 16/12/2025
-- Fecha de actualización: N/A
-- Autor: Ing. Ivan Marínez Aguas
----------------------------------------------------------

USE Master
GO

IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'RRHH')
BEGIN
	CREATE DATABASE RRHH
END
GO

USE RRHH
GO

IF NOT EXISTS (
    SELECT 1 
    FROM sys.tables 
    WHERE name = 'Employees' AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
	CREATE TABLE Employees
	(
		Id INT IDENTITY(1,1) PRIMARY KEY,
		FullName VARCHAR(150) NOT NULL,
		HireDate DATE NOT NULL,
		Position VARCHAR(100) NOT NULL,
		Salary DECIMAL(18,2) NOT NULL CHECK(Salary > 0),
		Department VARCHAR(100) NOT NULL,
	)
END
GO

INSERT INTO Employees
(
    FullName,
    HireDate,
    Position,
    Salary,
    Department
)
VALUES
(
    'SALAS NIETO JOSE AURELIO',
    '2025-01-04',
    'CONTADOR',
    7800000,
    'CONTABILIDAD'
)
GO


