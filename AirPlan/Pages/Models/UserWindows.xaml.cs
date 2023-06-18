using AirPlan.Models;
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
using System.Windows.Shapes;

namespace AirPlan.Pages.Models
{
    public partial class UserWindows : Window
    {

        List<logService> userIdLog;

        public UserWindows()
        {
            InitializeComponent();

            DateTime oldDays = DateTime.Now.AddDays(-30);
            try
            {
                List<TrackUser> userIdTrack = AirPlanEntities.GetContext().TrackUser.Where(i => i.Login > oldDays && i.UserId == CurrentUser.ID).ToList();
                userIdLog = AirPlanEntities.GetContext().logService.Where(i => i.idUser == CurrentUser.ID).ToList();

            TimeSpan? sessionUser = null;

            if (userIdTrack.Count > 0)
            {
                sessionUser = (userIdTrack[0].Out - userIdTrack[0].Login);
                for (int i = 1; i < userIdTrack.Count;i++) 
                {
                    sessionUser += userIdTrack[i].Out - userIdTrack[i].Login;
                }
            }


            TextBlockHello.Text = "Hi " + CurrentUser.FirstName + ", Welcome to AirPlan Airlines.";

            TextBlockCountCrashes.Text = "Count crashes: " + userIdLog.Where(x => x.state == "ERROR").ToList().Count;
            DataGridLogUsers.ItemsSource = userIdLog;
            TimeSpan timeSpan;
            TimeSpan.TryParse(sessionUser.ToString(), out timeSpan);
            TextBlockCountTimes.Text = "Session time for 30 days: " + timeSpan.ToString("%d") + " days " + timeSpan.ToString("%h") + " hours " + timeSpan.ToString("%m") + " minuts";
            
            }
            catch
            {
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
    }
}
