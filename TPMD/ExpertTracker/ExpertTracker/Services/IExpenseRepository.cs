using ExpertTracker.Models;
namespace ExpertTracker.Services;
public interface IExpenseRepository {
 Task<List<Expense>> GetAllAsync();
 Task<Expense?> GetByIdAsync(int id);
 Task<int> InsertAsync(Expense item);
 Task<int> UpdateAsync(Expense item);
 Task<int> DeleteAsync(Expense item);
}
