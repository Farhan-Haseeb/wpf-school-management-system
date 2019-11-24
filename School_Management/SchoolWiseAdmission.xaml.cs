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
    /// Interaction logic for SchoolWiseAdmission.xaml
    /// </summary>
    public partial class SchoolWiseAdmission
    {
        public SchoolWiseAdmission()
        {
            InitializeComponent();
        }
        SchoolEntities context = new SchoolEntities();
        private void searchButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            string schoolName = PreviousSchool.Text.ToString();
            var result = context.SearchStudentSchoolWise(schoolName).ToList();

            StudentSearchGrid.ItemsSource = result;
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
