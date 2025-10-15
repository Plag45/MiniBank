using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Interfaces;

public  interface ITransactable
{
    void Deposit(decimal amount);
    bool Withdraw(decimal amount, out string? error);
}

