## 🏥 NovaHealth Hospital Management System

Modern, multi‑layered **ASP.NET Core MVC** application for managing **patients, doctors, departments, appointments, and medical records** with role‑based access for **Admin**, **Doctor**, and **Patient**.  
Designed with a clean architecture, responsive UI, and bilingual support (EN/AR) for real‑world hospital workflows.

---

### ✨ Key Features

- **🔐 Authentication & Authorization**
  - Email/password registration and login.
  - Role‑based access: **Admin**, **Doctor**, **Patient**.
  - Remember‑me, password reset, and secure identity management.

- **🧑‍💼 Admin Portal**
  - Dashboard with overall hospital insights.
  - Full CRUD for **Patients**, **Doctors**, **Departments**.
  - Global appointment and scheduling management.

- **🩺 Doctor Portal**
  - Personalized dashboard and daily appointment list.
  - Access to assigned patients and **medical records**.
  - Update diagnoses, notes, and treatment plans.

- **👤 Patient Portal**
  - View upcoming and past appointments.
  - Self‑service appointment booking & cancellation (within rules).
  - Access own medical records and doctor information.

- **📅 Appointments & 📝 Medical Records**
  - Centralized **Appointment** module linking doctor and patient.
  - **Medical Record** module for diagnoses, history, and medications.

- **💎 Modern UI & UX**
  - Responsive **Bootstrap 5** layout.
  - **Light / Dark mode** and **language toggle (EN/AR)**.
  - Consistent component‑based design for dashboards and forms.

---

### 🧱 Project Structure

At the root you will find `HospitalMVCProject.sln` with three main projects:

```text
HospitalMVCProject/
└─ HospitalMVCProject/
   ├─ Hospital.PL/              # 🖥️ Presentation layer (ASP.NET Core MVC)
   │  ├─ Controllers/           # MVC controllers (Account, Home, Patient, Doctor, Department, Appointment, Dashboard, etc.)
   │  ├─ Views/                 # Razor views by feature (Account, Appointment, Doctor, Patient, Dashboard, Home, Shared, ...)
   │  ├─ ViewModels/            # View models for UI screens
   │  ├─ Helper/                # Helper classes (e.g. Email)
   │  ├─ wwwroot/               # Static assets (CSS, JS, images, Bootstrap, libs)
   │  └─ Program.cs             # Application startup
   │
   ├─ Hospital.BBL/             # ⚙️ Business Logic Layer
   │  ├─ DTOs/                  # Data Transfer Objects (Doctor, Patient, Department, Appointment, MedicalRecord)
   │  ├─ Mapping/               # Mapping profiles (e.g. AutoMapper) between entities and DTOs
   │  └─ Services/              # Service interfaces & implementations (business rules, orchestration)
   │
   └─ Hospital.DAL/             # 🗄️ Data Access Layer
      ├─ Contexts/              # EF Core DbContext (`ApplicationDbContext`)
      ├─ Models/                # Entity models (Appointment, Department, Doctor, Patient, MedicalRecord, Shared)
      ├─ Repositories/          # Repository interfaces & implementations
      └─ Migrations/            # EF Core migrations history
```

This structure follows a **clean separation of concerns**: UI (`Hospital.PL`), business logic (`Hospital.BBL`), and persistence (`Hospital.DAL`).

---

### 🛠️ Technology Stack

- **Backend**
  - ASP.NET Core MVC (`Hospital.PL`)
  - Layered architecture: **PL → BBL → DAL**
  - Repository & Service pattern

- **Data**
  - Entity Framework Core
  - SQL Server (LocalDB / full instance)

- **Frontend**
  - Razor Views (`.cshtml`)
  - Bootstrap 5 + Bootstrap Icons
  - Custom theming in `wwwroot/css`

---

### 🚀 Getting Started

#### ✅ Prerequisites

- **.NET SDK** (matching the projects, e.g. `.NET 9.0` – see `bin/Debug/net9.0`)
- **SQL Server** (LocalDB, Developer, or any reachable instance)
- **Visual Studio 2022** or **VS Code** with C# tools

#### 1️⃣ Clone the Repository

```bash
git clone <your-repo-url>
cd HospitalMVCProject/HospitalMVCProject
```

#### 2️⃣ Configure the Database

1. Open `Hospital.PL/appsettings.json`.
2. Update `ConnectionStrings:DefaultConnection` to your SQL Server instance.
3. From the solution root (where the `.sln` is), run:

```bash
cd Hospital.PL
dotnet ef database update
```

This applies all migrations from `Hospital.DAL/Migrations` and creates the database schema.

> If `dotnet ef` is missing, install EF Core CLI:
> ```bash
> dotnet tool install --global dotnet-ef
> ```

#### 3️⃣ Run the Application

From the `Hospital.PL` directory:

```bash
dotnet run
```

Or via **Visual Studio**:

- Set `Hospital.PL` as the **Startup Project**.
- Press **F5** (Debug) or **Ctrl+F5** (Run without debugging).

Open the browser at the shown URL (e.g. `https://localhost:<port>`) and use the navbar to **Register / Log In** based on your role.

---

### ⚙️ Configuration & Environments

- **`appsettings.json`** – Base configuration (connection strings, email settings, etc.).
- **`appsettings.Development.json`** – Development overrides.
- Active environment controlled by `ASPNETCORE_ENVIRONMENT` (`Development`, `Staging`, `Production`, ...).

Store sensitive values (e.g. SMTP passwords, production connection strings) **outside source control** (User Secrets, environment variables, or a secure config store).

---

### 👥 Authentication, Roles & Access Control

- Identity is wired through `ApplicationDbContext` in the DAL/PL.
- Supports **Admin**, **Doctor**, **Patient** with tailored UI.
- Controllers and views use `[Authorize(Roles = "...")]` and `User.IsInRole(...)` checks to control visibility and access.

When adding new roles or policies, configure them centrally in `Program.cs` and keep authorization logic declarative.

#### 🔑 Google Login (OAuth 2.0)

The app supports **“Continue with Google”** on the login page using Google OAuth:

- **1. Create OAuth client in Google Cloud Console**
  - Go to `APIs & Services` → `Credentials` → **Create Credentials** → **OAuth client ID** → **Web application**.
  - Add an **Authorized redirect URI** matching your dev URL, e.g.:
    - `https://localhost:5001/signin-google` (adjust port to your actual dev port).

- **2. Configure `appsettings.json`**
  - In `Hospital.PL/appsettings.json`, set:
    ```json
    "Authentication": {
      "Google": {
        "ClientId": "YOUR_GOOGLE_CLIENT_ID",
        "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET",
        "CallbackPath": "/signin-google"
      }
    }
    ```

- **3. How it works in the app**
  - `Program.cs` wires Google via `builder.Services.AddAuthentication().AddGoogle(...)` using the config above.
  - `AccountController` exposes `ExternalLogin` (POST) and `ExternalLoginCallback` (GET) actions.
  - `Views/Account/Login.cshtml` shows a **“Continue with Google”** button that posts to `ExternalLogin`.
  - On first Google login, a local user is created (default role: **Patient**) and linked to the Google account, then redirected to the appropriate dashboard.

> ⚠️ For real deployments, **do not commit** real Google secrets. Use **User Secrets** or environment variables and regenerate the secret if it was ever exposed.

---

### 🎨 Styling, Theming & Localization

- Global styles in `wwwroot/css` (including main theming file).
- Dark mode via CSS variables and theme toggles.
- EN/AR language toggling using paired `en-text` / `ar-text` elements and RTL support via `body.rtl`.
- Reusable layouts:
  - Public pages → `_Layout` / landing layout in `Views/Shared`.
  - Auth pages → `_AuthLayout` for login/register flows.

When adding new pages, reuse existing layouts and CSS utilities to keep UX consistent.

---

### 💡 Development Tips

- **Migrations**
  - Add:
    ```bash
    cd Hospital.DAL
    dotnet ef migrations add <MigrationName> -s ../Hospital.PL
    ```
  - Apply:
    ```bash
    cd Hospital.PL
    dotnet ef database update
    ```

- **Clean Architecture**
  - Keep controllers thin; push business rules into **services** in `Hospital.BBL`.
  - Use **DTOs** between BBL and PL; avoid exposing EF entities directly to the views.

- **Localization & RTL**
  - Use `en-text` / `ar-text` spans for text.
  - Use existing RTL classes and `body.rtl` to mirror layouts when needed.

---

### 🤝 Contributing

1. Fork the repository.
2. Create a feature branch:
   ```bash
   git checkout -b feature/my-new-feature
   ```
3. Commit changes with clear, descriptive messages.
4. Open a Pull Request describing:
   - What you changed.
   - Why it’s needed.
   - Any migration or configuration steps required.

Please follow existing **coding style**, respect the layered architecture, and add/update tests when relevant.

---

### 📄 License

This project is currently provided **for educational and internal use**.  
Add a formal license (MIT, Apache 2.0, proprietary, etc.) here before public distribution.
