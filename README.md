
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

# Repository Pattern, DI & Architecture

## Repository Design Decisions

- I created two repositories: IJobListingRepository and IApplicationRepository.
IJobListingRepository has all job listing queries and also handles company data fetch and creation.
- Company does not have its own repository because it has no independent use case in the CareerHub system, a company only needs to be searched and created when creating or updating a job listing. Because separating it into its own repository would require me to add individual endpoints for it. But I implemented CreateJobListingAsync and UpdateJobListingAsync methods on the repository to accept a companyName and industry and handle the find/create logic and keep the service logic clean and readable.
- IApplicationRepository owns all application queries and also handles Applicant find and creation, An applicant is created as part of submitting an application as there is no indvidual Applicant creation endpoint. Keeping this logic in the repository means the service only needs to pass the applicant's details and the repository decides whether to create a new record or reuse an existing one by email.

## What the Controller Lost

- The repository is the only class that talks to EF Core so it is the only location for all database operations because all business rules belongs in the service layer where they can be tested independently.
- I moved Company find and creation to JobListingRepository.CreateJobListingAsync and UpdateJobListingAsync.
- Duplicate job listing endpoint check also moved to JobListingService.CreateJobListingAsync.
- Closed Job listing check endpoint also moved to JobListingService.UpdateJobListingAsync.
- Company ownership check on update also moved to JobListingService.UpdateJobListingAsync checking that the company name on the request matches the existing listing is a business rule.
- The controller no longer builds JobListing or Company objects it only passes the request DTO to the service.
- Salary display formatting also moved to JobListingService.MapSalaryDisplay.

- IsListing open check method moved to ApplicationService.SubmitApplicationAsync. Because whether a listing is available or open is a business rule
- Applicant find and creation moved to ApplicationRepository.CreateApplicationAsync because database access belongs in the repository the controllers should not expose my database context.
- Duplicate application check moved to ApplicationService.SubmitApplicationAsync.
- Status transition validation moved to ApplicationService.UpdateApplicationStatusAsync. The valid transitions are defined once and checks are done in the service without a database query.
- IsApplicationExist check moved to ApplicationService. The controller no longer checks for null it calls the service and the service throws a typed exception which the GlobalExceptionHandler maps to a 404 status.


## Status Transition Design
- Valid Status transitions are defined using a single HashSet of (From, To) tuples inside ApplicationService
- The IsValidTransition method checks whether a status transition pair exists. It is a static method that can be called without a database query and tested independently.
- Adding a new valid transition allow from Offered to Accepted requires adding exactly one line to _validTransitions variable. 
- No switch statements, no if/else, no other files need to change. This implementation meets the requirement for the rules to be defined in one place and a future change will be localised to a single location.

## Lifetime Misconfiguration

- To test build-time DI validation I temporarily registered JobListingService as Singleton while it depended on IJobListingRepository which is Scoped
- The application initially failed to start and emitted because some services were not able to be executed.
- I got Cannot consume scoped service 'APIs.Repositories.IJobListingRepository'
from singleton 'APIs.Service error message
- The container did not allow this because a Singleton is created once and lives for the lifetime of the application. A Scoped service is created once per HTTP request and disposed at the end of that request. 
- If a Singleton holds a reference to a Scoped service, that scoped service is never disposed it will live forever inside the singleton. In my CareerHubDbContext this would mean a single database connection shared across all requests simultaneously, causing data corruption and other corruption errors.
- To fix it was I changed it back to AddScoped with help from Ai (of course) and my application started correctly after fix.

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
- cd CareerHub
   then
- cd APIs

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
