# Store Clean Architecture Web API

ASP.NET Core 8 Web API project using Clean Architecture.

## Projects

```text
src/
  Store.Domain/          Entities, constants, repository contracts
  Store.Application/     DTOs, service contracts, business services, exceptions
  Store.Infrastructure/  EF Core DbContext, repositories, JWT token service, DI
  Store.API/             Controllers, middleware, Program.cs, Swagger
```

## Main Features

- JWT authentication with `admin` and `user` roles
- Product CRUD for admins only
- Category read for authenticated users
- Category create/update/delete for admins only
- Order create for admins and users
- Order read/update/delete for authenticated users
- Product filtering by `minPrice`, `maxPrice`, `categoryId`
- Product sorting by `Price` or `Name` using `sortDirection=asc|desc`
- Order pagination with `pageNumber`, `pageSize`
- Order filtering by `orderDateFrom`, `orderDateTo`
- Order response includes `TotalPrice` and `IsExpensive`

## Run

```bash
dotnet restore Store.sln
dotnet run --project src/Store.API
```

Swagger is available at:

```text
https://localhost:7068/swagger
http://localhost:5068/swagger
```

## Authentication

Register:

```http
POST /api/auth/register
```

Example body:

```json
{
  "username": "admin",
  "password": "Admin123",
  "role": "admin"
}
```

Login:

```http
POST /api/auth/login
```

Copy the returned token and use Swagger's **Authorize** button with:

```text
Bearer <token>
```

## EF Core Migrations

Install the EF CLI if needed:

```bash
dotnet tool install --global dotnet-ef
```

Create the initial migration:

```bash
dotnet ef migrations add InitialCreate \
  --project src/Store.Infrastructure \
  --startup-project src/Store.API \
  --output-dir Persistence/Migrations
```

Apply migrations:

```bash
dotnet ef database update \
  --project src/Store.Infrastructure \
  --startup-project src/Store.API
```

The default connection string uses SQL Server Express:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=LAB322PC21\\SQLEXPRESS;Database=StoreDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

If your SQL Server instance name is different, change only the `Server=` value in
`src/Store.API/appsettings.json`.
