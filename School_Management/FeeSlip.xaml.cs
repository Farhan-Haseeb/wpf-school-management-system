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

namespace School_Management
{
    /// <summary>
    /// Interaction logic for FeeSlip.xaml
    /// </summary>
    public partial class FeeSlip
    {
        SchoolEntities context = new SchoolEntities();

        public FeeSlip()
        {
            InitializeComponent();
        }
        private void GradeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string grade = ((ComboBoxItem)Grade.SelectedItem).Tag.ToString();

            var result = context.SearchStudentInfoByGrade(active: "True", grade: grade).ToList();
            StudentSearchGrid.ItemsSource = result;
        }

        private void PrintAllSlipButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                FeeSlipViewer fsv = new FeeSlipViewer("");
                fsv.Show();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }
        private void PrintSlipButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string grade = ((ComboBoxItem)Grade.SelectedItem).Tag.ToString();
                FeeSlipViewer FSV = new FeeSlipViewer(grade);
                FSV.Show();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }
    }
}