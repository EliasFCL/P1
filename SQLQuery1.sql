CREATE DataBase Carniceria;

USE Carniceria;

CREATE TABLE Clientes (
id UNIQUEIDENTIFIER  PRIMARY KEY,
tipoIdentificacion NVARCHAR(50),
numId NVARCHAR(50) NOT NULL,
nombre NVARCHAR(100) NOT NULL,
apellido1 NVARCHAR(100) NOT NULL,
apellido2 NVARCHAR(100) NOT NULL,
fechaNacimiento DATETIME NOT NULL
);

Select * FROM Clientes;