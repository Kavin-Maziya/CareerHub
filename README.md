# CareerHub

## Overview
CareerHub API is a simple ASP.NET Core Web API built using .NET 10.  
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

## Architecture API Choice (Controllers)
I used Controllers because they offer a clear and readable project structure, separation of concerns, and are ideal for large applications with advanced feature support. They are the most logical choice for large ASP.NET Core projects.

---

## DTO Implementation

### Domain Model: 
- `PostedAt` is generated and controlled by the server when a job is created because clients should only see when a job was posted, not update or create it.

### Validation Rules:
- I used `IValidatableObject` to validate the relationship between `SalaryMin` and `SalaryMax` without using manual validation inside the controller. This approach keeps my `JobsController` cleaner and keeps validation responsibilities inside the `CreateJobsRequest` DTO model.

---

# Authentication & Authorization

## Stateless Authentication
- **Session-based authentication** stores user information on the server after a successful login. The server creates a session and keeps track of the user's identity between requests using a session ID. 
- **JWT-based authentication** is stateless. The server generates a JSON Web Token with user information and sends it to the client after login. The client uses the token for future requests, and the server validates the token without storing any session data.
- **Statelessness is important** for horizontally scaled APIs because requests can be handled by any server instance. Since user information is contained within the JWT, servers do not need to share session data, making the application easier to scale.

## 401 Unauthorized vs 403 Forbidden
- **401 Unauthorized** means the user has not provided valid login credentials. The authentication middleware generates the 401 response before the request reaches the controller.
- **403 Forbidden** means the user is authenticated but does not have the permission to perform the requested action. The authorization middleware generates the 403 response after authentication has succeeded.

## Token Storage
- Storing JWTs in `localStorage` is considered a security risk because JavaScript running in the browser can access it. If an application is vulnerable to a Cross-Site Scripting (XSS) attack, an attacker could steal the token.
- Storing tokens in **secure HttpOnly cookies** is a safer alternative. Because HttpOnly cookies cannot be accessed through JavaScript, tokens are protected from XSS scripts.

---

# Error Handling

## Custom Domain Exceptions (Advantages)
### Controller Thinning: 
Throwing a new `JobNotFoundException` instead of returning `NotFound()` separates business logic from HTTP concerns. Controllers focus on successful request processing, while the Global Exception Handler centrally translates exceptions into HTTP responses. 

**Advantages of throwing domain exceptions:**
- Reduces duplicated error-checking code.
- Keeps controllers thin, clean, and readable.
- Ensures consistent error formats throughout the entire API.

### Structured Logging with Serilog
Serilog stores log information as structured data (like JSON blocks) rather than flat, plain text lines, unlike traditional `Console.WriteLine()` statements.

**Advantages of using Serilog:**
- Faster error debugging with searchable properties.
- Better log filtering and querying capabilities in dashboard tools.
- Improved production diagnostics and consistent formatting.

---

# Database (EF Core + PostgreSQL)

### Change Tracker
The EF Core Change Tracker tracks entity state changes in the application's memory layer. When `SaveChangesAsync()` is called, it compares the original data to the current modified data and updates the database with only the necessary changes.

### Migrations as Version Control
- Generated migration files must be committed to source control alongside the code changes because they ensure a consistent database layout across all environments and allow team members to track schema evolution history.
- When a teammate pulls code that references a migration they have not applied locally, the application will fail to run or throw errors because the matching physical tables do not exist in their local database yet.

### Connection String Security
- Connection strings contain sensitive database credentials and should only be stored in `appsettings.Development.json` for local work to safely exclude them from production source control.
- **Safer production alternatives:**
  - Environment Variables
  - Docker Secrets
  - Cloud Vaults (Azure Key Vault / AWS Secrets Manager)

---

# Part 1 — Written Decisions (Testing & CI/CD)

## 1. Unit Tests vs. Integration Tests Matrix
- We use specific test strategies to handle different types of logic. Each testing type has hard technical limits on what it can actually verify:

- **Salary Range Check (`JobListingService.CreateAsync`) $\rightarrow$ Unit Test**
  * *Why:* It validates internal logical code by making sure minimum salary isn't higher than maximum instantly in-memory without a database.
  * *What it misses:* It cannot check if the database layout maps correctly or if database table constraints are broken.
- **The `[Authorize]` Security Rule $\rightarrow$ Integration Test**
  * *Why:* Validating route locks and user role flags depends directly on the ASP.NET Core middleware routing pipeline, not service code.
  * *What it misses:* It cannot verify whether database operations fail due to index issues or backend table connection lockouts.
- **Database Constraints (`SalaryMax > SalaryMin`) $\rightarrow$ Repository Test**
  * *Why:* This rule is hardcoded inside the physical PostgreSQL tables. It requires a live database engine instance to verify.
  * *What it misses:* It cannot check if your HTTP route links, URL paths, or API controller methods are spelled correctly.
- **API Version Headers (`api-supported-versions: 1.0`) $\rightarrow$ Integration Test**
  * *Why:* This header is injected by the API versioning middle-tier during an active HTTP request lifecycle execution.
  * *What it misses:* It cannot check if application database tracking states or records were updated correctly.
- **Compiled Application Queries (`HasAppliedAsync`) $\rightarrow$ Repository Test**
  * *Why:* It proves that our custom C# LINQ query translates cleanly into proper PostgreSQL SQL syntax without syntax crashes.
  * *What it misses:* It cannot check web security filters, rate limits, or user access permissions.

## 2. Why "In-Memory" Database Providers Aren't Good Enough
Using an EF Core "In-Memory" database provider is fast, but it is dangerous for CareerHub because:
1. **It completely ignores check constraints:** It will let you save a job with a minimum salary higher than the maximum salary, whereas the real PostgreSQL database would immediately reject it.
2. **It cannot handle Full-Text Search:** It doesn't understand PostgreSQL commands like `to_tsvector`. Testing search features on it will fail or bypass logic entirely.
3. **It skips query translation checks:** It runs queries within local memory. It skips translating C# code into SQL execution plans, which lets broken queries slip past your tests unnoticed.

## 3. Test Isolation Explained
- **Test Isolation** means that every single test runs inside its own clean environment without being altered by data from other tests. 

- If tests share a database without isolation, records left behind by one test will cause subsequent tests to fail due to duplicate keys or bad counts. We solve this by using **Testcontainers** to launch a fresh PostgreSQL container, and we run a helper method to clear out all data tables before every single test runs.

## 4. Why We Need a Centralized CI Pipeline
- Running tests on your own computer only proves the code works on your personal machine setup. 

- If Developer A changes a database column name on their branch, and Developer B writes code using the old column name on another branch, both will pass their tests locally. The **Continuous Integration (CI) pipeline** acts as an impartial validator by merging their branches together on a fresh server and running the tests from scratch, catching the conflict before it can break production.

---

# CI/CD Pipeline & Branch Rules

### GitHub Actions Workflow Configuration
Our automated pipeline (`.github/workflows/ci.yml`) runs the following steps on every push or pull request to the `main` branch:
1. Downloads the repository code.
2. Sets up the specialized .NET 10 runtime SDK.
3. Restores all required NuGet dependencies.
4. Compiles the application binaries.
5. Runs all Unit, Integration, and Repository tests.

### Branch Protection Settings
1. **Require status checks to pass before merging:** Blocks code merges into `main` unless our automated test workflow passes with a green checkmark.
2. **Require branches to be up to date before merging:** If another team member merges code first, GitHub forces you to update your branch and re-run tests. This prevents two separately passing branches from breaking each other when combined.
3. **Do not allow bypassing:** Applies these constraints to everyone (including administrators), ensuring that no unverified code can ever reach production.

---

# Test Structure & Design

### The Testing Pyramid
- **Unit Tests (10 tests):** The fast foundation layer. They test core C# validation rules and state modifications instantly.
- **Repository Tests (9 tests):** The middle database layer. They talk to a real containerized PostgreSQL engine to check table constraints.
- **Integration Tests** (10 tests): The top pipeline layer. They test the full runtime, checking endpoints, authorization, and headers.

### Clear Test Naming Patterns
I named my tests using the template: `MethodName_Scenario_ExpectedResult`. 
`CreateJobListingAsync_WhenSalaryMaxLessThanSalaryMin_ThrowsInvalidSalaryRangeException`
`GetJobById_WithMatchingETag_Returns304NotModified`

Generic names like `Test1` or `Test2` force you to dig through source code to understand a failure. Clear names tell you exactly what target, input scenario, and business rule broke directly inside the test runner logs.

### The `public partial class Program {}` Compiler Change
Adding `public partial class Program {}` at the bottom of `Program.cs` exposes the internal web application entry point to my external `API.Tests` project. This allows `WebApplicationFactory` to boot up the API server in memory for integration testing. It is a compile-time structure hint that adds zero performance or runtime overhead to production.

---

# Project Structure
```text
CareerHub/
│
├── APIs/
│ ├── Controllers/
│ │ ├── JobsController.cs
│ │ └── AuthController.cs
│ │
│ ├── DTOs/
│ │ ├── CreateJobRequest.cs
│ │ ├── LoginRequest.cs
│ │ ├── LoginResponse.cs
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