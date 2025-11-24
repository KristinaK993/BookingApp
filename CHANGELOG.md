# Changelog

All notable changes to this project will be documented in this file.

The format is simple: date + short description of what changed.

---

## 2025-11-24 – Initial boilerplate created

- Created solution with four main layers:
  - Api
  - Application
  - Domain
  - Infrastructure
- Added basic Clean Architecture structure.

## 2025-11-24 – Added Todo feature and generic repository

- Added `TodoItem` entity in Domain.
- Added `IEntity` and `IGenericRepository<T>` interfaces.
- Implemented `GenericRepository<T>` in Infrastructure (in-memory list, no real DB).
- Added DTO `TodoItemDto` in Application.
- Implemented use cases:
  - `CreateTodoHandler`
  - `GetAllTodosHandler`
- Added `TodoController` in Api with:
  - `POST /api/Todo`
  - `GET /api/Todo`
- Verified the API works via Swagger (200 OK responses).

## 2025-11-24 – Documentation added

- Created `README.md` with project purpose, structure, and startup instructions.
- Created `specs.md` describing features, use cases, and endpoints.
- Created `CHANGELOG.md` to track the history of changes.
