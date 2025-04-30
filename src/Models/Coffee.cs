using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Models
{
    public class Coffee
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public int Price { get; set; }

        public Coffee(string name, int price)
        {
            Name = name;
            Price = price;
        }
    }
}