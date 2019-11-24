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
using MahApps.Metro.Controls.Dialogs;

namespace School_Management
{
    /// <summary>
    /// Interaction logic for Family.xaml
    /// </summary>
    public partial class Family
    {
        public Family()
        {
            InitializeComponent();
        }

        SchoolEntities context = new SchoolEntities();

        //++++++++++++++++++++++++++++++++++++++++CREATE FAMILY++++++++++++++++++++++++++++++++++++++++
        private void searchButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sNameRadioButton.IsChecked == true && sName.Text != "")
            {
                var name = sName.Text.ToString();
                var result = context.SearchStudentInfoAllByName(name: name, active: "True").ToList();
                StudentSearchGrid.ItemsSource = result;
                createbutton.IsEnabled = true;
            }
            if (fNameRadioButton.IsChecked == true && fName.Text != "")
            {
                var fname = fName.Text.ToString();
                var result = context.SearchStudentInfoAllByFatherName(fatherName: fname, active: "True").ToList();
                StudentSearchGrid.ItemsSource = result;
                createbutton.IsEnabled = true;
            }
        }

        private void RadioButtonChecked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sNameRadioButton.IsChecked == true)
            {
                sName.Visibility = Visibility.Visible;
                fName.Clear();
                fName.Visibility = Visibility.Hidden;
            }
            if (fNameRadioButton.IsChecked == true)
            {
                sName.Visibility = Visibility.Hidden;
                sName.Clear();
                fName.Visibility = Visibility.Visible;
            }
        }
        public void createFamily()
        {
            FamilyTable modal = new FamilyTable();
            modal.FamilyName = familyName.Text.ToString();
            modal.CreatedOn = DateTime.Now.Date;

            context.FamilyTables.Add(modal);
            context.SaveChanges();
        }
        public int currentIndex()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
            conn.Open();

            string query = "SELECT IDENT_CURRENT('FamilyTable') AS CurrentID";
            SqlCommand command = new SqlCommand(query, conn);

            int fmilyId;

            using (var Reader = command.ExecuteReader())
            {
                Reader.Read();
                var rd = Reader["CurrentID"].ToString();
                int.TryParse(rd, out fmilyId);
            }
            return fmilyId;
        }
        public void clearCreateFields()
        {
            fNameRadioButton.IsChecked = false;
            sNameRadioButton.IsChecked = false;
            sName.Clear();
            fName.Clear();
            familyName.Clear();
            createbutton.IsEnabled = false;
        }
        private void createButtonClick(object sender, RoutedEventArgs e)
        {
            createFamily();
            currentIndex();

            if (sNameRadioButton.IsChecked == true && sName.Text != "")
            {
                var SelectedItem = new List<SearchStudentInfoAllByNameResult>();

                foreach (SearchStudentInfoAllByNameResult item in StudentSearchGrid.ItemsSource)
                {
                    if (((CheckBox)SelectedStudent.GetCellContent(item)).IsChecked == true)
                    {
                        var re = context.AddMembersToFamily(admissionNumber: item.AdmissionNumber, familyID: currentIndex(), date: DateTime.Now.Date);
                    }
                }
                this.ShowMessageAsync("Successful", "New Family Created and Member are added to the family");
            }
            if (fNameRadioButton.IsChecked == true && fName.Text != "")
            {
                var SelectedItem = new List<SearchStudentInfoAllByFatherNameResult>();

                foreach (SearchStudentInfoAllByFatherNameResult item in StudentSearchGrid.ItemsSource)
                {
                    if (((CheckBox)SelectedStudent.GetCellContent(item)).IsChecked == true)
                    {
                        var re = context.AddMembersToFamily(admissionNumber: item.AdmissionNumber, familyID: currentIndex(), date: DateTime.Now.Date);

                    }
                }
                this.ShowMessageAsync("Successful", "New Family Created and Member are added to the family");
            }
        }
        //++++++++++++++++++++++++++++++++++++++++++++++++++++ADD MEMBERS++++++++++++++++++++++++++++++++++++++++++
        private void FamilyWindowActivated(object sender, EventArgs e)
        {
            //onClick();
            //removeItem();
        }
        public void onClick()
        {
            var getFamily = context.GetFamilyTable();
            foreach (var family in getFamily)
            {
                families.Items.Add(family.FamilyName);
            }
        }

        int familyId;

        private void familiesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var getId = context.GetFamilyTable();
            var selected = families.SelectedItem.ToString();
            foreach (var fm in getId)
            {
                if (selected == fm.FamilyName)
                {
                    familyId = fm.FamilyID;
                }
            }
        }
        private void AddRadioButtonChecked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (AddsNameRadioButton.IsChecked == true)
            {
                AddsName.Visibility = Visibility.Visible;
                AddfName.Clear();
                AddfName.Visibility = Visibility.Hidden;
            }
            if (AddfNameRadioButton.IsChecked == true)
            {
                AddsName.Visibility = Visibility.Hidden;
                AddsName.Clear();
                AddfName.Visibility = Visibility.Visible;
            }
        }
        private void queryButtonClick(object sender, RoutedEventArgs e)
        {
            if (AddsNameRadioButton.IsChecked == true && AddsName.Text != "")
            {
                var name = AddsName.Text.ToString();
                var result = context.SearchStudentInfoAllByName(name: name, active: "True").ToList();
                SearchGrid.ItemsSource = result;
                createbutton.IsEnabled = true;
                addbutton.IsEnabled = true;
            }
            if (AddfNameRadioButton.IsChecked == true && AddfName.Text != "")
            {
                var fname = AddfName.Text.ToString();
                var result = context.SearchStudentInfoAllByFatherName(fatherName: fname, active: "True").ToList();
                SearchGrid.ItemsSource = result;
                createbutton.IsEnabled = true;
                addbutton.IsEnabled = true;
            }
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            if (AddsNameRadioButton.IsChecked == true && AddsName.Text != "")
            {
                var AddItem = new List<SearchStudentInfoAllByNameResult>();

                foreach (SearchStudentInfoAllByNameResult item in SearchGrid.ItemsSource)
                {
                    if (((CheckBox)AddStudent.GetCellContent(item)).IsChecked == true)
                    {
                        var re = context.AddMembersToFamily(admissionNumber: item.AdmissionNumber, familyID: familyId, date: DateTime.Now.Date);
                    }
                }
                this.ShowMessageAsync("Successful", "Member added!");
            }
            if (AddfNameRadioButton.IsChecked == true && AddfName.Text != "")
            {
                var AddItem = new List<SearchStudentInfoAllByFatherNameResult>();

                foreach (SearchStudentInfoAllByFatherNameResult item in SearchGrid.ItemsSource)
                {
                    if (((CheckBox)AddStudent.GetCellContent(item)).IsChecked == true)
                    {
                        var re = context.AddMembersToFamily(admissionNumber: item.AdmissionNumber, familyID: familyId, date: DateTime.Now.Date);
                    }
                }
                this.ShowMessageAsync("Successful", "Member added!");
            }
        }

        //++++++++++++++++++++++++++++++++++++++++++++++++++++ VIEW OR REMOVE +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public void removeItem()
        {
            var getFamily = context.GetFamilyTable();
            foreach (var family in getFamily)
            {
                famalia.Items.Add(family.FamilyName);
            }
        }
        int famaliaId;

        private void famaliaSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var getId = context.GetFamilyTable();
            var selected = famalia.SelectedItem.ToString();
            foreach (var fm in getId)
            {
                if (selected == fm.FamilyName)
                {
                    famaliaId = fm.FamilyID;
                }
            }
            var result = context.FamilyInfo(famaliaId).ToList();
            FindGrid.ItemsSource = result;
        }
        private void familyButtonClick(object sender, RoutedEventArgs e)
        {
            var remove = context.DeleteFamily(famaliaId);
            this.ShowMessageAsync("Done!", "Family Deleted");
        }
        private void removeButtonClick(object sender, RoutedEventArgs e)
        {
            var AddItem = new List<FamilyInfoResult>();

            foreach (FamilyInfoResult item in FindGrid.ItemsSource)
            {
                if (((CheckBox)RemoveStudent.GetCellContent(item)).IsChecked == true)
                {
                    var del = context.DeleteMember(item.AdmissionNumber);
                }
            }
            this.ShowMessageAsync("Successful", "Member Deleted!");
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

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            onClick();
            createbutton.IsEnabled = false;
            addbutton.IsEnabled = false;
            removeItem();
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

        private void VRLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            menu.SelectedIndex = 2;
        }

        private void addLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            menu.SelectedIndex = 1;
        }

        private void createLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            menu.SelectedIndex = 0;
        }
    }
}