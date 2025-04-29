using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee_Machine.Models
{
    public class Ingredients
    {
        public int Id { get; set; }

        public float Water { get; set; }
        public float Milk { get; set; }
        public float Coffee { get; set; }
        public float Sugar { get; set; }

        public Ingredients(float water, float milk, float coffee, float sugar)
        {
            Water = water;
            Milk = milk;
            Coffee = coffee;
            Sugar = sugar;
        }

        public bool UseIngredients(float water, float milk, float coffee, float sugar)
        {
            if (Water < water || Milk < milk || Coffee < coffee || Sugar < sugar)
                return false;

            Water -= water;
            Milk -= milk;
            Coffee -= coffee;
            Sugar -= sugar;
            return true;
        }
    }
}
