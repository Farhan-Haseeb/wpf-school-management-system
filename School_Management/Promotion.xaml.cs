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
using System.Windows.Media.Animation;

namespace School_Management
{
    /// <summary>
    /// Interaction logic for Promotion.xaml
    /// </summary>
    public partial class Promotion
    {
        public Promotion()
        {
            InitializeComponent();
        }

        // *******************************************************

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
        private void GradeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void ReloadButtonClick(object sender, RoutedEventArgs e)
        {
            if (searchGrid.Height != 0)
            {
                DoubleAnimation animation = new DoubleAnimation();

                animation.From = searchGrid.ActualHeight;
                animation.To = 0;
                animation.Duration = (Duration)TimeSpan.FromSeconds(0.3);
                searchGrid.BeginAnimation(HeightProperty, animation);
            }
            sId.IsChecked = false;
            sName.IsChecked = false;
            fName.IsChecked = false;
            emergencyNumber.IsChecked = false;

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
            else if (gradeRadioButton.IsChecked == true)
            {
                string grade = ((ComboBoxItem)Grade.SelectedItem).Tag.ToString();
                var result = context.SearchStudentInfoByGrade(active: "True", grade: grade).ToList();
                StudentSearchGrid.ItemsSource = result;
            }
            else
            {
                onLoad();
            }
            toggleAnimate();
        }

        private void withdrawAllButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string promoteGrade = ((ComboBoxItem)PromoteGrade.SelectedItem).Tag.ToString();
                if (sId.IsChecked == true)
                {
                    var selecteditem = new List<SearchStudentInfoAllByIdResult>();

                    foreach (SearchStudentInfoAllByIdResult item in StudentSearchGrid.ItemsSource)
                    {
                        int promoteId = item.AdmissionNumber;
                        context.PromoteStudents(admissionNumber: promoteId, grade: promoteGrade);
                    }
                    this.ShowMessageAsync("Done", "Operation succeded");
                    onLoad();
                }
                else if (sName.IsChecked == true)
                {
                    var selecteditem = new List<SearchStudentInfoAllByNameResult>();

                    foreach (SearchStudentInfoAllByNameResult item in StudentSearchGrid.ItemsSource)
                    {
                        int promoteId = item.AdmissionNumber;
                        context.PromoteStudents(admissionNumber: promoteId, grade: promoteGrade);
                    }
                    this.ShowMessageAsync("Done", "Operation succeded");
                    onLoad();
                }
                else if (fName.IsChecked == true)
                {
                    var selecteditem = new List<SearchStudentInfoAllByFatherNameResult>();

                    foreach (SearchStudentInfoAllByFatherNameResult item in StudentSearchGrid.ItemsSource)
                    {
                        int promoteId = item.AdmissionNumber;
                        context.PromoteStudents(admissionNumber: promoteId, grade: promoteGrade);
                    }
                    this.ShowMessageAsync("Done", "Operation succeded");
                    onLoad();
                }
                else if (emergencyNumber.IsChecked == true)
                {
                    var selecteditem = new List<SearchStudentInfoAllByPrimaryResult>();

                    foreach (SearchStudentInfoAllByPrimaryResult item in StudentSearchGrid.ItemsSource)
                    {
                        int promoteId = item.AdmissionNumber;
                        context.PromoteStudents(admissionNumber: promoteId, grade: promoteGrade);
                    }
                    this.ShowMessageAsync("Done", "Operation succeded");
                    onLoad();
                }
                else if (gradeRadioButton.IsChecked == true)
                {
                    var selecteditem = new List<SearchStudentInfoByGradeResult>();

                    foreach (SearchStudentInfoByGradeResult item in StudentSearchGrid.ItemsSource)
                    {
                        int promoteId = item.AdmissionNumber;
                        context.PromoteStudents(admissionNumber: promoteId, grade: promoteGrade);

                    }
                    this.ShowMessageAsync("Done", "Operation succeded");
                    onLoad();
                }
                else if (emergencyNumber.IsChecked == false && fName.IsChecked == false && sId.IsChecked == false && sName.IsChecked == false && gradeRadioButton.IsChecked == true)
                {
                    var selecteditem = new List<SearchStudentInfoAllResult>();

                    foreach (SearchStudentInfoAllResult item in StudentSearchGrid.ItemsSource)
                    {
                        int promoteId = item.AdmissionNumber;
                        context.PromoteStudents(admissionNumber: promoteId, grade: promoteGrade);

                    }
                    this.ShowMessageAsync("Done", "Operation succeded");
                    onLoad();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Error, Do not select while Pressing Pomote All");
            }
        }
        private void withdrawButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string promoteGrade = ((ComboBoxItem)PromoteGrade.SelectedItem).Tag.ToString();
                if (sId.IsChecked == true)
                {
                    var selecteditem = new List<SearchStudentInfoAllByIdResult>();

                    foreach (SearchStudentInfoAllByIdResult item in StudentSearchGrid.ItemsSource)
                    {
                        if (((CheckBox)SelectedStudent.GetCellContent(item)).IsChecked == true)
                        {
                            int promoteId = item.AdmissionNumber;
                            context.PromoteStudents(admissionNumber: promoteId, grade: promoteGrade);
                        }
                    }
                    this.ShowMessageAsync("Done", "Operation succeded");
                    onLoad();
                }
                else if (sName.IsChecked == true)
                {
                    var selecteditem = new List<SearchStudentInfoAllByNameResult>();

                    foreach (SearchStudentInfoAllByNameResult item in StudentSearchGrid.ItemsSource)
                    {
                        if (((CheckBox)SelectedStudent.GetCellContent(item)).IsChecked == true)
                        {
                            int promoteId = item.AdmissionNumber;
                            context.PromoteStudents(admissionNumber: promoteId, grade: promoteGrade);
                        }
                    }
                    this.ShowMessageAsync("Done", "Operation succeded");
                    onLoad();
                }
                else if (fName.IsChecked == true)
                {
                    var selecteditem = new List<SearchStudentInfoAllByFatherNameResult>();

                    foreach (SearchStudentInfoAllByFatherNameResult item in StudentSearchGrid.ItemsSource)
                    {
                        if (((CheckBox)SelectedStudent.GetCellContent(item)).IsChecked == true)
                        {
                            int promoteId = item.AdmissionNumber;
                            context.PromoteStudents(admissionNumber: promoteId, grade: promoteGrade);
                        }
                    }
                    this.ShowMessageAsync("Done", "Operation succeded");
                    onLoad();
                }
                else if (emergencyNumber.IsChecked == true)
                {
                    var selecteditem = new List<SearchStudentInfoAllByPrimaryResult>();

                    foreach (SearchStudentInfoAllByPrimaryResult item in StudentSearchGrid.ItemsSource)
                    {
                        if (((CheckBox)SelectedStudent.GetCellContent(item)).IsChecked == true)
                        {
                            int promoteId = item.AdmissionNumber;
                            context.PromoteStudents(admissionNumber: promoteId, grade: promoteGrade);
                        }
                    }
                    this.ShowMessageAsync("Done", "Operation succeded");
                    onLoad();
                }
                else if (gradeRadioButton.IsChecked == true)
                {
                    var selecteditem = new List<SearchStudentInfoByGradeResult>();

                    foreach (SearchStudentInfoByGradeResult item in StudentSearchGrid.ItemsSource)
                    {
                        if (((CheckBox)SelectedStudent.GetCellContent(item)).IsChecked == true)
                        {
                            int promoteId = item.AdmissionNumber;
                            context.PromoteStudents(admissionNumber: promoteId, grade: promoteGrade);
                        }
                    }

                    this.ShowMessageAsync("Done", "Operation succeded");
                    onLoad();
                }
                else if (emergencyNumber.IsChecked == false && fName.IsChecked == false && sId.IsChecked == false && sName.IsChecked == false && gradeRadioButton.IsChecked == true)
                {
                    var selecteditem = new List<SearchStudentInfoAllResult>();

                    foreach (SearchStudentInfoAllResult item in StudentSearchGrid.ItemsSource)
                    {
                        if (((CheckBox)SelectedStudent.GetCellContent(item)).IsChecked == true)
                        {
                            int promoteId = item.AdmissionNumber;
                            context.PromoteStudents(admissionNumber: promoteId, grade: promoteGrade);
                        }
                    }
                    this.ShowMessageAsync("Done", "Operation succeded");
                    onLoad();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
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

        private void stdIdChecked(object sender, RoutedEventArgs e)
        {
            makeVisible(emergency, fatherName, stdName, Grade, stdId);
        }
        private void stdNameChecked(object sender, RoutedEventArgs e)
        {
            makeVisible(stdId, emergency, fatherName, Grade, stdName);
        }
        private void fatherNameChecked(object sender, RoutedEventArgs e)
        {
            makeVisible(stdId, stdName, emergency, Grade, fatherName);
        }
        private void emergencyChecked(object sender, RoutedEventArgs e)
        {
            makeVisible(stdId, stdName, fatherName, Grade, emergency);
        }
        
        public void makeVisible(TextBox h1, TextBox h2, TextBox h3, ComboBox h4, TextBox beVisible)
        {
            h1.Visibility = Visibility.Hidden;
            h1.Clear();

            h2.Visibility = Visibility.Hidden;
            h2.Clear();

            h3.Visibility = Visibility.Hidden;
            h3.Clear();

            h4.Visibility = Visibility.Hidden;

            beVisible.Visibility = Visibility.Visible;
        }

        public void forCombo(TextBox h1, TextBox h2, TextBox h3, TextBox h4, ComboBox beVisible)
        {
            h1.Visibility = Visibility.Hidden;
            h1.Clear();

            h2.Visibility = Visibility.Hidden;
            h2.Clear();

            h3.Visibility = Visibility.Hidden;
            h3.Clear();

            h4.Visibility = Visibility.Hidden;

            beVisible.Visibility = Visibility.Visible;
        }
        private void gradeRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            forCombo(stdId, stdName, fatherName, emergency, Grade);
        }
        private void toggleAnimate()
        {
            if (searchGrid.Height == 0)
            {
                DoubleAnimation animation = new DoubleAnimation();

                animation.From = searchGrid.ActualHeight;
                animation.To = 345;
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
        private void SearchShowMouseDown(object sender, MouseButtonEventArgs e)
        {
            toggleAnimate();
        }
    }
}