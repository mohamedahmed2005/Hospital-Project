## NovaHealth Hospital Management System

Modern hospital management system built with **ASP.NET Core MVC**, implementing a multi‑role (Admin / Doctor / Patient) workflow for managing appointments, doctors, patients, departments, and medical records.

---

### Features

- **Authentication & Authorization**
  - Email/password registration and login.
  - Distinct roles: **Admin**, **Doctor**, **Patient**.
  - Remember‑me support and password reset flow.

- **Admin Portal**
  - Dashboard with overall hospital insights.
  - CRUD management for **Patients**, **Doctors**, **Departments**.
  - Global appointment management and schedule control.

- **Doctor Portal**
  - Personal dashboard and appointment list.
  - Access to assigned patients and **medical records**.
  - Ability to update diagnosis and treatment plans.

- **Patient Portal**
  - Personal dashboard with upcoming and past appointments.
  - Self‑service appointment booking and cancellation (within rules).
  - View own medical records and doctor information.

- **Appointments & Medical Records**
  - Central **Appointment** module with doctor/patient linkage.
  - **Medical Record** module for diagnoses, notes, and history.

- **Modern UI & UX**
  - Responsive layout using **Bootstrap 5**.
  - Shared **light / dark mode** and **language toggle (EN/AR)**.
  - Consistent, component‑based styling for navbar, auth pages, and dashboards.

---

### Technology Stack

- **Backend**
  - ASP.NET Core MVC (`Hospital.PL`)
  - Business Logic Layer (`Hospital.BBL`)
  - Repository & Service pattern (`Hospital.DAL`, `Hospital.BBL.Services`)

- **Data**
  - Entity Framework Core
  - SQL Server (default)

- **Frontend**
  - Razor views (`.cshtml`)
  - Bootstrap 5 + Bootstrap Icons
  - Custom theming in `wwwroot/css/style.css`

---

### Solution Structure

At the root you will find `HospitalMVCProject.sln` with three main projects:

- **`Hospital.PL`** – Presentation layer (ASP.NET Core MVC)
  - `Controllers/` – MVC controllers (Account, Home, Patient, Doctor, Department, Appointment, Dashboard, etc.).
  - `Views/` – Razor views organized by feature (Account, Appointment, Doctor, Patient, Dashboard, Home, Shared, etc.).
  - `ViewModels/` – View models for account and domain screens.
  - `wwwroot/` – Static assets (CSS, JS, images, Bootstrap bundles).

- **`Hospital.BBL`** – Business / service layer
  - `DTOs/` – Data transfer objects for each aggregate (Doctor, Patient, Department, Appointment, MedicalRecord).
  - `Services/` – Service interfaces and implementations encapsulating business rules.
  - `Mapping/` – Mapping profiles (AutoMapper) between entities and DTOs.

- **`Hospital.DAL`** – Data access layer
  - `Contexts/ApplicationDbContext.cs` – EF Core `DbContext`.
  - `Models/` – Entity models for all modules (Appointment, Department, Doctor, Patient, MedicalRecord, Shared).
  - `Repositories/` – Repository interfaces and implementations.
  - `Migrations/` – EF Core migrations for schema evolution.

---

### Getting Started

#### Prerequisites

- **.NET SDK** (version matching the projects, e.g. `.NET 9.0` based on the `bin/Debug/net9.0` output)
- **SQL Server** (LocalDB, Developer, or any reachable instance)
- An editor or IDE such as **Visual Studio 2022** or **VS Code** with C# support

#### 1. Clone the Repository

```bash
git clone <your-repo-url>
cd HospitalMVCProject/HospitalMVCProject
```

#### 2. Configure the Database

1. Open `Hospital.PL/appsettings.json`.
2. Update the `ConnectionStrings:DefaultConnection` value to point to your SQL Server instance.
3. From the solution root (where the `.sln` lives), open a terminal and run:

```bash
cd Hospital.PL
dotnet ef database update
```

This applies all migrations in `Hospital.DAL/Migrations` to create the database schema.

> If `dotnet ef` is not available, install the EF Core CLI tools:
> ```bash
> dotnet tool install --global dotnet-ef
> ```

#### 3. Run the Application

From the `Hospital.PL` project directory:

```bash
dotnet run
```

Or use **Visual Studio**:

- Set `Hospital.PL` as the **Startup Project**.
- Press **F5** (Debug) or **Ctrl+F5** (Run without debugging).

The app will start on the configured URL (typically `https://localhost:<port>`).  
Navigate to the home page and use the navbar **Log In / Sign Up** buttons to access the authentication flow.

---

### Environments & Configuration

- **`appsettings.json`** – Base configuration (connection strings, email settings, etc.).
- **`appsettings.Development.json`** – Development overrides.
- The active environment is controlled by the `ASPNETCORE_ENVIRONMENT` variable (`Development`, `Staging`, `Production`, …).

Sensitive settings (like SMTP passwords or production connection strings) should be stored **outside source control** (user secrets, environment variables, or secure config providers).

---

### Authentication & Roles

- Identity entities live in the **Shared** model and are wired through `ApplicationDbContext`.
- Registration supports different **user types** (Doctor / Patient) with a tailored UX.
- Role‑based checks are used throughout the UI (for example, the navbar shows different menu items for Admin, Doctor, Patient, or Guest).

If you add new roles or policies, centralize them in the authorization setup in `Program.cs` and use `[Authorize(Roles = "...")]` where appropriate.

---

### Styling & Theming

- Global theme variables and layout styles are defined in:
  - `wwwroot/css/style.css`
- Key UX elements:
  - `dark-mode` CSS variable overrides for dark theme.
  - Language switching (`en-text` / `ar-text`) and `body.rtl` support.
  - Reusable components such as:
    - Auth cards (`_AuthLayout`, `Login.cshtml`, `Register.cshtml`)
    - Public layout with navbar and footer (`_LandingLayout`, `_Navbar`).

When adding new pages, reuse the existing layouts:

- Public pages → `Views/_ViewStart.cshtml` (uses `_LandingLayout`).
- Auth pages → set `Layout = "_AuthLayout";` inside the view.

---

### Development Tips

- **Migrations**
  - Add a new migration:
    ```bash
    cd Hospital.DAL
    dotnet ef migrations add <MigrationName> -s ../Hospital.PL
    ```
  - Apply migrations:
    ```bash
    cd Hospital.PL
    dotnet ef database update
    ```

- **Layered Architecture**
  - Use **DTOs** and **services** (in `Hospital.BBL`) between controllers and EF entities.
  - Keep controllers lean; business rules belong in the **service layer**.

- **Localization / RTL**
  - Use paired `en-text` / `ar-text` spans and the language switcher to toggle languages.
  - For layout mirroring, rely on the `body.rtl` class and the RTL utilities already present in `style.css`.

---

### Contributing

1. Fork the repository.
2. Create a feature branch:
   ```bash
   git checkout -b feature/my-new-feature
   ```
3. Commit your changes with clear messages.
4. Open a pull request describing:
   - What you changed.
   - Why it’s needed.
   - Any migration or configuration steps required.

Please follow existing **coding style**, keep controllers thin, and add or update unit/integration tests where relevant.

---

### License

This project is currently provided **for educational and internal use**.  
Add an explicit license (MIT, Apache 2.0, proprietary, etc.) here if you intend to distribute it publicly.


