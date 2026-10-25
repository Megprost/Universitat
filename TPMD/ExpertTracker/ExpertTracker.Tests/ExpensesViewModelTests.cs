using ExpertTracker.Models;
using ExpertTracker.Services;
using ExpertTracker.ViewModels;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace ExpertTracker.Tests;
public class ExpensesViewModelTests
{
 private static ExpensesViewModel Create(Mock<IExpenseRepository> repo) =>
  new(repo.Object,NullLogger<ExpensesViewModel>.Instance,
   new Mock<IDraftStore>().Object);

 [Fact]
 public async Task Load_ShouldShowRows()
 {
  // Arrange
  var repo=new Mock<IExpenseRepository>();
  repo.Setup(x=>x.GetAllAsync()).ReturnsAsync(new List<Expense>{
   new(){Id=1,Description="Оренда",Category="Офіс",Amount=500m}});
  var vm=Create(repo);
  // Act
  await vm.LoadExpensesCommand.ExecuteAsync(null);
  // Assert
  Assert.Single(vm.Expenses);Assert.Equal(500m,vm.TotalAmount);
 }

 [Fact]
 public async Task Add_ShouldCallRepositoryAndReload()
 {
  var repo=new Mock<IExpenseRepository>();
  repo.Setup(x=>x.InsertAsync(It.IsAny<Expense>())).ReturnsAsync(1);
  repo.Setup(x=>x.GetAllAsync()).ReturnsAsync(new List<Expense>());
  var vm=Create(repo);
  vm.Description="Оренда";vm.Category="Офіс";vm.AmountText="250.00";
  await vm.AddExpenseCommand.ExecuteAsync(null);
  repo.Verify(x=>x.InsertAsync(It.Is<Expense>(e=>e.Amount==250m&&e.Description=="Оренда")),Times.Once);
  Assert.Equal("",vm.ErrorMessage);
 }

 [Fact]
 public async Task Load_OnRepositoryError_ShouldShowMessage()
 {
  var repo=new Mock<IExpenseRepository>();
  repo.Setup(x=>x.GetAllAsync()).ThrowsAsync(new InvalidOperationException("simulated"));
  var vm=Create(repo);
  await vm.LoadExpensesCommand.ExecuteAsync(null);
  Assert.Contains("Не вдалося",vm.ErrorMessage);
 }

 [Theory]
 [InlineData("")]
 [InlineData("-2")]
 [InlineData("text")]
 [InlineData("0")]
 [InlineData("1.234")]
 public async Task InvalidAmount_ShouldNotInsert(string badAmount)
 {
  var repo=new Mock<IExpenseRepository>();
  var vm=Create(repo);
  vm.Description="Тест";vm.Category="Тест";vm.AmountText=badAmount;
  await vm.AddExpenseCommand.ExecuteAsync(null);
  repo.Verify(x=>x.InsertAsync(It.IsAny<Expense>()),Times.Never);
  Assert.NotEmpty(vm.ErrorMessage);
 }
}
