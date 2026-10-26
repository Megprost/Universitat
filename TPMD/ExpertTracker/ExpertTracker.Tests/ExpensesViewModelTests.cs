using ExpertTracker.Models;
using ExpertTracker.Services;
using ExpertTracker.ViewModels;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace ExpertTracker.Tests;

public sealed class ExpensesViewModelTests
{
    private static ExpensesViewModel Create(Mock<IExpenseRepository> repository)
    {
        var draft = new Mock<IDraftStore>();
        draft.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>()))
             .Returns((string key, string fallback) => fallback);
        return new ExpensesViewModel(
            repository.Object,
            NullLogger<ExpensesViewModel>.Instance,
            draft.Object);
    }

    [Fact]
    public async Task Load_ShouldDisplayExpensesAndCorrectTotal()
    {
        // Arrange
        var repository = new Mock<IExpenseRepository>();
        repository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Expense>
        {
            new() { Id = 1, Description = "Оренда", Category = "Офіс", Amount = 500m },
            new() { Id = 2, Description = "Пальне", Category = "Транспорт", Amount = 75.50m }
        });
        var vm = Create(repository);

        // Act
        await vm.LoadExpensesCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal(2, vm.Expenses.Count);
        Assert.Equal(575.50m, vm.TotalAmount);
        Assert.Empty(vm.ErrorMessage);
    }

    [Fact]
    public async Task Add_ShouldInsertAndRefreshCollection()
    {
        // Arrange
        var repository = new Mock<IExpenseRepository>();
        var items = new List<Expense>();
        repository.Setup(x => x.InsertAsync(It.IsAny<Expense>()))
                  .Callback<Expense>(item => items.Add(item))
                  .ReturnsAsync(1);
        repository.Setup(x => x.GetAllAsync())
                  .ReturnsAsync(() => items.ToList());
        var vm = Create(repository);
        vm.Description = "Оренда";
        vm.Category = "Офіс";
        vm.AmountText = "250,25";

        // Act
        await vm.AddExpenseCommand.ExecuteAsync(null);

        // Assert
        repository.Verify(x => x.InsertAsync(It.Is<Expense>(
            e => e.Description == "Оренда" && e.Amount == 250.25m)), Times.Once);
        Assert.Single(vm.Expenses);
        Assert.Equal(250.25m, vm.TotalAmount);
        Assert.Empty(vm.ErrorMessage);
        Assert.Equal("", vm.Description);
        Assert.Equal("", vm.AmountText);
    }

    [Fact]
    public async Task Load_RepositoryException_ShouldShowFriendlyMessage()
    {
        var repository = new Mock<IExpenseRepository>();
        repository.Setup(x => x.GetAllAsync())
                  .ThrowsAsync(new InvalidOperationException("simulated storage failure"));
        var vm = Create(repository);

        await vm.LoadExpensesCommand.ExecuteAsync(null);

        Assert.Contains("Не вдалося", vm.ErrorMessage);
        Assert.Empty(vm.Expenses);
    }

    [Fact]
    public async Task Add_InsertException_ShouldShowFriendlyMessage()
    {
        var repository = new Mock<IExpenseRepository>();
        repository.Setup(x => x.InsertAsync(It.IsAny<Expense>()))
                  .ThrowsAsync(new InvalidOperationException("simulated storage failure"));
        var vm = Create(repository);
        vm.Description = "Оренда";
        vm.Category = "Офіс";
        vm.AmountText = "50";

        await vm.AddExpenseCommand.ExecuteAsync(null);

        Assert.Contains("Не вдалося", vm.ErrorMessage);
        Assert.Equal("Оренда", vm.Description);
    }

    [Fact]
    public async Task Add_ZeroAffectedRows_ShouldNotClearForm()
    {
        var repository = new Mock<IExpenseRepository>();
        repository.Setup(x => x.InsertAsync(It.IsAny<Expense>())).ReturnsAsync(0);
        var vm = Create(repository);
        vm.Description = "Оренда";
        vm.Category = "Офіс";
        vm.AmountText = "50";

        await vm.AddExpenseCommand.ExecuteAsync(null);

        Assert.NotEmpty(vm.ErrorMessage);
        Assert.Equal("Оренда", vm.Description);
        repository.Verify(x => x.GetAllAsync(), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("-2")]
    [InlineData("text")]
    [InlineData("0")]
    [InlineData("1.234")]
    [InlineData("1000000001")]
    public async Task Add_InvalidAmount_ShouldNotWrite(string amount)
    {
        var repository = new Mock<IExpenseRepository>();
        var vm = Create(repository);
        vm.Description = "Тест";
        vm.Category = "Офіс";
        vm.AmountText = amount;

        await vm.AddExpenseCommand.ExecuteAsync(null);

        repository.Verify(x => x.InsertAsync(It.IsAny<Expense>()), Times.Never);
        Assert.NotEmpty(vm.ErrorMessage);
    }

    [Theory]
    [InlineData("", "Офіс")]
    [InlineData("Оренда", "")]
    public async Task Add_MissingRequiredField_ShouldNotWrite(string description, string category)
    {
        var repository = new Mock<IExpenseRepository>();
        var vm = Create(repository);
        vm.Description = description;
        vm.Category = category;
        vm.AmountText = "100";

        await vm.AddExpenseCommand.ExecuteAsync(null);

        repository.Verify(x => x.InsertAsync(It.IsAny<Expense>()), Times.Never);
        Assert.NotEmpty(vm.ErrorMessage);
    }
}
