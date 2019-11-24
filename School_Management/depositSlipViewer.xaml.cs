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
using MahApps.Metro.Controls.Dialogs;
using System.Diagnostics;

namespace School_Management
{
    /// <summary>
    /// Interaction logic for depositSlipViewer.xaml
    /// </summary>
    public partial class depositSlipViewer
    {
        public depositSlipViewer(int Id, string month)
        {
            InitializeComponent();
            ofDate = month;
            ID = Id;
        }
        int ID;
        string ofDate;
        
        ReportDocument rd = new ReportDocument();

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                rd.Load(@"C:\\Program Files (x86)\CognitiveDev\TKS Management System\Reports\SlipSchema.rpt");
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
                conn.Open();

                SqlDataAdapter sda = new SqlDataAdapter("PrintDepositSlip", conn);
                sda.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;

                sda.SelectCommand.Parameters.AddWithValue("@AdmissionNumber", ID);
                sda.SelectCommand.Parameters.AddWithValue("@Month", ofDate);
                sda.SelectCommand.Parameters.AddWithValue("@Status", "Paid");

                DataSet ds = new DataSet();
                sda.Fill(ds, "SlipData");
                rd.SetDataSource(ds);
                depositViewer.ViewerCore.ReportSource = rd;
            }
            catch (Exception err)
            {
                this.ShowMessageAsync("No data", "There is no data to show");
                Debug.WriteLine(err.ToString());
            }
        }
    }
}
