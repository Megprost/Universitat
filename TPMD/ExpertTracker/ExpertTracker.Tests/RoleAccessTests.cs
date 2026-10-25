using ExpertTracker.Services;
using ExpertTracker.ViewModels;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
namespace ExpertTracker.Tests;
public class RoleAccessTests {
 [Fact]
 public async Task Accountant_CannotDeleteExpense() {
  var repo=new Mock<IExpenseRepository>();
  var auth=new Mock<IAuthService>();
  auth.SetupGet(x=>x.IsSignedIn).Returns(true);
  auth.SetupGet(x=>x.IsOwner).Returns(false);
  var device=new Mock<IDeviceService>();
  var vm=new ExpenseDetailsViewModel(repo.Object,
   NullLogger<ExpenseDetailsViewModel>.Instance,auth.Object,device.Object);
  Assert.False(vm.CanDelete);
  await vm.DeleteCommand.ExecuteAsync(null);
  repo.Verify(x=>x.DeleteAsync(It.IsAny<ExpertTracker.Models.Expense>()),Times.Never);
 }
}
