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
    /// <summary>
    /// Логика взаимодействия для shedulerWindows.xaml
    /// </summary>
    public partial class shedulerWindows : Window
    {
        private Schedules editshedulesSelectedItem;

        public shedulerWindows(Schedules editshedules)
        {
            InitializeComponent();
            editshedulesSelectedItem = editshedules;
            TextBlockFrom.Text = editshedules.Routes.Airports.IATACode;
            TextBlockTo.Text = editshedules.Routes.Airports1.IATACode;
            TextBlockAircraft.Text = editshedules.Aircrafts.Name;
            DatePickerEditDate.SelectedDate = editshedules.Date;
            TextBoxTime.Text = editshedules.Time.ToString();
            TextBoxPrice.Text = editshedules.EconomyPrice.ToString();
        }

        private void clickUpdate(object sender, RoutedEventArgs e)
        {
            string warningMassage = "";
            DateTime dateToSelect;

            if (!DateTime.TryParse(DatePickerEditDate.SelectedDate.ToString(), out dateToSelect))
            {
                warningMassage = "Выберите дату!";
            }
            TimeSpan time;
            if (!TimeSpan.TryParse(TextBoxTime.Text, out time))
            {
                warningMassage += "Время введено не корректно";
            }

            Decimal price;
            if (!Decimal.TryParse(TextBoxPrice.Text, out price))
            {
                warningMassage += "Цена должна быть целым положительным числом!";
            }
            if (warningMassage != "")
            {
                MessageBox.Show(warningMassage);
            }
            else
            {
                try
                {
                    editshedulesSelectedItem.Date = dateToSelect;
                    editshedulesSelectedItem.Time = time;
                    editshedulesSelectedItem.EconomyPrice = price;
                    //editshedulesSelectedItem.BuisnesPrice = Math.Round(price + price/100*30);
                    //editshedulesSelectedItem.FirstClass = Math.Round(price + (price / 100 * 30)/100*35);
                    AirPlanEntities.GetContext().SaveChanges();

                    MessageBox.Show("Обвновление данных прошло успешно");
                    (this.Owner as ManagerFly).updateDataGrid();
                    this.Close();

                }
                catch
                {
                    MessageBox.Show(MessageError.ErrrorDBConect);
                    MessageError.PrintErrorDBConect();
                }
            }
        }

        private void clickCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
