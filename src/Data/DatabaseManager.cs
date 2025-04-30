//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Data.Sqlite;

//namespace Coffee_Machine
//{
//    internal class DatabaseManager
//    {
//        private const string ConnectionString = "Data Source=coffee_machine.db";

//        public static void InitializeDatabase()
//        {
//            using var connection = new SqliteConnection(ConnectionString);
//            connection.Open();

//            var command = connection.CreateCommand();
//            command.CommandText =
//            @"
//                CREATE TABLE IF NOT EXISTS Ingredients (
//                    Id INTEGER PRIMARY KEY,
//                    Water REAL,
//                    Milk REAL,
//                    Coffee REAL,
//                    Sugar REAL
//                );

//                CREATE TABLE IF NOT EXISTS Coins (
//                    Id INTEGER PRIMARY KEY,
//                    Label TEXT,
//                    Value INTEGER
//                );

//                CREATE TABLE IF NOT EXISTS CoffeeTypes (
//                    Id INTEGER PRIMARY KEY,
//                    Name TEXT,
//                    Price INTEGER
//                );

//                CREATE TABLE IF NOT EXISTS CoinTransactions (
//                    Id INTEGER PRIMARY KEY,
//                    CoinValue INTEGER,
//                    Quantity INTEGER,
//                    Timestamp TEXT
//                );
//            ";
//            command.ExecuteNonQuery();
//        }

//        public static void SaveIngredients(Ingredients ingredients)
//        {
//            using var connection = new SqliteConnection(ConnectionString);
//            connection.Open();

//            var command = connection.CreateCommand();
//            command.CommandText =
//            @"
//                DELETE FROM Ingredients;
//                INSERT INTO Ingredients (Water, Milk, Coffee, Sugar)
//                VALUES ($water, $milk, $coffee, $sugar);
//            ";

//            command.Parameters.AddWithValue("$water", ingredients.Water);
//            command.Parameters.AddWithValue("$milk", ingredients.Milk);
//            command.Parameters.AddWithValue("$coffee", ingredients.Coffee);
//            command.Parameters.AddWithValue("$sugar", ingredients.Sugar);

//            command.ExecuteNonQuery();
//        }

//        public static void AddCoinTransaction(int coinValue)
//        {
//            using var connection = new SqliteConnection(ConnectionString);
//            connection.Open();

//            var command = connection.CreateCommand();
//            command.CommandText =
//            @"
//                INSERT INTO CoinTransactions (CoinValue, Quantity, Timestamp)
//                VALUES ($value, 1, $timestamp);
//            ";

//            command.Parameters.AddWithValue("$value", coinValue);
//            command.Parameters.AddWithValue("$timestamp", DateTime.Now.ToString("s"));
//            command.ExecuteNonQuery();
//        }

//        public static void SaveCoins(List<Coin> coins)
//        {
//            using var connection = new SqliteConnection(ConnectionString);
//            connection.Open();

//            var command = connection.CreateCommand();
//            command.CommandText = "DELETE FROM Coins;";
//            command.ExecuteNonQuery();

//            foreach (var coin in coins)
//            {
//                command.CommandText = "INSERT INTO Coins (Label, Value) VALUES ($label, $value);";
//                command.Parameters.Clear();
//                command.Parameters.AddWithValue("$label", coin.Label);
//                command.Parameters.AddWithValue("$value", coin.Value);
//                command.ExecuteNonQuery();
//            }
//        }

//        public static void SaveCoffeeTypes(List<Coffee> coffees)
//        {
//            using var connection = new SqliteConnection(ConnectionString);
//            connection.Open();

//            var command = connection.CreateCommand();
//            command.CommandText = "DELETE FROM CoffeeTypes;";
//            command.ExecuteNonQuery();

//            foreach (var coffee in coffees)
//            {
//                command.CommandText = "INSERT INTO CoffeeTypes (Name, Price) VALUES ($name, $price);";
//                command.Parameters.Clear();
//                command.Parameters.AddWithValue("$name", coffee.Name);
//                command.Parameters.AddWithValue("$price", coffee.Price);
//                command.ExecuteNonQuery();
//            }
//        }
//    }
//}
using src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Coffee_Machine.Data;

namespace src.Services
{
    internal static class DatabaseManager
    {
        public static void InitializeDatabase()
        {
            using var db = new ApplicationContext();
            db.Database.EnsureCreated(); 

            var ingredients = db.Ingredients.FirstOrDefault();
            if (ingredients == null)
            {
                ingredients = new Ingredients(1000, 1000, 500, 300);
                db.Ingredients.Add(ingredients);
                db.SaveChanges();
            }
            else
            {
                Console.WriteLine($"Ingredients loaded: Water={ingredients.Water}, Milk={ingredients.Milk}, Coffee={ingredients.Coffee}, Sugar={ingredients.Sugar}");
            }
        }



        public static void SaveIngredients(Ingredients ingredients)
        {
            using var db = new ApplicationContext();

            var existingIngredients = db.Ingredients.FirstOrDefault();

            if (existingIngredients != null)
            {
                existingIngredients.Water = ingredients.Water;
                existingIngredients.Milk = ingredients.Milk;
                existingIngredients.Coffee = ingredients.Coffee;
                existingIngredients.Sugar = ingredients.Sugar;

                db.SaveChanges();
            }
            else
            {
                db.Ingredients.Add(ingredients);
                db.SaveChanges();
            }
        }




        public static void AddCoinTransaction(int coinValue)
        {
            using var db = new ApplicationContext();

            var transaction = new CoinTransaction
            {
                CoinValue = coinValue,
                Quantity = 1,
                Timestamp = DateTime.Now
            };

            db.CoinTransactions.Add(transaction);
            db.SaveChanges();
        }

        public static void SaveCoins(List<Coin> coins)
        {
            using var db = new ApplicationContext();

            var existing = db.Coin.ToList();
            db.Coin.RemoveRange(existing);
            db.Coin.AddRange(coins);
            db.SaveChanges();
        }

        public static void SaveCoffeeTypes(List<Coffee> coffees)
        {
            using var db = new ApplicationContext();

            var existing = db.Coffee.ToList();
            db.Coffee.RemoveRange(existing);
            db.Coffee.AddRange(coffees);
            db.SaveChanges();
        }

        public static Ingredients LoadIngredients()
        {
            using var db = new ApplicationContext();
            var ingredients = db.Ingredients.FirstOrDefault();

            if (ingredients != null)
            {
                return ingredients;
            }

            ingredients = new Ingredients(1000, 1000, 500, 300);
            db.Ingredients.Add(ingredients);
            db.SaveChanges();
            return ingredients;
        }

    }
}

