using Coffee_Machine.Data;
using src.Models;
using src.Services;

namespace Coffee_Machine
{
    internal class Program
    {
        static Ingredients machineIngredients = DatabaseManager.LoadIngredients();
        private static int balance = 0;

        private static List<Coffee> coffeeList = new List<Coffee>
        {
            new Coffee("Americano", 100),
            new Coffee("Espresso", 150),
            new Coffee("Cappuccino", 200),
            new Coffee("Latte", 250)
        };

        public static List<Coin> coinList = new List<Coin>
        {
            new Coin("50 dram", 50),
            new Coin("100 dram", 100),
            new Coin("200 dram", 200),
            new Coin("500 dram", 500)
        };

        static void Main(string[] args)
        {
            DatabaseManager.InitializeDatabase();
            coinList = DatabaseManager.LoadCoins(); 
            DatabaseManager.SaveIngredients(machineIngredients);
            DatabaseManager.SaveCoins(coinList);
            DatabaseManager.SaveCoffeeTypes(coffeeList);

            while (true)
            {
                Thread.Sleep(1000);

                int mainChoice = ShowArrowMenu("Main Menu", new List<string> {
                    "Add Coins",
                    "Select Coffee",
                    "Refill Ingredients",
                    "Exit"
                });

                switch (mainChoice)
                {
                    case 0:
                        AddingCoin();
                        break;
                    case 1:
                        List<string> coffeeOptions = coffeeList.Select(c => $"{c.Name} - {c.Price} dram").ToList();
                        coffeeOptions.Add("Back");
                        int coffeeChoice = ShowArrowMenu("Select Coffee", coffeeOptions);
                        if (coffeeChoice < coffeeList.Count)
                            GetCoffeeType(coffeeChoice + 1);
                        break;
                    case 2:
                        RefillIngredients(1000, 500, 300, 200);
                        break;
                    case 3:
                        ExitAndGiveChange();
                        return;
                }
            }
        }

        public static int ShowArrowMenu(string title, List<string> options)
        {
            int selectedIndex = 0;
            ConsoleKey key;

            do
            {
                Console.Clear();

                Console.WriteLine($"Current Balance: {balance} dram\n");


                Console.WriteLine($"{title}:\n");

                for (int i = 0; i < options.Count; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"> {options[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"  {options[i]}");
                    }
                }

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex == 0) ? options.Count - 1 : selectedIndex - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % options.Count;
                        break;
                }

            } while (key != ConsoleKey.Enter);

            return selectedIndex;
        }

        public static void ShowCashType()
        {
            for (int i = 0; i < coinList.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {coinList[i].Value} dram");
            }
        }

        public static void LoadingImitation()
        {
            Console.WriteLine("Loading...");
            for (int i = 0; i < 5; i++)
            {
                Console.Write(".");
                System.Threading.Thread.Sleep(1000);
            }
            Console.WriteLine();
        }

        public static void AddingCoin()
        {
            List<string> coinOptions = coinList.Select(c => $"{c.Value} dram").ToList();
            coinOptions.Add("Back");

            int coinChoice = ShowArrowMenu("Insert Coin", coinOptions);

            if (coinChoice == coinOptions.Count - 1)
                return;

            Coin selectedCoin = coinList[coinChoice];
            balance += selectedCoin.Value;

            using (var db = new ApplicationContext())
            {
                db.CoinTransactions.Add(new CoinTransaction
                {
                    CoinValue = selectedCoin.Value,
                    Quantity = 1,
                    Timestamp = DateTime.Now
                });

                var coinInDb = db.Coin.FirstOrDefault(c => c.Value == selectedCoin.Value);
                if (coinInDb != null)
                {
                    coinInDb.Quantity += 1;
                    db.SaveChanges();
                }
            }

            Console.Clear();
            Console.WriteLine($"You inserted: {selectedCoin.Value} dram");
            Console.WriteLine($"Current Balance: {balance} dram");
        }


        public static Ingredients GetCoffeeType(int choice)
        {
            if (choice >= 1 && choice <= coffeeList.Count)
            {
                Coffee selected = coffeeList[choice - 1];
                if (CheckBalance(selected.Price))
                {
                    CoffeeType coffeeType = new CoffeeType(machineIngredients);
                    bool success = false;

                    switch (selected.Name.ToLower())
                    {
                        case "americano":
                            success = coffeeType.MakeAmericano();
                            break;
                        case "espresso":
                            success = coffeeType.MakeEspresso();
                            break;
                        case "latte":
                            success = coffeeType.MakeLatte();
                            break;
                        case "cappuccino":
                            success = coffeeType.MakeCappuccino();
                            break;
                    }

                    if (success)
                    {
                        LoadingImitation();
                        Console.WriteLine($"Your {selected.Name} is ready!");

                        Console.WriteLine($"Remaining Ingredients: Water={machineIngredients.Water}, Milk={machineIngredients.Milk}, Coffee={machineIngredients.Coffee}, Sugar={machineIngredients.Sugar}");
                    }
                    else
                    {
                        Console.WriteLine("Not enough ingredients. Please refill.");
                        balance += selected.Price;
                        Console.WriteLine($"Refunded: {selected.Price} dram. Current balance: {balance} dram");
                    }
                }
            }
            else if (choice == coffeeList.Count + 1)
            {
                Console.WriteLine("Returning to main menu...");
                return null;
            }
            else
            {
                Console.WriteLine("Invalid choice. Please try again.");
            }

            return null;
        }

        private static bool CheckBalance(int price)
        {
            if (balance >= price)
            {
                balance -= price;
                Console.WriteLine("Payment successful! Remaining Balance: " + balance + " dram");
                return true;
            }
            else
            {
                Console.WriteLine("Insufficient balance. Please add more coins.");
                return false;
            }
        }

        public static void GiveChange()
        {
            if (balance > 0)
            {
                int remainingBalance = balance;
                Dictionary<string, int> changeGiven = new Dictionary<string, int>();

                using (var db = new ApplicationContext())
                {
                    var availableCoins = db.Coin.ToList();

                    var tempCoins = availableCoins.Select(c => new
                    {
                        c.Label,
                        c.Value,
                        c.Quantity
                    }).ToList();

                    foreach (var coin in tempCoins.OrderByDescending(c => c.Value))
                    {
                        if (coin.Quantity == 0)
                            continue;

                        int coinCount = remainingBalance / coin.Value;
                        coinCount = Math.Min(coinCount, coin.Quantity);

                        if (coinCount > 0)
                        {
                            changeGiven[coin.Label] = coinCount;
                            remainingBalance -= coinCount * coin.Value;
                        }

                        if (remainingBalance == 0)
                            break;
                    }

                    if (remainingBalance > 0)
                    {
                        Console.Clear();
                        Console.WriteLine("⚠ Unable to provide exact change due to insufficient coins.");
                        Console.WriteLine($"Your remaining balance: {balance} dram");
                        Console.WriteLine("Please contact support.");
                        Console.ReadKey();
                        return; 
                    }

                    foreach (var coin in changeGiven)
                    {
                        var coinInDb = availableCoins.First(c => c.Label == coin.Key);
                        coinInDb.Quantity -= coin.Value;

                        db.CoinTransactions.Add(new CoinTransaction
                        {
                            CoinValue = coinInDb.Value,
                            Quantity = coin.Value,
                            Timestamp = DateTime.Now
                        });
                    }

                    db.SaveChanges();
                }

                Console.Clear();
                Console.WriteLine("Change returned:");
                foreach (var coin in changeGiven)
                {
                    Console.WriteLine($"{coin.Value} x {coin.Key}");
                }

                Console.WriteLine("Remaining Balance: 0 dram");
                balance = 0;
            }
            else
            {
                Console.WriteLine("No money left in the balance to return.");
            }
        }




        public static void ExitAndGiveChange()
        {
            GiveChange();

            Console.WriteLine("Exiting the program.");
            Environment.Exit(0);
        }

        public static void RefillIngredients(int water, int milk, int coffee, int sugar)
        {
            machineIngredients.Water += water;
            machineIngredients.Milk += milk;
            machineIngredients.Coffee += coffee;
            machineIngredients.Sugar += sugar;

            DatabaseManager.SaveIngredients(machineIngredients);

            Console.WriteLine("\nIngredients refilled successfully!");
            Console.WriteLine($"Current Ingredient Levels:");
            Console.WriteLine($"Water: {machineIngredients.Water}");
            Console.WriteLine($"Milk: {machineIngredients.Milk}");
            Console.WriteLine($"Coffee: {machineIngredients.Coffee}");
            Console.WriteLine($"Sugar: {machineIngredients.Sugar}");

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }

    }
}