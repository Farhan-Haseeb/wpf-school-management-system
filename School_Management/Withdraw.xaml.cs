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
using MahApps;
using System.Text.RegularExpressions;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Media.Animation;
using System.Diagnostics;

namespace School_Management
{
    /// <summary>
    /// Interaction logic for Withdraw.xaml
    /// </summary>
    public partial class Withdraw
    {
        public Withdraw()
        {
            InitializeComponent();
            withdrawButton.IsEnabled = false;
        }
        SchoolEntities context = new SchoolEntities();
        
        public void onLoad()
        {
            var result = context.SearchStudentInfoAll(active: "True").ToList();
            StudentSearchGrid.ItemsSource = result;
        }
        private void StudentSearchLoaded(object sender, RoutedEventArgs e)
        {
            onLoad();
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            if (stdId.Text != "")
            {
                int id = int.Parse(stdId.Text.ToString());
                var result = context.SearchStudentInfoAllById(admissionNumber: id, active: "True").ToList();
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
                        context.RemoveOrRestore(admissionNumber: withdrawNumber, active: "False", date: DateTime.Now.Date);
                    }
                }
                withdrawButton.IsEnabled = false;
                onLoad();
                this.ShowMessageAsync("Done", "Process completed");
            }
            catch (System.InvalidCastException err)
            {
                this.ShowMessageAsync("Selection Error!", "You didn't select any item or the search result doesn't belong to your query / Withdraw is only on ID");
                Debug.WriteLine(err.ToString());
            }
            clearAll(stdId);
            onLoad();
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