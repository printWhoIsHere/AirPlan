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
using AirPlan.Pages.Models;
using System.Data.Entity;

namespace AirPlan.Pages
{
    public partial class AdminPanelWindow : Window
    {
        List<Users> usersList = AirPlanEntities.GetContext().Users.ToList();

        public AdminPanelWindow()
        {
            InitializeComponent();
            try
            {
                Offices newOf = new Offices();
                newOf.Title = "Все типы";
                newOf.ID = -2;

                List<Offices> officesList = new List<Offices>().ToList();
                officesList.Add(newOf);

                foreach (Offices item in AirPlanEntities.GetContext().Offices.ToList() ) {
                    officesList.Add(item);
                }

                ComboBoxType.ItemsSource = officesList;
               
            }
            catch
            {
                MessageBox.Show(MessageError.ErrrorDBConect);
                MessageError.PrintErrorDBConect();
            }
            drawDataGrid();
        }

        public void drawDataGrid()
        {
            List<Users> FilterUsersList = usersList;
            DataGridUsersList.Items.Clear();


            if (ComboBoxType.SelectedIndex != -1)
                if (ComboBoxType.SelectedValue.ToString() != "-2")
                {
                    {
                        int idOffice = int.Parse(ComboBoxType.SelectedValue.ToString());
                        FilterUsersList = FilterUsersList.Where(i => i.OfficeID == idOffice).ToList();
                    }
                }
            foreach (Users uesrItem in FilterUsersList)
            {
                DataGridUsersList.Items.Add(uesrItem);
            }
        }

        private void clickExit(object sender, RoutedEventArgs e)
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

        private void selectionChangedComboBoxType(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //usersList = AirPlanEntities.GetContext().Users.ToList();
                //usersList = usersList.Where(i => i.OfficeID == int.Parse(ComboBoxType.SelectedValue.ToString())).ToList();
                drawDataGrid();
            }
            catch
            {
                MessageBox.Show(MessageError.ErrrorDBConect);
                MessageError.PrintErrorDBConect();
            }
        }

        public void updateAllMaterialList()
        {
            try
            {
                usersList = AirPlanEntities.GetContext().Users.ToList();
                drawDataGrid();
            }
            catch
            {
                MessageBox.Show(MessageError.ErrrorDBConect);
                MessageError.PrintErrorDBConect();
            }
        }

        private void ClickAddUsers(object sender, RoutedEventArgs e)
        {
            Pages.AddUserWindows openPage = new Pages.AddUserWindows();
            openPage.Owner = this;
            openPage.Show();
        }

        private void statusUser(object sender, RoutedEventArgs e)
        {
            if (DataGridUsersList.SelectedItems.Count > 0)
            {
                try
                {
                    for (int i = 0; i < DataGridUsersList.SelectedItems.Count; i++)
                    {
                        (DataGridUsersList.SelectedItems[i] as Users).Active = !(DataGridUsersList.SelectedItems[i] as Users).Active;
                        AirPlanEntities.GetContext().SaveChanges();
                    }
                    updateAllMaterialList();
                }
                catch
                {
                    MessageError.PrintErrorDBConect();
                    MessageBox.Show(MessageError.ErrrorDBConect);
                }
            }
        }

        private void ClickChangeRole(object sender, RoutedEventArgs e)
        {
            if (DataGridUsersList.SelectedItems.Count > 0)
            {
                try
                {
                    for (int i = 0; i < DataGridUsersList.SelectedItems.Count; i++)
                    {
                        Users editUser = DataGridUsersList.SelectedItems[i] as Users;
                        if (editUser.RoleID == 1)
                        {
                            editUser.RoleID = 2;
                        }
                        else
                        {
                            editUser.RoleID = 1;
                        }
                        AirPlanEntities.GetContext().SaveChanges();
                    }
                }
                catch
                {
                    MessageBox.Show(MessageError.ErrrorDBConect);
                    MessageError.PrintErrorDBConect();
                }
                drawDataGrid();
            }
            else
            {
                MessageBox.Show(MessageError.NoSelect);
            }
        }
    }
}
