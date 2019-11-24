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
using CrystalDecisions.CrystalReports.Engine;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using MahApps.Metro.Controls.Dialogs;

namespace School_Management
{
    /// <summary>
    /// Interaction logic for Affidavit.xaml
    /// </summary>
    public partial class Affidavit
    {
        int _admissionNumber;
        string _active;
        public Affidavit(int id, string active)
        {
            _admissionNumber = id;
            _active = active;
            InitializeComponent();
        }
        
        ReportDocument rd = new ReportDocument();

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try {
                if (_admissionNumber != 0)
                {
                    rd.Load(@"C:\\Program Files (x86)\CognitiveDev\TKS Management System\Reports\FilledAffidavit.rpt");
                    SqlConnection conn = new SqlConnection();
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
                    conn.Open();

                    SqlDataAdapter sda = new SqlDataAdapter("SearchStudentInfoAllById", conn);
                    sda.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    sda.SelectCommand.Parameters.AddWithValue("@AdmissionNumber", _admissionNumber);
                    sda.SelectCommand.Parameters.AddWithValue("@Active", _active);

                    DataSet ds = new DataSet();
                    sda.Fill(ds, "StudentTable");
                    rd.SetDataSource(ds);
                    affidavitViewer.ViewerCore.ReportSource = rd;
                }
            else
                {
                    rd.Load(@"C:\\Program Files (x86)\CognitiveDev\TKS Management System\Reports\FilledAffidavit.rpt");
                    SqlConnection conn = new SqlConnection();
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
                    conn.Open();

                    SqlDataAdapter sda = new SqlDataAdapter("SearchStudentInfoAllById", conn);
                    sda.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    sda.SelectCommand.Parameters.AddWithValue("@AdmissionNumber", _admissionNumber);
                    sda.SelectCommand.Parameters.AddWithValue("@Active", _active);

                    DataSet ds = new DataSet();
                    sda.Fill(ds, "StudentTable");
                    rd.SetDataSource(ds);
                    affidavitViewer.ViewerCore.ReportSource = rd;
                }
            }
            catch (Exception err)
            {
                this.ShowMessageAsync("No data", err.ToString());
                Debug.WriteLine(err.ToString());
            }
        }
    }
}
