# ExpertTracker — лабораторна №4 (варіант 2)
Трекер бізнес-витрат ФОП на .NET MAUI 10.

## Функціональність №1–4
№1: MVVM. №2: Shell, Preferences, ILifecycleAware. №3: SQLite + Repository + CRUD.
№4: Microsoft.Extensions.Logging із власним файловим ILoggerProvider; журнал FileSystem.AppDataDirectory/experttracker.log, DI ILogger<T> в DatabaseService і ViewModel, LogInformation/LogWarning/LogError/LogCritical, try-catch і зрозумілі UI-повідомлення, глобальні обробники AppDomain/TaskScheduler.

## Перевірка
Запустити застосунок, виконати успішний CRUD, перевірити записи Information. Змоделювати контрольовану помилку репозиторію в тестовому середовищі та перевірити Error і повідомлення користувача. Не вважати глобальний обробник гарантією продовження роботи після критичного збою.
**Новий код №4 на пристрої ще не перевірено.**
[Інструкція ЛР №4](Reports/Lab04/README.md)

Поточна ціль .NET 10; методичка вказує .NET 8.
