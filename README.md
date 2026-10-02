# Car Rental Management System

Web application developed as a technical challenge for managing vehicles, clients and rental contracts.

The application was built with ASP.NET Core MVC and focuses on business rule validation, vehicle availability management and a clear rental workflow.

## Technologies

- ASP.NET Core MVC
- C#
- .NET 10
- Entity Framework Core
- SQL Server
- Docker
- HTML
- CSS
- JavaScript
- Bootstrap
- xUnit
- Entity Framework Core InMemory (automated tests)

## Features

### Vehicles

- Create, view, edit and delete vehicles
- Required field validation
- Manufacturing year validation
- Unique license plate validation
- Automatic vehicle status:
  - Available
  - Rented
- Protection against deleting vehicles associated with rental contracts

### Clients

- Create, view, edit and delete clients
- Required field validation
- Email format validation
- Unique email validation
- Phone number validation
- Protection against deleting clients associated with rental contracts

### Rental Contracts

- Create and list rental contracts
- Select clients and vehicles from available data
- Start and end date validation
- Initial mileage registration
- Prevention of overlapping rental periods for the same vehicle
- Vehicle availability calendar
- Rental lifecycle management:
  - Scheduled
  - Active
  - Pending completion
  - Completed
  - Cancelled
- Final mileage registration when completing a rental
- Editing restrictions according to contract status
- Cancellation of scheduled contracts

### Dashboard

The dashboard provides an overview of:

- Total vehicles
- Available and rented vehicles
- Total clients
- Active rental contracts
- Upcoming rental contracts

## Business Rules

The application implements the following main business rules:

- Vehicle brand, model, license plate, manufacturing year and fuel type are required.
- Manufacturing year cannot be greater than the current year.
- License plates must be unique.
- Client name, email, phone number and driving licence are required.
- Emails must be valid and unique.
- Phone numbers must contain exactly 9 numeric digits.
- Rental contracts require a client and a vehicle.
- Start date cannot be earlier than the current date when creating a rental.
- End date must be later than start date.
- Initial mileage is required and cannot be negative.
- A vehicle cannot have overlapping active or scheduled rental contracts.
- Vehicle status is automatically determined from its current rental contracts.

## Database

The application uses SQL Server with Entity Framework Core using the Code First approach.

SQL Server can be started using Docker:

```bash
docker run \
  --name carrental-sql \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=YOUR_STRONG_PASSWORD" \
  -p 1433:1433 \
  -d mcr.microsoft.com/mssql/server:2025-latest
```

> Replace `YOUR_STRONG_PASSWORD` with a secure password.

## Configuration

The database connection string is stored using .NET User Secrets and is not committed to the repository.

From the project directory:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=CarRentalDb;User Id=sa;Password=YOUR_STRONG_PASSWORD;TrustServerCertificate=True;"
```

Replace `YOUR_STRONG_PASSWORD` with the password configured for the SQL Server container.

## Database Setup

Apply the Entity Framework Core migrations:

```bash
dotnet ef database update
```

This will create/update the database according to the existing migrations.

## Running the Application

Restore the dependencies:

```bash
dotnet restore
```

Run the application:

```bash
dotnet run
```

The terminal will display the local URL where the application is available.

## Automated Tests

The solution includes automated tests covering the main validation and business requirements.

Run all tests with:

```bash
dotnet test CarRental.slnx
```

The test suite covers, among other rules:

- Required vehicle fields
- Manufacturing year validation
- Duplicate license plates
- Required client fields
- Email validation
- Duplicate emails
- Phone number validation
- Rental date validation
- Invalid final mileage
- Rental start dates in the past
- Vehicle rental status

## Project Structure

```text
CarRental/
├── Controllers/
├── Data/
├── Migrations/
├── Models/
├── Views/
├── wwwroot/
├── CarRental.Tests/
├── CarRental.csproj
└── CarRental.slnx
```

## Notes

The project includes additional usability and business-rule improvements beyond the basic requirements, including rental overlap prevention, availability visualization, contract lifecycle management and safeguards for related data deletion.