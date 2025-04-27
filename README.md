
# DataCenter Backend

**DataCenter** is a personal backend service developed to manage, store, and track files within a closed, local network environment.

The project was originally intended to be hosted on a **Raspberry Pi**, providing access to a single user within a secured, isolated network.

This project was created primarily for **personal use**, with a focus on experimenting with real-world backend systems and advancing expertise in technologies such as **.NET 8**, **PostgreSQL**, and **Entity Framework Core**.

Although designed for a minimal user base (primarily myself), the system has been developed with **best practices** in mind — ensuring a clean, maintainable, and extensible architecture should future needs evolve.

### **Project Objectives**

- Implement and apply software engineering best practices and design patterns across the solution (including **SOLID principles**, **Repository Pattern**, **Strategy Pattern**, **Domain-Driven Design (DDD)**, **Decorator Pattern**, and more).

- Gain hands-on experience in database management, migrations, and background job processing using production-grade tools and frameworks.

- Build a system that can easily be scaled, adapted, or integrated with additional services if requirements grow over time.

- Create a project structure that mirrors professional software standards, helping bridge the gap between personal experimentation and real-world application development.

---

### **Why This Project Matters**

Beyond technical exploration, **DataCenter** represents a hands-on exercise in system design, problem solving, and operational thinking.

It was built with attention to detail in areas often overlooked in personal projects: reliability, error handling, database integrity, and long-term maintainability.

It acts as a foundation for future expansion — whether adding a front-end UI, deploying in a containerized environment (like Docker), or extending it with new services and APIs.

---

### 🚀 **Technologies**

- **ASP.NET Core 8** — Web application framework for building scalable and high-performance applications.

- **Entity Framework Core (EF Core)** — Object-Relational Mapper (ORM) for interacting with relational databases.

- **PostgreSQL** — Open-source relational database management system.

- **Hangfire** — Background job processing for reliable and efficient asynchronous task execution.

- **Docker** — Containerization for seamless local database setup and environment consistency.

- **JWT Authentication** — Secure token-based authentication for user validation.

- **TOTP Authentication** — Time-based One-Time Password authentication for an added layer of security.

- **Serilog** — Advanced logging framework for structured log data, with support for writing logs to specific text files for future server analysis.

- **SignalR** — Real-time communication framework for broadcasting updates to clients for file upload progress notifications.

---

###  🛠 **Features**

- **Comprehensive File Management** — Supports file upload, download, deletion, recovery, and tracking of stored files.

- **Background Job Management** — Utilizes Hangfire to schedule and execute background tasks, such as automatically deleting files older than one month.

- **Environment Flexibility** — Full support for multiple environments: **Development** and **Production**.

- **Docker Integration** — Streamlined setup for local database instances using Docker, ensuring easy and consistent deployment.

- **Authentication Security** — Secure access management that requires the user to provide an email, password, TOTP, and IP address whitelisting to gain access to files.

---
## **🏁 Getting Started**

### **Prerequisites**

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)

- [Docker](https://www.docker.com/) (optional, for running project and PostgreSQL locally)

- [PostgreSQL](https://www.postgresql.org/) (if not using Docker)

---
### **Setup Instructions**

1. **Clone the Repository**

```bash
git clone https://github.com/your-username/DataCenter.git
cd DataCenter
```

2. **Install Dependencies**

```bash
dotnet restore
```

3. **Database Setup**

If using Docker

```bash
docker-compose up -d
```

Otherwise, set up a PostgreSQL instance manually.

4. **Apply Migrations**

- In **Development**, the app **automatically applies** migrations on startup.
- Or manually run

```bash
dotnet ef database update
```

5. **Run the Application**

```bash
dotnet run
```

6. **Access the API**

Navigate to:

```
http://localhost:5089/swagger
```

to view the Swagger API documentation.

## **⚙ Configuration**

- **Environment Variables**:
    
    - ASPNETCORE_ENVIRONMENT should be set to Development during development.

- **App Settings**:
    
    - appsettings.json — base settings.
        
    - appsettings.Development.json — development-specific settings (used automatically when ASPNETCORE_ENVIRONMENT=Development).

## **📚 Documentation**

- API Documentation available via Swagger (/swagger endpoint when running).

- Entity-relationship mappings handled via EF Core.

- Migrations created and applied using Entity Framework Core tooling.