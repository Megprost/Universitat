# ExpertTracker — лабораторна робота №1

**Дисципліна:** «Технології розробки мобільних додатків на C#»  
**Варіант 2:** Трекер бізнес-витрат для фізичної особи-підприємця.

## Стан проєкту

Реалізовано навчальний застосунок .NET MAUI за патерном MVVM:

- модель `Expense` (Id, Description, Category, Amount, Date);
- `ExpensesViewModel : ObservableObject` з `ObservableCollection<Expense>`, властивостями введення, перевіркою даних і `AddExpenseCommand`;
- сторінку `Views/ExpensesPage.xaml` з формою, `CollectionView` та загальною сумою;
- зв'язування даних через `BindingContext`, реєстрацію View/ViewModel через DI у `MauiProgram.cs`;
- пакет `CommunityToolkit.Mvvm` **8.4.2**.

Витрати зберігаються **лише в оперативній пам'яті**; SQLite планується на лабораторну №3.

## Середовище

- Visual Studio Insiders, C#;
- .NET **10**, оскільки поточне середовище користувача має SDK 10.0.401;
- цільові платформи .NET MAUI: Android, iOS, Mac Catalyst, Windows.

> **Відхилення від методички:** лабораторна №1 вказує .NET 8 SDK. Поточна збірка виконана на .NET 10. Питання відповідності версії необхідно узгодити з викладачем.

## Перевірені результати

- `Build: 1 succeeded, 0 failed`;
- запуск на **Windows Machine**;
- початковий порожній список і підсумок `0,00 грн`;
- повідомлення «Введіть назву та категорію.» при порожніх обов'язкових полях;
- після додавання записів на `1 500,00 грн` та `18 500,00 грн` відображено підсумок `20 000,00 грн`.

**Android-емулятор/фізичний пристрій на момент оформлення не підтверджено.** Скріншоти фактичного тестування Windows наведено в окремому звіті.

## Ключові файли

- `ExpertTracker/Models/Expense.cs`
- `ExpertTracker/ViewModels/ExpensesViewModel.cs`
- `ExpertTracker/Views/ExpensesPage.xaml`
- `ExpertTracker/Views/ExpensesPage.xaml.cs`
- `ExpertTracker/MauiProgram.cs`
- `ExpertTracker/AppShell.xaml`

## Перевірка

Відкрити `ExpertTracker.sln` у Visual Studio з .NET MAUI workload, вибрати `Windows Machine`, зібрати і запустити програму. Для перевірки `CollectionView` додати дві витрати та перевірити загальну суму.

## Звіт і журнал промптів

Документація: `Reports/Lab01/README.md` і `Reports/Lab01/LLM-journal.md`.

**Гілка для лабораторної №1:** [lab-01](https://github.com/Megprost/Universitat/tree/lab-01).  
Основна гілка `main` використовується для подальшого розвитку застосунку.
