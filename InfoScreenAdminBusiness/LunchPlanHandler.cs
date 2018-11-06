using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoScreenAdminDAL.Entities;

namespace InfoScreenAdminBusiness
{
    public class LunchPlanHandler : DBHandler
    {
        public LunchPlan GetLunchPlan(int id)
        {
            LunchPlan lunchPlan = Model.LunchPlans.Where(m => m.Id == id).FirstOrDefault();
            return lunchPlan;
        }
        public bool AddLunchPlan(LunchPlan lunchPlan)
        {
            try
            {
                DbAccess.AddLunchPlan(lunchPlan);
                Model.LunchPlans.Add(lunchPlan);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        public bool DeleteLunchPlan(int id)
        {
            try
            {
                DbAccess.DeleteLunchPlan(id);
                Model.LunchPlans.RemoveAt(Model.LunchPlans.IndexOf(Model.LunchPlans.Where(l => l.Id == id).FirstOrDefault()));
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        public bool UpdateLunchPlan(LunchPlan lunchPlan)
        {
            try
            {
                DbAccess.UpdateLunchPlan(lunchPlan);
                var lp = Model.LunchPlans.Where(l => l.Id == lunchPlan.Id).FirstOrDefault();
                lp.Date = lunchPlan.Date;
                lp.Meal = lunchPlan.Meal;
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
    }
}
