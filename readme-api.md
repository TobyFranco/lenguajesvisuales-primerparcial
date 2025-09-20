# API REST Biblioteca - ASP.NET Core 8

## Descripción
API RESTful completa para la gestión de una biblioteca desarrollada con ASP.NET Core 8 Web API, Entity Framework Core y autenticación JWT.

## 🏗️ Arquitectura del Sistema

### Base de Datos (5 Tablas + Identity)
1. **Usuarios** (ApplicationUser - extiende IdentityUser)
2. **Autores** - Información de autores
3. **Categorías** - Clasificación de libros
4. **Libros** - Catálogo de libros
5. **Préstamos** - Registro de préstamos
6. **LibroAutor** - Tabla intermedia (relación N:M)

### Relaciones
- **Usuario 1:N Préstamos** - Un usuario puede tener múltiples préstamos
- **Libro 1:N Préstamos** - Un libro puede ser prestado múltiples veces
- **Categoría 1:N Libros** - Una categoría puede tener múltiples libros
- **Libro N:M Autores** - Un libro puede tener múltiples autores y un autor múltiples libros

## 🔧 Configuración e Instalación

### 1. Crear el Proyecto
```bash
dotnet new webapi -n BibliotecaAPI
cd BibliotecaAPI
```

### 2. Instalar Paquetes NuGet
```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Swashbuckle.AspNetCore
dotnet add package System.IdentityModel.Tokens.Jwt
```

### 3. Estructura de Carpetas
```
BibliotecaAPI/
├── Controllers/
│   ├── AuthController.cs
│   ├── AutoresController.cs
│   ├── CategoriasController.cs
│   ├── LibrosController.cs
│   └── PrestamosController.cs
├── Data/
│   └── ApplicationDbContext.cs
├── DTOs/
├── Models/
├── Services/
├── Program.cs
├── appsettings.json
└── appsettings.Development.json
```

### 4. Migraciones y Base de Datos
```bash
# Agregar migración inicial
dotnet ef migrations add InitialCreate

# Actualizar base de datos
dotnet ef database update

# Ver migraciones
dotnet ef migrations list
```

## 🔐 Seguridad JWT

### Configuración
- **Validez del Token**: 1 hora
- **Algoritmo**: HMAC SHA256
- **Claims incluidos**: UserId, Name, Email, Roles, Nombre, Apellido

### Endpoints Protegidos
Requieren autenticación (Bearer Token):
- `POST /api/autores`
- `PUT /api/autores/{id}`
- `DELETE /api/autores/{id}`
- `POST /api/categorias`
- `PUT /api/categorias/{id}`
- `DELETE /api/categorias/{id}`
- `POST /api/libros`
- `PUT /api/libros/{id}`
- `DELETE /api/libros/{id}`
- `GET /api/prestamos`
- `POST /api/prestamos`
- `PUT /api/prestamos/{id}/devolver`

### Usuarios de Prueba
Al ejecutar la aplicación se crean automáticamente:

**Administrador:**
- Email: `admin@biblioteca.com`
- Password: `Admin123!`
- Rol: `Admin`

**Usuario Regular:**
- Email: `user@biblioteca.com`
- Password: `User123!`
- Rol: `User`

## 📋 API Endpoints

### Autenticación
```http
POST /api/auth/login
POST /api/auth/register
GET /api/auth/profile [Auth Required]
```

### Autores
```http
GET /api/autores
GET /api/autores/{id}
POST /api/autores [Auth Required]
PUT /api/autores/{id} [Auth Required]
DELETE /api/autores/{id} [Auth Required]
```

### Categorías
```http
GET /api/categorias
GET /api/categorias/{id}
POST /api/categorias [Auth Required]
PUT /api/categorias/{id} [Auth Required]
DELETE /api/categorias/{id} [Auth Required]
```

### Libros
```http
GET /api/libros?titulo=&categoriaId=&disponible=
GET /api/libros/{id}
POST /api/libros [Auth Required]
PUT /api/libros/{id} [Auth Required]
DELETE /api/libros/{id} [Auth Required]
```

### Préstamos
```http
GET /api/prestamos?usuarioId=&estado= [Auth Required]
GET /api/prestamos/{id} [Auth Required]
GET /api/prestamos/mis-prestamos [Auth Required]
POST /api/prestamos [Auth Required]
PUT /api/prestamos/{id}/devolver [Auth Required]
```

## 🚀 Ejecutar la Aplicación

### 1. Configurar Connection String
Actualizar `appsettings.json` con tu cadena de conexión a SQL Server.

### 2. Ejecutar
```bash
dotnet run
```

### 3. Acceder a Swagger
- URL: `https://localhost:7xxx/swagger`
- Documentación interactiva de la API

## 🧪 Ejemplos de Uso

### 1. Autenticación
```json
// POST /api/auth/login
{
  "email": "user@biblioteca.com",
  "password": "User123!"
}

// Respuesta
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2024-01-15T15:30:00Z",
  "userId": "user-id",
  "email": "user@biblioteca.com",
  "nombre": "Usuario Prueba"
}
```

### 2. Crear Autor
```json
// POST /api/autores
// Header: Authorization: Bearer {token}
{
  "nombre": "Mario",
  "apellido": "Vargas Llosa",
  "fechaNacimiento": "1936-03-28",
  "nacionalidad": "Peruano"
}
```

### 3. Crear Libro
```json
// POST /api/libros
// Header: Authorization: Bearer {token}
{
  "titulo": "La ciudad y los perros",
  "isbn": "978-84-376-0123-4",
  "añoPublicacion": 1963,
  "numeroPaginas": 418,
  "descripcion": "Primera novela de Mario Vargas Llosa",
  "categoriaId": 1,
  "autoresIds": [6]
}
```

### 4. Crear Préstamo
```json
// POST /api/prestamos
// Header: Authorization: Bearer {token}
{
  "libroId": 6,
  "fechaDevolucionEsperada": "2024-02-15T00:00:00",
  "observaciones": "Préstamo para lectura personal"
}
```

## 🔍 Características Técnicas

### Validaciones
- Validación de modelos con Data Annotations
- Validación de negocio en servicios
- Manejo de errores con códigos HTTP apropiados

### Filtros y Búsquedas
- Libros por título, categoría y disponibilidad
- Préstamos por usuario y estado
- Paginación (implementable)

### Seguridad
- Tokens JWT con expiración de 1 hora
- Protección de endpoints sensibles
- Validación de permisos por usuario

### Base de Datos
- Migraciones con Entity Framework Core
- Seed data automático
- Relaciones bien definidas
- Índices para optimización

### Documentación
- Swagger/OpenAPI integrado
- Soporte para autenticación en Swagger
- Documentación de todos los endpoints

## 🛠️ Comandos Útiles

```bash
# Ver logs de Entity Framework
dotnet ef database update --verbose

# Revertir migración
dotnet ef database update {MigracionAnterior}

# Eliminar migración
dotnet ef migrations remove

# Generar script SQL
dotnet ef migrations script

# Limpiar y reconstruir
dotnet clean && dotnet build
```

## 📈 Posibles Mejoras

1. **Paginación**: Implementar paginación en endpoints de listado
2. **Caché**: Añadir Redis para caché de consultas frecuentes
3. **Logs**: Implementar logging estructurado con Serilog
4. **Testing**: Agregar pruebas unitarias e integración
5. **Rate Limiting**: Implementar límites de rate por endpoint
6. **Versionado**: Añadir versionado de API
7. **Health Checks**: Implementar health checks
8. **Métricas**: Integrar Application Insights o Prometheus

## 💡 Notas Importantes

- Los tokens JWT expiran en 1 hora por requerimiento
- La aplicación crea datos de prueba automáticamente
- Se incluye validación de préstamos vencidos
- Los libros se marcan como no disponibles al prestarlos
- Swagger está configurado para autenticación JWT

Esta implementación cumple con todos los requerimientos técnicos solicitados y proporciona una base sólida para un sistema de gestión de biblioteca.