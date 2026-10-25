using SQLite;
using ExpertTracker.Models;
namespace ExpertTracker.Services;
public sealed class DatabaseService : IExpenseRepository {
 private readonly SQLiteAsyncConnection db;
 private readonly SemaphoreSlim gate = new(1,1);
 private bool ready;
 public DatabaseService(string path) { db = new SQLiteAsyncConnection(path); }
 private async Task Init() {
  if(ready)return;
  await gate.WaitAsync();
  try { if(!ready){ await db.CreateTableAsync<Expense>(); ready=true; } }
  finally { gate.Release(); }
 }
 public async Task<List<Expense>> GetAllAsync() {
  await Init(); return await db.Table<Expense>().OrderByDescending(x=>x.Id).ToListAsync();
 }
 public async Task<Expense?> GetByIdAsync(int id) {
  await Init(); return await db.Table<Expense>().Where(x=>x.Id==id).FirstOrDefaultAsync();
 }
 public async Task<int> InsertAsync(Expense x) { await Init(); return await db.InsertAsync(x); }
 public async Task<int> UpdateAsync(Expense x) { await Init(); return await db.UpdateAsync(x); }
 public async Task<int> DeleteAsync(Expense x) { await Init(); return await db.DeleteAsync(x); }
}
