using MiniBank.Models;


var loan = new LoanAccount("Carol", initialLoanAmount: 1000m, monthlyInterestRate: 0.02m);

loan.Deposit(200m);                      
Console.WriteLine($"După repay: {loan.Balance:C}");

loan.Withdraw(500m, out var e1);         
Console.WriteLine(e1 ?? $"După borrow: {loan.Balance:C}");

loan.ApplyMonthlyInterest();             
Console.WriteLine($"După dobândă: {loan.Balance:C}");

loan.PrintStatement();
