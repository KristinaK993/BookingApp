# 📘 BookingApp-API  
*A clean and testable backend API for the BookingApp ecosystem.*

BookingApp-API is a .NET backend for managing **bookings, customers, employees, and services**.  
It is built with **Clean Architecture**, separating concerns across API, Application, Domain, and Infrastructure layers.

This API is consumed by the `BookingApp-FE` React frontend.

---

## 🚀 Features

### 📅 Booking Management
- Create bookings  
- Update bookings  
- Cancel or change booking status  
- Get a booking by ID  
- List all bookings  

### 👥 Customers & Employees
- CRUD operations for **Customers**  
- CRUD operations for **Employees**  
- Endpoints used for frontend dropdowns (returning names instead of IDs)  

### 🛎 Services
- CRUD operations for services  
- Used to attach service information to bookings  

### ✔ Validation & Error Handling
- ASP.NET Core model validation  
- Centralized custom exception middleware  
- Consistent error responses for the frontend  

### 🧱 Architecture
- Clean Architecture structure:
  - **API** (controllers, middleware)
  - **Application** (business rules, interfaces)
  - **Domain** (core entities)
  - **Infrastructure** (EF Core, database implementation)

### 🧪 Testing
- Integration tests (`API.IntegrationTests`)  
- Application layer tests (`Application.Tests`)  

---

## 🛠 Tech Stack

| Category        | Technology |
|-----------------|------------|
| **Framework**   | ASP.NET Core Web API (.NET) |
| **Architecture**| Clean Architecture |
| **ORM**         | Entity Framework Core |
| **Database**    | SQL Server |
| **Testing**     | NUnit, Integration Tests & Application Tests |
| **Docs**        | Swagger / OpenAPI |

---

## 📦 Installation & Setup

### 1️⃣ Clone the repository
```bash
git clone https://github.com/your-username/BookingApp-API.git
cd BookingApp-API
```

### 2️⃣ Configure the database connection
Edit `API/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=BookingApp;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3️⃣ Apply Entity Framework migrations
```bash
dotnet ef database update --project API
```

### 4️⃣ Run the API
```bash
dotnet run --project API
```

Your API will be available at:  
👉 https://localhost:7263  
Swagger UI:  
👉 https://localhost:7263/swagger  

---

## 🔌 API Overview

| Resource    | Example Endpoint                    | Description                         |
|------------|--------------------------------------|-------------------------------------|
| **Bookings**  | `GET /api/bookings?companyId=1`      | Retrieve all bookings               |
| **Bookings**  | `POST /api/bookings`                | Create a new booking                |
| **Customers** | `GET /api/customers?companyId=1`     | Retrieve all customers              |
| **Employees** | `GET /api/employees?companyId=1`     | Retrieve all employees              |
| **Services**  | `GET /api/services?companyId=1`      | Retrieve all services               |

---

## 📁 Project Structure

```text
API/
 ├─ Controllers/              # Endpoints (Bookings, Customers, Employees, Services)
 ├─ Data/                     # EF Core DbContext, seeding
 ├─ Dtos/                     # Request & response DTOs
 ├─ Middleware/               # Exception handling middleware
 ├─ Migrations/               # EF Core migrations
 ├─ Models/                   # API-level models
 ├─ Services/                 # Business logic services
 ├─ API.http                  # Test requests
 ├─ appsettings.json          # Config
 └─ Program.cs                # Entry point

API.IntegrationTests/
 ├─ BookingTests/             # Booking endpoint tests
 │   ├─ CreateBookingTests.cs
 │   └─ GetAllBookingsTests.cs
 └─ Helpers/                  # CustomWebApplicationFactory, utilities

Application/                  # Use cases, business logic interfaces  
Application.Tests/            # Tests for Application layer  
Domain/                       # Entities, enums, business rules  
Infrastructure/               # EF Core, repositories, database logic  
```

Built to be:  
✔ **Scalable**  
✔ **Testable**  
✔ **Easy to maintain**

---

## 🔮 Future Improvements  
- Background tasks (email notifications)  
