using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
using CrystalDecisions.CrystalReports.Engine;
using MahApps;

namespace School_Management
{
    /// <summary>
    /// Interaction logic for BCViewer.xaml
    /// </summary>
    public partial class BCViewer
    {
        int id;
        string active;
        string signal;

        public BCViewer(int d, string act, string s)
        {
            InitializeComponent();
            id = d;
            active = act;
            signal = s;
        }

        ReportDocument newRd = new ReportDocument();

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(signal == "SLC")
            {
                slc();
            }
            else
            {
                bc();
            }
        }

        private void bc()
        {
            newRd.Load(@"C:\\Program Files (x86)\CognitiveDev\TKS Management System\Reports\BirthCertificate.rpt");

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
            conn.Open();

            SqlDataAdapter sda = new SqlDataAdapter("SearchStudentInfoAllById", conn);
            sda.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sda.SelectCommand.Parameters.AddWithValue("@AdmissionNumber", id);
            sda.SelectCommand.Parameters.AddWithValue("@Active", active);

            DataSet ds = new DataSet();
            sda.Fill(ds, "StudentTable");
            newRd.SetDataSource(ds);
            DefaulterViewer.ViewerCore.ReportSource = newRd;
        }

        private void slc()
        {
            newRd.Load(@"C:\\Program Files (x86)\CognitiveDev\TKS Management System\Reports\SLC.rpt");

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
            conn.Open();

            SqlDataAdapter sda = new SqlDataAdapter("SearchStudentInfoAllById", conn);
            sda.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sda.SelectCommand.Parameters.AddWithValue("@AdmissionNumber", id);
            sda.SelectCommand.Parameters.AddWithValue("@Active", active);

            DataSet ds = new DataSet();
            sda.Fill(ds, "SLC");
            newRd.SetDataSource(ds);
            DefaulterViewer.ViewerCore.ReportSource = newRd;
        }
    }
}
