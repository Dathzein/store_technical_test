# ServerCloudStore - Sistema de Gestión de Productos

Sistema FullStack para gestión de productos de servidores y cloud con carga masiva asíncrona, notificaciones en tiempo real y autenticación JWT.

## 📋 Tabla de Contenidos

- [Características](#características)
- [Arquitectura](#arquitectura)
- [Tecnologías](#tecnologías)
- [Requisitos Previos](#requisitos-previos)
- [Instalación y Ejecución](#instalación-y-ejecución)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [API Endpoints](#api-endpoints)
- [Testing](#testing)
- [Docker](#docker)
- [Escalabilidad](#escalabilidad)
- [Decisiones Técnicas](#decisiones-técnicas)

## ✨ Características

### Backend (.NET 8)
- ✅ **Arquitectura por Capas**: Separación clara de responsabilidades
- ✅ **Autenticación JWT**: Sistema de roles y permisos
- ✅ **CRUD Completo**: Productos y Categorías con validaciones
- ✅ **Carga Masiva**: Procesamiento asíncrono de hasta 100k productos
- ✅ **SignalR**: Notificaciones en tiempo real del progreso de importación
- ✅ **Dapper + PostgreSQL**: Alto rendimiento con queries optimizadas
- ✅ **FluentValidation**: Validaciones robustas de DTOs
- ✅ **Logging**: Serilog con logs estructurados
- ✅ **Manejo de Errores**: Middleware centralizado de excepciones

### Frontend (React + TypeScript)
- ✅ **SPA Moderna**: React 18 con TypeScript
- ✅ **Autenticación**: Login con JWT y guards de rutas
- ✅ **CRUD de Productos**: Interfaz completa con validaciones
- ✅ **Búsqueda y Filtros**: Filtrado por categoría, precio, búsqueda de texto
- ✅ **Paginación**: Listado paginado de productos
- ✅ **Carga Masiva**: Modal con progreso en tiempo real vía SignalR
- ✅ **UI/UX Moderna**: Diseño responsivo y atractivo

## 🏗️ Arquitectura

### Arquitectura por Capas

```
┌─────────────────────────────────────────────────────────┐
│                    API Layer                             │
│  Controllers, Middleware, SignalR Hub, Program.cs       │
└─────────────────────────────────────────────────────────┘
                            ↕
┌─────────────────────────────────────────────────────────┐
│              Application Layer                           │
│  Validaciones, Excepciones, DTOs, Mapeos, Servicios    │
└─────────────────────────────────────────────────────────┘
                            ↕
┌─────────────────────────────────────────────────────────┐
│                  Domain Layer                           │
│  Entidades, Lógica de Negocio, Interfaces              │
└─────────────────────────────────────────────────────────┘
                            ↕
┌─────────────────────────────────────────────────────────┐
│            Infrastructure Layer                         │
│  DbContext, Repositorios (Dapper), SQL Queries          │
└─────────────────────────────────────────────────────────┘
                            ↕
┌─────────────────────────────────────────────────────────┐
│              Transversal Layer                          │
│  Response<T>, Mappings, Utilidades Comunes             │
└─────────────────────────────────────────────────────────┘
```

### Flujo de Datos

1. **Request** → Controller (validación de modelo)
2. **Controller** → Application Service (validación con FluentValidation)
3. **Application** → Domain Service (lógica de negocio)
4. **Domain** → Infrastructure Repository (Dapper + SQL)
5. **Repository** → PostgreSQL
6. **Response** ← Mapeo de Domain a DTO
7. **Controller** ← Response<T> estandarizada

## 🛠️ Tecnologías

### Backend
- **.NET 8**: Framework principal
- **PostgreSQL**: Base de datos relacional
- **Dapper**: Micro-ORM para alto rendimiento
- **JWT**: Autenticación y autorización
- **SignalR**: WebSocket para notificaciones en tiempo real
- **FluentValidation**: Validaciones declarativas
- **Serilog**: Logging estructurado
- **BCrypt**: Hash de contraseñas

### Frontend
- **React 18**: Librería UI
- **TypeScript**: Tipado estático
- **Vite**: Build tool moderno
- **React Router**: Enrutamiento
- **Axios**: Cliente HTTP
- **SignalR Client**: WebSocket client
- **React Hook Form**: Gestión de formularios
- **Zod**: Validación de schemas

### DevOps
- **Docker**: Containerización
- **Docker Compose**: Orquestación local
- **GitHub Actions**: CI/CD
- **Nginx**: Servidor web para frontend

## 📦 Requisitos Previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- [PostgreSQL 16+](https://www.postgresql.org/download/) (opcional si usas Docker)
- [Docker](https://www.docker.com/get-started) (opcional)

## 🚀 Instalación y Ejecución

### Opción 1: Con Docker (Recomendado)

```bash
# 1. Clonar el repositorio
git clone <repository-url>
cd finanzauto

# 2. Levantar todos los servicios
docker-compose up -d

# 3. Acceder a las aplicaciones
# Frontend: http://localhost:3000
# Backend API: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

### Opción 2: Ejecución Local

#### Backend

```bash
# 1. Configurar PostgreSQL
# Crear base de datos 'servercloudstore'

# 2. Configurar connection string
# Editar ServerCloudStore.API/appsettings.Development.json

# 3. Restaurar dependencias y ejecutar
cd ServerCloudStore.API
dotnet restore
dotnet run

# API disponible en: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

#### Frontend

```bash
# 1. Instalar dependencias
cd frontend
npm install

# 2. Configurar variables de entorno
# El archivo vite.config.ts ya tiene la configuración por defecto

# 3. Ejecutar en modo desarrollo
npm run dev

# Frontend disponible en: http://localhost:3000
```

## 📁 Estructura del Proyecto

```
finanzauto/
├── ServerCloudStore.API/                # Capa de presentación
│   ├── Controllers/                     # Endpoints REST
│   ├── Hubs/                           # SignalR hubs
│   ├── Middleware/                     # Middlewares personalizados
│   └── Program.cs                      # Configuración de la app
├── ServerCloudStore.Application/        # Capa de aplicación
│   ├── DTOs/                           # Data Transfer Objects
│   ├── Services/                       # Servicios de aplicación
│   ├── Validators/                     # FluentValidation
│   └── Mappers/                        # Mapeo DTO ↔ Domain
├── ServerCloudStore.Domain/             # Capa de dominio
│   ├── Entities/                       # Entidades del dominio
│   ├── Repositories/                   # Interfaces de repositorios
│   └── Services/                       # Servicios de dominio
├── ServerCloudStore.Infrastructure/     # Capa de infraestructura
│   ├── Data/                           # DbContext, Initializer
│   ├── Repositories/                   # Implementación con Dapper
│   ├── Scripts/                        # Scripts SQL
│   └── Services/                       # Servicios de infraestructura
├── ServerCloudStore.Transversal/        # Capa transversal
│   ├── Common/                         # Response<T>, utilidades
│   └── Mappings/                       # Mapeo de datos
├── ServerCloudStore.Tests.Unit/         # Tests unitarios
├── ServerCloudStore.Tests.Integration/  # Tests de integración
├── frontend/                            # Aplicación React
│   ├── src/
│   │   ├── components/                 # Componentes reutilizables
│   │   ├── pages/                      # Páginas/vistas
│   │   ├── services/                   # Servicios API
│   │   ├── context/                    # Context API
│   │   ├── guards/                     # Guards de rutas
│   │   └── types/                      # Tipos TypeScript
│   └── package.json
├── docker-compose.yml                   # Orquestación Docker
├── Dockerfile.backend                   # Dockerfile del backend
└── Dockerfile.frontend                  # Dockerfile del frontend
```

## 🔌 API Endpoints

### Autenticación

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| POST | `/api/Auth/login` | Iniciar sesión | No |

**Request Body:**
```json
{
  "username": "admin",
  "password": "admin123"
}
```

**Response:**
```json
{
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "expiresAt": "2024-12-25T10:00:00Z",
    "user": {
      "id": 1,
      "username": "admin",
      "email": "admin@example.com",
      "role": {
        "id": 1,
        "name": "Admin",
        "permissions": ["ReadProducts", "WriteProducts"]
      }
    }
  },
  "isSuccess": true,
  "message": "Login exitoso",
  "code": 200
}
```

### Productos

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/Product` | Listar productos (paginado) | Sí |
| GET | `/api/Product/{id}` | Obtener producto por ID | Sí |
| POST | `/api/Product` | Crear producto | Sí (Admin) |
| PUT | `/api/Product/{id}` | Actualizar producto | Sí (Admin) |
| DELETE | `/api/Product/{id}` | Eliminar producto | Sí (Admin) |

**Query Parameters (GET /api/Product):**
- `pageNumber` (int): Número de página (default: 1)
- `pageSize` (int): Tamaño de página (default: 10)
- `searchTerm` (string): Búsqueda por nombre/descripción
- `categoryId` (int): Filtrar por categoría
- `minPrice` (decimal): Precio mínimo
- `maxPrice` (decimal): Precio máximo
- `sortBy` (string): Campo para ordenar
- `sortOrder` (string): 'asc' o 'desc'

### Categorías

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/Category` | Listar categorías | Sí |
| GET | `/api/Category/{id}` | Obtener categoría por ID | Sí |
| POST | `/api/Category` | Crear categoría | Sí (Admin) |
| PUT | `/api/Category/{id}` | Actualizar categoría | Sí (Admin) |
| DELETE | `/api/Category/{id}` | Eliminar categoría | Sí (Admin) |

### Carga Masiva

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| POST | `/api/BulkImport` | Iniciar importación masiva | Sí (Admin) |
| GET | `/api/BulkImport/{jobId}` | Obtener estado del job | Sí (Admin) |
| GET | `/api/BulkImport` | Listar todos los jobs | Sí (Admin) |

**Request (multipart/form-data):**
- `CsvFile` (file): Archivo CSV (opcional)
- `GenerateCount` (int): Cantidad a generar (opcional)

*Nota: Debe proporcionar CsvFile O GenerateCount*

### SignalR Hub

**Hub URL:** `/hubs/import`

**Eventos:**
- `ImportProgress`: Notificación de progreso de importación

**Métodos del cliente:**
- `JoinJobGroup(jobId)`: Unirse al grupo de un job específico

## 🧪 Testing

### Tests Unitarios

```bash
# Ejecutar todos los tests unitarios
dotnet test ServerCloudStore.Tests.Unit

# Con cobertura
dotnet test ServerCloudStore.Tests.Unit --collect:"XPlat Code Coverage"
```

### Tests de Integración

```bash
# Requiere PostgreSQL en ejecución
dotnet test ServerCloudStore.Tests.Integration
```

### Cobertura de Código

```bash
# Generar reporte de cobertura
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

**Nota:** La fase de testing (Fase 8) está pendiente de implementación para alcanzar el 92% de cobertura objetivo.

## 🐳 Docker

### Construcción de Imágenes

```bash
# Backend
docker build -f Dockerfile.backend -t servercloudstore-backend .

# Frontend
docker build -f Dockerfile.frontend -t servercloudstore-frontend .
```

### Docker Compose

```bash
# Levantar servicios
docker-compose up -d

# Ver logs
docker-compose logs -f

# Detener servicios
docker-compose down

# Limpiar volúmenes
docker-compose down -v
```

### Servicios Disponibles

- **postgres**: Base de datos PostgreSQL (puerto 5432)
- **backend**: API .NET (puerto 5000)
- **frontend**: React App (puerto 3000)

## 📊 Escalabilidad

### Estrategia de Escalabilidad Horizontal

#### 1. Load Balancer
- **Nginx** o **Azure Application Gateway** delante de múltiples instancias de API
- Distribución de carga con round-robin o least connections
- Health checks para detectar instancias no disponibles

#### 2. Base de Datos
- **Read Replicas** de PostgreSQL para lecturas
- **Connection Pooling** con PgBouncer
- **Índices optimizados** para queries frecuentes:
  - Índice en `products.name` y `products.categoryId`
  - Índice full-text search para búsquedas
  - Índices compuestos para filtros complejos

#### 3. Cache (Redis)
- **Cache de productos frecuentes**: TTL de 5-10 minutos
- **Cache de categorías**: TTL de 1 hora
- **Cache de sesiones JWT**: Validación rápida de tokens
- **Patrón Cache-Aside**: Invalidación selectiva

#### 4. Message Queue
- **RabbitMQ** o **Azure Service Bus** para:
  - Procesamiento asíncrono de carga masiva
  - Desacoplamiento de servicios
  - Retry automático en caso de fallos
  - Dead letter queue para errores

#### 5. SignalR Scaling
- **Azure SignalR Service** para entornos cloud
- **Redis Backplane** para on-premise
- Permite múltiples instancias de API compartiendo conexiones

#### 6. Storage
- **Azure Blob Storage** o **AWS S3** para:
  - Imágenes de categorías
  - Archivos CSV de importación
  - Archivos de logs

#### 7. CDN
- **CloudFront** o **Azure CDN** para:
  - Servir assets estáticos del frontend
  - Reducir latencia global
  - Offload del servidor de aplicaciones

#### 8. Monitoreo y Observabilidad
- **Application Insights** / **Prometheus** para métricas
- **ELK Stack** para logs centralizados
- **Health checks** endpoints
- **Distributed tracing** con OpenTelemetry

### Configuración de Escalabilidad

```yaml
# docker-compose.scale.yml
version: '3.8'

services:
  backend:
    deploy:
      replicas: 3  # 3 instancias del backend
      
  redis:
    image: redis:alpine
    
  nginx:
    image: nginx:alpine
    volumes:
      - ./nginx-lb.conf:/etc/nginx/nginx.conf
```

## 💡 Decisiones Técnicas

### ¿Por qué Dapper en lugar de Entity Framework Core?

**Ventajas de Dapper:**
- ✅ **Alto rendimiento**: ~50% más rápido que EF Core en queries simples
- ✅ **Control total sobre SQL**: Queries optimizadas para PostgreSQL
- ✅ **Ideal para carga masiva**: Batch inserts extremadamente rápidos
- ✅ **Menor overhead**: No tracking de entidades
- ✅ **Curva de aprendizaje simple**: SQL directo

**Desventajas:**
- ❌ No tiene migraciones automáticas (solucionado con scripts SQL)
- ❌ Requiere escribir SQL manualmente
- ❌ Menos abstracción

**Conclusión:** Para este proyecto con carga masiva de 100k productos, Dapper es la elección correcta.

### ¿Por qué Arquitectura por Capas?

- **Separación de responsabilidades**: Cada capa tiene un propósito claro
- **Testabilidad**: Fácil mockear dependencias
- **Mantenibilidad**: Cambios aislados por capa
- **Escalabilidad**: Fácil agregar nuevas funcionalidades
- **Clean Architecture**: Principios SOLID aplicados

### ¿Por qué SignalR para notificaciones?

- **Integración nativa con .NET**: No requiere infraestructura adicional
- **WebSocket con fallback**: Funciona en cualquier navegador
- **Grupos**: Fácil enviar notificaciones a usuarios específicos
- **Bidireccional**: Cliente puede invocar métodos del servidor

### ¿Por qué PostgreSQL?

- **Open source y robusto**: Sin costos de licencias
- **Excelente rendimiento**: Maneja millones de registros
- **Soporte JSON**: Útil para campos flexibles
- **Índices avanzados**: GIN, GiST para búsquedas complejas
- **Madurez**: 30+ años de desarrollo

## 👤 Credenciales de Prueba

### Usuario Admin
- **Usuario:** `admin`
- **Contraseña:** `admin123`
- **Permisos:** ReadProducts, WriteProducts

### Usuario Regular (si existe)
- **Usuario:** `user`
- **Contraseña:** `user123`
- **Permisos:** ReadProducts

## 📝 Variables de Entorno

### Backend (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=servercloudstore;Username=postgres;Password=postgres123"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyForJWTTokenGenerationMinimum32Chars",
    "Issuer": "ServerCloudStoreAPI",
    "Audience": "ServerCloudStoreClients",
    "ExpirationMinutes": 60
  }
}
```

### Frontend (.env.local)

```bash
VITE_API_BASE_URL=http://localhost:5000/api
```

## 🐛 Troubleshooting

### Error: Cannot connect to PostgreSQL

```bash
# Verificar que PostgreSQL está en ejecución
docker ps | grep postgres

# O si está instalado localmente
sudo systemctl status postgresql  # Linux
brew services list  # macOS
```

### Error: CORS policy

El backend ya tiene CORS configurado para `AllowAll` en desarrollo. Si tienes problemas:

```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

### Error: SignalR connection failed

Verifica que:
1. El backend está en ejecución
2. La URL del hub es correcta: `http://localhost:5000/hubs/import`
3. El token JWT es válido

## 📚 Recursos Adicionales

- [Plan de Implementación Completo](PLAN_DE_IMPLEMENTACION.md)
- [Documentación de .NET 8](https://learn.microsoft.com/en-us/dotnet/)
- [React + TypeScript](https://react.dev/)
- [Dapper](https://github.com/DapperLib/Dapper)
- [SignalR](https://learn.microsoft.com/en-us/aspnet/core/signalr/)

## 📄 Licencia

Este proyecto es privado y fue desarrollado como prueba técnica.

## 👨‍💻 Autor

Desarrollado como parte de la prueba técnica para Finanzauto.

---

**Nota:** El proyecto está completo excepto la Fase 8 (Testing con 92% de cobertura), que requiere implementación adicional de tests unitarios e integración.

