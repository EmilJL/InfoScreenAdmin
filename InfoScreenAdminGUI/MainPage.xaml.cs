using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using InfoScreenAdminDAL.Entities;
using InfoScreenAdminDAL;
using InfoScreenAdminBusiness;
using System.Globalization;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace InfoScreenAdminGUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DBHandler dbHandler;
        private LunchPlanHandler lunchPlanHandler;
        private Model model;
        private AdminHandler adminHandler;
        private MealHandler mealHandler;
        private MessageHandler messageHandler;
        public MainPage()
        {
            this.InitializeComponent();
            lunchPlanHandler = new LunchPlanHandler();
            mealHandler = new MealHandler();
            dbHandler = new DBHandler();
            messageHandler = new MessageHandler();
            model = dbHandler.Model;
            CmbBoxWeekNumbers.ItemsSource = Enumerable.Range(1, 51);
            CmbBoxWeekNumbers.SelectedIndex = GetIso8601WeekOfYear(DateTime.Now) - 1;
            ListViewDatabaseDishes.ItemsSource = model.Meals.OrderByDescending(m => m.TimesChosen);
        }
        public void ShowSelectedLunchPlan(int week)
        {
            try
            {
                if (lunchPlanHandler.GetLunchPlanForWeek(week) != null)
                {
                    LunchPlan lunchplan = lunchPlanHandler.GetLunchPlanForWeek(week);
                    if (lunchplan.Meals.Count > 0)
                        TBoxMonday.Text = lunchplan.Meals[0].Description;
                    if (lunchplan.Meals.Count > 1)
                        TBoxTuesday.Text = lunchplan.Meals[1].Description;
                    if (lunchplan.Meals.Count > 2)
                        TBoxWednesday.Text = lunchplan.Meals[2].Description;
                    if (lunchplan.Meals.Count > 3)
                        TBoxThursday.Text = lunchplan.Meals[3].Description;
                    if (lunchplan.Meals.Count > 4)
                        TBoxFriday.Text = lunchplan.Meals[4].Description;
                }
                else
                {
                    TBoxMonday.Text = "";
                    TBoxTuesday.Text = "";
                    TBoxWednesday.Text = "";
                    TBoxThursday.Text = "";
                    if (week % 2 == 0)
                    {
                        TBoxFriday.Text = "Fri";
                    }
                    else
                    {
                        TBoxFriday.Text = "";
                    }
                }
            }
            catch (ArgumentNullException e)
            {
                Debug.Write(e);
            }
        }


        // This presumes that weeks start with Monday.
        // Week 1 is the 1st week of the year with a Thursday in it.
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        private void CmbBoxWeekNumbers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowSelectedLunchPlan((int) CmbBoxWeekNumbers.SelectedItem);
        }

        private void BtnCurrentFoodPlan_Click(object sender, RoutedEventArgs e)
        {
            CmbBoxWeekNumbers.SelectedIndex = GetIso8601WeekOfYear(DateTime.Now) - 1;
        }

        private void BtnAddDishMonday_Click(object sender, RoutedEventArgs e)
        {
            TBoxMonday.Text = ListViewDatabaseDishes.SelectedItem.ToString();
        }

        private void BtnAddDishTuesdsay_Click(object sender, RoutedEventArgs e)
        {
            TBoxTuesday.Text = ListViewDatabaseDishes.SelectedItem.ToString();
        }

        private void BtnAddDishWednesday_Click(object sender, RoutedEventArgs e)
        {
            TBoxWednesday.Text = ListViewDatabaseDishes.SelectedItem.ToString();
        }

        private void BtnAddDishThursday_Click(object sender, RoutedEventArgs e)
        {
            TBoxThursday.Text = ListViewDatabaseDishes.SelectedItem.ToString();
        }

        private void BtnAddDishFriday_Click(object sender, RoutedEventArgs e)
        {
            TBoxThursday.Text = ListViewDatabaseDishes.SelectedItem.ToString();
        }

        // MAKE THIS BE ON TEXT CHANGED EVENT - ALSO CHANGE TBOX
        private void TestSearcher()
        {
            ListViewDatabaseDishes.ItemsSource = model.Meals.Where(m => m.Description.ToLower().Contains(TBoxFriday.Text.ToLower())).OrderByDescending(m => m.TimesChosen);
        }

        // ADD YEAR TO LUNCHPLAN? OR DELETE LUNCHPLANS EVERY YEAR - ALSO, MAYBE MAKE IT POSSIBLE TO NOT HAVE TO FILL OUT ENTIRE WEEK? ADD TEXTBLOCK FOR ERRORMESSAGE
        private void BtnSavePlan_Click(object sender, RoutedEventArgs e)
        {
            List<string> mealsOfWeek = new List<string>();
            mealsOfWeek.Add(TBoxMonday.Text);
            mealsOfWeek.Add(TBoxTuesday.Text);
            mealsOfWeek.Add(TBoxWednesday.Text);
            mealsOfWeek.Add(TBoxThursday.Text);
            mealsOfWeek.Add(TBoxFriday.Text);
            if (mealsOfWeek.Any(s => s == ""))
            {
                // ADD ERRORMESSAGE
                return;
            }
            LunchPlan lunchPlan = new LunchPlan();
            List<Meal> meals = new List<Meal>();
            
            int currentWeekNumber = int.Parse(CmbBoxWeekNumbers.SelectedValue.ToString());
            if (model.LunchPlans.Any(l => l.Week == currentWeekNumber))
            {
                foreach (var stringMeal in mealsOfWeek)
                {
                    model.Meals.Where(m => m.Description == stringMeal).FirstOrDefault().LunchPlanId = model.LunchPlans.Where(l => l.Week == currentWeekNumber).FirstOrDefault().Id;
                    mealHandler.UpdateMeal(model.Meals.Where(m => m.Description == stringMeal).FirstOrDefault());
                }
                model.LunchPlans.Where(l => l.Week == currentWeekNumber).FirstOrDefault().Meals = meals;
            }
            else
            {
                lunchPlan.Week = currentWeekNumber;
                lunchPlanHandler.AddLunchPlan(lunchPlan);
                model.LunchPlans.Add(lunchPlanHandler.GetLunchPlanForWeek(currentWeekNumber));
                foreach (var stringMeal in mealsOfWeek)
                {
                    model.Meals.Where(m => m.Description == stringMeal).FirstOrDefault().LunchPlanId = model.LunchPlans.Where(l => l.Week == currentWeekNumber).FirstOrDefault().Id;
                    mealHandler.UpdateMeal(model.Meals.Where(m => m.Description == stringMeal).FirstOrDefault());
                }
                model.LunchPlans.Where(l => l.Week == currentWeekNumber).FirstOrDefault().Meals = meals;
            }
        }

        //PERHAPS ENABLE SETTING DATE AND HAVING MULTIPLE MESSAGES STORED?
        private void BtnSaveMessage_Click(object sender, RoutedEventArgs e)
        {
            Message message = model.Messages.Last();
            message.Text = TBoxMessage.Text;
            message.Header = TBoxTitle.Text;
            //INSERT CURRENT ADMIN
            message.AdminId = 1;
            message.Date = DateTime.Now;
            messageHandler.UpdateMessage(message);
        }
    }
}
