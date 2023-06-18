using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AirPlan
{
    using AirPlan.Pages.Models;

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    using Models;
    using System.Security.Cryptography;

    public partial class MainWindow : Window
    {

        private int FailCount = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void drawTimer(int time)
        {
            await Task.Delay(1000);
            if (time > 0)
            {
                ButtonLogin.IsEnabled = false;
                TextBlockTimerBlock.Visibility = System.Windows.Visibility.Visible; 
                TextBlockTimerBlock.Text = "Вход в систему заблокирован на "+time+" секунд";
                drawTimer(time - 1);
            }
            else
            {
                TextBlockTimerBlock.Visibility = System.Windows.Visibility.Collapsed;
                ButtonLogin.IsEnabled = true;
                FailCount = 0;
            }
        }

        private void ClickButtonLogin(object sender, RoutedEventArgs e)
        {
            try
            {
                string MD5HashString = String.Join("", MD5.Create().ComputeHash(Encoding.Unicode.GetBytes(TextBoxPassword.Password)).Select(hashbyte => hashbyte.ToString("x2")));
                List <Users> searchUser = AirPlanEntities.GetContext().Users.Where(i => i.Email == TextBoxLogin.Text && i.Password == MD5HashString).ToList();
                
            if (searchUser.Count > 0)
            {
                if (searchUser[0].Active == true)
                {
                    CurrentUser.Email = searchUser[0].Email;
                    CurrentUser.ID = searchUser[0].ID;
                    CurrentUser.RoleID = searchUser[0].RoleID;
                    CurrentUser.Password = searchUser[0].Password;
                    CurrentUser.LastName = searchUser[0].LastName;
                    CurrentUser.FirstName = searchUser[0].FirstName;

                    TrekingLogin.LogIN = DateTime.Now;

                    if (searchUser[0].RoleID == 1)
                    {
                        //MessageBox.Show("Your Administrator");
                        Pages.AdminPanelWindow AdminPage = new Pages.AdminPanelWindow();
                        AdminPage.Show();
                    }
                    else if (searchUser[0].RoleID == 2)
                    {
                        Pages.Models.UserWindows UserPage = new Pages.Models.UserWindows();
                        UserPage.Show();
                    }else if (searchUser[0].RoleID == 3)
                    {
                        Pages.Models.ManagerFly managerPage = new Pages.Models.ManagerFly();
                        managerPage.Show();
                    }else if (searchUser[0].RoleID == 4)
                    {
                        Pages.Models.SearchForFlightWindows searchFly = new Pages.Models.SearchForFlightWindows();
                        searchFly.Show();
                    }

                }
                else
                {
                    MessageBox.Show("Акаунт заблокирован");
                }
            }
            else
            {
                MessageBox.Show("Error login");
                FailCount++;
                if (FailCount > 2)
                {
                    drawTimer(10);
                }
            }
            }
            catch
            {
                MessageBox.Show(MessageError.ErrrorDBConect);
                MessageError.PrintErrorDBConect();
            }
        }

        private void ClickExit(object sender, RoutedEventArgs e)
        {
            TrekingLogin.LogOUT = DateTime.Now;
            MessageError.PrintInLogLogaut();

            CurrentUser.ID = -1;
            CurrentUser.RoleID = -1;
            CurrentUser.Email = null;
            CurrentUser.Password = null;
            CurrentUser.FirstName = null;
            CurrentUser.LastName = null;
            
            this.Close();

        }

        private void ClickSearchTickets(object sender, RoutedEventArgs e)
        {
            (new SearchTicketsWindows()).Show();
        }

    }
}
