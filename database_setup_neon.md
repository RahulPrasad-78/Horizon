# 🛢️ Database Setup Guide (PostgreSQL / Neon)

All Horizon data services use **PostgreSQL** through EF Core (`Npgsql.EntityFrameworkCore.PostgreSQL`), one database per service:

| Service | Config key | Database |
| :--- | :--- | :--- |
| identity-service | `ConnectionStrings:DefaultConnection` | `horizonDB` |
| course-service | `ConnectionStrings:DefaultConnection` | `horizonCourseDB` |
| student-service | `ConnectionStrings:DefaultConnection` | `horizonStudentDB` |
| chat-service | `ConnectionStrings:WebApplication1Context` | `horizonChatDB` |

teacher-service, the gateway and Horizon.MVC have no database.

You can run PostgreSQL **locally** (Option A) or use a free serverless database on **Neon** (Option B).

---

## 🐘 Option A: Local PostgreSQL

1. Install PostgreSQL 16+ ([postgresql.org/download](https://www.postgresql.org/download/)) **or** run it in Docker:
   ```powershell
   docker run -d --name horizon-pg -e POSTGRES_PASSWORD=postgres -p 5432:5432 postgres:16
   ```
2. The committed `appsettings.json` files already point to it:
   ```text
   Host=localhost;Port=5432;Database=<service db>;Username=postgres;Password=postgres
   ```
   You don't need to create the databases yourself. Each service creates its database on first start.

---

## 🚀 Option B: Neon (serverless PostgreSQL)

1. Sign in to the [Neon Console](https://neon.tech/) and click **Create Project** (e.g. `Horizon`), choosing the region closest to you.
2. Open **Connection string**, select **ADO.NET**, and copy it:
   ```text
   Host=<your-neon-host>.neon.tech;Database=neondb;Username=<user>;Password=<password>;SSL Mode=Require
   ```
3. Use a separate database per service: change `Database=` to `horizonDB`, `horizonCourseDB`, `horizonStudentDB` and `horizonChatDB` (create them in the Neon console if your role can't create databases).
4. Prefer overriding the connection string with **user-secrets** or **environment variables** instead of committing credentials:
   ```powershell
   cd services/course-service
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=...;Database=horizonCourseDB;Username=...;Password=...;SSL Mode=Require"
   ```
   (For chat-service, the key is `ConnectionStrings:WebApplication1Context`.)

---

## 🔄 Migrations

- Each data service applies its EF Core migrations automatically on startup (`Database.Migrate()`), creating the database and all tables.
- To change a model, add a new migration from the service folder:
  ```powershell
  dotnet tool install --global dotnet-ef   # once
  cd services/student-service
  dotnet ef migrations add <Name>
  ```

---

## 💡 Tech details

- **Provider:** `Npgsql.EntityFrameworkCore.PostgreSQL` 10.0.2 replaces `Microsoft.EntityFrameworkCore.SqlServer` in identity, course, student and chat. `Program.cs` calls `.UseNpgsql(...)`.
- **Migrations:** the old SQL Server migrations were removed. Each service has one fresh PostgreSQL `InitialCreate` migration.
- **DateTime handling:** each data service sets `AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true)`, so `DateTime` maps to `timestamp without time zone`, matching the old SQL Server `datetime2` behaviour.
