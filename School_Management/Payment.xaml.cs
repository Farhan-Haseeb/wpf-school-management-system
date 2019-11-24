using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Globalization;
using MahApps.Metro.Controls.Dialogs;

namespace School_Management
{
    /// <summary>
    /// Interaction logic for Payment.xaml
    /// </summary>
    public partial class Payment
    {
        public Payment()
        {
            InitializeComponent();
            CurrentYear(Year);
            CurrentYear(historyYear);
            DepositDate.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
            Load();
        }
        SchoolEntities context = new SchoolEntities();

        public void Load()
        {
            var result = context.PayementHistory().ToList();
            HistoryDataGrid.ItemsSource = result;
        }
        public void CurrentYear(ComboBox y)
        {
            DateTime a = DateTime.Now;

            for (int i = 2005; i <= a.Year + 1; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = i;
                y.Items.Add(item);
            }

            y.SelectedIndex = y.Items.Count - 2;
        }
        public string GetFeeMonth()
        {
            string mo = "";
            string ye = "";

            string m = Month.SelectedItem.ToString();
            string y = Year.SelectedItem.ToString();

            if (m.Contains("System.Windows.Controls.ComboBoxItem: ") && y.Contains("System.Windows.Controls.ComboBoxItem: "))
            {
                mo = m.Remove(0, 38);
                ye = y.Remove(0, 38);
            }
            string FeeMonth = mo + "-" + ye;
            return FeeMonth;
        }

        private void SearchbuttonClick(object sender, RoutedEventArgs e)
        {
            int Id;
            int.TryParse(employeeId.Text.ToString(), out Id);
            try
            {
                var search = context.EmployeeTables.Find(Id);
                employeeName.Text = search.Name;
                employeeFatherName.Text = search.FatherName;
            }
            catch (Exception)
            {
                this.ShowMessageAsync("Not Found", "The Id doesn't exisits");
            }
        }
        private void DepositButtonClick(object sender, RoutedEventArgs e)
        {
            int Id;
            int.TryParse(employeeId.Text.ToString(), out Id);

            try
            {
                PaymentTable newPayment = new PaymentTable();

                newPayment.EmployeeId = Id;
                newPayment.Month = GetFeeMonth();
                newPayment.Amount = int.Parse(Paid.Text.ToString());
                newPayment.PaidOn = DateTime.ParseExact(DepositDate.Text.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                context.PaymentTables.Add(newPayment);
                context.SaveChanges();
                clearAll();
                Load();
            }
            catch (Exception err)
            {
                this.ShowMessageAsync("Error", err.ToString());
            }
        }
        public void clearAll()
        {
            employeeFatherName.Clear();
            employeeName.Clear();
            Paid.Clear();
            employeeId.Clear();
        }
        private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex Numeric = new Regex("[^0-9]+"); e.Handled = Numeric.IsMatch(e.Text);
        }
        private void AlphabetOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex Alphabet = new Regex("[^A-Za-z ]+$"); e.Handled = Alphabet.IsMatch(e.Text);
        }
        private void AlphaNumeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex CharAndDigit = new Regex("[^0-9A-Za-z ]+$"); e.Handled = CharAndDigit.IsMatch(e.Text);
        }
        //===================================History Section============================================
        private void radioButtonChecked(object sender, RoutedEventArgs e)
        {
            if (yearRadionButton.IsChecked == true)
            {
                historyMonth.Visibility = Visibility.Hidden;
                historyYear.Visibility = Visibility.Visible;
            }
            else
            {
                historyMonth.Visibility = Visibility.Visible;
                historyYear.Visibility = Visibility.Visible;
            }
        }
        private void searchHistoryClick(object sender, RoutedEventArgs e)
        {
            if (yearRadionButton.IsChecked == true)
            {
                string yeild = "";
                if (historyYear.SelectedItem.ToString().Contains("System.Windows.Controls.ComboBoxItem: "))
                {
                    yeild = historyYear.SelectedItem.ToString().Remove(0, 38);
                }
                int year = int.Parse(yeild);

                var result = context.PaymentHistoryByYear(year).ToList();
                HistoryDataGrid.ItemsSource = result;
            }
            else
            {
                var selected = ((ComboBoxItem)historyMonth.SelectedItem).Tag.ToString();

                string yeild = "";
                if (historyYear.SelectedItem.ToString().Contains("System.Windows.Controls.ComboBoxItem: "))
                {
                    yeild = historyYear.SelectedItem.ToString().Remove(0, 38);
                }

                int hYear = int.Parse(yeild);
                int yearMonth = int.Parse(selected);

                var result = context.PaymentHistoryMY(month: yearMonth, year: hYear).ToList();
                HistoryDataGrid.ItemsSource = result;

            }
        }
    }
}