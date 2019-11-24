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
using System.Data.SqlClient;
using System.Globalization;
using MahApps.Metro.Controls.Dialogs;
using System.Data.Entity.Validation;

namespace School_Management
{
    /// <summary>
    /// Interaction logic for Employee.xaml
    /// </summary>
    public partial class Employee
    {
        public Employee()
        {
            InitializeComponent();
            employeeName.Focus();
        }

        SchoolEntities context = new SchoolEntities();

        int entered;

        private void ResetOnLostFocus(object sender, RoutedEventArgs e)
        {
            if (employeeId.Text == "")
            {
                nextAdmission();
                clearAll();
                updateButton.Visibility = Visibility.Hidden;
                deleteButton.Visibility = Visibility.Hidden;
                saveButton.Visibility = Visibility.Visible;
                employeeName.Focus();
            }
            else if (employeeId.Text != "")
            {
                updateButton.Visibility = Visibility.Visible;
                deleteButton.Visibility = Visibility.Visible;
                saveButton.Visibility = Visibility.Hidden;
                
                Int32.TryParse(employeeId.Text.ToString(), out entered);
                forUpdate(entered);
                employeeName.Focus();
            }
        }
        EmployeeTable search;
        public void forUpdate(int val)
        {
            try
            {
                search = context.EmployeeTables.Find(val);
                if (search.Gender == "Male")
                {
                    MaleRadioButton.IsChecked = true;
                }
                else
                {
                    FemaleRadioButton.IsChecked = true;
                }
                employeeName.Text = search.Name;
                employeeFatherName.Text = search.FatherName;
                CNIC.Text = search.CNIC;
                HomeAddressText.Text = search.Adress;
                primaryContact.Text = search.PrimaryContact;
                secondaryContact.Text = search.SecondaryContact;
                subject.Text = search.Subject;
                joinedOn.Text = search.JoinedOn.ToString("dd/MM/yyyy");
                qualification.Text = search.Qualifications;
                age.Text = search.Age.ToString();
            }
            catch (Exception err)
            {
                this.ShowMessageAsync("Doesn't not Exits!", "The Student does not exits in the database with this Employee ID "
                    + val + " . Please enter a valid Employee ID.");
                Console.WriteLine(err.ToString());
            }
        }
        private void deleteButtonClick(object sender, RoutedEventArgs e)
        {
            try {
                var found = context.EmployeeTables.Find(entered);
                context.EmployeeTables.Remove(found);
                context.SaveChanges();
                clearAll();
                load();
            }
            catch(Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }
        private void updateButtonClick(object sender, RoutedEventArgs e)
        {
            string Gender = null;
            if (MaleRadioButton.IsChecked == true)
            {
                Gender = "Male";
            }
            else
            {
                Gender = "Female";
            }
            DateTime join = DateTime.ParseExact(joinedOn.Text.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

            search.Name = employeeName.Text.ToString();
            search.FatherName = employeeFatherName.Text.ToString();
            search.JoinedOn = join;
            search.Age = int.Parse(age.Text.ToString());

            search.Qualifications = qualification.Text.ToString();
            search.CNIC = CNIC.Text.ToString();

            search.Subject = subject.Text.ToString();
            search.Gender = Gender;

            search.Adress = HomeAddressText.Text.ToString();
            search.PrimaryContact = primaryContact.Text.ToString();
            search.SecondaryContact = secondaryContact.Text.ToString();
            
            try
            {
                //3. Mark entity as modified
                context.Entry(search).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                this.ShowMessageAsync("Employee Updated", "Mr/Miss." + employeeName.Text + " S/O " + employeeFatherName.Text + " updated");

                updateButton.Visibility = Visibility.Hidden;
                saveButton.Visibility = Visibility.Visible;
                clearAll();
                load();
            }
            catch (DbEntityValidationException err)
            {
                this.ShowMessageAsync("Error", err.ToString());

                foreach (var eve in err.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
            }
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            employeeName.Focus();
            joinedOn.Text = DateTime.Now.ToString("dd/MM/yyyy");
            nextAdmission();
            load();
        }

        public void nextAdmission()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
            conn.Open();

            string query = "SELECT IDENT_CURRENT('EmployeeTable')+1 AS Id";
            var command = new SqlCommand(query, conn);

            using (var Reader = command.ExecuteReader())
            {
                Reader.Read();
                employeeId.Text = Reader["Id"].ToString();
            }
        }

        private void saveButtonClick(object sender, RoutedEventArgs e)
        {
            string Gender = null;
            if (MaleRadioButton.IsChecked == true)
            {
                Gender = "Male";
            }
            else
            {
                Gender = "Female";
            }

            DateTime join = DateTime.ParseExact(joinedOn.Text.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            EmployeeTable modal = new EmployeeTable();

            modal.Name = employeeName.Text.ToString();
            modal.FatherName = employeeFatherName.Text.ToString();
            modal.Age = int.Parse(age.Text.ToString());
            modal.Subject = subject.Text.ToString();
            modal.CNIC = CNIC.Text.ToString();
            modal.PrimaryContact = primaryContact.Text.ToString();
            modal.SecondaryContact = secondaryContact.Text.ToString();
            modal.Qualifications = qualification.Text.ToString();
            modal.JoinedOn = join;
            modal.Adress = HomeAddressText.Text.ToString();
            modal.Gender = Gender;
            try
            {
                context.EmployeeTables.Add(modal);
                context.SaveChanges();
            }
            catch (DbEntityValidationException err)
            {
                this.ShowMessageAsync("Error", "A field is empty");
                foreach (var eve in err.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
            }
            this.ShowMessageAsync("Done!", "Employee added");
            clearAll();
            load();
        }
        public void clearAll()
        {
            employeeFatherName.Clear();
            employeeName.Clear();
            age.Clear();
            CNIC.Clear();
            primaryContact.Clear();
            qualification.Clear();
            HomeAddressText.Clear();
            subject.Clear();
            secondaryContact.Clear();
            MaleRadioButton.IsChecked = false;
            FemaleRadioButton.IsChecked = false;
            nextAdmission();
        }

        //=======================================VIEW STAFF ==================================
        public void load()
        {
            var result = context.GetEmployeeTable().ToList();
            EmployeeGrid.ItemsSource = result;
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

        private void printButtonClick(object sender, RoutedEventArgs e)
        {
            simpleViewer sv = new simpleViewer("From Employee");
            sv.Show();
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

        private void viewLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            menu.SelectedIndex = 1;
        }

        private void employeeLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            menu.SelectedIndex = 0;
        }
    }
}
