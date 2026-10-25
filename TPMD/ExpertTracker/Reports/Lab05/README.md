# Лабораторна №5 — xUnit, Moq, SQLite

Створено окремий тестовий проєкт `ExpertTracker.Tests` для Windows .NET 10, додано до .slnx.
Unit-тести: завантаження, додавання через mock, обробка помилки, Theory/InlineData.
Інтеграційний тест: Create → Read → Update → Delete в окремій тимчасовій SQLite.
`IDraftStore` винесено для ізоляції Preferences API під час тестів.

**Статус:** тести написані, але `dotnet test` не запущено, прохідність не підтверджена.
Команда: `dotnet test ExpertTracker.Tests/ExpertTracker.Tests.csproj` з папки `TPMD/ExpertTracker`.

Звіт та журнал LLM оформлюються лише з фактичним скриншотом `dotnet test` і рішеннями студента.
