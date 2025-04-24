using System;
using System.Collections.Generic;
using System.Linq;

namespace FINAL_midterms_project
{
    public class Program
    {
        public class Transaction
        {
            public string Description { get; set; }
            public double Amount { get; set; }
            public string Type { get; set; }
            public string Category { get; set; }
            public DateTime Date { get; set; }

            public Transaction(string description, double amount, string type, string category, DateTime date)
            {
                Description = description;
                Amount = amount;
                Type = type;
                Category = category;
                Date = date;
            }
        }

        public class BudgetTracker
        {
            public List<Transaction> Transactions { get; private set; } = new List<Transaction>();

            public void AddTransaction(Transaction transaction)
            {
                Transactions.Add(transaction);
            }

            public double CalculateTotalIncome()
            {
                return Transactions
                    .Where(t => t.Type == "Income")
                    .Sum(t => t.Amount);
            }

            public double CalculateTotalExpenses()
            {
                return Transactions
                    .Where(t => t.Type == "Expense")
                    .Sum(t => t.Amount);
            }

            public double CalculateNetSavings()
            {
                return CalculateTotalIncome() - CalculateTotalExpenses();
            }

            public List<Transaction> SortByDate()
            {
                return Transactions.OrderBy(t => t.Date).ToList();
            }

            public string MostSpentCategory()
            {
                return Transactions
                    .Where(t => t.Type == "Expense")
                    .GroupBy(t => t.Category)
                    .OrderByDescending(g => g.Sum(t => t.Amount))
                    .Select(g => g.Key)
                    .FirstOrDefault();
            }

            public double AverageMonthlySavings()
            {
                var monthlySavings = Transactions
                    .GroupBy(t => new { t.Date.Year, t.Date.Month })
                    .Select(g => new
                    {
                        Month = g.Key,
                        Savings = g.Where(t => t.Type == "Income").Sum(t => t.Amount) -
                                  g.Where(t => t.Type == "Expense").Sum(t => t.Amount)
                    });

                return monthlySavings.Average(ms => ms.Savings);
            }

            public void DisplayCategoryBreakdown()
            {
                Console.WriteLine("\n--- Expense Breakdown by Category ---");
                var categoryBreakdown = Transactions
                    .Where(t => t.Type == "Expense")
                    .GroupBy(t => t.Category)
                    .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Amount) });

                foreach (var category in categoryBreakdown)
                {
                    Console.WriteLine($"{category.Category}: {category.Total}");
                }
            }
        }

        public static void Main(string[] args)
        {
            BudgetTracker budgetTracker = new BudgetTracker();
            bool running = true;

            while (running)
            {
                Console.WriteLine("\n--- Personal Budget Tracker ---");
                Console.WriteLine("1. Add Transaction (Expense)");
                Console.WriteLine("2. Add Income");
                Console.WriteLine("3. View Insights");
                Console.WriteLine("4. Exit");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": // Add Expense
                        AddTransaction(budgetTracker, "Expense");
                        break;

                    case "2": // Add Income
                        AddTransaction(budgetTracker, "Income");
                        break;

                    case "3": // View Insights
                        ViewInsights(budgetTracker);
                        break;

                    case "4": // Exit
                        running = false;
                        Console.WriteLine("Exiting... Goodbye!");
                        break;

                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }

        public static void AddTransaction(BudgetTracker budgetTracker, string type)
        {
            Console.Write($"Enter date (YYYY-MM-DD) for {type}: ");
            DateTime date = DateTime.Parse(Console.ReadLine());

            Console.WriteLine("\nChoose Category:");
            Console.WriteLine("1. Food");
            Console.WriteLine("2. Rent");
            Console.WriteLine("3. Transportation");
            Console.WriteLine("4. Others");
            Console.Write("Enter choice (1-4): ");
            string categoryChoice = Console.ReadLine();
            string category = categoryChoice switch
            {
                "1" => "Food",
                "2" => "Rent",
                "3" => "Transportation",
                "4" => "Others",
                _ => "Others" 
            };

            Console.Write("Enter description: ");
            string description = Console.ReadLine();
            Console.Write("Enter amount: ");
            double amount = double.Parse(Console.ReadLine());


            budgetTracker.AddTransaction(new Transaction(description, amount, type, category, date));
            Console.WriteLine($"\n{type} transaction added successfully!");

            Console.WriteLine("\n--- Updated Summary ---");
            Console.WriteLine($"Total Income: {budgetTracker.CalculateTotalIncome()}");
            Console.WriteLine($"Total Expenses: {budgetTracker.CalculateTotalExpenses()}");
            Console.WriteLine($"Net Savings: {budgetTracker.CalculateNetSavings()}");
        }

        public static void DisplayCategoryGraph(BudgetTracker budgetTracker)
        {
            Console.WriteLine("\n--- Expense Graph by Category ---");

            var categoryBreakdown = budgetTracker.Transactions
                .Where(t => t.Type == "Expense")
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Amount) });

            double maxExpense = categoryBreakdown.Max(c => c.Total);

            foreach (var category in categoryBreakdown)
            {
                int barLength = (int)((category.Total / maxExpense) * 50); 
                Console.WriteLine($"{category.Category.PadRight(15)}: {category.Total,-10} | " + new string('*', barLength));
            }
        }

        public static void DisplayFinancialGraph(BudgetTracker budgetTracker)
        {
            Console.WriteLine("\n--- Financial Overview Graph ---");

            double totalIncome = budgetTracker.CalculateTotalIncome();
            double netSavings = budgetTracker.CalculateNetSavings();

            double maxValue = Math.Max(totalIncome, netSavings);

            int incomeBar = (int)((totalIncome / maxValue) * 50);
            int savingsBar = (int)((netSavings / maxValue) * 50);

            Console.WriteLine($"Total Income:  {totalIncome,-10} | " + new string('*', incomeBar));
            Console.WriteLine($"Net Savings:   {netSavings,-10} | " + new string('*', savingsBar));
        }

        public static void ViewInsights(BudgetTracker budgetTracker)
        {
            Console.WriteLine("\n--- Financial Insights ---");
            Console.WriteLine($"Most Spent Category: {budgetTracker.MostSpentCategory()}");
            Console.WriteLine($"Average Monthly Savings: {budgetTracker.AverageMonthlySavings()}");

            budgetTracker.DisplayCategoryBreakdown();
            DisplayCategoryGraph(budgetTracker); 
            DisplayFinancialGraph(budgetTracker); 
        }
    }
}
