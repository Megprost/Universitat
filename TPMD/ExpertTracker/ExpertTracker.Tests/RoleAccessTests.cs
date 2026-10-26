using ExpertTracker.Models;
using ExpertTracker.Services;
using ExpertTracker.ViewModels;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace ExpertTracker.Tests;

public sealed class RoleAccessTests
{
    private static ExpenseDetailsViewModel Create(
        Mock<IExpenseRepository> repo, Mock<IAuthService> auth, Mock<IDeviceService> device) =>
        new(repo.Object, NullLogger<ExpenseDetailsViewModel>.Instance, auth.Object, device.Object);

    [Fact]
    public async Task Accountant_CannotDeleteExpense()
    {
        var repo = new Mock<IExpenseRepository>();
        var auth = new Mock<IAuthService>();
        auth.SetupGet(a => a.IsSignedIn).Returns(true);
        auth.SetupGet(a => a.IsOwner).Returns(false);
        var device = new Mock<IDeviceService>();
        var vm = Create(repo, auth, device);

        Assert.False(vm.CanDelete);
        await vm.DeleteCommand.ExecuteAsync(null);

        repo.Verify(r => r.DeleteAsync(It.IsAny<Expense>()), Times.Never);
        Assert.Contains("власник", vm.Message);
    }

    [Fact]
    public void Owner_CanSeeDeleteAction()
    {
        var repo = new Mock<IExpenseRepository>();
        var auth = new Mock<IAuthService>();
        auth.SetupGet(a => a.IsSignedIn).Returns(true);
        auth.SetupGet(a => a.IsOwner).Returns(true);
        var vm = Create(repo, auth, new Mock<IDeviceService>());
        Assert.True(vm.CanDelete);
    }

    [Fact]
    public async Task AnonymousUser_CannotSaveExpense()
    {
        var repo = new Mock<IExpenseRepository>();
        var auth = new Mock<IAuthService>();
        auth.SetupGet(a => a.IsSignedIn).Returns(false);
        var vm = Create(repo, auth, new Mock<IDeviceService>());

        await vm.SaveCommand.ExecuteAsync(null);

        repo.Verify(r => r.UpdateAsync(It.IsAny<Expense>()), Times.Never);
        Assert.Contains("увійдіть", vm.Message);
    }

    [Fact]
    public async Task CameraDenied_ShouldNotModifyExpense()
    {
        var repo = new Mock<IExpenseRepository>();
        var auth = new Mock<IAuthService>();
        auth.SetupGet(a => a.IsSignedIn).Returns(true);
        var device = new Mock<IDeviceService>();
        device.Setup(d => d.CaptureReceiptAsync()).ReturnsAsync((string?)null);
        var vm = Create(repo, auth, device);

        await vm.AttachReceiptCommand.ExecuteAsync(null);

        repo.Verify(r => r.UpdateAsync(It.IsAny<Expense>()), Times.Never);
        Assert.Contains("дозвіл", vm.Message);
    }

    [Fact]
    public async Task LocationDenied_ShouldNotModifyExpense()
    {
        var repo = new Mock<IExpenseRepository>();
        var auth = new Mock<IAuthService>();
        auth.SetupGet(a => a.IsSignedIn).Returns(true);
        var device = new Mock<IDeviceService>();
        device.Setup(d => d.GetCoordinatesAsync()).ReturnsAsync((Coordinates?)null);
        var vm = Create(repo, auth, device);

        await vm.CaptureLocationCommand.ExecuteAsync(null);

        repo.Verify(r => r.UpdateAsync(It.IsAny<Expense>()), Times.Never);
        Assert.Contains("дозвіл", vm.Message);
    }
}
