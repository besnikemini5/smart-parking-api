# Smart Parking API

Smart Parking API is a RESTful Web API built with .NET 8. It lets users search parking lots and parking spots, create reservations, and manage parking data. Administrators can manage parking lots, spots, and reservations. The API also integrates a third-party weather service.

## Technologies Used

- .NET 8 Web API
- Entity Framework Core (code-first migrations)
- SQL Server LocalDB
- JWT Authentication with role-based authorization
- Swagger / OpenAPI
- Repository Pattern
- DTOs and AutoMapper
- Dependency Injection
- Global exception-handling middleware and logging
- Open-Meteo Weather API
- xUnit-style unit tests (Moq, FluentAssertions)

## Main Features

- User registration and login
- JWT token authentication
- Role-based authorization (Guest, User, Admin)
- CRUD operations for parking lots, parking spots, and reservations
- Pagination, sorting, and filtering for parking lots
- Logging for monitoring API actions
- Third-party weather API integration

## User Roles

| Role  | Permissions |
|-------|-------------|
| Guest | View parking lots, view parking spots, check weather information |
| User  | Everything a guest can do, plus login, create reservations, and manage their reservations |
| Admin | Manage parking lots, parking spots, and reservations |

## Project Structure

```
Solution.sln
SmartParkingAPI/          # Web API project
  Controllers/            # API endpoints
  Data/                   # DbContext
  DTOs/                   # Data transfer objects
  Mappings/               # AutoMapper profiles
  Middleware/             # Global exception handling
  Migrations/             # EF Core migrations
  Models/                 # Entities
  Repositories/           # Repository pattern
  Services/               # Business logic
SmartParkingAPI.Tests/    # Unit tests
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or newer
- SQL Server LocalDB (installed with Visual Studio, or via the SQL Server Express LocalDB installer)
- EF Core CLI tool:

  ```
  dotnet tool install --global dotnet-ef
  ```

### Setup

1. Clone the repository and open a terminal in the API project folder:

   ```
   git clone https://github.com/<your-username>/<repo-name>.git
   cd <repo-name>/SmartParkingAPI
   ```

2. Set a JWT signing key (at least 32 characters). It is stored with .NET user-secrets, so it never goes into the repository:

   ```
   dotnet user-secrets init
   dotnet user-secrets set "Jwt:Key" "replace-with-a-long-random-string-32-chars-min"
   ```

3. Create the database by applying the migrations:

   ```
   dotnet ef database update
   ```

   If you use Visual Studio instead, run `Update-Database` in the Package Manager Console.

4. Run the API:

   ```
   dotnet run
   ```

5. Open Swagger in your browser: <http://localhost:5198/swagger>

### Using protected endpoints

1. Register a user and log in through the auth endpoints in Swagger.
2. Copy the JWT token from the login response.
3. Click **Authorize** in Swagger and enter `Bearer <your-token>`.

### Running the tests

From the repository root:

```powershell
dotnet test
```

## Configuration

Settings live in `SmartParkingAPI/appsettings.json`:

- `ConnectionStrings:DefaultConnection` points to SQL Server LocalDB by default. Change it if you use another SQL Server instance.
- `Jwt:Issuer` and `Jwt:Audience` define the token issuer and audience.
- `Jwt:Key` is a placeholder. Set the real value with user-secrets as shown above.