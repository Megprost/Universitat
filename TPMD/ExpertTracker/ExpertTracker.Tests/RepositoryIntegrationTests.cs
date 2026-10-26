using ExpertTracker.Models;
using ExpertTracker.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ExpertTracker.Tests;

public sealed class RepositoryIntegrationTests
{
    private static string NewDatabasePath() =>
        Path.Combine(Path.GetTempPath(), "experttracker-test-" + Guid.NewGuid().ToString("N") + ".db3");

    [Fact]
    public async Task Sqlite_CompleteCrud_ShouldInsertReadUpdateDelete()
    {
        string path = NewDatabasePath();
        var repository = new DatabaseService(path, NullLogger<DatabaseService>.Instance);
        try
        {
            // Arrange
            var expense = new Expense
            {
                Description = "Пальне", Category = "Транспорт", Amount = 75.50m
            };

            // Act + Assert: Insert
            Assert.Equal(1, await repository.InsertAsync(expense));
            Assert.True(expense.Id > 0);

            // Read
            var all = await repository.GetAllAsync();
            Assert.Single(all);
            var saved = await repository.GetByIdAsync(expense.Id);
            Assert.NotNull(saved);
            Assert.Equal(75.50m, saved.Amount);

            // Update
            saved.Amount = 90.25m;
            Assert.Equal(1, await repository.UpdateAsync(saved));
            Assert.Equal(90.25m, (await repository.GetByIdAsync(saved.Id))!.Amount);

            // Delete
            Assert.Equal(1, await repository.DeleteAsync(saved));
            Assert.Empty(await repository.GetAllAsync());
        }
        finally
        {
            await repository.CloseAsync();
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public async Task Sqlite_ReopeningDatabase_ShouldRetainSavedExpense()
    {
        string path = NewDatabasePath();
        var first = new DatabaseService(path, NullLogger<DatabaseService>.Instance);
        try
        {
            var expense = new Expense
            {
                Description = "Оренда", Category = "Офіс", Amount = 1500m
            };
            await first.InsertAsync(expense);
            await first.CloseAsync();

            var reopened = new DatabaseService(path, NullLogger<DatabaseService>.Instance);
            try
            {
                var loaded = await reopened.GetAllAsync();
                Assert.Single(loaded);
                Assert.Equal("Оренда", loaded[0].Description);
                Assert.Equal(1500m, loaded[0].Amount);
            }
            finally
            {
                await reopened.CloseAsync();
            }
        }
        finally
        {
            // SQLiteAsyncConnection.CloseAsync is idempotent for this test connection pool.
            await first.CloseAsync();
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
