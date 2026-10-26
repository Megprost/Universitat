using ExpertTracker.Models;
using Microsoft.Extensions.Logging;
using SQLite;

namespace ExpertTracker.Services;

public sealed class DatabaseService : IExpenseRepository
{
    private readonly SQLiteAsyncConnection _database;
    private readonly ILogger<DatabaseService> _logger;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _initialized;

    public DatabaseService(string path, ILogger<DatabaseService> logger)
    {
        _database = new SQLiteAsyncConnection(path);
        _logger = logger;
    }

    private async Task EnsureInitializedAsync()
    {
        if (_initialized) return;

        await _initLock.WaitAsync();
        try
        {
            if (_initialized) return;
            await _database.CreateTableAsync<Expense>();
            _initialized = true;
            _logger.LogInformation("Expenses table initialized");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SQLite initialization failed");
            throw;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task<List<Expense>> GetAllAsync()
    {
        try
        {
            await EnsureInitializedAsync();
            var items = await _database.Table<Expense>()
                .OrderByDescending(x => x.Id).ToListAsync();
            _logger.LogInformation("Loaded {Count} expense records", items.Count);
            return items;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not load expense records");
            throw;
        }
    }

    public async Task<Expense?> GetByIdAsync(int id)
    {
        try
        {
            await EnsureInitializedAsync();
            var item = await _database.Table<Expense>()
                .Where(x => x.Id == id).FirstOrDefaultAsync();
            if (item is null)
                _logger.LogWarning("Expense with ID {ExpenseId} not found", id);
            return item;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not read expense ID {ExpenseId}", id);
            throw;
        }
    }

    public async Task<int> InsertAsync(Expense item)
    {
        try
        {
            await EnsureInitializedAsync();
            int count = await _database.InsertAsync(item);
            _logger.LogInformation("Inserted expense ID {ExpenseId}", item.Id);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not insert expense");
            throw;
        }
    }

    public async Task<int> UpdateAsync(Expense item)
    {
        try
        {
            await EnsureInitializedAsync();
            int count = await _database.UpdateAsync(item);
            if (count == 0)
                _logger.LogWarning("Expense ID {ExpenseId} was not updated", item.Id);
            else
                _logger.LogInformation("Updated expense ID {ExpenseId}", item.Id);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not update expense ID {ExpenseId}", item.Id);
            throw;
        }
    }

    public async Task<int> DeleteAsync(Expense item)
    {
        try
        {
            await EnsureInitializedAsync();
            int count = await _database.DeleteAsync(item);
            if (count == 0)
                _logger.LogWarning("Expense ID {ExpenseId} was not deleted", item.Id);
            else
                _logger.LogInformation("Deleted expense ID {ExpenseId}", item.Id);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not delete expense ID {ExpenseId}", item.Id);
            throw;
        }
    }
}
