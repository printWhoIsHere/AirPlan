using AirPlan.Models;
using AirPlan.Pages.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AirPlan
{
    internal class MessageError
    {
        public static string ErrrorDBConect = "Warnin 505. Не удалось установить соединение с базой данных";
        public static string NoSelect = "Warnin 422. Выберите хотя бы один элемент из списка";
        public static string GoodLogin = "Вход в систему";
        public static string LogAUT = "Выход из системы";

        public static void PrintErrorDBConect()
        {
            try
            {
            logService newLog = new logService();
            newLog.log = ErrrorDBConect;
            newLog.state = "ERROR";
            newLog.date = DateTime.Now;
            newLog.idUser = CurrentUser.ID;
            AirPlanEntities.GetContext().logService.Add(newLog);
            AirPlanEntities.GetContext().SaveChanges();
            }
            catch
            {

            }
        }

        public static void PrintInLogLogin()
        {
            try
            {
                logService newLog = new logService();
                newLog.log = GoodLogin;
                newLog.state = "LOGIN";
                newLog.date = DateTime.Now;
                newLog.idUser = CurrentUser.ID;
                AirPlanEntities.GetContext().logService.Add(newLog);
                AirPlanEntities.GetContext().SaveChanges();
            }
            catch
            {
                
            }
        }

        public static void PrintInLogLogaut()
        {
            try
            {
                logService newLog = new logService();
                newLog.log = LogAUT;
                newLog.state = "LOGAUT";
                newLog.date = DateTime.Now;
                newLog.idUser = CurrentUser.ID;
                AirPlanEntities.GetContext().logService.Add(newLog);
                AirPlanEntities.GetContext().SaveChanges();

                TrackUser newTrek = new TrackUser();
                newTrek.UserId = AirPlan.Pages.Models.CurrentUser.ID;
                newTrek.Login = AirPlan.Pages.Models.TrekingLogin.LogIN;
                newTrek.Out = AirPlan.Pages.Models.TrekingLogin.LogOUT;
                AirPlanEntities.GetContext().TrackUser.Add(newTrek);
                AirPlanEntities.GetContext().SaveChanges();
            }
            catch
            {
                
            }

        }

    }
}
