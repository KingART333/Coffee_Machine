using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Models
{
    public class Coin
    {
        public int Id { get; set; }

        public string Label { get; set; }
        public int Value { get; set; }
        public int Quantity { get; set; }

        public Coin(string label, int value, int quantity = 0)
        {
            Label = label;
            Value = value;
            Quantity = quantity;

        }
    }
}
