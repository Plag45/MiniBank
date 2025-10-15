using MiniBank.Models;
using System.Collections.Generic;
using System.Linq;

namespace MiniBank.Services;

public class AccountRegistry
{
    private readonly List<BankAccount> _accounts = new();
    private int _nextId = 1;

    public IReadOnlyList<BankAccount> All => _accounts;

    public T Add<T>(T account) where T : BankAccount
    {
        account.Id = _nextId++;
        _accounts.Add(account);
        return account;
    }

    public BankAccount? Find(int id) => _accounts.FirstOrDefault(a => a.Id == id);
}
