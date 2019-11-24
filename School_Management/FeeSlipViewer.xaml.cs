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
    /// Interaction logic for FeeSlipViewer.xaml
    /// </summary>
    public partial class FeeSlipViewer
    {
        public FeeSlipViewer(string g)
        {
            InitializeComponent();
            grade = g;
        }
        string grade;

        ReportDocument rd = new ReportDocument();

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (grade is "" || grade is null)
            {
                rd.Load(@"C:\\Program Files (x86)\CognitiveDev\TKS Management System\Reports\FeeSlip.rpt");

                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
                conn.Open();
                using (SqlDataAdapter sda = new SqlDataAdapter("FeeSlipViewData", conn))
                {
                    sda.SelectCommand.CommandType = CommandType.StoredProcedure;

                    using (DataSet ds = new DataSet())
                    {
                        sda.Fill(ds, "FeeSlip");
                        rd.SetDataSource(ds);
                        reportView.ViewerCore.ReportSource = rd;
                    }
                }
            }
            else
            {
                rd.Load(@"C:\\Program Files (x86)\CognitiveDev\TKS Management System\Reports\FeeSlip.rpt");

                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
                conn.Open();
                using (SqlDataAdapter sda = new SqlDataAdapter("FeeSlipViewDataByGrade", conn))
                {
                    sda.SelectCommand.CommandType = CommandType.StoredProcedure;
                    sda.SelectCommand.Parameters.AddWithValue("@grade", grade);

                    using (DataSet ds = new DataSet())
                    {
                        sda.Fill(ds, "FeeSlip");
                        rd.SetDataSource(ds);
                        reportView.ViewerCore.ReportSource = rd;
                    }
                }
            }
            
        }

        public void loadData(int gotId)
        {
           
            
        }
    }
}
