using MiniBank.Models;

// Savings: fără overdraft + dobândă lunară
var s = new SavingsAccount("Bob", 500m, monthlyInterestRate: 0.01m);

s.Withdraw(600m, out var e1);
Console.WriteLine(e1 ?? $"OK, bal={s.Balance:C}");  

s.Withdraw(100m, out var e2);
Console.WriteLine(e2 ?? $"OK, bal={s.Balance:C}");  

s.ApplyMonthlyInterest();                           
Console.WriteLine($"După dobândă: {s.Balance:C}");  

s.PrintStatement();
