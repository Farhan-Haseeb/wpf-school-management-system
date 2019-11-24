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
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using MahApps.Metro.Controls.Dialogs;
using System.Diagnostics;

namespace School_Management
{
    /// <summary>
    /// Interaction logic for Stocks.xaml
    /// </summary>
    public partial class Stocks
    {
        StockTable search;
        SchoolEntities context = new SchoolEntities();
        int selectedId;

        public Stocks()
        {
            InitializeComponent();
        }
        private void MetroWindowLoaded(object sender, RoutedEventArgs e)
        {
            nextStock();
            load();
            populate();
        }
        public void populate()
        {
            var stocks = context.GetStockTable();
            foreach(var stock in stocks)
            {
                stocksBox.Items.Add(stock.Name);
            }
        }
        public void load()
        {
            var result = context.GetStockTable().ToList();
            SearchGrid.ItemsSource = result;
        }
        
        public void nextStock()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
            conn.Open();

            string query = "SELECT IDENT_CURRENT('StockTable')+1 AS ID";
            var command = new SqlCommand(query, conn);

            using (var Reader = command.ExecuteReader())
            {
                Reader.Read();
                stockID.Text = Reader["ID"].ToString();
            }
        }

        private void ResetOnLostFocus(object sender, RoutedEventArgs e)
        {
            if (stockID.Text == "")
            {
                nextStock();
                clearAll();
                updateButton.Visibility = Visibility.Hidden;
                deleteButton.Visibility = Visibility.Hidden;
                addButton.Visibility = Visibility.Visible;
                stockTitle.Focus();
            }
            else if (stockID.Text != "")
            {
                updateButton.Visibility = Visibility.Visible;
                deleteButton.Visibility = Visibility.Visible;
                addButton.Visibility = Visibility.Hidden;
                int entered;
                Int32.TryParse(stockID.Text.ToString(), out entered);
                forUpdate(entered);
                stockTitle.Focus();
            }
        }
        public void clearAll()
        {
            nextStock();
            stockTitle.Clear();
            amount.Clear();
            quantity.Clear();
        }
        public void forUpdate(int id)
        {
            try
            {
                search = context.StockTables.Find(id);
                stockID.Text = search.StockId.ToString();
                stockTitle.Text = search.Name;
                quantity.Text = search.Quantity.ToString();
                amount.Text = search.Amount.ToString();
            }
            catch(Exception err)
            {
                Debug.WriteLine(err.ToString());
            }
        }
        private void updateButtonClick(object sender, RoutedEventArgs e)
        {
            int a = int.Parse(amount.Text.ToString());
            int q = int.Parse(quantity.Text.ToString());
            search.Amount = a;
            search.Name = stockTitle.Text.ToString();
            search.Quantity = q;
            search.TotalAmount = (q * a);

            context.Entry(search).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
            this.ShowMessageAsync("Done", "Stock Updated");
            load();
            clearAll();
            nextStock();
            updateButton.Visibility = Visibility.Hidden;
            deleteButton.Visibility = Visibility.Hidden;
            addButton.Visibility = Visibility.Visible;
        }

        private void deleteButtonClick(object sender, RoutedEventArgs e)
        {
            context.StockTables.Remove(search);
            context.SaveChanges();
            this.ShowMessageAsync("Done", "Stock Deleted");
            load();
            clearAll();
            nextStock();
            updateButton.Visibility = Visibility.Hidden;
            deleteButton.Visibility = Visibility.Hidden;
            addButton.Visibility = Visibility.Visible;
        }

        private void addButtonClick(object sender, RoutedEventArgs e)
        {
            int a = int.Parse(amount.Text.ToString());
            int q = int.Parse(quantity.Text.ToString());

            StockTable modal = new StockTable();
            modal.AddedOn = DateTime.Now.Date;
            modal.Amount = a;
            modal.Name = stockTitle.Text.ToString();
            modal.Quantity = q;
            modal.TotalAmount = (a * q);

            context.StockTables.Add(modal);
            context.SaveChanges();
            this.ShowMessageAsync("Done", "New Stock added");
            load();
            clearAll();
        }
        //==========================================VIEW STOCK======================================

        private void StockSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var stocks = context.GetStockTable();

            var choosen = stocksBox.SelectedItem.ToString();
            foreach (var stock in stocks)
            {
                if(stock.Name == choosen)
                {
                    selectedId = stock.StockId;
                }
            }
        }

        private void sellButtonClick(object sender, RoutedEventArgs e)
        {
            try {
                SoldStockTable modal = new SoldStockTable();
                modal.BuyerName = buyerName.Text.ToString();
                modal.SellQuantity = int.Parse(sellQuantity.Text.ToString());
                modal.StockId = selectedId;
                modal.Price = int.Parse(sellPrice.Text.ToString());
                modal.SoldOn = DateTime.Now.Date;

                context.SoldStockTables.Add(modal);
                context.SaveChanges();
                subtract();
                load();
            }
            catch(Exception err)
            {
                Debug.WriteLine(err.ToString());
            }
            this.ShowMessageAsync("Done", "Stock Sold");
            buyerName.Clear();
            sellQuantity.Clear();
            sellPrice.Clear();
            load();
        }

        private void subtract()
        {
            var sub = context.StockTables.Find(selectedId);

            sub.Quantity = (sub.Quantity - int.Parse(sellQuantity.Text.ToString()));
            context.Entry(sub).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
        }

        private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex Numeric = new Regex("[^0-9/]+"); e.Handled = Numeric.IsMatch(e.Text);
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