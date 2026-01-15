PS : The URL Doesnt Contain CORS Use Endpoints as you wish ...:)


A secure ASP.NET Core Web API for user authentication and management with JWT tokens and PostgreSQL database and Docker image.


## Tech Stack

- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- Docker
- JWT Authentication
- FluentValidation
- Supabase

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- PostgreSQL database
- Supabase account 

### Installation

1. Clone the repository:
```bash
git clone [repository-url]
cd Wajeb.API
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Configure the application:
   - Copy `appsettings.json.example` to `appsettings.json`
   - Update the connection string in `appsettings.json`
   - Set your JWT secret key in `AppSettings:Key`

4. Run database migrations:
```bash
dotnet ef database update
```

5. Run the application:
```bash
dotnet run
```

## API Endpoints

### Authentication

#### Register
- POST `/Auth/register`
- Body: `{ "username": "string", "email": "string", "password": "string" }`
- Returns: User details and JWT token

#### Login
- POST `/Auth/login`
- Body: `{ "email": "string", "password": "string" }`
- Returns: User details and JWT token

### User Management (Requires Authentication)

#### Get Current User
- GET `/Auth/me`
- Headers: `Authorization: Bearer {token}`
- Returns: Current user details

#### Change Password
- PUT `/Auth/me/password`
- Headers: `Authorization: Bearer {token}`
- Body: `{ "oldPassword": "string", "newPassword": "string" }`
- Returns: 204 No Content

#### Delete Account
- DELETE `/Auth/me`
- Headers: `Authorization: Bearer {token}`
- Returns: 204 No Content

## Configuration

### Database Connection

Configure your PostgreSQL connection in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your connection string here"
  }
}
```

### JWT Settings

Configure JWT settings in `appsettings.json`:

```json
{
  "AppSettings": {
    "Key": "Your secret key here",
    "Issuer": "WajebAPI",
    "Audience": "WajebAPIUsers"
  }
}
```



## Docker Support

The project includes Docker support. To build and run with Docker:

```bash
docker build -t wajeb-api .
docker run -p 5000:5000 wajeb-api
```

Or use Docker Compose:

```bash
docker-compose up
```

## Project Structure

```
Wajeb.API/
├── Controllers/       # API controllers
├── Data/             # Database context
├── Dtos/             # Data transfer objects
├── Models/           # Data models
├── Services/         # Business logic services
├── Migrations/       # Database migrations
└── Program.cs        # Application entry point
```

## License

This project is licensed under the MIT License.
