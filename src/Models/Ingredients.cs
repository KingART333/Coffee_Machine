using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coffee_Machine.Data;

namespace src.Models
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

            // Save the updated ingredients back to the database
            using (var db = new ApplicationContext())
            {
                db.Ingredients.Update(this); // Update the current ingredients
                db.SaveChanges();
            }
            return true;
        }
    }
}
