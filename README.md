# DotnetCoreBankingBackend

A secure Banking REST API built with **ASP.NET Core Minimal APIs**, **Entity Framework Core**, **PostgreSQL**, and **JWT Authentication**.

This project demonstrates a complete banking backend with authentication, account management, money transfers, and transaction history. It was built as a portfolio project following modern .NET development practices.

---

## Features

### Authentication
- User Registration
- User Login
- JWT Authentication
- Protected Endpoints
- Password Hashing with BCrypt

### Account Management
- Create Bank Account
- View User Accounts
- Multiple Accounts Per User
- Primary Account Support

### Banking Operations
- Deposit Money
- Withdraw Money
- Transfer Money Between Accounts
- Balance Validation
- Transaction Recording

### Transactions
- Deposit History
- Withdrawal History
- Transfer History
- Reference Account Information
- Account Statement Endpoint

---

## Tech Stack

| Technology | Description |
|------------|-------------|
| ASP.NET Core | Minimal APIs |
| Entity Framework Core | ORM |
| PostgreSQL | Database |
| JWT | Authentication |
| BCrypt | Password Hashing |
| C# | Programming Language |

---

## Project Structure

```
BankingApp.Api
│
├── DTOs
├── Data
├── EndPoints
├── Enums
├── Migrations
├── Models
├── Services
├── Program.cs
```

---

## Database Schema

### Users

- Id
- FullName
- Email
- PasswordHash

### Accounts

- Id
- UserId
- AccountNumber
- AccountType
- Balance
- IsPrimary

### Transactions

- Id
- AccountId
- Amount
- Type
- ReferenceAccountNumber
- CreatedAt

---

## API Endpoints

### Authentication

| Method | Endpoint |
|---------|----------|
| POST | /register |
| POST | /login |

---

### Accounts

| Method | Endpoint |
|---------|----------|
| GET | /accounts |
| POST | /accounts |
| PUT | /accounts/{id} |

---

### Transactions

| Method | Endpoint |
|---------|----------|
| POST | /accounts/{id}/deposit |
| POST | /accounts/{id}/withdraw |
| POST | /accounts/transfer |
| GET | /accounts/{accountNumber}/transactions |

---

## Authentication

After logging in, include the JWT token in every protected request.

```
Authorization: Bearer YOUR_JWT_TOKEN
```

---

## Running the Project

### Clone Repository

```bash
git clone https://github.com/shehryarazizbhutta/DotnetCoreBankingBackend.git
```

### Navigate

```bash
cd DotnetCoreBankingBackend
```

### Restore Packages

```bash
dotnet restore
```

### Update Database

```bash
dotnet ef database update
```

### Run Project

```bash
dotnet run
```

---

## Configuration

Create an `appsettings.json` file with your own values.

Example:

```json
{
  "ConnectionStrings": {
    "BankingAppDb": "Host=localhost;Port=5432;Database=BankingApp;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "Key": "YourSecretKey",
    "Issuer": "BankingApp",
    "Audience": "BankingAppUsers"
  }
}
```

---

## Current Features

- JWT Authentication
- Entity Framework Core
- PostgreSQL
- Minimal APIs
- DTO Pattern
- Dependency Injection
- REST API Design
- Account Management
- Deposit
- Withdrawal
- Transfer
- Transaction History
- Secure Password Hashing

---

## Planned Improvements

- Clean Architecture
- Repository Pattern
- Service Layer
- Swagger Documentation
- Global Exception Handling
- Logging
- Docker Support
- Azure Deployment
- Unit Testing
- Integration Testing
- Pagination
- Refresh Tokens

---

## Future Projects

This backend will be integrated with:

- Flutter Mobile Application
- React/Angular Web Application

---

## Author

**Shehryar Aziz**

Senior Software Developer

- Flutter
- ASP.NET Core
- Android
- PostgreSQL
- REST APIs

GitHub:
https://github.com/shehryarazizbhutta

---

## License

This project is created for learning and portfolio purposes.
