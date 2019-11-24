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

namespace School_Management
{
    /// <summary>
    /// Interaction logic for Accounts.xaml
    /// </summary>
    public partial class Accounts
    {
        public Accounts()
        {
            InitializeComponent();
            CurrentYear(historyYear);
            CurrentYear(StockYear);
            loadData();
        }
        public void loadData()
        {
            var result = context.GetSlipData().ToList();
            invoiceGrid.ItemsSource = result;
        }
        SchoolEntities context = new SchoolEntities();

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
        //================ Invoice History Section  ============
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

        int hYear;
        int yearMonth;
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

                var result = context.GetSlipDataByYear(year).ToList();
                invoiceGrid.ItemsSource = result;
            }
            else
            {
                var selected = ((ComboBoxItem)historyMonth.SelectedItem).Tag.ToString();

                string yeild = "";
                if (historyYear.SelectedItem.ToString().Contains("System.Windows.Controls.ComboBoxItem: "))
                {
                    yeild = historyYear.SelectedItem.ToString().Remove(0, 38);
                }

                hYear = int.Parse(yeild);
                yearMonth = int.Parse(selected);

                var result = context.GetSlipDataByYM(month: yearMonth, year: hYear).ToList();
                invoiceGrid.ItemsSource = result;

            }
        }
        private void printButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (yearRadionButton.IsChecked == true)
                {
                    InvoiceViewer iv = new InvoiceViewer(hYear, yearMonth, "Year");
                    iv.Show();
                }
                else
                {
                    InvoiceViewer iv = new InvoiceViewer(hYear, yearMonth, "Year Month");
                    iv.Show();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        //================== STOCK SECTION ============================
        private void radioChecked(object sender, RoutedEventArgs e)
        {
            if (StockYearRadionButton.IsChecked == true)
            {
                StockMonth.Visibility = Visibility.Hidden;
                StockYear.Visibility = Visibility.Visible;
            }
            else
            {
                StockMonth.Visibility = Visibility.Visible;
                StockYear.Visibility = Visibility.Visible;
            }
        }

        int stockYear;
        int stockYearMonth;
        private void searchStockHistoryClick(object sender, RoutedEventArgs e)
        {
            if (StockYearRadionButton.IsChecked == true)
            {
                string yeild = "";
                if (StockYear.SelectedItem.ToString().Contains("System.Windows.Controls.ComboBoxItem: "))
                {
                    yeild = StockYear.SelectedItem.ToString().Remove(0, 38);
                }
                stockYear = int.Parse(yeild);

                var result = context.SalesRecordYearly(stockYear).ToList();
                StockGrid.ItemsSource = result;
            }
            else
            {
                var selected = ((ComboBoxItem)StockMonth.SelectedItem).Tag.ToString();

                string yeild = "";
                if (StockYear.SelectedItem.ToString().Contains("System.Windows.Controls.ComboBoxItem: "))
                {
                    yeild = StockYear.SelectedItem.ToString().Remove(0, 38);
                }

                stockYear = int.Parse(yeild);
                stockYearMonth = int.Parse(selected);

                var result = context.SalesRecordMonthly(month: stockYearMonth, year: stockYear).ToList();
                StockGrid.ItemsSource = result;

            }
        }

        private void printSalesButtonClick(object sender, RoutedEventArgs e)
        {
            if (StockYearRadionButton.IsChecked == true)
            {
                StocksViewer sv = new StocksViewer(stockYearMonth, stockYear, "singal");
                sv.Show();
            }
            else
            {
                StocksViewer sv = new StocksViewer(stockYearMonth, stockYear, "dual");
                sv.Show();
            }
        }
        private void printStockButtonClick(object sender, RoutedEventArgs e)
        {
            simpleViewer sv = new simpleViewer("From Stock");
            sv.Show();
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
    }
}