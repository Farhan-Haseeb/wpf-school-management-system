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
using System.Windows.Media.Animation;

namespace School_Management
{
    /// <summary>
    /// Interaction logic for StudentSearch.xaml
    /// </summary>
    public partial class StudentSearch 
    {
        public StudentSearch()
        {
            InitializeComponent();
        }

        SchoolEntities context = new SchoolEntities();

        public void onLoad()
        {
            var result = context.SearchStudentInfoAll(active: "True").ToList();
            StudentSearchGrid.ItemsSource = result;
        }
        private void StudentSearchLoaded(object sender, RoutedEventArgs e)
        {
            onLoad();
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            if (sId.IsChecked == true && stdId.Text != "")
            {
                int id = int.Parse(stdId.Text.ToString());
                var result = context.SearchStudentInfoAllById(admissionNumber: id, active: "True").ToList();
                StudentSearchGrid.ItemsSource = result;
            }
            else if (sName.IsChecked == true && stdName.Text != "")
            {
                var name = stdName.Text.ToString();
                var result = context.SearchStudentInfoAllByName(name: name, active: "True").ToList();
                StudentSearchGrid.ItemsSource = result;
            }
            else if (fName.IsChecked == true && fatherName.Text != "")
            {
                var fname = fatherName.Text.ToString();
                var result = context.SearchStudentInfoAllByFatherName(fatherName: fname, active: "True").ToList();
                StudentSearchGrid.ItemsSource = result;
            }
            else if (emergencyNumber.IsChecked == true && emergency.Text != "")
            {
                var eNum = emergency.Text.ToString();
                var result = context.SearchStudentInfoAllByPrimary(emergency: eNum, active: "True").ToList();
                StudentSearchGrid.ItemsSource = result;
            }
            else
            {
                onLoad();
            }
            if (searchGrid.Height != 0)
            {
                DoubleAnimation animation = new DoubleAnimation();

                animation.From = searchGrid.ActualHeight;
                animation.To = 0;
                animation.Duration = (Duration)TimeSpan.FromSeconds(0.3);
                searchGrid.BeginAnimation(HeightProperty, animation);
            }
        }
        private void stdIdChecked(object sender, RoutedEventArgs e)
        {
            makeVisible(emergency, fatherName, stdName, stdId);
        }
        private void stdNameChecked(object sender, RoutedEventArgs e)
        {
            makeVisible(stdId, emergency, fatherName, stdName);
        }
        private void fatherNameChecked(object sender, RoutedEventArgs e)
        {
            makeVisible(stdId, stdName, emergency, fatherName);
        }
        private void emergencyChecked(object sender, RoutedEventArgs e)
        {
            makeVisible(stdId, stdName, fatherName, emergency);
        }
        public void makeVisible(TextBox h1, TextBox h2, TextBox h3, TextBox beVisible)
        {
            h1.Visibility = Visibility.Hidden;
            h1.Clear();

            h2.Visibility = Visibility.Hidden;
            h2.Clear();

            h3.Visibility = Visibility.Hidden;
            h3.Clear();

            beVisible.Visibility = Visibility.Visible;
        }
        public void clearAll(TextBox h1, RadioButton r1)
        {
            h1.Visibility = Visibility.Hidden;
            r1.IsChecked = false;
            h1.Clear();
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

        private void SearchShowMouseDown(object sender, MouseButtonEventArgs e)
        {
            toggleAnimate();
        }
        private void toggleAnimate()
        {
            if (searchGrid.Height == 0)
            {
                DoubleAnimation animation = new DoubleAnimation();

                animation.From = searchGrid.ActualHeight;
                animation.To = 330;
                animation.Duration = (Duration)TimeSpan.FromSeconds(0.3);
                searchGrid.BeginAnimation(HeightProperty, animation);
            }
            else
            {
                DoubleAnimation animation = new DoubleAnimation();

                animation.From = searchGrid.ActualHeight;
                animation.To = 0;
                animation.Duration = (Duration)TimeSpan.FromSeconds(0.3);
                searchGrid.BeginAnimation(HeightProperty, animation);
            }
        }
    }
}
