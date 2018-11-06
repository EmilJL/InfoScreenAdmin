using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoScreenAdminDAL.Entities;

namespace InfoScreenAdminDAL
{
    public class Model
    {
        DbAccess dbAccess = new DbAccess();
        public Model()
        {
            dbAccess = new DbAccess();
            Model model = dbAccess.GetDataAndCreateModel();
        }
        public Model(ObservableCollection<Admin> admins, ObservableCollection<LunchPlan> lunchPlans, ObservableCollection<Message> messages)
        {
            Admins = admins;
            LunchPlans = lunchPlans;
            Messages = messages;
        }
        public ObservableCollection<Admin> Admins { get; private set; }
        public ObservableCollection<LunchPlan> LunchPlans { get; private set; }
        public ObservableCollection<Message> Messages { get; private set; }
    }
}
