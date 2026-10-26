# ExpertTracker — Лабораторна робота №5

Дисципліна: Технології розробки мобільних додатків на C#. Варіант 2 — трекер бізнес-витрат ФОП.

## Реалізація

- Застосунок .NET MAUI 10 з MVVM, SQLite та ILogger<T>.
- Окремий тестовий проєкт `ExpertTracker.Tests` на xUnit і Moq.
- Ізоляція ViewModel через `IExpenseRepository` та `IDraftStore`.
- Unit-тести (13 сценаріїв): завантаження, успішне додавання, помилки читання/запису, нульовий результат вставки, перевірка неправильних сум та обов'язкових полів.
- Integration-тести (2 сценарії): повний CRUD-цикл у тимчасовій SQLite та перевірка збереження після повторного відкриття бази.
- Тестовий проєкт використовує Windows target `net10.0-windows10.0.19041.0` та не запускає графічний інтерфейс.

## Запуск тестів

У Developer PowerShell (Windows):

```powershell
cd D:\Universitat\TPMD\ExpertTracker
dotnet restore ExpertTracker.Tests\ExpertTracker.Tests.csproj
dotnet test ExpertTracker.Tests\ExpertTracker.Tests.csproj --no-restore
```

Тести також доступні через Test Explorer у Visual Studio. 

## GitHub

https://github.com/Megprost/Universitat/tree/lab-05
