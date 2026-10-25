# ExpertTracker — лабораторна №5 (варіант 2)

## Реалізовано
Попередні можливості №1–4 і тестовий проєкт ExpertTracker.Tests на xUnit + Moq. `IDraftStore` відділяє Preferences від ViewModel. Unit-тести використовують mock IExpenseRepository. Інтеграційний тест використовує окремий тимчасовий файл SQLite й перевіряє Insert → Read → Update → Delete. Додано [Fact], [Theory] / [InlineData], обробку винятків і перевірку валідації.

## Запуск
```powershell
cd D:\Universitat\TPMD\ExpertTracker
dotnet test ExpertTracker.Tests/ExpertTracker.Tests.csproj
```

**Тести ще не запускалися в Visual Studio, результат PASS не підтверджено.**
[Інструкція ЛР №5](Reports/Lab05/README.md)

.NET 10 замість .NET 8 потребує погодження.
