# ExpertTracker — Лабораторна робота №1

**Дисципліна:** Технології розробки мобільних додатків на C#  
**Варіант 2:** Трекер бізнес-витрат ФОП  
**Технології:** C#, .NET MAUI 10, MVVM

## Реалізація: MVVM

- Модель `Expense`: назва, категорія, сума, дата та ID.
- `ExpensesViewModel`: перевірка введення, команда додавання, `ObservableCollection`, загальна сума.
- `ExpensesPage`: форма введення та `CollectionView`.
- `MauiProgram.cs`: Dependency Injection для сторінки та ViewModel.

## Запуск і перевірка

Відкрити `ExpertTracker.slnx` у Visual Studio, обрати Windows Machine, зібрати проєкт і запустити.

## Код

Основний проєкт: `ExpertTracker/ExpertTracker.csproj`.  
Гілка роботи: [`lab-01`](https://github.com/Megprost/Universitat/tree/lab-01).
