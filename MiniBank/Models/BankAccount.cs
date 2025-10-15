using MiniBank.Interfaces;
using System;
using System.Collections.Generic;

namespace MiniBank.Models;

public abstract class BankAccount : ITransactable, IStatement
{
    private readonly List<string> _ops = new();

    public int Id { get; internal set; }
    public string Owner { get; init; }
    public decimal Balance { get; protected set; }

    protected BankAccount(string owner, decimal openingAmount)
    {
        if (string.IsNullOrWhiteSpace(owner)) throw new ArgumentException("Owner required", nameof(owner));
        Owner = owner.Trim();
        Balance = openingAmount;
        Log($"OPEN {openingAmount:C}");
    }

    public virtual void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be > 0");
        Balance += amount;
        Log($"DEPOSIT {amount:C}");
    }

    public abstract bool Withdraw(decimal amount, out string? error);

    protected void Log(string message) =>
        _ops.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm}] {message}; BAL={Balance:C}");

    public void PrintStatement()
    {
        Console.WriteLine($"\n#{Id} {GetType().Name} for {Owner} | BAL {Balance:C}");
        foreach (var line in _ops)
            Console.WriteLine("  • " + line);
    }
}
