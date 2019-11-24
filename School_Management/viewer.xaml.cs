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

namespace School_Management
{
    /// <summary>
    /// Interaction logic for viewer.xaml
    /// </summary>
    public partial class viewer
    {
        string ofStatus;
        string ofDate;
        public viewer(string status, string feeMonth)
        {
            InitializeComponent();
            ofStatus = status;
            ofDate = feeMonth;
        }

        ReportDocument rd = new ReportDocument();

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //rd.Load(@"C:\\Users\Haseeb\Documents\Visual Studio 2015\Projects\School_Management\School_Management\Assets\Reports\pendingList.rpt");

            // ****************** FOR PUBLISHING*************************************
            rd.Load(@"C:\\Program Files (x86)\CognitiveDev\TKS Management System\Reports\pendingList.rpt");
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
            conn.Open();

            SqlDataAdapter sda = new SqlDataAdapter("CheckForStudentsWhoDidnotPay", conn);
            sda.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sda.SelectCommand.Parameters.AddWithValue("@Status", ofStatus);
            sda.SelectCommand.Parameters.AddWithValue("@FeeMonth", ofDate);

            DataSet ds = new DataSet();
            sda.Fill(ds, "pending");
            rd.SetDataSource(ds);
            reportView.ViewerCore.ReportSource = rd;
        }
    }
}
