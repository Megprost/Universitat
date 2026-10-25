# Лабораторна робота №2 — інструкція з перевірки

**Предметна область:** трекер бізнес-витрат ФОП, варіант 2.  
**Тема:** Життєвий цикл, навігація та збереження стану в .NET MAUI.

## Що реалізовано в коді

- Другий екран `ExpenseDetailsPage` і ViewModel `ExpenseDetailsViewModel`.
- Маршрут `ExpenseDetailsPage` в `AppShell.xaml.cs`.
- Вибір витрати з `CollectionView` та `GoToAsync` з параметром `id`.
- Отримання `id` через `IQueryAttributable`.
- Контракт `ILifecycleAware` на обох ViewModel. У code-behind сторінок лише делегування подій.
- Діагностика `App.CreateWindow`, `Window.Created`, `Activated`, `Deactivated`, `Stopped`, `Resumed`, `Destroying`.
- Збереження та відновлення полів чернетки (назва, категорія, сума) через `Preferences.Default`.
- Спільне in-memory сховище `InMemoryExpenseStore` для двох екранів (SQLite буде в ЛР №3).

## Що перевірити на комп'ютері

1. `Build → Rebuild Solution`: зафіксувати результат (не перевірено через GitHub).
2. Запустити Android-емулятор або фізичний пристрій (Windows використати для попередньої діагностики).
3. Додати дві витрати. Натиснути на одну, перевірити дані на екрані деталей та повернення назад.
4. Ввести **незавершену** назву й категорію, згорнути програму, відновити; перевірити чернетку.
5. Закрити застосунок повністю та запустити знову: чернетка має залишитися, а сам список (до ЛР №3) може бути порожнім.
6. У Visual Studio `Output → Debug` знайти `[Lab02]`, записати **фактичну** послідовність подій.
7. Зробити скриншоти: список → деталі, чернетка до → після відновлення.

**Особливість версій:** методичка використовує назви `OnStart`, `OnSleep`, `OnResume`; тут використані актуальні Window lifecycle events для .NET MAUI 10. Методичка вказує .NET 8, а поточний проєкт використовує .NET 10. Різницю треба пояснити у звіті й за потреби узгодити з викладачем.

## Джерела

- https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/app-lifecycle?view=net-maui-10.0
- https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/shell/navigation?view=net-maui-10.0
- https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/storage/preferences?view=net-maui-10.0

**Статус:** код реалізовано; збірка/запуск і результати перевірки на пристрої поки не підтверджені.
