-- Script para crear las tablas de la base de datos
-- Ejecutar este script primero

-- Tabla de Roles
CREATE TABLE IF NOT EXISTS Roles (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE,
    Permissions TEXT NOT NULL DEFAULT '[]',
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Tabla de Usuarios
CREATE TABLE IF NOT EXISTS Users (
    Id SERIAL PRIMARY KEY,
    Username VARCHAR(100) NOT NULL UNIQUE,
    Email VARCHAR(255) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    RoleId INTEGER NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- Tabla de Categorías
CREATE TABLE IF NOT EXISTS Categories (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL UNIQUE,
    Description TEXT,
    ImageUrl VARCHAR(500),
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP
);

-- Tabla de Productos
CREATE TABLE IF NOT EXISTS Products (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    Price DECIMAL(18, 2) NOT NULL CHECK (Price >= 0),
    Stock INTEGER NOT NULL DEFAULT 0 CHECK (Stock >= 0),
    CategoryId INTEGER NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);

-- Tabla de Trabajos de Importación Masiva
CREATE TABLE IF NOT EXISTS BulkImportJobs (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Status VARCHAR(50) NOT NULL DEFAULT 'Pending',
    TotalRecords INTEGER NOT NULL DEFAULT 0,
    ProcessedRecords INTEGER NOT NULL DEFAULT 0,
    FailedRecords INTEGER NOT NULL DEFAULT 0,
    StartedAt TIMESTAMP,
    CompletedAt TIMESTAMP,
    ErrorMessage TEXT,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

