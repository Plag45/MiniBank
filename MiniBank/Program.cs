using MiniBank.Interfaces;
using MiniBank.Models;
using MiniBank.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var registry = new AccountRegistry();

while (true)
{
    Console.WriteLine("\n=== MINIBANK ===");
    Console.WriteLine("1. List accounts");
    Console.WriteLine("2. Create account");
    Console.WriteLine("3. Deposit");
    Console.WriteLine("4. Withdraw");
    Console.WriteLine("5. View statement");
    Console.WriteLine("6. Run month-end");
    Console.WriteLine("7. Exit");
    Console.Write("Select: ");

    var input = Console.ReadLine()?.Trim();
    if (input == "7") break;


    int ReadAccountId()
    {
        Console.Write("Account id: ");
        var raw = Console.ReadLine();
        if (!int.TryParse(raw, out var id) || id <= 0)
            throw new ArgumentException("Account id must be a positive integer.");
        return id;
    }

    decimal ReadPositiveAmount(string prompt)
    {
        Console.Write(prompt);
        var raw = Console.ReadLine();
        if (!decimal.TryParse(raw, out var amount) || amount <= 0)
            throw new ArgumentException("Amount must be a positive number.");
        return decimal.Round(amount, 2);
    }



    switch (input)
    {
        case "1":
            {
                if (registry.All.Count == 0)
                {
                    Console.WriteLine("No accounts yet.");
                    break;
                }

                foreach (var acc in registry.All)
                    Console.WriteLine($"#{acc.Id} {acc.Owner} | {acc.GetType().Name} | BAL {acc.Balance:C}");
                break;
            }

        case "2":
            {
                // === CREATE ACCOUNT 

                Console.Write("Owner: ");
                var owner = (Console.ReadLine() ?? "").Trim();
                if (string.IsNullOrWhiteSpace(owner))
                {
                    Console.WriteLine("Owner is required.");
                    break;
                }

                Console.WriteLine("Select account type:");
                Console.WriteLine("  1) Checking");
                Console.WriteLine("  2) Savings");
                Console.WriteLine("  3) Loan");
                Console.Write("Choice [1-3]: ");
                var choice = (Console.ReadLine() ?? "").Trim();


                try
                {
                    switch (choice)
                    {
                        case "1": // Checking
                            {
                                
                                if (registry.Exists<CheckingAccount>(owner))
                                {
                                    Console.WriteLine("Error: This owner already has a Checking account.");
                                    break;
                                }

                                var opening = ReadPositiveAmount("Opening deposit: ");
                                Console.Write("Overdraft limit (default 200): ");
                                var odRaw = Console.ReadLine();
                                decimal overdraft = 200m;
                                if (!string.IsNullOrWhiteSpace(odRaw) && decimal.TryParse(odRaw, out var parsed))
                                    overdraft = Math.Abs(parsed);

                                var acc = new CheckingAccount(owner, opening, overdraft);
                                registry.Add(acc);
                                Console.WriteLine($"Created #{acc.Id} Checking for {owner} with BAL {acc.Balance:C} (OD {acc.OverdraftLimit:C}).");
                                break;
                            }

                        case "2": // Savings
                            {
                                if (registry.Exists<SavingsAccount>(owner))
                                {
                                    Console.WriteLine("Error: This owner already has a Savings account.");
                                    break;
                                }

                                var opening = ReadPositiveAmount("Opening deposit: ");
                                Console.Write("Monthly interest (default 0.01 = 1%): ");
                                var rateRaw = Console.ReadLine();
                                decimal rate = 0.01m;
                                if (!string.IsNullOrWhiteSpace(rateRaw) && decimal.TryParse(rateRaw, out var parsed))
                                    rate = parsed;

                                var acc = new SavingsAccount(owner, opening, rate);
                                registry.Add(acc);
                                Console.WriteLine($"Created #{acc.Id} Savings for {owner} with BAL {acc.Balance:C}.");
                                break;
                            }

                        case "3": // Loan
                            {
                                if (registry.Exists<LoanAccount>(owner))
                                {
                                    Console.WriteLine("Error: This owner already has a Loan account.");
                                    break;
                                }

                                var principal = ReadPositiveAmount("Initial loan amount: ");
                                Console.Write("Monthly interest (default 0.02 = 2%): ");
                                var rateRaw = Console.ReadLine();
                                decimal rate = 0.02m;
                                if (!string.IsNullOrWhiteSpace(rateRaw) && decimal.TryParse(rateRaw, out var parsed))
                                    rate = parsed;

                                var acc = new LoanAccount(owner, principal, rate);
                                registry.Add(acc);
                                Console.WriteLine($"Created #{acc.Id} Loan for {owner} with BAL {acc.Balance:C}.");
                                break;
                            }

                        default:
                            Console.WriteLine("Unknown choice. Please select 1, 2, or 3.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                break;
            }


        case "3":
            {
                // === DEPOSIT ===
                try
                {
                    var id = ReadAccountId();

                    var acc = registry.Find(id);
                    if (acc is null)
                    {
                        Console.WriteLine("Account not found.");
                        break;
                    }

                    var amount = ReadPositiveAmount("Amount to deposit: ");
                    acc.Deposit(amount);

                    Console.WriteLine($"Deposited {amount:C} into #{acc.Id} ({acc.GetType().Name}). New balance: {acc.Balance:C}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                break;
            }

        case "4":
            {
                // === WITHDRAW ===
                try
                {
                    var id = ReadAccountId();

                    var acc = registry.Find(id);
                    if (acc is null)
                    {
                        Console.WriteLine("Account not found.");
                        break;
                    }

                    var amount = ReadPositiveAmount("Amount to withdraw: ");

                    if (acc.Withdraw(amount, out var error))
                    {
                        Console.WriteLine($"Withdrew {amount:C} from #{acc.Id} ({acc.GetType().Name}). New balance: {acc.Balance:C}");
                    }
                    else
                    {
                        Console.WriteLine($"Withdraw failed: {error}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                break;
            }

        case "5":
            {
                // === VIEW STATEMENT ===
                try
                {
                    var id = ReadAccountId();

                    var acc = registry.Find(id);
                    if (acc is null)
                    {
                        Console.WriteLine("Account not found.");
                        break;
                    }

                    acc.PrintStatement();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                break;
            }

        case "6":
            {
               
                int count = 0;
                foreach (var acc in registry.All)
                {
                    if (acc is IInterest ib)
                    {
                        ib.ApplyMonthlyInterest();
                        count++;
                    }
                }
                Console.WriteLine($"Month-end applied to {count} account(s).");
                break;
            }

        default:
            {
                Console.WriteLine("Invalid option. Try again.");
                break;
            }
    }
}

Console.WriteLine("Goodbye!");

