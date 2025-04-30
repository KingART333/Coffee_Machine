using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using src.Models;

namespace src.Services
{
    public class CoffeeType
    {
        private readonly Ingredients _ingredients;

        public CoffeeType(Ingredients ingredients)
        {
            _ingredients = ingredients;
        }

        public bool MakeAmericano()
        {
            return _ingredients.UseIngredients(100, 0, 15, 5);
        }

        public bool MakeEspresso()
        {
            return _ingredients.UseIngredients(50, 0, 20, 5);
        }

        public bool MakeCappuccino()
        {
            return _ingredients.UseIngredients(80, 60, 20, 10);
        }

        public bool MakeLatte()
        {
            return _ingredients.UseIngredients(60, 100, 15, 5);
        }
    }
}

