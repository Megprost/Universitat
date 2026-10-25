# ExpertTracker — лабораторні роботи №1–2

**Варіант 2:** Трекер бізнес-витрат фізичної особи-підприємця.  
**Дисципліна:** Технології розробки мобільних додатків на C#.

## Реалізовано

### ЛР №1 — MVVM
- `Expense` (Model), `ExpensesPage` (View), `ExpensesViewModel` (ViewModel);
- `CommunityToolkit.Mvvm`, `ObservableCollection`, `RelayCommand`, Binding, Dependency Injection;
- форма додавання, список витрат і підсумкова сума.

### ЛР №2 — Навігація, життєвий цикл, стан
- другий екран `ExpenseDetailsPage` та `ExpenseDetailsViewModel`;
- Shell-навігація за вибраною витратою з параметром `id`;
- передавання ID через `GoToAsync` та отримання через `IQueryAttributable`;
- `ILifecycleAware` для обох ViewModel;
- діагностика Window lifecycle (Created, Activated, Deactivated, Stopped, Resumed, Destroying);
- збереження та відновлення чернетки назви, категорії й суми через `Preferences`.

На цій стадії **витрати зберігаються лише в оперативній пам'яті**. Для деталей використовується спільне `InMemoryExpenseStore`. SQLite — наступна лабораторна робота №3.

## Середовище

Поточний проєкт: .NET MAUI 10, Visual Studio Insiders, C#, `CommunityToolkit.Mvvm 8.4.2`.
Методичка вказує .NET 8 — різницю версій необхідно погодити з викладачем.

## Стан перевірки

ЛР №1: раніше перевірено збірку й запуск на Windows, додавання витрат і підсумок.
ЛР №2: **код додано, але збірка/поведінка на Windows і Android ще потребують фактичної перевірки**. Не позначати її як протестовану.

## Як запустити

У Visual Studio відкрити рішення `ExpertTracker.sln`, обрати `Windows Machine` або Android-емулятор, виконати `Build → Rebuild Solution`, потім запустити. Додати витрату, натиснути на її запис і перевірити екран деталей. Ввести незавершену чернетку, згорнути та відновити застосунок.

Докладний протокол перевірки: [Reports/Lab02/README.md](Reports/Lab02/README.md).

## Git-гілки

- [lab-01](https://github.com/Megprost/Universitat/tree/lab-01) — зафіксована лабораторна №1.
- [lab-02](https://github.com/Megprost/Universitat/tree/lab-02) — реалізація лабораторної №2.
- `main` — базова гілка для подальшого розвитку.
