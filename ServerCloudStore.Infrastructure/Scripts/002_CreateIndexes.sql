-- Script para crear índices para optimizar consultas
-- Ejecutar después de 001_CreateTables.sql

-- Índices para Users
CREATE INDEX IF NOT EXISTS IX_Users_Username ON Users(Username);
CREATE INDEX IF NOT EXISTS IX_Users_Email ON Users(Email);
CREATE INDEX IF NOT EXISTS IX_Users_RoleId ON Users(RoleId);

-- Índices para Products
CREATE INDEX IF NOT EXISTS IX_Products_CategoryId ON Products(CategoryId);
CREATE INDEX IF NOT EXISTS IX_Products_Name ON Products(Name);
CREATE INDEX IF NOT EXISTS IX_Products_Price ON Products(Price);
CREATE INDEX IF NOT EXISTS IX_Products_Stock ON Products(Stock);
CREATE INDEX IF NOT EXISTS IX_Products_CreatedAt ON Products(CreatedAt);

-- Índice compuesto para búsquedas de productos
CREATE INDEX IF NOT EXISTS IX_Products_CategoryId_Name ON Products(CategoryId, Name);

-- Índices para BulkImportJobs
CREATE INDEX IF NOT EXISTS IX_BulkImportJobs_Status ON BulkImportJobs(Status);
CREATE INDEX IF NOT EXISTS IX_BulkImportJobs_CreatedAt ON BulkImportJobs(CreatedAt);

