using MiniBank.Interfaces;

namespace MiniBank.Models;

public class CheckingAccount : BankAccount, IOverdraft
{
   
    public decimal OverdraftLimit { get; }

    public CheckingAccount(string owner, decimal openingAmount, decimal overdraftLimit = 200m)
        : base(owner, openingAmount)
    {
        OverdraftLimit = -Math.Abs(overdraftLimit);
    }

   
    public override bool Withdraw(decimal amount, out string? error)
    {
        error = null;
        if (amount <= 0) { error = "Amount must be > 0"; return false; }

        var newBalance = Balance - amount;
        if (newBalance < OverdraftLimit)
        {
            error = $"Insufficient funds: overdraft limit {OverdraftLimit:C}";
            return false;
        }

        Balance = newBalance;
        Log($"WITHDRAW {amount:C}");
        return true;
    }
}
