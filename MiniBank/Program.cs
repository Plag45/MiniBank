using MiniBank.Models;
using MiniBank.Interfaces;
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
                // === CREATE ACCOUNT ===
                Console.Write("Type (Checking/Savings/Loan): ");
                var type = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

                Console.Write("Owner: ");
                var owner = (Console.ReadLine() ?? "").Trim();
                if (string.IsNullOrWhiteSpace(owner))
                {
                    Console.WriteLine("Owner is required.");
                    break;
                }

                
                decimal ReadPositiveAmount(string prompt)
                {
                    Console.Write(prompt);
                    var raw = Console.ReadLine();
                    if (!decimal.TryParse(raw, out var amount) || amount <= 0)
                        throw new ArgumentException("Amount must be a positive number.");
                    return decimal.Round(amount, 2);
                }

                try
                {
                    switch (type)
                    {
                        case "checking":
                        case "c":
                            {
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
                        case "savings":
                        case "s":
                            {
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
                        case "loan":
                        case "l":
                            {
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
                            Console.WriteLine("Unknown type. Please choose Checking / Savings / Loan.");
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
            Console.WriteLine("TODO: Deposit flow.");
            break;

        case "4":
            Console.WriteLine("TODO: Withdraw flow ");
            break;

        case "5":
            Console.WriteLine("TODO: View statement ");
            break;

        case "6":
            {
                
                int count = 0;
                foreach (var acc in registry.All)
                {
                    if (acc is MiniBank.Interfaces.IInterest ib)
                    {
                        ib.ApplyMonthlyInterest();
                        count++;
                    }
                }
                Console.WriteLine($"Month-end applied to {count} account(s).");
                break;
            }

        default:
            Console.WriteLine("Invalid option. Try again.");
            break;
    }
}

Console.WriteLine("Goodbye!");
