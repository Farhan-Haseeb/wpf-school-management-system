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
using CrystalDecisions.CrystalReports.Engine;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using MahApps.Metro.Controls.Dialogs;

namespace School_Management
{
    /// <summary>
    /// Interaction logic for Views.xaml
    /// </summary>
    public partial class Views 
    {
        public Views()
        {
            InitializeComponent();
        }
        
        SchoolEntities context = new SchoolEntities();

        public void CurrentYear(ComboBox box)
        {
            DateTime a = DateTime.Now;
            for (int i = 2005; i <= a.Year + 1; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = i;
                box.Items.Add(item);
            }
            box.SelectedIndex = box.Items.Count - 2;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            CurrentYear(ClassYear);
        }

        private void SearchButtonClick(object sender, RoutedEventArgs    e)
        {
            var result = context.CheckForStudentsWhoDidnotPay(status: "Pending", feeMonth: getDate()).ToList();
            StudentSearchGrid.ItemsSource = result;
            SearchGrid.ItemsSource = result;
        }
        public string getDate()
        {
            {
                string mo = "";
                string ye = "";
                
                string m = ClassMonth.SelectedItem.ToString();
                string y = ClassYear.SelectedItem.ToString();
                if (m.Contains("System.Windows.Controls.ComboBoxItem: "))
                {
                    mo = m.Remove(0, 38);
                }
                if (y.Contains("System.Windows.Controls.ComboBoxItem: "))
                {
                    ye = y.Remove(0, 38);
                }
                string FeeMonth;
                return FeeMonth = mo + "-" + ye;
            }
        }
        private void printButtonClick(object sender, RoutedEventArgs e)
        {
            try { 
            viewer viewReport = new viewer("Pending", getDate());
            viewReport.Show();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }

        }
        //===========================================DEFAULTERS LIST================================
        
        private void defaulterSearchClick(object sender, RoutedEventArgs e)
        {
            if(DuesAmount.Text == "")
            {
                var result = context.DefaultersList(0).ToList();
                DefaulterGrid.ItemsSource = result;
            }
            else
            {
                var result = context.DefaultersList(int.Parse(DuesAmount.Text.ToString())).ToList();
                DefaulterGrid.ItemsSource = result;
            }
        }
        private void PrintDefaultersClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DuesAmount.Text == "")
                {
                    DefaultersPrintWindow DPW = new DefaultersPrintWindow(0);
                    DPW.Show();
                }
                else
                {
                    DefaultersPrintWindow DPW = new DefaultersPrintWindow(int.Parse(DuesAmount.Text.ToString()));
                    DPW.Show();
                }

            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }
        //++++++++++++++++++++++++++++++++++++++ VIEW CLASS STRENGTH+++++++++++++++++++++++++++

        private void printStrengthClick(object sender, RoutedEventArgs e)
        {
            try
            {
                simpleViewer sv = new simpleViewer("From Strength");
                sv.Show();
            }
            catch(Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private void printStudentsClassWiseClick(object sender, RoutedEventArgs e)
        {
            try
            {
                simpleViewer sv = new simpleViewer("From SCW");
                sv.Show();
            }
            catch(Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private void LabelMouseEnter(object sender, MouseEventArgs e)
        {
            Label lab = sender as Label;

            lab.FontSize = 16.0;
            var bd = new BrushConverter();
            lab.Background = (Brush)bd.ConvertFrom("#4CFBFBFB");
        }

        private void LabelMouseLeave(object sender, MouseEventArgs e)
        {
            Label lab = sender as Label;
            lab.FontSize = 14.0;
            var bd = new BrushConverter();
            lab.Background = Brushes.Transparent;
        }

        private void pendingFeeLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            menu.SelectedIndex = 0;
        }

        private void defaultersLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            menu.SelectedIndex = 1;
        }
        private void strengthLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            menu.SelectedIndex = 2;
        }

        private void certificateLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            menu.SelectedIndex = 3;
        }

        //++++++++++++++++++++++++++++++++++++++ CERTIFICATES +++++++++++++++++++++++++++

        //++++++++++++++++++++++++++++++++++++++ BIRTH CERTIFICATES +++++++++++++++++++++++++++
        private void PrintBCClick(object sender, RoutedEventArgs e)
        {
            if (admissionNumber.Text == "")
            {
                MessageBox.Show("Admission Number is empty");
                admissionNumber.Focus();
            }
            else
            {
                if (isWithdrawn.IsChecked == true)
                {
                    var id = int.Parse(admissionNumber.Text.ToString());
                    BCViewer bv = new BCViewer(id, "True", "BC");
                    bv.Show();
                }
                else
                {
                    var id = int.Parse(admissionNumber.Text.ToString());
                    BCViewer bv = new BCViewer(id, "False", "BC");
                    bv.Show();
                }
            }
        }

        private void admissionNumberLostFocus(object sender, RoutedEventArgs e)
        {
            if (admissionNumber.Text != "")
            {
                try
                {
                    var an = int.Parse(admissionNumber.Text.ToString());
                    if (isWithdrawn.IsChecked == true)
                    {
                        process(an, "True");
                    }
                    else
                    {
                        process(an, "False");
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                }
            }
            else
            {
                MessageBox.Show("You haven't entered admission number.");
                //admissionNumber.Focus();
            }

        }

        private void process(int id, string active)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
            conn.Open();

            var cmd = new SqlCommand("SearchStudentInfoAllById", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AdmissionNumber", id);
            cmd.Parameters.AddWithValue("@Active", active);

            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                try
                {
                    studentName.Text = reader["Name"].ToString();
                    studentFatherName.Text = reader["FatherName"].ToString();
                    studentDOB.Text = reader["DOB"].ToString();

                }
                catch (System.InvalidOperationException)
                {
                    this.ShowMessageAsync("Not Found", "This is doesn't exist.");
                }
            }
        }

        //++++++++++++++++++++++++++++++++++++++ CHARACTER CERTIFICATES +++++++++++++++++++++++++++
        private void PrintCCClick(object sender, RoutedEventArgs e)
        {
            simpleViewer sv = new simpleViewer("From Character");
            sv.Show();
        }
        
        //++++++++++++++++++++++++++++++++++++++ SCHOOL LEAVING CERTIFICATES +++++++++++++++++++++++++++
        private void SLCadmissionNumberLostFocus(object sender, RoutedEventArgs e)
        {
            if (SLCadmissionNumber.Text != "")
            {
                try
                {
                    var an = int.Parse(SLCadmissionNumber.Text.ToString());
                    SLCprocess(an, "False");
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                }
            }
            else
            {
                MessageBox.Show("You haven't entered admission number.");
                //SLCadmissionNumber.Focus();
            }

        }

        private void SLCprocess(int id, string active)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
            conn.Open();

            var cmd = new SqlCommand("SearchStudentInfoAllById", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AdmissionNumber", id);
            cmd.Parameters.AddWithValue("@Active", active);

            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                try
                {
                    studentNameSLC.Text = reader["Name"].ToString();
                    studentFatherNameSLC.Text = reader["FatherName"].ToString();
                    studentDOBSLC.Text = reader["DOB"].ToString();
                }
                catch (System.InvalidOperationException)
                {
                    this.ShowMessageAsync("Not Found", "This is doesn't exist.");
                }
            }
        }

        private void PrintSLCClick(object sender, RoutedEventArgs e)
        {
            if (SLCadmissionNumber.Text == "")
            {
                MessageBox.Show("Admission Number is empty");
                SLCadmissionNumber.Focus();
            }
            else
            {

                var id = int.Parse(SLCadmissionNumber.Text.ToString());
                SLCTable modal = new SLCTable();
                modal.AdmissionNumber = id;
                modal.DateIssued = DateTime.Now.Date;
                modal.IssuedOnce = "True";
                context.SLCTables.Add(modal);
                context.SaveChanges();

                BCViewer bv = new BCViewer(id, "False", "SLC");
                bv.Show();

            }
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
    