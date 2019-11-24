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

namespace School_Management
{
    /// <summary>
    /// Interaction logic for UpdateORCreateFee.xaml
    /// </summary>
    public partial class UpdateORCreateFee 
    {
        public UpdateORCreateFee()
        {
            InitializeComponent();
        }
        SchoolEntities context = new SchoolEntities();
        int selected;
        int selectedAmount;

        private void createbuttonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var FeeName = feeName.Text.ToString();
                int FeeAmount;
                int.TryParse(amount.Text.ToString(), out FeeAmount);
                int DiscountLimit;
                int.TryParse(discountLimit.Text.ToString(), out DiscountLimit);

                FeeTable Modal = new FeeTable();
                Modal.FeeTitle = FeeName;
                Modal.Amount = FeeAmount;
                Modal.DiscountLimit = DiscountLimit;
                check_for_nulls_then_save(Modal);
            }
            catch(Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }
        
        private void check_for_nulls_then_save(FeeTable m)
        {
            if(feeName.Text == "")
            {
                this.ShowMessageAsync("Error", "Fee Name must be provided");
                feeName.Focus();
            }
            else if (amount.Text == "")
            {
                this.ShowMessageAsync("Error", "Amount must be provided");
                amount.Focus();
            }
            else if (discountLimit.Text == "")
            {
                this.ShowMessageAsync("Error", "Discount Limit must be provided");
                discountLimit.Focus();
            }
            else
            {
                context.FeeTables.Add(m);
                context.SaveChanges();
                feeName.Clear();
                amount.Clear();
                discountLimit.Clear();
                loadData();
            }
        }
        private void updatebuttonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var FeeName = UpdatefeeName.Text.ToString();
                int FeeAmount;
                int.TryParse(updateAmount.Text.ToString(), out FeeAmount);
                int DiscountLimit;
                int.TryParse(updateDiscountLimit.Text.ToString(), out DiscountLimit);

                var Update = context.FeeTables.Find(selected);

                Update.FeeTitle = FeeName;
                Update.Amount = FeeAmount;
                Update.DiscountLimit = DiscountLimit;

                check_for_nulls_then_update(Update);
            }
            catch(Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private void check_for_nulls_then_update(FeeTable u)
        {
            if (UpdatefeeName.Text == "")
            {
                this.ShowMessageAsync("Error", "Fee Name must be provided");
                UpdatefeeName.Focus();
            }
            else if (updateAmount.Text == "")
            {
                this.ShowMessageAsync("Error", "Amount must be provided");
                updateAmount.Focus();
            }
            else if (updateDiscountLimit.Text == "")
            {
                this.ShowMessageAsync("Error", "Discount Limit must be provided");
                updateDiscountLimit.Focus();
            }
            else
            {
                context.Entry(u).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                UpdatefeeName.Clear();
                updateAmount.Clear();
                updateDiscountLimit.Clear();
                loadData();
            }
        }
        
        private void FeeNameSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var feeName = context.GetFeeTable();
            var selectedName = FeeName.SelectedItem.ToString();

            foreach (var fee in feeName)
            {
                if (selectedName == fee.FeeTitle)
                {
                    selected = fee.FeeID;
                    selectedAmount = fee.Amount;

                    Console.WriteLine(FeeName.SelectedItem.ToString());
                    Console.WriteLine(selected + " " + selectedName);
                }
            }
            UpdatefeeName.Text = selectedName.ToString();
            updateAmount.Text = selectedAmount.ToString();
        }

        public void onLoad()
        {
            loadData();
            var feeName = context.GetFeeTable();
            foreach (var fee in feeName)
            {    
                FeeName.Items.Add(fee.FeeTitle);
                DeleteFeeName.Items.Add(fee.FeeTitle);
            }
        }

        public void loadData()
        {
            var data = context.GetFeeTable().ToList();
            feeGrid.ItemsSource = data;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            onLoad();
        }

        private void deletebuttonClick(object sender, RoutedEventArgs e)
        {
            //context.FeeTables.Find(selected);
            var feeName = context.GetFeeTable();
            var selectedName = DeleteFeeName.SelectedItem.ToString();
           
            int delID = 0;
            foreach (var fee in feeName)
            {
                if (selectedName == fee.FeeTitle)
                {
                    delID = fee.FeeID;
                }
            }
            var del = context.FeeTables.Find(delID);
            try
            {
                context.FeeTables.Remove(del);

                context.SaveChanges();
                DeleteFeeName.SelectedIndex = DeleteFeeName.SelectedIndex - 1;
            }

            catch (ArgumentNullException)
            {
                this.ShowMessageAsync("Already Deleted", "The fee you are trying to delete is already deleted");
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
            }
            loadData();
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
