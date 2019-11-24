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
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using MahApps.Metro.Controls.Dialogs;
using System.Data.SqlClient;
using System.Globalization;
using CrystalDecisions.CrystalReports.Engine;
using FontAwesomeWPF;

namespace School_Management
{
    /// <summary>
    /// Interaction logic for DepositFee.xaml
    /// </summary>
    public partial class DepositFee
    {
        SchoolEntities context = new SchoolEntities();
        int Id;
        public DepositFee()
        {
            InitializeComponent();
        }
        private void DeopsitLoaded(object sender, RoutedEventArgs e)
        {

            CurrentYear();
            nextInvoice();
            DepositDate.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
            loadData();
            printButton.IsEnabled = true;
            plusIcon.Content = Fa.Plus;
            minusIcon.Content = Fa.Minus;
        }
        public void loadData()
        {
            var result = context.GetSlipData().ToList();
            slipGrid.ItemsSource = result;
            deleteGrid.ItemsSource = result;
        }
        public void CurrentYear()
        {
            DateTime a = DateTime.Now;

            for (int i = 2005; i <= a.Year + 1; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = i;
                Year.Items.Add(item);
            }

            Year.SelectedIndex = Year.Items.Count - 2;
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
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
            conn.Open();

            int.TryParse(AdmissionNumber.Text.ToString(), out Id);

            string checkNow = check(conn, Id);

            if (checkNow == "True")
            {
                this.ShowMessageAsync("Paid", "Student already paid for the month");
            }
            else
            {
                process(conn, Id);
            }
        }
        public string check(SqlConnection conn, int d)
        {
            SqlCommand cmd = new SqlCommand("CheckIfPaid", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AdmissionNumber", d);
            cmd.Parameters.AddWithValue("@FeeMonth", GetFeeMonth());
            cmd.ExecuteNonQuery();
            using (var rd = cmd.ExecuteReader())
            {
                rd.Read();
                string feePaid = rd["Paid"].ToString();
                return feePaid;
            }
        }
        public void process(SqlConnection conn, int d)
        {
            var FM = GetFeeMonth();

            using (SqlCommand command = new SqlCommand("StudentFeeData", conn))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@AdmissionNumber", d);
                command.Parameters.AddWithValue("@FeeMonth", GetFeeMonth());

                using (SqlDataAdapter sda = new SqlDataAdapter(command))
                {
                    System.Data.DataTable dt = new System.Data.DataTable();

                    sda.Fill(dt);
                    FeeDataGrid.ItemsSource = dt.DefaultView;
                }
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    try {
                        StdName.Text = reader["Name"].ToString();
                        FatheName.Text = reader["FatherName"].ToString();
                        Grade.Text = reader["Grade"].ToString();
                        TotalFee.Text = reader["TotalFee"].ToString();

                        var c = int.Parse(reader["CalulatedTotal"].ToString());
                        int ca = 0;

                        if (reader["TotalDiscount"] != DBNull.Value)
                        {
                            ca = int.Parse(reader["TotalDiscount"].ToString());
                        }
                        else
                        {
                            ca = 0;
                        }

                        int pd;
                        if (reader["PreviousDues"] != DBNull.Value)
                        {
                            PreviousDues.Text = reader["PreviousDues"].ToString();
                            pd = int.Parse(reader["PreviousDues"].ToString());
                        }
                        else
                        {
                            pd = 0;
                            PreviousDues.Text = "0";
                        }
                        Calculated.Text = (c - ca + pd).ToString();
                    }
                    catch (System.InvalidOperationException)
                    {
                        this.ShowMessageAsync("Not Found", "This is doesn't exist or doesn't have Fee assigned to it.");
                    }
                }
            }
        }
        private void PaidLostFocus(object sender, RoutedEventArgs e)
        {
            if (Paid.Text == "")
            {
                this.ShowMessageAsync("Empty", "Paid can't be empty");
            }
            else
            {
                int deposit = 0;
                int.TryParse(Paid.Text.ToString(), out deposit);

                int cal = int.Parse(Calculated.Text.ToString());

                Dues.Text = (cal - deposit).ToString();
            }
        }
        private void DepositButtonClick(object sender, RoutedEventArgs e)
        {
            try {
                InvoiceTable newDeposit = new InvoiceTable();

                newDeposit.AdmissionNumber = Id;
                newDeposit.PayingMonth = GetFeeMonth();
                newDeposit.TotalFeeOfMonth = int.Parse(Calculated.Text.ToString());
                newDeposit.Paid = int.Parse(Paid.Text.ToString());
                newDeposit.Dues = int.Parse(Dues.Text.ToString());

                if (DepositDate.Text.Contains("/"))
                {
                    newDeposit.InvoiceDate = DateTime.ParseExact(DepositDate.Text.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                else
                {
                    newDeposit.InvoiceDate = DateTime.ParseExact(DepositDate.Text.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }

                context.InvoiceTables.Add(newDeposit);
                context.SaveChanges();

                context.FeePaid(admissionNumber: Id, feeMonth: GetFeeMonth());

                nextInvoice();
                loadData();
                clearAll();
                this.ShowMessageAsync("Done", "Fee Deposit");
            }
            catch (FormatException)
            {
                if (AdmissionNumber.Text == "")
                {
                    this.ShowMessageAsync("Obstructed", "Couldn't calculate the students fee of the month or the Admission ID is missing");
                    AdmissionNumber.Focus();
                }
                else if (Paid.Text == "")
                {
                    Paid.Focus();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        public void clearAll()
        {
            AdmissionNumber.Clear();
            StdName.Clear();
            FatheName.Clear();
            Grade.Clear();
            TotalFee.Clear();
            PreviousDues.Clear();
            Calculated.Clear();
            Paid.Clear();
            Dues.Clear();

        }
        private void printButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                depositSlipViewer dsv = new depositSlipViewer(Id, GetFeeMonth());
                dsv.Show();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }

         
            slipId.Clear();
            loadData();
        }

        public void nextInvoice()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
            conn.Open();

            string query = "SELECT IDENT_CURRENT('InvoiceTable')+1 AS InvoiceId";
            var command = new SqlCommand(query, conn);
        
            using (var Reader = command.ExecuteReader())
            {
                Reader.Read();
                invoiceId.Text = Reader["InvoiceId"].ToString();
            }
        }
        // ===========================================DELETE DEPOSTI=============================

        private void deleteButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                int id = int.Parse(delId.Text.ToString());
            
            
            var delObj = context.InvoiceTables.Find(id);

            context.InvoiceTables.Remove(delObj);
            context.SaveChanges();

            int undoID = delObj.AdmissionNumber;
            string undoMonth = delObj.PayingMonth;

            context.UndoFeePaid(admissionNumber: undoID, feeMonth: undoMonth);

            loadData();
            delId.Clear();
            }
            catch (FormatException)
            {
                this.ShowMessageAsync("Not Valid", "Please enter a vaild entry.");
            }
        }
        // ========================================PRINT SLIP========================================
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

        private void LabelMouseEnter(object sender, MouseEventArgs e)
        {
            Label lab = sender as Label;

            lab.FontSize = 16.0;
            var bd = new BrushConverter();
            lab.Background = (System.Windows.Media.Brush)bd.ConvertFrom("#4CFBFBFB");
        }

        private void LabelMouseLeave(object sender, MouseEventArgs e)
        {
            Label lab = sender as Label;
            lab.FontSize = 14.0;
            var bd = new BrushConverter();
            lab.Background = System.Windows.Media.Brushes.Transparent;
        }

        private void feeLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            menu.SelectedIndex = 0;
        }

        private void deleteLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            menu.SelectedIndex = 1;
        }
        private void printLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            menu.SelectedIndex = 2;
        }
        
    }
}