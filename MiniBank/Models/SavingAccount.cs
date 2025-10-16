using MiniBank.Interfaces;

namespace MiniBank.Models;

public class SavingsAccount : BankAccount, IInterest
{
    public decimal MonthlyInterestRate { get; }

    public SavingsAccount(string owner, decimal openingAmount, decimal monthlyInterestRate = 0.01m)
        : base(owner, openingAmount)
    {
        MonthlyInterestRate = monthlyInterestRate;
    }

   
    public override bool Withdraw(decimal amount, out string? error)
    {
        error = null;
        if (amount <= 0) { error = "Amount must be > 0"; return false; }
        if (amount > Balance) { error = "Cannot overdraft a savings account"; return false; }

        Balance -= amount;
        Log($"WITHDRAW {amount:C}");
        return true;
    }

   
    public void ApplyMonthlyInterest()
    {
        if (Balance <= 0) return;
        var interest = Math.Round(Balance * MonthlyInterestRate, 2, MidpointRounding.ToZero);
        if (interest <= 0) return;

        Balance += interest;
        Log($"INTEREST +{interest:C}");
    }
}
