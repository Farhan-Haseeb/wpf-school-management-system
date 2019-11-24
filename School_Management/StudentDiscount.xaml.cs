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
using MahApps.Metro.Controls.Dialogs;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Diagnostics;

namespace School_Management
{
    /// <summary>
    /// Interaction logic for StudentDiscount.xaml
    /// </summary>
    public partial class StudentDiscount
    {

        SchoolEntities context = new SchoolEntities();
        DiscountTable searchDiscount;

        int choose;
        int applyDiscount;
        int serachID;
        int stdId;

        public StudentDiscount()
        {
            InitializeComponent();
        }

        private void MetroWindowLoaded(object sender, RoutedEventArgs e)
        {
            populateFeeBox(FeeName);
            populateFeeBox(ViewFeeName);
            populateDiscount();
            newDiscount();
        }

        public void newDiscount()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
            conn.Open();

            string query = "SELECT IDENT_CURRENT('DiscountTable')+1 AS DiscountId";
            var command = new SqlCommand(query, conn);

            using (var Reader = command.ExecuteReader())
            {
                Reader.Read();
                newDiscountId.Text = Reader["DiscountId"].ToString();
            }
        }


        private void ResetOnLostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (newDiscountId.Text == "")
                {
                    UpdateDiscountButton.Visibility = Visibility.Hidden;
                    CreateDiscountButton.Visibility = Visibility.Visible;
                    newDiscount();
                }
                else
                {
                    UpdateDiscountButton.Visibility = Visibility.Visible;
                    CreateDiscountButton.Visibility = Visibility.Hidden;

                    int.TryParse(newDiscountId.Text.ToString(), out serachID);

                    searchDiscount = context.DiscountTables.Find(serachID);

                    DiscountTitle.Text = searchDiscount.DiscountTitle;
                    DiscountAmount.Text = searchDiscount.DiscountAmount.ToString();
                }
            }
            catch (NullReferenceException err)
            {
                this.ShowMessageAsync("Invalid", "The Id box is empty or the entered Id doesn't exists");
                Debug.WriteLine(err.ToString());

            }
        }

        private void createButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DiscountTable modal = new DiscountTable();

                modal.DiscountAmount = 0;
                modal.DiscountTitle = DiscountTitle.Text.ToString();
                modal.DiscountByPercentage = int.Parse(DiscountPercentage.Text.ToString());
                context.DiscountTables.Add(modal);
                context.SaveChanges();
                this.ShowMessageAsync("Created", "New Discount created successfully!");
                newDiscount();
            }
            catch (FormatException)
            {
                this.ShowMessageAsync("Format Exception", "The fields weren't filled properly");
            }
            clearNow();
        }

        private void updateButtonClick(object sender, RoutedEventArgs e)
        {
            forUpdate(searchDiscount);
            clearNow();
        }

        protected void forUpdate(DiscountTable dt)
        {
            try
            {
                dt.DiscountTitle = DiscountTitle.Text.ToString();
                dt.DiscountAmount = 0;
                dt.DiscountByPercentage = int.Parse(DiscountPercentage.Text.ToString());

                context.Entry(dt).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                this.ShowMessageAsync("Done", "Discount Updated");

                UpdateDiscountButton.Visibility = Visibility.Hidden;
                CreateDiscountButton.Visibility = Visibility.Visible;
            }
            catch (NullReferenceException)
            {
                this.ShowMessageAsync("Invalid", "Some values are invalid");
            }

        }

        public void clearNow()
        {
            DiscountPercentage.Clear();
            DiscountTitle.Clear();
            newDiscount();
        }
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public void populateFeeBox(ComboBox c)
        {
            var feeTable = context.FeeTables;

            foreach (var fee in feeTable)
            {
                c.Items.Add(fee.FeeTitle);
            }
        }
        public void populateDiscount()
        {
            var discountTable = context.DiscountTables;

            foreach (var discount in discountTable)
            {
                giveDiscount.Items.Add(discount.DiscountTitle);
            }
        }

        private void FeeNameSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string now = FeeName.SelectedItem.ToString();
            var feeTable = context.FeeTables;

            foreach (var fee in feeTable)
            {
                if (fee.FeeTitle == now)
                {
                    choose = fee.FeeID;
                    FeeAmount.Text = fee.Amount.ToString();
                }
            }
        }
        private void giveDiscountSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedDiscount = giveDiscount.SelectedItem.ToString();

            var discountTable = context.DiscountTables;

            foreach (var discount in discountTable)
            {
                if (discount.DiscountTitle == selectedDiscount && selectedDiscount != "")
                {
                    applyDiscount = discount.DiscountID;
                    int feeAmount = int.Parse(FeeAmount.Text.ToString());

                    feeAfter.Text = (feeAmount * discount.DiscountByPercentage / 100).ToString();
                }
            }
        }

        private void saveButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var FeeAfter = int.Parse(feeAfter.Text.ToString());
                var id = int.Parse(Id.Text.ToString());

                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
                conn.Open();

                SqlCommand cmd = new SqlCommand("DicountExists", conn);

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AdmissionNumber", id);
                cmd.Parameters.AddWithValue("@FeeID", choose);

                string exist = null;
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    exist = reader["Discount"].ToString();
                }
                if (exist == "True")
                {
                    var c = context.UpdateDiscount
                        (
                        admissionNumber: id,
                        feeID: choose,
                        discountID: applyDiscount
                        );

                    this.ShowMessageAsync("Updated", "Student already existed with the discount on the fee.");
                }
                else
                {
                    StudentDiscountTable modal = new StudentDiscountTable();

                    modal.AdmissionNumber = id;
                    modal.FeeID = choose;
                    modal.DiscountID = applyDiscount;
                    modal.DiscountedFee = FeeAfter;

                    context.StudentDiscountTables.Add(modal);
                    context.SaveChanges();

                    searchDiscount = context.DiscountTables.Find(applyDiscount);
                    searchDiscount.DiscountAmount = FeeAfter;

                    context.Entry(searchDiscount).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();

                    this.ShowMessageAsync("Added", "Student discount added to the fee");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
            clearAll();
        }

        private void SearchOnLostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Id.Text == "")
                {
                   
                }
                else
                {
                    int.TryParse(Id.Text.ToString(), out stdId);
                    var std = context.StudentTables.Find(stdId);

                    StudentNameText.Text = std.Name;
                    FatherNameText.Text = std.FatherName;
                    grade.Text = std.Grade;
                }
            }catch(NullReferenceException err)
            {
                MessageBox.Show("This admission number is not valid");
            }
        }

        public void clearAll()
        {
            Id.Clear();
            FatherNameText.Clear();
            StudentNameText.Clear();
            grade.Clear();
        }
        //+++++++++++++++++++++++++++++++ VIEW DELETE+++++++++++++++++++++++++

        int del;
        private void ViewFeeNameSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int de = 0;
            string now = FeeName.SelectedItem.ToString();
            var feeTable = context.FeeTables;
            foreach (var fee in feeTable)
            {
                if (fee.FeeTitle == now)
                {
                    del = fee.FeeID;
                    de = fee.FeeID;
                    onLoad(de);
                }
            }
        }

        public void onLoad(int d)
        {
            var result = context.StudentDiscountByFee(d).ToList();
            DiscountDataGrid.ItemsSource = result;
        }
        private void removeButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                context.DeleteDiscount(admissionNumber: int.Parse(admissionID.Text.ToString()), feeID: del);
                this.ShowMessageAsync("Deleted", "Student discount deleted on the selected fee");
            }
            catch(Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private void refreshButtonClick(object sender, RoutedEventArgs e)
        {
            var result = context.StudentDiscountAll().ToList();
            DiscountDataGrid.ItemsSource = result;
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
