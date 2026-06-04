
# CareerHub

## Overview
CareerHub API is a simple ASP.NET Core Web API built using .NET 10  
It represents the foundation of a job board backend system that will eventually support a full CareerHub platform.

This version of the API focuses on building clean architecture fundamentals, HTTP routing, and asynchronous endpoint handling using in-memory data.

---

## Project Purpose
The purpose of this project is to demonstrate:
- ASP.NET Core Web API setup (.NET 10)
- Controller-based architecture
- RESTful API design principles
- Asynchronous programming using async/await
- Proper HTTP status code handling
- Dependency Injection
- Custom domain exception handling
- Global exception handling using IExceptionHandler
- Problem Details responses
- Structured logging using Serilog
- Separation of concerns between controllers, domain logic, and error handling

---

## Architecture API Choice(Controllers)

I used Controllers because they offer clear and readable project structure,  separation of concerns and it's also ideal for large applications and support advanced feature support.  They are also the more logical choice for large ASP.NET Core projects.

## DTO implementation

### Domain Model: 
- PostedAt is generated and controlled by the server when a job is created because clients should only see when a job was posted not update or create it.
### Validation Rules:
- I used IValidatableObject to validate the relationship between SalaryMin and SalaryMax without using manual validation inside the controller. This approach keeps my JobsController cleaner and keeps validation responsibilities inside the CreateJobsRequest DTO model.

# Authentication & Authorization
## Stateless Authentication

- Session-based authentication stores user information on the server after a successful login. The server creates a session and keeps track of the user's identity between requests using a session ID. 
- JWT-based authentication is stateless, the server generates a JSON Web Token with user information and sends it to the client after login. The client uses the token for future requests, and the server validates the token without storing any session data.

- Statelessness is important for horizontally scaled APIs because requests can be handled by any server instance. Since user information is contained within the JWT, servers do not need to share session data, making the application easier to maintain.

## 401 Unauthorized vs 403 Forbidden

- 401 Unauthorized response means the user has not provided valid login credentials. The authentication middleware generates the 401 response before the request reaches the controller.

- 403 Forbidden response means the user is authenticated but does not have the permission to perform the requested action. The authorization middleware generates the 403 response after authentication has succeeded.

## Token Storage

- Storing JWTs in localStorage is considered a security risk because JavaScript running in the browser can access localStorage. If an application is vulnerable to a Cross-Site Scripting (XSS) attack, an attacker could steal the token.

- Storing tokens in a secure HttpOnly cookies is a safer alternative. Because HttpOnly cookies cannot be accessed through JavaScript, which protects them from attacks.


# Error Handling
## Custom Domain Exceptions (advantages)
## Controller Thinning: 
Throwing a new JobNotFoundException instead of returning NotFound() separate business logic from HTTP concerns. Controllers focus on successful request processing while the Global Exception Handler centrally translates exceptions into HTTP responses. 

Advantages of throwing JobNotFoundExceprion instead of returning NotFound:
- Reduces duplicated code
- Keeps controllers clean and readable
- Ensures consistent error responses throughout the API
- Structured Logging with Serilog

### Structured Logging: 

Serilog stores log information as structured data rather than plain text, Unlike traditional Console.WriteLine() statements.

Advantages of using SeriLog instead of traditional Console.WriteLine():
- Easier error debugging
- Better search and filtering capabilities
- Improved monitoring and diagnostics
- Consistent log formatting

## Database (EF Core + PostgreSQL)


### Change Tracker
- EF Core changes tracker tracks entity state changes in the project's memory. When SaveChangesAsync() is called, it compares original data to the current data and saves the changes.

---

### Migrations as version Control

- The generated migration file must be committed to source control alongside the code that caused it because they ensure consistent database structure across all environments and allow project contributors to rebuild the database and see Migrations history

- When a teammate pulls code that references a migration they have not applied the application wil fail to run and tables will not exist in the database

---

### Connection String Security
- Connection strings contain sensitive credentials and should be stored in appsettings.Development.json for local development to exclude it from source control

- Safer production alternatives:
  - Environment variables
  - Docker secrets
  - Azure Key Vault / AWS Secrets Manager

---

# Project Structure
CareerHub/
│
├── APIs/
│ ├── Controllers/
| | ├── JobsController.cs
│ │ └── AuthController.cs
│ │
│ ├── DTOs/
│ │ ├── CreateJobRequest.cs
| | ├── LoginRequest.cs
| | ├── LoginResponse.cs
│ │ ├── UpdateJobRequest.cs
│ │ └── JobResponse.cs
│ │
│ ├── Models/
│ │ ├── JobListing.cs
│ │ └── JobType.cs
│ │
│ ├── Data/
│ │ └── CareerHubDbContext.cs
│ │
│ ├── Exceptions/
│ │ ├── JobNotFoundException.cs
│ │ └── DuplicateJobException.cs
│ │
│ ├── Middleware/
│ │ └── GlobalExceptionHandler.cs
│ │
│ └── Program.cs
│
└── README.md

# Key Features

- RESTful API design
- PostgreSQL persistence with EF Core
- Async/await for all endpoints
- DTO-based architecture
- Centralized exception handling
- ProblemDetails error responses
- Custom domain exceptions
- Structured logging with Serilog
- Dependency Injection
- Database migrations
- Scalar API testing UI support


# How to Run the Project

1. Clone the repository
git clone https://github.com/Kavin-Maziya/CareerHub.git

2. Open the Terminal and Navigate into the project
cd API

3. Run the application
dotnet run

4. Copy and paste this link into your browser URL
http://localhost:5059

5. Edit the link in the address bar to use Scalar UI
http://localhost:5059/scalar

6. Run API Tests
- NB: Manually add id value when running API to get jobs by id to successfully run tests

## Technologies Used
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Docker
- C#
- Serilog
- OpenAPI (Scalar UI)


//docker run --name careerhub-db -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=password123 -e POSTGRES_DB=CareerHub -p 5432:5432 -d postgres