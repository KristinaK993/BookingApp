# Clean Architecture Boilerplate (.NET)

Det här projektet är ett boilerplate-backend byggt med Clean Architecture. 
Syftet är att ha en färdig mall man kan klona, forka och använda när man startar nya backend-projekt.

## ?? Syfte
- Träna Clean Architecture-struktur
- Tydlig separation mellan lager
- Använda Generic Repository Pattern
- Ha ett projekt som går att starta direkt
- Perfekt som skolprojekt eller framtida mall

## ?? Projektstruktur


## ?? Clean Architecture-lager

### Domain
- Entiteter (t.ex. `TodoItem`)
- Interface `IGenericRepository<T>`
- Domänlogik

### Application
- Use cases (`CreateTodoHandler`, `GetAllTodosHandler`)
- DTOs
- Ingen dataaccess direkt

### Infrastructure
- FakeRepository som lagrar data i en lista
- Ingen riktig databas (enkel och snabb att starta)

### Api
- Controllers
- Använder use cases via dependency injection
- Exponerar endpoints för Todo

## ?? Starta projektet

I solution root: 
dotnet build
dotnet run --project Api

Swagger öppnas automatiskt:
https://localhost:<port>/swagger


## ?? Generic Repository Pattern

- `IGenericRepository<T>` definieras i Domain
- `GenericRepository<T>` implementeras i Infrastructure
- API och use cases kommunicerar alltid via interface
- Ingen direkt dataaccess i API-lagret

## ?? Dokument

- `README.md` ? första intrycket, hur man kör och hur arkitekturen funkar
- `specs.md` ? vilka features, endpoints och regler systemet har
- `CHANGELOG.md` ? historik över ändringar i projektet




