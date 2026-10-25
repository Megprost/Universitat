using SQLite;
namespace ExpertTracker.Models;
[Table("Expenses")]
public class Expense
{
 [PrimaryKey,AutoIncrement] public int Id { get; set; }
 [MaxLength(200)] public string Description { get; set; } = "";
 [MaxLength(100)] public string Category { get; set; } = "";
 public long AmountCents { get; set; }
 [Ignore] public decimal Amount { get => AmountCents/100m; set => AmountCents=decimal.ToInt64(decimal.Round(value*100m,0)); }
 public DateTime Date { get; set; } = DateTime.Today;
 public string? ImagePath {get;set;}
 public double? Latitude {get;set;}
 public double? Longitude {get;set;}
}
