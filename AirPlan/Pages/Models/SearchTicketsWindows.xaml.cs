using AirPlan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AirPlan.Pages.Models
{

    public partial class SearchTicketsWindows : Window
    {

        decimal countPrice;
        List<Tickets> searchTickets;

        public SearchTicketsWindows()
        {
            InitializeComponent();
        }

        private void ClickExit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ClickButtonSearchUser(object sender, RoutedEventArgs e)
        {
            try
            {
                // && i.Schedules.Date > DateTime.Now
                //
                searchTickets = AirPlanEntities.GetContext().Tickets.Where(i => i.BookingReference == TextBoxCodeTickets.Text && i.Schedules.Date > DateTime.Today
                ).ToList();
                if (searchTickets.Count > 0)
                {
                    StackPanelSearchTockets.Visibility = Visibility.Visible;
                    ComboBoxListTickeets.ItemsSource = searchTickets;
                    GridInfo.Visibility = Visibility.Visible;
                }
                else
                {
                    GridInfo.Visibility = Visibility.Collapsed;
                    MessageBox.Show("Билеты не были найдены");
                }
            }
            catch
            {
                MessageError.PrintErrorDBConect();
            }
            
        }

        private void ClickShowAmenities(object sender, RoutedEventArgs e)
        {
            if (ComboBoxListTickeets.SelectedIndex != -1)
            {
                StackPanelInfoUsers.Visibility = Visibility.Visible;
                TextBlockFullName.Text = (ComboBoxListTickeets.SelectedItem as Tickets).Firstname + (ComboBoxListTickeets.SelectedItem as Tickets).Lastname;
                TextBlockPasportNumber.Text = (ComboBoxListTickeets.SelectedItem as Tickets).PassportNumber;
                TextBlockCabinClass.Text = (ComboBoxListTickeets.SelectedItem as Tickets).CabinTypes.Name;

                List<Amenities> amenitiesList = AirPlanEntities.GetContext().Amenities.ToList();
                StackPanelAmentities.Children.Clear();

                for (int i = 0; i < amenitiesList.Count; i++)
                {
                    CheckBox newCheckBox = new CheckBox();
                    newCheckBox.Content = amenitiesList[i].Service + " " + amenitiesList[i].Price + " $";
                    newCheckBox.TabIndex = amenitiesList[i].ID;
                    //newCheckBox.Name = amenitiesList[i].ID.ToString();

                    List<AmenitiesTickets> fkAmenitiesTickets = (ComboBoxListTickeets.SelectedItem as Tickets).AmenitiesTickets.ToList();
                    if (amenitiesList[i].Price == 0)
                    {
                        newCheckBox.IsChecked = true;
                        newCheckBox.IsEnabled = false;
                    }

                    if (fkAmenitiesTickets.Where(x => x.AmenityID == amenitiesList[i].ID).ToList().Count > 0)
                    {
                        countPrice += amenitiesList[i].Price;
                        newCheckBox.IsChecked = true;
                    }

                    StackPanelAmentities.Children.Add(newCheckBox);
                }

            }
            else
            {
                MessageBox.Show("Выберите билет");
            }
        }

        private void ClickSaveButton(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (CheckBox stackPanelButton in StackPanelAmentities.Children)
                {

                    if (stackPanelButton.IsChecked != false)
                    {
                        Amenities selectAirPlanEntities = AirPlanEntities.GetContext().Amenities.Find(stackPanelButton.TabIndex);
                        AmenitiesTickets editTicket = new AmenitiesTickets();
                        editTicket.Amenities = selectAirPlanEntities;
                        editTicket.Price = selectAirPlanEntities.Price;
                        editTicket.AmenityID = selectAirPlanEntities.ID;

                        countPrice += selectAirPlanEntities.Price;

                        (ComboBoxListTickeets.SelectedItem as Tickets).AmenitiesTickets.Add(editTicket);
                        AirPlanEntities.GetContext().SaveChanges();

                        CountItemSelect.Text = countPrice.ToString();
                        TextBoxDuties.Text = (countPrice * 0.05m).ToString();
                        TextBoxTotal.Text = (countPrice + (countPrice * 0.05m)).ToString();
                    }
                }
                MessageBox.Show("Изменния в билете были успешно сохранены");
            }
            catch
            {
                MessageError.PrintErrorDBConect();
            }
        }

    }
}
