-- Script para insertar datos iniciales
-- Ejecutar después de crear las tablas e índices

-- Insertar Roles
INSERT INTO Roles (Name, Permissions, CreatedAt)
VALUES 
    ('Admin', '["products:read", "products:write", "products:delete", "categories:read", "categories:write", "categories:delete", "users:read", "users:write", "bulk-import:execute"]', CURRENT_TIMESTAMP),
    ('User', '["products:read", "categories:read"]', CURRENT_TIMESTAMP)
ON CONFLICT (Name) DO NOTHING;

-- NOTA: Los usuarios se crearán mediante DatabaseSeeder en tiempo de ejecución
-- para poder generar los hashes de contraseñas correctamente con BCrypt
-- Usuarios por defecto:
-- - admin / Admin123! (rol Admin)
-- - user / User123! (rol User)

-- Insertar Categorías base
INSERT INTO Categories (Name, Description, ImageUrl, CreatedAt)
VALUES 
    ('SERVIDORES', 'Servidores físicos y virtuales para infraestructura empresarial', 'https://example.com/images/servidores.jpg', CURRENT_TIMESTAMP),
    ('CLOUD', 'Soluciones cloud y servicios en la nube', 'https://example.com/images/cloud.jpg', CURRENT_TIMESTAMP)
ON CONFLICT (Name) DO NOTHING;

