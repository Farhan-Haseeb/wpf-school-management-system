using CrystalDecisions.CrystalReports.Engine;
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
using MahApps;

namespace School_Management
{
    /// <summary>
    /// Interaction logic for StocksViewer.xaml
    /// </summary>
    public partial class StocksViewer
    {
        int _year;
        int _month;
        string _action;
        public StocksViewer(int m, int y, string action)
        {
            _year = y;
            _month = m;
            _action = action;
            InitializeComponent();
        }
        ReportDocument rd = new ReportDocument();

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
          if(_action == "singal")
            {
                singal();
            }
            else
            {
                dual();
            }
        }

        private void singal()
        {
            rd.Load(@"C:\\Program Files (x86)\CognitiveDev\TKS Management System\Reports\StockSales.rpt");
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
            conn.Open();

            SqlDataAdapter sda = new SqlDataAdapter("SalesRecordYearly", conn);
            sda.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sda.SelectCommand.Parameters.AddWithValue("@Year", _year);

            DataSet ds = new DataSet();
            sda.Fill(ds, "SalesData");
            rd.SetDataSource(ds);
            reportView.ViewerCore.ReportSource = rd;
        }
        private void dual()
        {
            rd.Load(@"C:\\Program Files (x86)\CognitiveDev\TKS Management System\Reports\StockSales.rpt");
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
            conn.Open();

            SqlDataAdapter sda = new SqlDataAdapter("SalesRecordMonthly", conn);
            sda.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sda.SelectCommand.Parameters.AddWithValue("@Year", _year);
            sda.SelectCommand.Parameters.AddWithValue("@Month", _month);

            DataSet ds = new DataSet();
            sda.Fill(ds, "SalesData");
            rd.SetDataSource(ds);
            reportView.ViewerCore.ReportSource = rd;

        }
    }
}
