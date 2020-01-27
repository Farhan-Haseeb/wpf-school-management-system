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
using System.Globalization;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;

namespace School_Management
{
    /// <summary>
    /// Interaction logic for AssignFee.xaml
    /// </summary>
    public partial class AssignFee
    {
        public AssignFee()
        {
            InitializeComponent();    
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            onLoad();
        }
        SchoolEntities context = new SchoolEntities();

        //+++++++++++++++++++++++++++++++++++++++ AUTO ASSIGN ++++++++++++++++++++++++++++++++++++++
        int selectedID = 0;
        int selectedAmount = 0;
        public void onLoad()
        {
            DueDateText.Text = DateTime.Now.AddMonths(1).ToString("05/MM/yyyy");
            ClassDueDate.Text = DateTime.Now.AddMonths(1).ToString("05/MM/yyyy");
            CurrentYear(Year);
            CurrentYear(ClassYear);
            feeNameBox(FeeName);
            feeNameBox(ClassFeeName);
        }
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
        public void feeNameBox(ComboBox box)
        {
            var feeName = context.GetFeeTable();
            foreach(var fee in feeName)
            {
                box.Items.Add(fee.FeeTitle);
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
                    selectedID = fee.FeeID;
                    selectedAmount = fee.Amount;
                }
            }
        }
        private void AssignFeeButtonClick(object sender, RoutedEventArgs e)
        {
            autoAssign();
            this.ShowMessageAsync("Done", "Fee asssigned to every Active student in database.");
        }
        public int getFeeAmount()
        {
            int feeAmount;
            if (FeeAmountText.Text != "")
            {
                int.TryParse(FeeAmountText.Text.ToString(), out feeAmount);
            }
            else
            {
                feeAmount = selectedAmount;
            }
            return feeAmount;
        }
        public int getFineAmount()
        {
            int fineAmount;


            if (FineText.Text != "")
            {
                int.TryParse(FineText.Text.ToString(), out fineAmount);
            }
            else
            {
                fineAmount = 0;
            }
            return fineAmount;
        }
        public string getDate()
        {
            string mo = "";
            string ye = "";


            string m = Month.SelectedItem.ToString();
            string y = Year.SelectedItem.ToString();

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
        public void autoAssign()
        {
            DateTime DueDate;
            try
            {
                if (DueDateText.Text.Contains("/"))
                {
                  DueDate = DateTime.ParseExact(DueDateText.Text.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                else
                {
                    DueDate = DateTime.ParseExact(DueDateText.Text.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }

                DateTime AppliedDate = DateTime.Now.Date;

                context.AutoAssignFeeUpdate
                    (
                    feeID: selectedID,
                    feeAmount: getFeeAmount(),
                    fine: getFineAmount(),
                    dueDate: DueDate,
                    appliedON: AppliedDate,
                    feeMonth: getDate()
                    );
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("FeeAssignedTo", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@FeeID", selectedID);
                    cmd.Parameters.AddWithValue("@FeeMonth", getDate());

                    using (var Reader = cmd.ExecuteReader())
                    {
                        Reader.Read();
                        noOfStudents.Content = Reader["AppliedOn"].ToString();
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }
        private void DeleteFeeButtonClick(object sender, RoutedEventArgs e)
        {
            this.ShowMessageAsync("Are you sure!?", 
                "This action auto deletes the Fee of selected name, month and year. Which can't be retrived after. If the fee is being paid it will be erased from invoice and accounts");
            context.AutoDeleteFee(feeID: selectedID, feeMonth: getDate());
        }
        //+++++++++++++++++++++++++++++++++++++++ AUTO ASSIGN ++++++++++++++++++++++++++++++++++++++

        private void GradeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string grade = ((ComboBoxItem)Grade.SelectedItem).Tag.ToString();

            var result = context.SearchStudentInfoByGrade(active: "True", grade: grade).ToList();
            StudentSearchGrid.ItemsSource = result;
        }
        int ClassSelectedID = 0;
        int ClassSelectedAmount = 0;
        
        private void ClassFeeNameSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var classfeeName = context.GetFeeTable();
            var selectedName = ClassFeeName.SelectedItem.ToString();
            foreach (var fee in classfeeName)
            {
                if (selectedName == fee.FeeTitle)
                {
                    ClassSelectedID = fee.FeeID;
                    ClassSelectedAmount = fee.Amount;
                }
            }
        }
        private void ClasssAssignFeeButtonClick(object sender, RoutedEventArgs e)
        {

            try
            {
                string grade = ((ComboBoxItem)Grade.SelectedItem).Tag.ToString();

                DateTime DueDate;
                if (DueDateText.Text.Contains("/"))
                {
                    DueDate = DateTime.ParseExact(ClassDueDate.Text.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                else
                {
                    DueDate = DateTime.ParseExact(ClassDueDate.Text.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
                DateTime AppliedDate = DateTime.Now.Date;

                var selectedItem = new List<SearchStudentInfoByGradeResult>();

                //Console.WriteLine(ClassSelectedID.ToString(), "feeID");
                //Console.WriteLine(ClassGetFeeAmount().ToString(), "ClassGetFeeAmount");
                //Console.WriteLine(ClassgetFineAmount().ToString(), "ClassgetFineAmount");
                //Console.WriteLine(DueDate.ToString(), "DueDate");
                //Console.WriteLine(DueDate.ToString(), "DueDate");
                //Console.WriteLine(AppliedDate.ToString(), "appliedON");
                //Console.WriteLine(ClassGetDate().ToString(), "feeMonth");

                foreach (SearchStudentInfoByGradeResult item in StudentSearchGrid.ItemsSource)
                {
                    //if (((CheckBox)SelectedStudent.GetCellContent(item)).IsChecked == true)
                    if (((CheckBox)SelectedStudent.GetCellContent(item)).IsChecked == true)
                    {
                        Console.WriteLine(ClassSelectedID.ToString() + " feeID");
                        Console.WriteLine(ClassGetFeeAmount().ToString() + " ClassGetFeeAmount");
                        Console.WriteLine(ClassgetFineAmount().ToString() + " ClassgetFineAmount");
                        Console.WriteLine(DueDate.ToString() + " DueDate");
                        Console.WriteLine(item.AdmissionNumber.ToString() + " item.AdmissionNumber");
                        Console.WriteLine(AppliedDate.ToString() + " appliedON");
                        Console.WriteLine(ClassGetDate().ToString() + " feeMonth");

                    }
                }
                
                this.ShowMessageAsync("Done", "Process completed");
            }
            catch (System.InvalidCastException err)
            {
                this.ShowMessageAsync("Selection Error!", "You didn't select any item or the search result doesn't belong to your query / Withdraw is only on ID");
                Debug.WriteLine(err.ToString());
            }
        }

        private void ToAllClasssAssignFeeButtonClick(object sender, RoutedEventArgs e)
        {
            ToAllClassAutoAssign();
        }

        public int ClassGetFeeAmount()
        {
            int classfeeAmount;
            if (ClassFeeAmount.Text != "")
            {
                int.TryParse(ClassFeeAmount.Text.ToString(), out classfeeAmount);
            }
            else
            {
                classfeeAmount = ClassSelectedAmount;
            }
            return classfeeAmount;
        }
        public int ClassgetFineAmount()
        {
            int classfineAmount;


            if (ClassFineText.Text != "")
            {
                int.TryParse(ClassFineText.Text.ToString(), out classfineAmount);
            }
            else
            {
                classfineAmount = 0;
            }
            return classfineAmount;
        }
        public string ClassGetDate()
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
        public void ClassAutoAssign()
        {
            try
            {
                string grade = ((ComboBoxItem)Grade.SelectedItem).Tag.ToString();

                DateTime DueDate;
                if (DueDateText.Text.Contains("/"))
                {
                    DueDate = DateTime.ParseExact(ClassDueDate.Text.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                else
                {
                    DueDate = DateTime.ParseExact(ClassDueDate.Text.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
                DateTime AppliedDate = DateTime.Now.Date;

                var selectedItem = new List<SearchStudentInfoByGradeResult>();

                //Console.WriteLine(ClassSelectedID.ToString(), "feeID");
                //Console.WriteLine(ClassGetFeeAmount().ToString(), "ClassGetFeeAmount");
                //Console.WriteLine(ClassgetFineAmount().ToString(), "ClassgetFineAmount");
                //Console.WriteLine(DueDate.ToString(), "DueDate");
                //Console.WriteLine(DueDate.ToString(), "DueDate");
                //Console.WriteLine(AppliedDate.ToString(), "appliedON");
                //Console.WriteLine(ClassGetDate().ToString(), "feeMonth");

                foreach (SearchStudentInfoByGradeResult item in StudentSearchGrid.ItemsSource)
                {
                    if (((CheckBox)SelectedStudent.GetCellContent(item)).IsChecked == true)
                    {
                        Console.WriteLine(ClassSelectedID.ToString() +  " feeID");
                        Console.WriteLine(ClassGetFeeAmount().ToString()+ " ClassGetFeeAmount");
                        Console.WriteLine(ClassgetFineAmount().ToString()+  " ClassgetFineAmount");
                        Console.WriteLine(DueDate.ToString() + " DueDate");
                        Console.WriteLine(item.AdmissionNumber.ToString()+ " item.AdmissionNumber");
                        Console.WriteLine(AppliedDate.ToString() + " appliedON");
                        Console.WriteLine(ClassGetDate().ToString()+ " feeMonth");


                       context.ClassAssignFeeUpdate
                       (
                       feeID: ClassSelectedID,
                       feeAmount: ClassGetFeeAmount(),
                       fine: ClassgetFineAmount(),
                       dueDate: DueDate,
                       appliedON: AppliedDate,
                       feeMonth: ClassGetDate(),
                       admissionNumber: item.AdmissionNumber
                       );
                    }
                }

                this.ShowMessageAsync("Done", "Fee assigned successfully");
            }
            catch (Exception err)
            {
                this.ShowMessageAsync("Empty", "No grade selected");
                //this.ShowMessageAsync("Error", err.ToString());
            }
        }
        public void ToAllClassAutoAssign()
        {
            try
            {
                string grade = ((ComboBoxItem)Grade.SelectedItem).Tag.ToString();
                
                DateTime DueDate;
                if (DueDateText.Text.Contains("/"))
                {
                    DueDate = DateTime.ParseExact(ClassDueDate.Text.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                else
                {
                    DueDate = DateTime.ParseExact(ClassDueDate.Text.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
                DateTime AppliedDate = DateTime.Now.Date;

                var selectedItem = new List<SearchStudentInfoByGradeResult>();

                foreach (SearchStudentInfoByGradeResult item in StudentSearchGrid.ItemsSource)
                {
                        context.ClassAssignFeeUpdate
                        (
                        feeID: ClassSelectedID,
                        feeAmount: ClassGetFeeAmount(),
                        fine: ClassgetFineAmount(),
                        dueDate: DueDate,
                        appliedON: AppliedDate,
                        feeMonth: ClassGetDate(),
                        admissionNumber: item.AdmissionNumber
                        );
                }
                this.ShowMessageAsync("Done", "Fee assigned successfully");
            }
            catch (NullReferenceException)
            {
                this.ShowMessageAsync("Empty", "No grade selected");
            }
        }
        private void ClassDeleteFeeButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string grade = ((ComboBoxItem)Grade.SelectedItem).Tag.ToString();

                var selectedItem = new List<SearchStudentInfoByGradeResult>();

                foreach (SearchStudentInfoByGradeResult item in StudentSearchGrid.ItemsSource)
                {
                    if (((CheckBox)SelectedStudent.GetCellContent(item)).IsChecked == true)
                    {
                        context.ClassDeleteFee
                        (
                        feeID: selectedID,
                        feeMonth: ClassGetDate(),
                        admissionNumber: item.AdmissionNumber
                        );
                    }
                }
                this.ShowMessageAsync("Are you sure!?",
                    "This action auto deletes the Fee of selected name, month and year. Which can't be retrived after. If the fee is being paid it will be erased from invoice and accounts");
                Grade.SelectedIndex = 0;
            }
            catch (NullReferenceException)
            {
                this.ShowMessageAsync("No selection made", "The grade is not selected");
                Grade.Focus();
            }
        }

        private void FromAllClassDeleteFeeButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string grade = ((ComboBoxItem)Grade.SelectedItem).Tag.ToString();

                var selectedItem = new List<SearchStudentInfoByGradeResult>();

                foreach (SearchStudentInfoByGradeResult item in StudentSearchGrid.ItemsSource)
                {
                    context.ClassDeleteFee
                    (
                    feeID: selectedID,
                    feeMonth: ClassGetDate(),
                    admissionNumber: item.AdmissionNumber
                    );

                }
                this.ShowMessageAsync("Are you sure!?",
                    "This action auto deletes the Fee of selected name, month and year. Which can't be retrived after. If the fee is being paid it will be erased from invoice and accounts");
                Grade.SelectedIndex = 0;
            }
            catch (NullReferenceException)
            {
                this.ShowMessageAsync("Selection error", "The 'GRADE' to which fee is to be deleted is not selected!");
                Grade.Focus();
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
        private void selectAllBoxChecked(object sender, RoutedEventArgs e)
        {
           
        }
        private void selectAllBoxUnchecked(object sender, RoutedEventArgs e)
        {

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

        private void IndividualLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            menu.SelectedIndex = 1;
        }

        private void autoBatchLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            menu.SelectedIndex = 0;
        }
    }
}
