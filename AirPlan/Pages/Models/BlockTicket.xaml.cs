using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Policy;
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
using AirPlan.Models;

namespace AirPlan.Pages.Models
{
    /// <summary>
    /// Логика взаимодействия для BlockTicket.xaml
    /// </summary>
    public partial class BlockTicket : Window
    {

        int idCabainSelect, countPassagerSelect;
        Schedules schedulesToSelect;
        Schedules schedulesOUTSelect = null;

        public BlockTicket(Schedules schedulesTo, Schedules schedulesOUT, int idCabain, int countPassager)
        {
            InitializeComponent();
            try
            {
                idCabainSelect = idCabain;
                countPassagerSelect = countPassager;
                schedulesToSelect = schedulesTo;
                schedulesOUTSelect = schedulesOUT;

                TextBoxPasportCountry.ItemsSource = AirPlanEntities.GetContext().Countries.ToList();

                TextBoxToFrom.Text = schedulesTo.Routes.Airports1.Name;
                TextBoxToTo.Text = schedulesTo.Routes.Airports.Name;
                TextBoxCabinTypeTo.Text = AirPlanEntities.GetContext().CabinTypes.Find(idCabain).Name;
                TextBoxDateTo.Text = schedulesTo.Date.ToString();
                TextBoxCabinFlightNumberTo.Text = schedulesTo.FlightNumber;

                if (schedulesOUT != null)
                {
                    TextBoxToReturn.Text = schedulesOUT.Routes.Airports1.Name;
                    TextBoxToFromReturn.Text = schedulesOUT.Routes.Airports.Name;
                    TextBoxCabinTypeReturn.Text = AirPlanEntities.GetContext().CabinTypes.Find(idCabain).Name;
                    TextBoxDateReturn.Text = schedulesOUT.Date.ToString();
                    TextBoxCabinFlightNumberReturn.Text = schedulesOUT.FlightNumber;
                }
                else
                {
                    StackPanelReturn.Visibility = Visibility.Collapsed;
                    TextBlockReturn.Visibility = Visibility.Collapsed;
                }

            }
            catch
            {
                MessageError.PrintErrorDBConect();
            }

        }

        private void ClickSaveTocket(object sender, RoutedEventArgs e)
        {
            decimal price = 0;
            for (int i = 0; i < DataGridPassager.Items.Count; i++)
            {
                Tickets newTickets = (DataGridPassager.Items[i] as Tickets);

                MessageBox.Show(newTickets.ScheduleID.ToString());

                AirPlanEntities.GetContext().Tickets.Add(newTickets);
                AirPlanEntities.GetContext().SaveChanges();
                if (idCabainSelect == 1)
                    price += newTickets.Schedules.EconomyPrice;
                if (idCabainSelect == 2)
                    price += newTickets.Schedules.EconomyPrice * (1.35m);
                if (idCabainSelect == 3)
                    price += newTickets.Schedules.EconomyPrice * (1.35m) * 1.3m;
            }
            try
            {
                (new ConfirmationWindow(price)).Show();
            }
            catch
            {
                MessageError.PrintErrorDBConect();
            }

            if (DataGridPassager.Items.Count > 0)
                MessageBox.Show("Билеты были успешно забронированы, код брони:" + (DataGridPassager.Items[0] as Tickets).BookingReference);
            this.Close();

        }

        private void ClickBackSearch(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RemoveAddPassenger(object sender, RoutedEventArgs e)
        {
            DataGridPassager.Items.Remove(DataGridPassager.SelectedItem);
        }

        private void ClickAddPassenger(object sender, RoutedEventArgs e)
        {
            string messageError = "";
            if (TextBoxFirstName.Text.Length < 2)
                messageError = "Введите имя";
            if (TextBoxLastName.Text.Length < 2)
                messageError += "\n Введите фамилию";
            if (TextBoxPasportCountry.SelectedIndex == -1)
                messageError += "\n Выберите страну паспорта";
            if (TextBoxPasportNum.Text.Length != 6)
                messageError += "\n Не корректно введен номер паспорта";
            if (TextBoxPhone.Text.Length != 12)
                messageError += "\n Телефон должен состоять из 12 сиволов";
            if (messageError.Length < 2)
            {
                Tickets newTicketTo = new Tickets();
                newTicketTo.UserID = CurrentUser.ID;
                newTicketTo.Users = AirPlanEntities.GetContext().Users.Find(CurrentUser.ID);
                newTicketTo.Schedules = schedulesToSelect;
                newTicketTo.ScheduleID = schedulesToSelect.ID;
                newTicketTo.CabinTypeID = idCabainSelect;
                newTicketTo.Firstname = TextBoxFirstName.Text;
                newTicketTo.Lastname = TextBoxLastName.Text;
                newTicketTo.Phone = TextBoxPhone.Text;
                newTicketTo.PassportNumber = TextBoxPasportNum.Text;
                newTicketTo.PassportCountryID = int.Parse(TextBoxPasportCountry.SelectedValue.ToString());
                newTicketTo.BookingReference = TextBoxLastName.Text[0] + TextBoxFirstName.Text;
                newTicketTo.Confirmed = true;

                DataGridPassager.Items.Add(newTicketTo);

                if (schedulesOUTSelect != null)
                {
                    Tickets newTicketFrom = new Tickets();
                    newTicketFrom.Users = AirPlanEntities.GetContext().Users.Find(CurrentUser.ID);
                    newTicketFrom.UserID = CurrentUser.ID;
                    newTicketFrom.CabinTypeID = idCabainSelect;
                    newTicketFrom.Firstname = TextBoxFirstName.Text;
                    newTicketFrom.Lastname = TextBoxLastName.Text;
                    newTicketFrom.Phone = TextBoxPhone.Text;
                    newTicketFrom.PassportNumber = TextBoxPasportNum.Text;
                    newTicketFrom.PassportCountryID = int.Parse(TextBoxPasportCountry.SelectedValue.ToString());
                    newTicketFrom.BookingReference = TextBoxLastName.Text[0] + TextBoxFirstName.Text;
                    newTicketFrom.Schedules = schedulesOUTSelect;
                    newTicketFrom.ScheduleID = schedulesOUTSelect.ID;

                    DataGridPassager.Items.Add(newTicketFrom);
                }

            }
            else
            {
                MessageBox.Show(messageError);
            }
        }
    }
}
