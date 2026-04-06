# PLDMS - Peer Learning Day Management System

PLDMS is a comprehensive platform designed to manage **Peer Learning Days (PLD)**. It enables mentors to create sessions, assign exercises, and group students dynamically. Students can collaborate on tasks, submit their code via GitHub, and perform peer reviews, simulating a professional software development workflow.

## Key Features

### Role-Based Portals
- **Admin**: Manage users (mentors/students), cohorts, and academic programs.
- **Mentor**: Orchestrate learning sessions, create and manage exercises/test cases, review student submissions, and assign peer reviews.
- **Student**: Join active sessions, collaborate in randomized groups, submit code to GitHub, and review peer projects.

### Core Functionality
- **Session Management**: Automated session lifecycle (Upcoming → Active → Finished).
- **Randomized Grouping**: Dynamic student grouping for every session based on requested per-group size.
- **Automated Evaluation**: Integration with **Judge0** for real-time code execution and test case validation.
- **GitHub Integration**: Automated branch management and commit tracking for group submissions.
- **Peer Review System**: Structured review workflow (Pending → Under Review → Reviewed → Accepted/Rejected) with scoring and feedback.

### Tech Stack
- **Backend**: .NET 10 (ASP.NET Core MVC)
- **Database**: PostgreSQL
- **Frontend**: Razor Views, Tailwind CSS, Monaco Editor
- **Infrastructure**: Judge0 API, GitHub API
- **Architecture**: N-Layered Architecture (Core, Data, Business, Presentation)

---

## Project Structure

```bash
src/
├── PLDMS.Core/        # Domain entities, Interfaces, Enums
├── PLDMS.DL/          # Data Access Layer, AppDbContext, Migrations
├── PLDMS.BL/          # Business Logic, Services, DTOs
└── PLDMS.PL/          # Presentation Layer (MVC Controllers, Views, Areas)
    ├── Areas/
    │   ├── Admin/     # Administrative logic
    │   ├── Mentor/    # Mentor-specific features
    │   └── Student/   # Student dashboard and workspace
    └── wwwroot/       # Static assets (Tailwind CSS, JS)
```

---

## Development Setup

### Prerequisites
- .NET 10 SDK
- PostgreSQL Server
- GitHub Personal Access Token (for automation)
- Judge0 API Keys (or local instance)

### Installation
1.  **Clone the repository:**
    ```bash
    git clone https://github.com/amin-baghiyev/holbertonschool-final-project.git
    cd holbertonschool-final-project
    ```

2.  **Configure Database:**
    Update the `ConnectionStrings:PostgreSQL` in `src/PLDMS.PL/appsettings.Development.json`.

3.  **Apply Migrations:**
    ```bash
    dotnet ef database update --project src/PLDMS.DL --startup-project src/PLDMS.PL
    ```

4.  **Run the application:**
    ```bash
    dotnet run --project src/PLDMS.PL
    ```

---

## Business Rules
- **One Session Policy**: Students can only participate in one active session at a time.
- **Immutable Active Sessions**: Once a session starts, exercises cannot be changed, and the session cannot be deleted.
- **Submission Window**: Submissions are only allowed within the active session timeframe.
- **Review Lifecycle**: Peer reviews are triggered after session completion to ensure focused execution.

---

*This project was developed for the Holberton School Final Project.*
