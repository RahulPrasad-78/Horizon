# 🛢️ Database Migration & Setup Guide (Neon PostgreSQL)

This document provides a step-by-step procedure to create a serverless PostgreSQL database on **Neon** and configure the `student-service` to connect to it.

---

## 🚀 Step 1: Create a PostgreSQL Database on Neon

1. **Sign Up / Log In**:
   - Navigate to [Neon Console](https://neon.tech/) and sign in or create a new account.

2. **Create a Project**:
   - Click **Create Project**.
   - **Name**: e.g., `Horizon-SMS`
   - **Database Version**: Keep default (PostgreSQL 16 or latest).
   - **Region**: Choose the AWS region closest to you or your servers (e.g., `US East (N. Virginia)`).
   - Click **Create Project**.

3. **Retrieve the Connection String**:
   - After creation, you will see a connection string screen.
   - Choose **Connection string** in the dropdown.
   - Select **ADO.NET** from the framework/language dropdown list, which gives a format suited for .NET:
     ```text
     Host=<your-neon-host>.neon.tech;Database=neondb;Username=alex;Password=<your-password>;SSL Mode=Require;Trust Server Certificate=True;
     ```
   - Copy this connection string.

---

## 🛠️ Step 2: Configure the Connection String in the Project

1. Open the configuration file [services/student-service/appsettings.json](file:///C:/Users/ravis/Desktop/Horizon/services/student-service/appsettings.json).
2. Find the `"ConnectionStrings"` section and update `"DefaultConnection"` with your Neon connection string:

```json
  "ConnectionStrings": {
    "DefaultConnection": "Host=ep-YOUR-HOST.us-east-2.aws.neon.tech;Database=neondb;Username=YOUR-USERNAME;Password=YOUR-PASSWORD;SSL Mode=Require;Trust Server Certificate=True;"
  }
```

---

## 🔄 Step 3: Run the Application & Migrate Automatically

1. Open a terminal in the root of the project.
2. Build the project:
   ```powershell
   dotnet build
   ```
3. Run the student service:
   ```powershell
   dotnet run --project services/student-service/LearningPlatform.StudentService.csproj
   ```
4. On startup, the service executes `context.Database.MigrateAsync()`, which connects to Neon and automatically creates all necessary schemas and tables (`Bookmarks`, `ProgressRecords`, `StudentProfiles`, `Enrollments`, `__EFMigrationsHistory`).

---

## 💡 Tech Details: How this was migrated in code

- **NuGet Package Added**: `Npgsql.EntityFrameworkCore.PostgreSQL` to [LearningPlatform.StudentService.csproj](file:///C:/Users/ravis/Desktop/Horizon/services/student-service/LearningPlatform.StudentService.csproj).
- **Service Registration Changed**: Updated `builder.Services.AddDbContext<StudentDbContext>` in [Program.cs](file:///C:/Users/ravis/Desktop/Horizon/services/student-service/Program.cs#L100-L101) to use `.UseNpgsql(...)` instead of `.UseSqlServer(...)`.
- **Migrations Cleaned**: Deleted legacy SQL Server migrations and generated fresh, PostgreSQL-compliant migrations.
