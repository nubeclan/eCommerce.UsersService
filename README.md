# eCommerce Users Service API

## Descripción
Este proyecto es un microservicio de usuarios para una plataforma de eCommerce, desarrollado con .NET 10. Proporciona funcionalidades completas de gestión de usuarios, incluyendo registro, autenticación JWT, y operaciones CRUD (Crear, Leer, Actualizar, Eliminar).

## Arquitectura

### Patrón Arquitectónico
El proyecto sigue una arquitectura **CQRS (Command Query Responsibility Segregation)** con el patrón **Mediator** para el manejo de comandos y consultas. Esto permite una separación clara entre operaciones de escritura (Commands) y lectura (Queries), facilitando la escalabilidad y mantenibilidad.

### Tecnologías Principales
- **.NET 10**: Framework principal para el desarrollo de la API.
- **Carter**: Librería para crear APIs minimales con enrutamiento simple y declarativo.
- **Entity Framework Core**: ORM para el acceso a datos, configurado con PostgreSQL.
- **PostgreSQL**: Base de datos relacional para persistencia de datos.
- **FluentValidation**: Librería para validación de datos de entrada.
- **Mapster**: Librería para mapeo de objetos (DTOs a entidades y viceversa).
- **BCrypt.Net-Next**: Librería para hashing seguro de contraseñas.
- **JWT (JSON Web Tokens)**: Para autenticación segura.

### Estructura del Proyecto
El proyecto está organizado en las siguientes capas y carpetas:

- **Contracts**: Define los contratos de entrada y salida (DTOs) para las APIs, como `RegisterRequest`, `LoginRequest`, `AuthenticationResponse`.
- **Entities**: Contiene las entidades del dominio, como `User`.
- **Database**: Incluye el `ApplicationDbContext`, migraciones de EF Core y configuraciones de entidades.
- **Features**: Agrupa las funcionalidades por módulos (e.g., `Users`, `Auth`), cada uno con comandos, validadores, handlers y endpoints.
- **Configurations**: Contiene configuraciones específicas, como `Authentication` para JWT.
- **Abstractions**: Define interfaces y abstracciones comunes, como el sistema de mensajería (CQRS).
- **Shared**: Contiene elementos compartidos como bases de respuesta, comportamientos (behaviors) y excepciones.

### Funcionalidades Implementadas

#### Gestión de Usuarios (CRUD)
- **Crear Usuario**: `POST /api/users/register` - Registra un nuevo usuario con validación y hashing de contraseña.
- **Obtener Todos los Usuarios**: `GET /api/users` - Lista todos los usuarios.
- **Obtener Usuario por ID**: `GET /api/users/{userId}` - Obtiene un usuario específico.
- **Actualizar Usuario**: `PUT /api/users/{userId}` - Actualiza la información de un usuario.
- **Eliminar Usuario**: `DELETE /api/users/{userId}` - Elimina un usuario.

#### Autenticación
- **Login**: `POST /api/auth/login` - Autentica un usuario y genera un token JWT.

### Flujo de una Operación (Ejemplo: Registro de Usuario)
1. **Endpoint**: El endpoint `POST /api/users/register` recibe un `RegisterRequest`.
2. **Mapeo**: Mapster convierte el request en un `Command`.
3. **Validación**: FluentValidation valida el comando antes de procesarlo.
4. **Dispatcher**: El `Dispatcher` envía el comando al handler correspondiente.
5. **Handler**: El `Handler` ejecuta la lógica de negocio, interactuando con EF Core para persistir en PostgreSQL.
6. **Respuesta**: Se devuelve una `BaseResponse` con el resultado.

### Configuración y Dependencias
- **Program.cs**: Configura servicios, incluyendo EF Core, Carter, JWT y dependencias personalizadas.
- **DependencyInjection.cs**: Registra handlers, validadores, mapeos y servicios compartidos.
- **appsettings.json**: Contiene la cadena de conexión a PostgreSQL y configuración JWT.

### Seguridad
- Las contraseñas se hashean usando BCrypt para almacenamiento seguro.
- Autenticación JWT implementada con tokens que expiran en 60 minutos.
- Validación de issuer, audience y firma del token.

### Migraciones de Base de Datos
Las migraciones de EF Core están incluidas en `Database/Migrations`. Ejecuta `dotnet ef database update` para aplicar cambios.

### Ejecutar el Proyecto
1. Asegúrate de tener PostgreSQL corriendo y configura la conexión en `appsettings.json` o `appsettings.Development.json`.
2. Restaura paquetes: `dotero