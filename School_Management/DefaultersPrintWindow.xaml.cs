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
    /// Interaction logic for DefaultersPrintWindow.xaml
    /// </summary>
    public partial class DefaultersPrintWindow 
    {
        int endLimit;
        public DefaultersPrintWindow(int limit)
        {
            InitializeComponent();
            endLimit = limit;
        }

        ReportDocument newRd = new ReportDocument();

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //newRd.Load(@"C:\\Users\Haseeb\Documents\Visual Studio 2015\Projects\School_Management\School_Management\Assets\Reports\DefaultersList.rpt");

            // ****************** FOR PUBLISHING*************************************
            newRd.Load(@"C:\\Program Files (x86)\CognitiveDev\TKS Management System\Reports\DefaultersList.rpt");

            
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
            conn.Open();

            SqlDataAdapter sda = new SqlDataAdapter("DefaultersList", conn);
            sda.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sda.SelectCommand.Parameters.AddWithValue("@Dues", endLimit);

            DataSet ds = new DataSet();
            sda.Fill(ds, "DefaultersTable");
            newRd.SetDataSource(ds);
            DefaulterViewer.ViewerCore.ReportSource = newRd;
        }
    }
}
