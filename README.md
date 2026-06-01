
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


# Project Structure
CareerHub/
│
├── APIs/
│
├── Controllers/
│   └── JobsController.cs
│
├── DTOs/
│   ├── CreateJobRequest.cs
│   ├── UpdateJobRequest.cs
│   └── JobResponse.cs
│
├── Models/
│   ├── JobListing.cs
│   └── JobType.cs
│
├── Data/
│   └── JobListingStore.cs
│
├── Exceptions/
│   ├── JobNotFoundException.cs
│   └── DuplicateJobListingException.cs
│
├── Middleware/
│   └── GlobalExceptionHandler.cs
│
├── Program.cs
│
└── README.md

# Key Features
- RESTful API design
- Async/await implementation for all endpoints
- DTO-based request and response models
- Centralized exception handling
- Problem Details support
- Custom domain exceptions
- Structured logging using Serilog
- Dependency Injection
- Controller-based architecture
- OpenAPI documentation support
- Scalar UI API testing interface
<!-- ## Project Structure

CareerHub/
│
├── APIs/
│ │
│ ├── Controllers/
│ │ └── JobsController.cs
│ │
│ ├── DTOs/
│ │ ├── CreateJobRequest.cs
│ │ ├── UpdateJobRequest.cs
│ │ └── JobResponse.cs
│ │
│ ├── Models/
│ │ ├── JobListing.cs
│ │ └── JobType.cs
│ │
│ ├── Data/
│ │ └── JobListingStore.cs
│ │
│ └── Program.cs
│
└── README.md

---

## Key Features
- RESTful API design
- Async/await implementation for all endpoints
- Proper HTTP status code handling (200, 404)
- Clean separation of controller and data service
- Dependency Injection using ASP.NET Core built-in container
- OpenAPI support for testing via Scalar UI -->

## How to Run the Project

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
- C#
- OpenAPI (Scalar UI)
- Testing: All endpoints were tested using Scalar UI:
