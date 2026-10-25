using System.Collections.ObjectModel;
using ExpertTracker.Models;

namespace ExpertTracker.Services;

// Спільне сховище в оперативній пам'яті для двох екранів (SQLite буде у ЛР №3).
public sealed class InMemoryExpenseStore
{
    public ObservableCollection<Expense> Expenses { get; } = new();

    public Expense Add(string description, string category, decimal amount)
    {
        int nextId = Expenses.Count == 0 ? 1 : Expenses.Max(x => x.Id) + 1;
        var item = new Expense
        {
            Id = nextId,
            Description = description,
            Category = category,
            Amount = amount,
            Date = DateTime.Today
        };

        Expenses.Add(item);
        return item;
    }

    public Expense? GetById(int id) => Expenses.FirstOrDefault(x => x.Id == id);
}
