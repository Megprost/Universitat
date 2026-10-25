using ExpertTracker.Models;
using ExpertTracker.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ExpertTracker.Tests;
public class RepositoryIntegrationTests
{
 [Fact]
 public async Task Sqlite_CompleteCrud_ShouldPersistAndRemove()
 {
  string path=Path.Combine(Path.GetTempPath(),"experttracker-test-"+Guid.NewGuid().ToString("N")+".db3");
  var repo=new DatabaseService(path,NullLogger<DatabaseService>.Instance);
  try
  {
   // Arrange
   var item=new Expense{Description="Пальне",Category="Транспорт",Amount=75.50m};
   // Act / Assert: Create
   Assert.Equal(1,await repo.InsertAsync(item));
   Assert.True(item.Id>0);
   // Read
   var saved=await repo.GetByIdAsync(item.Id);
   Assert.NotNull(saved);
   Assert.Equal(75.50m,saved.Amount);
   // Update
   saved.Amount=90.25m;
   Assert.Equal(1,await repo.UpdateAsync(saved));
   Assert.Equal(90.25m,(await repo.GetByIdAsync(saved.Id))!.Amount);
   // Delete
   Assert.Equal(1,await repo.DeleteAsync(saved));
   Assert.Empty(await repo.GetAllAsync());
  }
  finally
  {
   await repo.CloseAsync();
   if(File.Exists(path))File.Delete(path);
  }
 }
}
