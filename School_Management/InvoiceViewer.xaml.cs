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
using CrystalDecisions.ReportAppServer.DataDefModel;

namespace School_Management
{
    /// <summary>
    /// Interaction logic for InvoiceViewer.xaml
    /// </summary>
    public partial class InvoiceViewer
    {
        int year;
        int month;
        string run;

        public InvoiceViewer(int y, int m, string which)
        {
            year = y;
            month = m;
            run = which;
            InitializeComponent();
        }

        ReportDocument rd = new ReportDocument();

        public void yearlyReport()
        {
            //rd.Load(@"C:\\Users\Haseeb\Documents\Visual Studio 2015\Projects\School_Management\School_Management\Assets\Reports\Transaction.rpt");

            // ****************** FOR PUBLISHING*************************************
            rd.Load(@"C:\\Program Files (x86)\CognitiveDev\TKS Management System\Reports\Transaction.rpt");

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
            conn.Open();

            SqlDataAdapter sda = new SqlDataAdapter("GetSlipDataByYear", conn);
            sda.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sda.SelectCommand.Parameters.AddWithValue("@Year", year);

            System.Data.DataSet ds = new System.Data.DataSet();
            sda.Fill(ds, "Invoice");
            rd.SetDataSource(ds);
            InvoiceReport.ViewerCore.ReportSource = rd;
        }
        public void monthlyReport()
        {
            rd.Load(@"C:\\Program Files (x86)\CognitiveDev\TKS Management System\Reports\Transaction.rpt");

            // ****************** FOR PUBLISHING*************************************
            //rd.Load(@"D:\\New School Management System\TKS\Reports\Transaction.rpt");

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
            conn.Open();

            SqlDataAdapter sda = new SqlDataAdapter("GetSlipDataByYM", conn);
            sda.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sda.SelectCommand.Parameters.AddWithValue("@Year", year);
            sda.SelectCommand.Parameters.AddWithValue("@Month", month);

            System.Data.DataSet ds = new System.Data.DataSet();
            sda.Fill(ds, "Invoice");
            rd.SetDataSource(ds);
            InvoiceReport.ViewerCore.ReportSource = rd;
        }
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(run == "Year")
            {
                yearlyReport();
            }
            else
            {
                monthlyReport();
            }
        }
    }
}
