USE master
GO

-- Para cerrar las conexiones que tenga abiertas la BD y se pueda eliminar
ALTER DATABASE Carniceria SET SINGLE_USER WITH ROLLBACK IMMEDIATE

DROP DATABASE IF EXISTS Carniceria
GO

CREATE DATABASE Carniceria
GO

USE Carniceria
GO

--Borrar datos y tablas por jerarquía
IF OBJECT_ID('DetallesFactura', 'U') IS NOT NULL
BEGIN
    DELETE FROM DetallesFactura;

    DROP TABLE DetallesFactura;
END

IF OBJECT_ID('Facturas', 'U') IS NOT NULL
BEGIN
    DELETE FROM Facturas;

    DROP TABLE Facturas;
END
GO

IF OBJECT_ID('Productos', 'U') IS NOT NULL
BEGIN
    DELETE FROM Productos;

    DROP TABLE Productos;
END
GO

IF OBJECT_ID('Proveedores', 'U') IS NOT NULL
BEGIN
    DELETE FROM Proveedores;

    DROP TABLE Proveedores;
END
GO

IF OBJECT_ID('Empleados', 'U') IS NOT NULL
BEGIN
    DELETE FROM Empleados;

    DROP TABLE Empleados;
END
GO

IF OBJECT_ID('Clientes', 'U') IS NOT NULL
BEGIN
    DELETE FROM Clientes;

    DROP TABLE Clientes;
END
GO

IF OBJECT_ID('Puestos', 'U') IS NOT NULL
BEGIN
    DELETE FROM Puestos;

    DROP TABLE Puestos;
END
GO

IF OBJECT_ID('AreaTrabajo', 'U') IS NOT NULL
BEGIN
    DELETE FROM AreaTrabajo;

    DROP TABLE AreaTrabajo;
END
GO

--Tabla Clientes
CREATE TABLE Clientes (
    id UNIQUEIDENTIFIER PRIMARY KEY,
    tipoIdentificacion NVARCHAR(50),
    numId NVARCHAR(50) NOT NULL,
    nombre NVARCHAR(100) NOT NULL,
    apellido1 NVARCHAR(100) NOT NULL,
    apellido2 NVARCHAR(100) NOT NULL,
    fechaNacimiento DATETIME NOT NULL
);

--Tabla para las áreas de trabajo
CREATE TABLE AreaTrabajo (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    nombre NVARCHAR(100) NOT NULL UNIQUE
);

--Tabla para los puestos
CREATE TABLE Puestos (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    nombre NVARCHAR(100) NOT NULL,
    AreaTrabajoId UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (AreaTrabajoId) REFERENCES AreaTrabajo(Id)
);

--Tabla Empleados (Para que funcione se deben insertar los datos de las tablas AreaTrabajo y Puestos al final del Script)
CREATE TABLE Empleados (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    tipoIdentificacion NVARCHAR(50) NOT NULL,
    numId NVARCHAR(50) NOT NULL UNIQUE,
    nombre NVARCHAR(100) NOT NULL,
    apellido1 NVARCHAR(100) NOT NULL,
    apellido2 NVARCHAR(100) NOT NULL,
    fechaNacimiento DATETIME NULL,
    salario DECIMAL(18,2) NOT NULL,
    fechaIngreso DATETIME NOT NULL,
    turno NVARCHAR(50) NOT NULL,
    PuestoId UNIQUEIDENTIFIER NOT NULL,
    areaTrabajo NVARCHAR(100) NOT NULL,
    cargo NVARCHAR(100) NOT NULL,
    FOREIGN KEY (PuestoId) REFERENCES Puestos(Id)
);

--Tabla proveedores
CREATE TABLE Proveedores (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    tipoIdentificacion NVARCHAR(50) NOT NULL,
    numId NVARCHAR(50) NOT NULL UNIQUE,
    nombre NVARCHAR(100) NOT NULL,
    RazonSocial NVARCHAR(150) NOT NULL,
    Telefono INT NOT NULL,
    CorreoElectronico NVARCHAR(100) NOT NULL,
    TipoSuministro NVARCHAR(50) NOT NULL,
    FrecuenciaEntrega NVARCHAR(50) NOT NULL,
    apellido1 NVARCHAR(100) NULL,
    apellido2 NVARCHAR(100) NULL,
    fechaNacimiento DATETIME NULL
);

--Tabla productos
CREATE TABLE Productos (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    codigo NVARCHAR(50) NOT NULL UNIQUE,
    nombre NVARCHAR(100) NOT NULL,
    categoria NVARCHAR(50) NOT NULL,
    unidadMedida NVARCHAR(50) NOT NULL,
    precio DECIMAL(18,2) NOT NULL,
    existenciaActual DECIMAL(18,2) NOT NULL,
    existenciaMinima DECIMAL(18,2) NOT NULL,
    ProveedorId UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (ProveedorId) REFERENCES Proveedores(Id)
);

--Tabla facturas
CREATE TABLE Facturas (
    numeroFactura NVARCHAR(50) PRIMARY KEY,
    fecha DATETIME NOT NULL,
    ClienteId UNIQUEIDENTIFIER NOT NULL,
    EmpleadoId UNIQUEIDENTIFIER NOT NULL,
    metodoPago NVARCHAR(50) NOT NULL,
    descuento DECIMAL(5,2) NOT NULL,
    estado NVARCHAR(50) NOT NULL,
    subtotal DECIMAL(18,2) NOT NULL,
    montoDescuento DECIMAL(18,2) NOT NULL,
    baseGravable DECIMAL(18,2) NOT NULL,
    iva DECIMAL(18,2) NOT NULL,
    total DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (ClienteId) REFERENCES Clientes(Id),
    FOREIGN KEY (EmpleadoId) REFERENCES Empleados(Id)
);

--Tabla detalles
CREATE TABLE DetallesFactura (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    numeroFactura NVARCHAR(50) NOT NULL, 
    ProductoId UNIQUEIDENTIFIER NOT NULL, 
    unidadMedida NVARCHAR(50) NOT NULL,
    cantidad DECIMAL(18,2) NOT NULL,
    precioUnitario DECIMAL(18,2) NOT NULL, 
    subtotal DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (numeroFactura) REFERENCES Facturas(numeroFactura) ON DELETE CASCADE, 
    FOREIGN KEY (ProductoId) REFERENCES Productos(Id)
);

--Clientes
INSERT INTO Clientes (id,tipoIdentificacion,numId,nombre,apellido1,apellido2,fechaNacimiento)
VALUES
('11111111-2222-3333-4444-555555555555','Cédula','1-1111-1111','Jessica','Ramirez','Quiros','2004-04-04'),
('AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEEE','DIMEX','123456654321','María','Antonieta','Nieves','1995-05-02');

--Necesarios para registrar al empleado
INSERT INTO AreaTrabajo (Id, nombre) VALUES 
('11111111-1111-1111-1111-111111111111', 'Administracion'), 
('11111111-1111-1111-1111-111111111112', 'Ventas y Caja'),
('11111111-1111-1111-1111-111111111113', 'Servicio al cliente'),
('11111111-1111-1111-1111-111111111114', 'Corte y Preparacion'), 
('11111111-1111-1111-1111-111111111115', 'Bodega e Inventario'),
('11111111-1111-1111-1111-111111111116', 'Distribucion'),
('11111111-1111-1111-1111-111111111117', 'Limpieza');

INSERT INTO Puestos (Id, nombre, AreaTrabajoId) VALUES 
('22222222-2222-2222-2222-222222222222', 'Administrador', '11111111-1111-1111-1111-111111111111'),
('22222222-2222-2222-2222-222222222223', 'Asistente administrativo', '11111111-1111-1111-1111-111111111111'),
('22222222-2222-2222-2222-222222222224', 'Cajero', '11111111-1111-1111-1111-111111111112'),
('22222222-2222-2222-2222-222222222225', 'Vendedor', '11111111-1111-1111-1111-111111111112'),
('22222222-2222-2222-2222-222222222226', 'Servicio al cliente', '11111111-1111-1111-1111-111111111113'),
('22222222-2222-2222-2222-222222222227', 'Carnicero', '11111111-1111-1111-1111-111111111114'),
('22222222-2222-2222-2222-222222222228', 'Despostador', '11111111-1111-1111-1111-111111111114'),
('22222222-2222-2222-2222-222222222229', 'Empacador', '11111111-1111-1111-1111-111111111114'),
('22222222-2222-2222-2222-222222222230', 'Bodeguero', '11111111-1111-1111-1111-111111111115'),
('22222222-2222-2222-2222-222222222231', 'Auxiliar de inventario', '11111111-1111-1111-1111-111111111115'),
('22222222-2222-2222-2222-222222222232', 'Conductor', '11111111-1111-1111-1111-111111111116'),
('22222222-2222-2222-2222-222222222233', 'Ayudante de distribución', '11111111-1111-1111-1111-111111111116'),
('22222222-2222-2222-2222-222222222234', 'Personal de limpieza', '11111111-1111-1111-1111-111111111117');

--Empleados
INSERT INTO Empleados (Id,tipoIdentificacion,numId,nombre,apellido1,apellido2,fechaNacimiento,salario,fechaIngreso,turno,PuestoId,areaTrabajo,cargo)
VALUES
('22222222-3333-4444-5555-666666666666','Cédula','2-2222-2222','Julian','Ramírez','Zamora','2003-07-18',650000.00,'2023-10-10','Noche','22222222-2222-2222-2222-222222222224','Ventas y Caja','Cajero'),
('BBBBBBBB-CCCC-DDDD-EEEE-FFFFFFFFFFFF','DIMEX','909090878787','Ellionor','Ramierez','Santamaria','1990-01-22',750000.00,'2021-03-01','Tarde','22222222-2222-2222-2222-222222222230','Bodega e Inventario','Bodeguero');

--Provedores
INSERT INTO Proveedores (Id, tipoIdentificacion, numId, nombre, RazonSocial, Telefono, CorreoElectronico, TipoSuministro, FrecuenciaEntrega, apellido1, apellido2, fechaNacimiento)
VALUES
('33333333-4444-5555-6666-777777777777', 'Cédula jurídica', '8-987-76597', 'Distribuidora el bosque', 'Distribuidora el bosque SA', 23455432, 'Distribosq@gmail.com', 'Empaques', 'Quincenal','AAAA' , 'BBBB', '1977-01-15'),
('CCCCCCCC-DDDD-EEEE-FFFF-AAAAAAAAAAAA', 'Cédula nacional', '6-8762-6970', 'Alberto', 'Proveedor de pollo', 12344321, 'Alber@gmail.com', 'Pollo', 'Diaria', 'Gomez', 'Bolaños', '1980-02-25');

--Productos
INSERT INTO Productos (Id, codigo, nombre, categoria, unidadMedida, precio, existenciaActual, existenciaMinima, ProveedorId)
VALUES
('44444444-5555-6666-7777-888888888888', 'P1001', 'Lomo', 'Carne de res', 'Unidad', 3200.00, 500.00, 100.00, '33333333-4444-5555-6666-777777777777'),
('22222222-3333-4444-5555-666666666666', 'P2002', 'Pechuga', 'Pollo', 'Bandeja', 2500.00, 150.00, 30.00, 'CCCCCCCC-DDDD-EEEE-FFFF-AAAAAAAAAAAA');

--Facturas
INSERT INTO Facturas (numeroFactura, fecha, ClienteId, EmpleadoId, metodoPago, descuento, estado, subtotal, montoDescuento, baseGravable, iva, total)
VALUES
('F-AAA111', '2026-11-26 10:30:00', '11111111-2222-3333-4444-555555555555', '22222222-3333-4444-5555-666666666666', 'Efectivo', 5.00, 'Pendiente', 128000.00, 6400.00, 121600.00, 15808.00, 137408.00),
('F-BBB222', '2026-11-26 14:45:00', 'AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEEE', 'BBBBBBBB-CCCC-DDDD-EEEE-FFFFFFFFFFFF', 'Tarjeta', 2.00, 'Pagada', 12500.00, 0.00, 12500.00, 1625.00, 14125.00);

--DetallesFactura
INSERT INTO DetallesFactura (Id, numeroFactura, ProductoId, unidadMedida, cantidad, precioUnitario, subtotal)
VALUES
('AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEEE', 'F-AAA111', '44444444-5555-6666-7777-888888888888', 'Unidad', 40.00, 3200.00, 128000.00),
('BBBBBBBB-CCCC-DDDD-EEEE-FFFFFFFFFFFF', 'F-BBB222', '22222222-3333-4444-5555-666666666666', 'Kilogramo', 5.00, 2500.00, 12500.00);