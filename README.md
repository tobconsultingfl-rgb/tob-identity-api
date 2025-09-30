# TOB Identity API

A multi-tenant identity and access management API built with ASP.NET Core and Azure services.

## Overview

The TOB Identity API provides comprehensive identity management capabilities including user management, role-based access control (RBAC), and multi-tenant support. It integrates with Azure Active Directory for authentication and Azure Key Vault for secure configuration management.

## Architecture

The solution follows Clean Architecture principles with clear separation of concerns:

```
src/
├── TOB.Identity.API/          # Web API layer (Controllers, Startup)
├── TOB.Identity.Services/     # Business logic layer (Services)
├── TOB.Identity.Infrastructure/ # Data access layer (Repositories, DbContext, Validation)
└── TOB.Identity.Domain/       # Domain models (DTOs, Requests, Constants)
```

## Features

- **Multi-Tenant Management**: Complete tenant lifecycle management with isolation
- **User Management**: Full CRUD operations for users with hierarchical manager relationships
- **Role-Based Access Control**: Flexible RBAC with roles and permissions
- **User-Role Assignments**: Dynamic user-role mapping system
- **Azure AD Integration**: JWT Bearer token authentication
- **Azure Key Vault**: Secure secrets and configuration management
- **Microsoft Graph API**: Integration for Azure AD operations
- **RESTful API**: Well-documented endpoints with Swagger/OpenAPI
- **Entity Framework Core**: Code-first database with migrations
- **AutoMapper**: Object-to-object mapping
- **FluentValidation**: Request validation

## Technology Stack

- **.NET 9.0**
- **ASP.NET Core Web API**
- **Entity Framework Core 9.0** with SQL Server
- **Microsoft Identity Web** for Azure AD authentication
- **Azure Key Vault** for configuration secrets
- **Microsoft Graph SDK** for Azure AD operations
- **AutoMapper** for object mapping
- **FluentValidation** for input validation
- **Swagger/Swashbuckle** for API documentation

## Prerequisites

- .NET 9.0 SDK
- SQL Server (2019 or later)
- Azure subscription with:
  - Azure AD tenant
  - Azure Key Vault instance
  - SQL Database (for production)

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "IdentityDBContext": "Server=localhost;Database=IdentityDB;Trusted_Connection=True;"
  },
  "KeyVault": {
    "VaultUri": "https://your-keyvault.vault.azure.net/"
  },
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "ClientId": "your-client-id",
    "Domain": "yourdomain.onmicrosoft.com",
    "TenantId": "your-tenant-id",
    "ClientSecret": "your-client-secret"
  },
  "AllowedOrigins": "http://localhost:5173"
}
```

### Azure Key Vault

The application automatically loads configuration from Azure Key Vault when the `KeyVault:VaultUri` setting is present. Ensure your application has appropriate access using Azure Managed Identity or DefaultAzureCredential.

### Environment Variables

For local development, you can override settings using environment variables or User Secrets:

```bash
dotnet user-secrets set "ConnectionStrings:IdentityDBContext" "your-connection-string"
dotnet user-secrets set "AzureAd:ClientSecret" "your-client-secret"
```

## Getting Started

### 1. Clone the repository

```bash
git clone <repository-url>
cd tob-identity-api
```

### 2. Configure the database

Update the connection string in `appsettings.json` or use User Secrets:

```bash
cd src/TOB.Identity.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:IdentityDBContext" "Server=localhost;Database=IdentityDB;Trusted_Connection=True;"
```

### 3. Apply database migrations

```bash
cd src/TOB.Identity.API
dotnet ef database update --project ../TOB.Identity.Infrastructure
```

### 4. Configure Azure AD

1. Register an application in Azure AD
2. Add API permissions if needed
3. Create a client secret
4. Update the `AzureAd` section in configuration

### 5. Run the application

```bash
cd src/TOB.Identity.API
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:5001/swagger`

## API Endpoints

### Users

- `GET /users/me` - Get current authenticated user
- `GET /users/{userId}` - Get user by ID
- `GET /users?tenantId={tenantId}` - Get users by tenant
- `GET /users/usernameexists/{userName}` - Check username availability
- `POST /users` - Create a new user
- `PUT /users/{userId}` - Update user
- `DELETE /users/{userId}` - Deactivate user

### Tenants

- `GET /tenants` - Get all tenants
- `GET /tenants/{tenantId}` - Get tenant by ID
- `POST /tenants` - Create a new tenant
- `PUT /tenants/{tenantId}` - Update tenant
- `DELETE /tenants/{tenantId}` - Deactivate tenant

### Roles

- `GET /roles` - Get all roles
- `GET /roles/{roleId}` - Get role by ID
- `POST /roles` - Create a new role
- `PUT /roles/{roleId}` - Update role
- `DELETE /roles/{roleId}` - Delete role

### User Roles

- `GET /userroles` - Get user role mappings
- `POST /userroles` - Assign role to user
- `DELETE /userroles/{mappingId}` - Remove role from user

## Database Schema

### Core Tables

- **Tenants**: Organization/company records
- **Users**: User accounts with tenant association
- **Roles**: System roles (Super Admin, Admin, Power User, User)
- **Permissions**: Granular permissions
- **UserRoleMappings**: User-to-role assignments
- **RolePermissionMappings**: Role-to-permission assignments

### Seed Data

The database includes seed data for:
- 4 default roles (Super Admin, Admin, Power User, User)
- 8 default permissions (tenant and user management operations)

## Authentication & Authorization

The API uses JWT Bearer tokens from Azure AD:

1. Obtain a token from Azure AD
2. Include the token in the Authorization header:
   ```
   Authorization: Bearer {your-token}
   ```
3. All endpoints require authentication (marked with `[Authorize]`)

## Docker Support

### Build Docker Image

```bash
docker build -t tob-identity-api .
```

### Run Docker Container

```bash
docker run -p 8080:80 \
  -e ConnectionStrings__IdentityDBContext="your-connection-string" \
  -e AzureAd__ClientId="your-client-id" \
  -e AzureAd__ClientSecret="your-client-secret" \
  -e AzureAd__TenantId="your-tenant-id" \
  tob-identity-api
```

## Deployment

The project includes Azure DevOps pipelines and Kubernetes manifests in the `/deploy` directory:

- `application-pipelines.yml` - CI/CD pipeline for application
- `infrastructure-pipelines.yml` - Infrastructure deployment pipeline
- `manifests/` - Kubernetes deployment manifests

### Kubernetes Deployment

The application can be deployed to Azure Kubernetes Service (AKS) with Azure Key Vault integration using the CSI driver.

## Development

### Entity Framework Migrations

Create a new migration:

```bash
cd src/TOB.Identity.API
dotnet ef migrations add MigrationName --project ../TOB.Identity.Infrastructure
```

Update database:

```bash
dotnet ef database update --project ../TOB.Identity.Infrastructure
```

### Testing the API

Use the Swagger UI at `/swagger` or tools like:
- Postman
- curl
- HTTP Client extensions in VS Code

## Project Structure

```
tob-identity-api/
├── src/
│   ├── TOB.Identity.API/
│   │   ├── Controllers/         # API Controllers
│   │   ├── Extensions/          # Service configuration extensions
│   │   ├── Program.cs           # Application entry point
│   │   └── Startup.cs           # Service and middleware configuration
│   ├── TOB.Identity.Services/
│   │   ├── Implementations/     # Service implementations
│   │   └── I*Service.cs         # Service interfaces
│   ├── TOB.Identity.Infrastructure/
│   │   ├── Data/
│   │   │   ├── Entities/        # EF Core entities
│   │   │   └── IdentityDBContext.cs
│   │   ├── Repositories/        # Data access repositories
│   │   ├── Mapping/             # AutoMapper profiles
│   │   └── Validation/          # FluentValidation validators
│   └── TOB.Identity.Domain/
│       ├── Models/              # DTOs
│       ├── Requests/            # Request models
│       ├── AppSettings/         # Configuration models
│       └── Constants.cs         # Application constants
├── deploy/                      # Deployment configurations
├── Dockerfile                   # Docker build configuration
└── README.md                    # This file
```

## Security Considerations

- All secrets should be stored in Azure Key Vault
- Connection strings should never be committed to source control
- Use Managed Identity for Azure resource authentication in production
- Enable HTTPS in production environments
- Configure appropriate CORS policies for your frontend applications
- Review and configure the `AllowedOrigins` setting

## CORS Configuration

Update the `AllowedOrigins` setting to include your frontend application URLs (semicolon-separated):

```json
"AllowedOrigins": "http://localhost:5173;https://yourapp.com"
```

## License

[Specify your license here]

## Support

For issues and questions, please [create an issue](link-to-your-issues-page) in the repository.
