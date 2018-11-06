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
        private int week;
        private List<Meal> meals;
        public LunchPlan()
        {

        }
        public LunchPlan(int week, List<Meal> meals)
        {
            Week = week;
            Meals = meals;
        }
        public int Week
        {
            get { return week; }
            set { week = value; }
        }
        public List<Meal> Meals
        {
            get { return meals; }
            set { meals = value; }
        }
    }
}
