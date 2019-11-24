using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Media.Animation;
using System.Diagnostics;

namespace School_Management
{
    /// <summary>
    /// Interaction logic for WithdrawRegister.xaml
    /// </summary>
    public partial class WithdrawRegister
    {
        public WithdrawRegister()
        {
            InitializeComponent();
            withdrawButton.IsEnabled = false;
        }
        SchoolEntities context = new SchoolEntities();

        public void onLoad()
        {
            var result = context.SearchStudentInfoAll(active: "False").ToList();
            StudentSearchGrid.ItemsSource = result;
        }
        private void StudentSearchLoaded(object sender, RoutedEventArgs e)
        {
            onLoad();
        }

        private void ReloadButtonClick(object sender, RoutedEventArgs e)
        {
            clearAll(stdId);
            onLoad();
        }
        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            if (stdId.Text != "")
            {
                int id = int.Parse(stdId.Text.ToString());
                var result = context.SearchStudentInfoAllById(admissionNumber: id, active: "False").ToList();
                StudentSearchGrid.ItemsSource = result;
                withdrawButton.IsEnabled = true;
            }

        }
        private void withdrawButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var selecteditem = new List<SearchStudentInfoAllByIdResult>();

                foreach (SearchStudentInfoAllByIdResult item in StudentSearchGrid.ItemsSource)
                {
                    if (((CheckBox)SelectedStudent.GetCellContent(item)).IsChecked == true)
                    {
                        int withdrawNumber = item.AdmissionNumber;
                        context.RemoveOrRestore(admissionNumber: withdrawNumber, active: "True", date: null);
                    }
                }
                onLoad();
                withdrawButton.IsEnabled = false;
                this.ShowMessageAsync("Done", "Process completed");
            }
            catch (System.InvalidCastException err)
            {
                this.ShowMessageAsync("Selection Error!", "You didn't select any item or the search result doesn't belong to your query/ Withdraw is only on ID");
                Debug.WriteLine(err.ToString());
            }
            clearAll(stdId);
        }

        private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex Numeric = new Regex("[^0-9/]+"); e.Handled = Numeric.IsMatch(e.Text);
        }


        public void clearAll(TextBox h1)
        {
            h1.Clear();
        }
    }
}