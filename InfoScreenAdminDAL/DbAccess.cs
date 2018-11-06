using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoScreenAdminDAL.Entities;

namespace InfoScreenAdminDAL
{
    public class DbAccess
    {
        private const string connectionString = @"Data Source=cvdb3,1488;Initial Catalog=DAHO.AspITInfoScreen;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public Model GetDataAndCreateModel()
        {
            const string GetAdminsQuery = "SELECT * from Admins";
            const string GetLunchPlansQuery = "SELECT * from LunchPlans";
            const string GetMessagesQuery = "SELECT * from Messages";
            const string GetMealsQuery = "SELECT * from Meals";
            var admins = new ObservableCollection<Admin>();
            var lunchPlans = new ObservableCollection<LunchPlan>();
            var messages = new ObservableCollection<Message>();
            var meals = new ObservableCollection<Meal>();
            
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = GetAdminsQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var admin = new Admin();
                                    admin.Id = reader.GetInt32(0);
                                    admin.Username = reader.GetString(1);
                                    admin.PasswordSalt = (byte[])reader.GetValue(2);
                                    admin.PasswordHash = (byte[])reader.GetValue(3);
                                    admins.Add(admin);
                                }
                            }
                            cmd.CommandText = GetMessagesQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var message = new Message();
                                    message.Id = reader.GetInt32(0);
                                    message.AdminId = reader.GetInt32(1);
                                    message.Date = reader.GetDateTime(2);
                                    message.Text = reader.GetString(3);
                                    message.Header = reader.GetString(4);
                                    messages.Add(message);
                                }
                            }
                            cmd.CommandText = GetMealsQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var meal = new Meal();
                                    meal.Id = reader.GetInt32(0);
                                    meal.LunchPlanId = reader.GetInt32(1);
                                    meal.TimesChosen = reader.GetInt32(2);
                                    meals.Add(meal);
                                }
                            }
                            cmd.CommandText = GetLunchPlansQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var lunchPlan = new LunchPlan();
                                    lunchPlan.Id = reader.GetInt32(0);
                                    lunchPlan.Meals = meals.Where(m => m.LunchPlanId == lunchPlan.Id).ToList();
                                    lunchPlan.Week = reader.GetInt32(1);
                                    lunchPlans.Add(lunchPlan);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            Model model = new Model(admins, lunchPlans, messages, meals);
            return model;
        }
        public void AddAdmin(Admin admin)
        {
            string AddAdminQuery = $"INSERT into Admins (Username, PasswordSalt, PasswordHash) VALUES (@Username, @PasswordSalt, @PasswordHash)";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = AddAdminQuery;
                            cmd.Parameters.AddWithValue("@Username", admin.Username);
                            cmd.Parameters.AddWithValue("@PasswordSalt", admin.PasswordSalt);
                            cmd.Parameters.AddWithValue("@PasswordHash", admin.PasswordHash);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }

        }
        public void DeleteAdmin(int adminId)
        {
            string Id = adminId.ToString();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand("DELETE FROM Admins WHERE Id = @Id", conn))
                    {
                        command.Parameters.AddWithValue("@Id", Id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
        }
        
        public void UpdateAdmin(Admin admin)
        {
            string Id = admin.Id.ToString();
            string passwordHash = Convert.ToBase64String(admin.PasswordHash);
            string passwordSalt = Convert.ToBase64String(admin.PasswordSalt);
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand("UPDATE Admins SET Username = @username, PasswordSalt = @PasswordSalt, PasswordHash = @PasswordHash WHERE Id = @Id", conn))
                    {
                        command.Parameters.AddWithValue("@Id", Id);
                        command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                        command.Parameters.AddWithValue("@PasswordSalt", passwordSalt);
                        command.Parameters.AddWithValue("@Username", admin.Username);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
        }
        public void AddLunchPlan(LunchPlan lunchPlan)
        {
            string AddLunchPlanQuery = $"INSERT into LunchPlans (Week) VALUES (@Week)";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = AddLunchPlanQuery;
                            cmd.Parameters.AddWithValue("@Week", lunchPlan.Week);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
        }
        public void DeleteLunchPlan(int lunchPlanId)
        {
            string Id = lunchPlanId.ToString();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand("DELETE FROM LunchPlans WHERE Id = @Id", conn))
                    {
                        command.Parameters.AddWithValue("@Id", Id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
        }
        public void UpdateLunchPlan(LunchPlan lunchPlan)
        {
            string Id = lunchPlan.Id.ToString();
            string week = lunchPlan.Week.ToString();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand("UPDATE LunchPlans SET Week = @Week WHERE Id = @Id", conn))
                    {
                        command.Parameters.AddWithValue("@Id", Id);
                        command.Parameters.AddWithValue("@Week", week);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
        }
        public void AddMessage(Message message)
        {
            string AddMessageQuery = $"INSERT into Messages (AdminId, Date, Text, Header) VALUES (@AdminId, @Date, @Text, @Header)";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = AddMessageQuery;
                            cmd.Parameters.AddWithValue("@AdminId", message.AdminId);
                            cmd.Parameters.AddWithValue("@Date", message.Date);
                            cmd.Parameters.AddWithValue("@Text", message.Text);
                            cmd.Parameters.AddWithValue("@Header", message.Header);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
        }
        public void DeleteMessage(int messageId)
        {
            string Id = messageId.ToString();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand("DELETE FROM Messages WHERE Id = @id", conn))
                    {
                        command.Parameters.AddWithValue("@Id", Id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
        }
        public void UpdateMessage(Message message)
        {
            string Id = message.Id.ToString();
            string adminId = message.AdminId.ToString();
            string date = message.Date.ToLongDateString();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand("UPDATE Messages SET AdminId = @AdminId, Date = @Date, Text = @Text, Header = @Header WHERE Id = @Id", conn))
                    {
                        command.Parameters.AddWithValue("@Id", Id);
                        command.Parameters.AddWithValue("@AdminId", adminId);
                        command.Parameters.AddWithValue("@Date", date);
                        command.Parameters.AddWithValue("@Text", message.Text);
                        command.Parameters.AddWithValue("@Header", message.Header);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
        }
        public void AddMeal(Meal meal)
        {
            string AddMealQuery = $"INSERT into Meals (Description, LunchPlanId, TimesChosen) VALUES (@Description, @LunchPlanId, @TimesChosen)";
            string LunchPlanId = meal.LunchPlanId.ToString();
            string TimesChosen = meal.TimesChosen.ToString();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = AddMealQuery;
                            cmd.Parameters.AddWithValue("@Description", meal.Description);
                            cmd.Parameters.AddWithValue("@LunchPlanId", LunchPlanId);
                            cmd.Parameters.AddWithValue("@TimesChosen", TimesChosen);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
        }
        public void DeleteMeal(int mealId)
        {
            string Id = mealId.ToString();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand("DELETE FROM Meals WHERE Id = @Id", conn))
                    {
                        command.Parameters.AddWithValue("@Id", Id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
        }
        public void UpdateMeal(Meal meal)
        {
            string Id = meal.Id.ToString();
            string lunchPlanId = meal.LunchPlanId.ToString();
            string timesChosen = meal.TimesChosen.ToString();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand("UPDATE Meals SET Description = @description, LunchPlanId = @lunchPlanId, TimesChosen = @timesChosen WHERE Id = @Id", conn))
                    {
                        command.Parameters.AddWithValue("@Id", Id);
                        command.Parameters.AddWithValue("@description", meal.Description);
                        command.Parameters.AddWithValue("@lunchPlanId", lunchPlanId);
                        command.Parameters.AddWithValue("@timesChosen", timesChosen);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
        }
    }
}
