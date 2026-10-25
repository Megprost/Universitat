using SQLite;
using Microsoft.Extensions.Logging;
using ExpertTracker.Models;
namespace ExpertTracker.Services;
public sealed class DatabaseService : IExpenseRepository {
 private readonly SQLiteAsyncConnection db;
 private readonly ILogger<DatabaseService> logger;
 private readonly SemaphoreSlim gate = new(1,1);
 private bool ready;
 public Task CloseAsync() => db.CloseAsync();
 public DatabaseService(string path, ILogger<DatabaseService> log) { db = new SQLiteAsyncConnection(path);logger=log; }
 private async Task Init() {
  if(ready)return;
  await gate.WaitAsync();
  try { if(!ready){ await db.CreateTableAsync<Expense>(); ready=true;logger.LogInformation("SQLite Expenses table initialized"); } }
  finally { gate.Release(); }
 }
 public async Task<List<Expense>> GetAllAsync() {
  await Init(); try {var rows=await db.Table<Expense>().OrderByDescending(x=>x.Id).ToListAsync();logger.LogInformation("Loaded {Count} expenses",rows.Count);return rows;} catch(Exception ex){logger.LogError(ex,"Failed loading expenses");throw;}
 }
 public async Task<Expense?> GetByIdAsync(int id) {
  await Init(); try {return await db.Table<Expense>().Where(x=>x.Id==id).FirstOrDefaultAsync();}catch(Exception ex){logger.LogError(ex,"Failed reading expense {Id}",id);throw;}
 }
 public async Task<int> InsertAsync(Expense x) { await Init(); try {int n=await db.InsertAsync(x);logger.LogInformation("Inserted expense {Id}",x.Id);return n;}catch(Exception ex){logger.LogError(ex,"Insert failed");throw;} }
 public async Task<int> UpdateAsync(Expense x) { await Init(); try {int n=await db.UpdateAsync(x);logger.LogInformation("Updated expense {Id}",x.Id);return n;}catch(Exception ex){logger.LogError(ex,"Update failed for expense {Id}",x.Id);throw;} }
 public async Task<int> DeleteAsync(Expense x) { await Init(); try {int n=await db.DeleteAsync(x);logger.LogInformation("Deleted expense {Id}",x.Id);return n;}catch(Exception ex){logger.LogError(ex,"Delete failed for expense {Id}",x.Id);throw;} }
}
