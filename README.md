# 🐘 TaskMaster CLI

**TaskMaster CLI** is a robust task management tool built with **.NET 9** and **PostgreSQL**. This project was designed following clean architecture principles, design patterns, and infrastructure-oriented development using Docker.

---

## 🚀 Tech Stack
* **Runtime:** .NET 9 (C#)
* **Database:** PostgreSQL 16 (Alpine)
* **ORM:** Entity Framework Core (Code First)
* **Dependency Injection:** Microsoft Extensions
* **Infrastructure:** Docker & Docker Compose

---

## 🏗️ Architecture & Features
The project implements a decoupled and professional architecture:
* **Repository Pattern:** Uses interfaces (`ITaskRepository`) to separate persistence logic from business logic.
* **Dependency Injection:** Native .NET DI container for managing the lifecycle of services and database contexts.
* **Defensive Programming:** Exhaustive validations in the repository layer and global exception handling in the CLI.
* **Docker-First Approach:** Fully automated infrastructure to ensure portability across different development environments.

---

## 🛠️ Setup & Run

### Prerequisites
* Docker Desktop
* .NET 9 SDK
* `dotnet-ef` global tool (`dotnet tool install --global dotnet-ef`)

### Steps to Run
1. **Spin up the Database:**
   ```bash
   docker-compose up -d
   ```

2. **Apply Migrations (Database Schema):**
   ```bash
   dotnet ef database update --project TaskMaster.CLI
   ```
   3. **Run the CLI Application:**
   ```bash
  dotnet run --project TaskMaster.CLI
   ```

   🧠 Lessons Learned
Throughout the development of TaskMaster, I achieved the following milestones:

Infrastructure Management: Configured persistent database environments using Docker Volumes.

Inversion of Control (IoC): Implemented IoC to create testable and maintainable systems.

Database Synchronization: Mastered the use of dotnet-ef tools to keep the PostgreSQL schema in sync with the C# models.

Portability: Successfully migrated the project between different machines, verifying that it runs on the first attempt thanks to containerization and clear documentation.

### 🚀 Final Push Instructions

1.  **Save the file** as `README.md` in your root folder.
2.  **Commit and Push:**
    ```powershell
    git add .
    git commit -m "docs: added professional README in English"
    git push origin docs/final-readme
    ```
3.  **Merge the PR:** Go to GitHub and merge this final branch into `main`.



### 🏆 Project Completed!
Congratulations, Alvin! You have officially finished the **TaskMaster CLI** project. You have a solid codebase that demonstrates your skills in **Backend Development, Databases, and DevOps**.

**Would you like me to help you draft the summary for your LinkedIn "Featured" section to showcase this project to recruiters?** 🐘🏅🔥
