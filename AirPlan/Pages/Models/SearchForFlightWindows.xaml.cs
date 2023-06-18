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
    /// Логика взаимодействия для SearchForFlightWindows.xaml
    /// </summary>
    public partial class SearchForFlightWindows : Window
    {
        List<Schedules> schedulesList;

        public SearchForFlightWindows()
        {
            InitializeComponent();
            List<string> listType = new List<string>() { "Return", "One way" };
            ComboBoxTypeTicket.ItemsSource = listType;
            try
            {
                ComboBoxAirportFrom.ItemsSource = AirPlanEntities.GetContext().Airports.ToList();
                ComboBoxAirportTo.ItemsSource = AirPlanEntities.GetContext().Airports.ToList();
                ComboBoxCabinType.ItemsSource = AirPlanEntities.GetContext().CabinTypes.ToList();
                schedulesList = AirPlanEntities.GetContext().Schedules.Where(i => i.Confirmed == true).ToList();
            }
            catch
            {
                MessageBox.Show(MessageError.ErrrorDBConect);
                MessageError.PrintErrorDBConect();
            }

            //Type Tickets

        }
        private void selectionChangedComboBoxAiraports(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxAirportFrom.SelectedIndex == ComboBoxAirportTo.SelectedIndex)
            {
                MessageBox.Show("Место вылета и прибытия не могут совпадать");
                ComboBoxAirportTo.SelectedItem = null;
            }
        }

        private void SelectionChangedComboBoxTypeTicket(object sender, SelectionChangedEventArgs e)
        {
            if(ComboBoxTypeTicket.SelectedIndex != -1)
            {
                if(ComboBoxTypeTicket.SelectedIndex == 0)
                {
                    DataGridOUT.Visibility = Visibility.Visible;
                }
                if(ComboBoxTypeTicket.SelectedIndex == 1)
                {
                    DataGridOUT.Visibility = Visibility.Hidden;
                }
            }
        }

        public void UpdateList()
        {
            schedulesList = AirPlanEntities.GetContext().Schedules.Where(i => i.Confirmed == true).ToList();
            DrawDataGrid();
        }

        private void DrawDataGrid()
        {
            List <Schedules> listSortTo = schedulesList;
            List <Schedules> listSortFrom = schedulesList;

            
            int idAirportsTo, idAirportsFrom;
            
            if (ComboBoxAirportFrom.SelectedIndex != -1)
            {
                int.TryParse(ComboBoxAirportFrom.SelectedValue.ToString(), out idAirportsFrom);
                listSortTo = listSortTo.Where(i => i.Routes.Airports.ID == idAirportsFrom).ToList();
                listSortFrom = listSortFrom.Where(i => i.Routes.Airports1.ID == idAirportsFrom).ToList();
            }
            if (ComboBoxAirportTo.SelectedIndex != -1)
            {
                int.TryParse(ComboBoxAirportTo.SelectedValue.ToString(), out idAirportsTo);
                listSortTo = listSortTo.Where(i => i.Routes.Airports1.ID == idAirportsTo).ToList();
                listSortFrom = listSortFrom.Where(i => i.Routes.Airports.ID == idAirportsTo).ToList();
                
            }

            DateTime dateToSelectTo;

            if (DateTime.TryParse(DatePickerOutboubd.SelectedDate.ToString(), out dateToSelectTo))
            {
                if (RadioButtonTo.IsChecked == true) {
                    listSortTo = listSortTo.Where(i => i.Date >= dateToSelectTo.AddDays(-3)).ToList();
                    listSortTo = listSortTo.Where(i => i.Date <= dateToSelectTo.AddDays(3)).ToList();
                    listSortFrom = listSortFrom.Where(i => i.Date > dateToSelectTo.AddDays(-3)).ToList();
                }
                else
                {
                    listSortTo = listSortTo.Where(i => i.Date == dateToSelectTo).ToList();
                }
                
                
            }

            DateTime dateToSelectFrom;
            if (DateTime.TryParse(DatePickerReturn.SelectedDate.ToString(), out dateToSelectFrom))
            {
                if (RadioButtonOut.IsChecked == true)
                {
                    listSortFrom = listSortFrom.Where(i => i.Date >= dateToSelectFrom.AddDays(-3)).ToList();
                    listSortFrom = listSortFrom.Where(i => i.Date <= dateToSelectFrom.AddDays(3)).ToList();
                }
                else
                {
                    listSortFrom = listSortFrom.Where(i => i.Date == dateToSelectTo).ToList();
                }
            }

            DataGridTo.ItemsSource = listSortTo;
            DataGridOUT.ItemsSource = listSortFrom;
        }

        private void ClickButtonSearch(object sender, RoutedEventArgs e)
        {
            DrawDataGrid();
        }

        private void ClickButtonBlock(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TextBoxCountPassager.Text, out int PrintCountPassager))
            {
                if (DataGridTo.SelectedItems.Count > 0)
                {
                    string messageError = "";

                    Schedules selectSchedles = (DataGridTo.SelectedItems[0] as Schedules);
                    var listPassager = AirPlanEntities.GetContext().Tickets.GroupBy(x => x.ScheduleID).Select(g => new { ScheduleID = g.Key, Summ = g.Count() }).ToList();
                    int countPasseger = 0;
                    if (listPassager.Count != 0)
                        countPasseger = int.Parse(listPassager.Where(x => x.ScheduleID == selectSchedles.ID).ToList()[0].Summ.ToString());

                    if (ComboBoxCabinType.SelectedIndex != -1) {
                        if (int.Parse(ComboBoxCabinType.SelectedValue.ToString()) == 1 && selectSchedles.Aircrafts.EconomySeats > countPasseger + PrintCountPassager) {
                            //MessageBox.Show("Economy class");
                        }else if (int.Parse(ComboBoxCabinType.SelectedValue.ToString()) == 2 && selectSchedles.Aircrafts.BusinessSeats > countPasseger + PrintCountPassager)
                        {
                            //MessageBox.Show("Buisnes class");
                        }
                        else if (int.Parse(ComboBoxCabinType.SelectedValue.ToString()) == 3 && selectSchedles.Aircrafts.TotalSeats > countPasseger + PrintCountPassager)
                        {
                           // MessageBox.Show("First class");
                        }
                        else
                        {
                            messageError = "На выбранный рейс \"Туда\" нехватает мест для " + PrintCountPassager + " пассажиров";
                        }
                    }
                    else
                    {
                        messageError += "Не выбран тип класса билета";
                    }


                    //Проверка на билет обратно
                    if (DataGridOUT.SelectedItems.Count > 0)
                    {
                        Schedules selectSchedlesFrom = (DataGridOUT.SelectedItems[0] as Schedules);

                        if (ComboBoxCabinType.SelectedIndex != -1)
                        {
                            if (int.Parse(ComboBoxCabinType.SelectedValue.ToString()) == 1 && selectSchedlesFrom.Aircrafts.EconomySeats > countPasseger + PrintCountPassager)
                            {
                               // MessageBox.Show("Economy class");
                            }
                            else if (int.Parse(ComboBoxCabinType.SelectedValue.ToString()) == 2 && selectSchedlesFrom.Aircrafts.BusinessSeats > countPasseger + PrintCountPassager)
                            {
                               // MessageBox.Show("Buisnes class");
                            }
                            else if (int.Parse(ComboBoxCabinType.SelectedValue.ToString()) == 3 && selectSchedlesFrom.Aircrafts.TotalSeats > countPasseger + PrintCountPassager)
                            {
                               // MessageBox.Show("First class");
                            }
                            else
                            {
                                messageError += "\nНа обратный рейс нехватает мест для " + PrintCountPassager + " пассажиров";
                            }
                        }
                    }

                    if (messageError.Length < 2)
                    {
                        if (DataGridOUT.Visibility == Visibility.Visible && DataGridOUT.SelectedItems.Count > 0)
                        {
                            Schedules selectSchedlesFrom = (DataGridOUT.SelectedItems[0] as Schedules);
                            BlockTicket newPage = new BlockTicket(selectSchedles, selectSchedlesFrom, int.Parse(ComboBoxCabinType.SelectedValue.ToString()), countPasseger);
                            newPage.Show();
                        }
                        else if (DataGridOUT.Visibility == Visibility.Hidden)
                        {
                            BlockTicket newPage = new BlockTicket(selectSchedles, null, int.Parse(ComboBoxCabinType.SelectedValue.ToString()), countPasseger);
                            newPage.Show();
                        }
                        else
                        {
                            MessageBox.Show("Не выбран белет в обратную сторону, если вы хотете взять билет только в одну сторону - выберите параметр \"one way\"");
                        }
                    }
                    else
                    {
                        MessageBox.Show(messageError);
                    }


                }
                else
                {
                    MessageBox.Show("Не выбран рейс из списка");
                }
            }
            else
            {
                MessageBox.Show("Напишите количество пассажирова");
            }
        }
    }
}
