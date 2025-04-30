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
            DatabaseManager.SaveIngredients(machineIngredients);
            DatabaseManager.SaveCoins(coinList);
            DatabaseManager.SaveCoffeeTypes(coffeeList);

            while (true)
            {
                Console.WriteLine("Current Balance: " + balance + " dram");
                Console.WriteLine("1. Add Coins");
                Console.WriteLine("2. Select Coffee");
                Console.WriteLine("3. Exit");
                Console.Write("Enter your choice: ");

                if (!int.TryParse(Console.ReadLine(), out int mainChoice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                switch (mainChoice)
                {
                    case 1:
                        AddingCoin();
                        break;
                    case 2:
                        ShowMenu();
                        Console.WriteLine();
                        if (!int.TryParse(Console.ReadLine(), out int coffeeChoice))
                        {
                            Console.WriteLine("Invalid input. Please enter a number.");
                            continue;
                        }
                        GetCoffeeType(coffeeChoice);
                        break;
                    case 3:
                        Console.WriteLine("Exiting the program.");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        public static void ShowMenu()
        {
            for (int i = 0; i < coffeeList.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {coffeeList[i].Name} \t\t {coffeeList[i].Price} dram");
            }
            Console.WriteLine($"{coffeeList.Count + 1}. Back");
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
            ShowCashType();
            Console.Write("Enter the coin value to add: ");
            if (!int.TryParse(Console.ReadLine(), out int coin))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                return;
            }

            bool validCoin = coinList.Any(c => c.Value == coin);

            if (!validCoin)
            {
                Console.WriteLine("Invalid coin. Please enter a valid coin value.");
                return;
            }

            balance += coin;
            //DatabaseManager.AddCoinTransaction(coin); // sqlite
            Coin selectedCoin = coinList.FirstOrDefault(c => c.Value == coin);
            if (selectedCoin == null)
            {
                Console.WriteLine("Coin not recognized.");
                return;
            }

            using (var db = new ApplicationContext())
            {
                db.CoinTransactions.Add(new CoinTransaction
                {
                    CoinValue = selectedCoin.Value,
                    Quantity = 1,
                    Timestamp = DateTime.Now
                });

                db.SaveChanges();
            }

            Console.WriteLine("Coin added successfully! Current Balance: " + balance + " dram");
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
    }
}