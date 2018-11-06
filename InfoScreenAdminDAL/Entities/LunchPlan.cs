using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoScreenAdminDAL.Entities
{
    public class LunchPlan
    {
        public int Id { get; set; }
        private DateTime date;
        private string meal;

        public LunchPlan(DateTime date, string meal)
        {
            Date = date;
            Meal = meal;
        }

        public string Meal
        {
            get { return meal; }
            set
            {
                meal = value;
            }
        }
        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
            }
        }
    }
}
