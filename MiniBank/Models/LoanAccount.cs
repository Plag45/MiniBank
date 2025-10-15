using MiniBank.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Models;

public class LoanAccount : BankAccount, IInterest
{
    public decimal MonthlyInterestRate { get; }

  
    public LoanAccount(string owner, decimal initialLoanAmount, decimal monthlyInterestRate = 0.02m)
        : base(owner, -Math.Abs(initialLoanAmount))
    {
        MonthlyInterestRate = monthlyInterestRate;
    }


    public override bool Withdraw(decimal amount, out string? error)
    {
        error = null;
        if (amount <= 0) { error = "Amount must be > 0"; return false; }
        Balance -= amount;
        Log($"BORROW {amount:C}");
        return true;
    }

    
    public override void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        Balance += amount;
        Log($"REPAY {amount:C}");
    }

   
    public void ApplyMonthlyInterest()
    {
        if (Balance >= 0) return; 
        var interest = Math.Round(Math.Abs(Balance) * MonthlyInterestRate, 2, MidpointRounding.ToZero);
        if (interest <= 0) return;

        Balance -= interest; 
        Log($"INTEREST -{interest:C}");
    }
}