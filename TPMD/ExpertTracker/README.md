# ExpertTracker — лабораторна №3 (варіант 2)
Трекер бізнес-витрат ФОП на .NET MAUI 10.

## Уже реалізовано
- №1: MVVM, CollectionView, команда додавання витрат.
- №2: Shell-навігація до деталей за ID, ILifecycleAware, Preferences чернетки.
- №3: sqlite-net-pcl 1.11.285, code-first модель Expense, IExpenseRepository, DatabaseService з async CRUD (GetAll, GetById, Insert, Update, Delete). ViewModel працюють із репозиторієм; редагування і видалення на екрані деталей. База: FileSystem.AppDataDirectory/expenses.db3.

## Перевірка
Build → Rebuild Solution; створити запис, відредагувати, закрити і знову відкрити програму (дані мають залишитися), потім видалити запис і перезапустити. **Нову збірку та поведінку SQLite ще не перевірено на пристрої.**

[Інструкція ЛР №3](Reports/Lab03/README.md)

Відхилення від методички: використано .NET 10 замість .NET 8, погодити з викладачем.
