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
    /// Interaction logic for simpleViewer.xaml
    /// </summary>
    public partial class simpleViewer
    {
        string signal = null;

        public simpleViewer(string s)
        {
            InitializeComponent();
            signal = s;
        }

        ReportDocument newRd = new ReportDocument();

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (signal == "From Employee")
                {
                    employeeReport();
                }
                else if (signal == "From Strength")
                {
                    strengthReport();
                }
                else if (signal == "From SCW")
                {
                    studentClassWiseReport();
                }
                else if (signal == "From Stock")
                {
                    stockReport();
                }
            }
            catch(Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }
        private void employeeReport()
        {
            newRd.Load(@"C:\\Program Files (x86)\CognitiveDev\TKS Management System\Reports\EmployeeReport.rpt");

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
            conn.Open();

            string query = "SELECT * FROM EmployeeTable;";
            SqlDataAdapter sda = new SqlDataAdapter(query, conn);
            sda.SelectCommand.CommandType = System.Data.CommandType.Text;

            DataSet ds = new DataSet();
            sda.Fill(ds, "EmployeeTable");
            newRd.SetDataSource(ds);
            GenerateView.ViewerCore.ReportSource = newRd;
        }

        private void strengthReport()
        {
            try
            {
                newRd.Load(@"C:\\Program Files (x86)\CognitiveDev\TKS Management System\Reports\Strength.rpt");

                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
                    conn.Open();

                    //SqlDataAdapter sda = new SqlDataAdapter("ClassStrength", conn);
                    //sda.SelectCommand.CommandType = CommandType.StoredProcedure;

                    //DataSet ds = new DataSet();
                    //sda.Fill(ds, "Strength");
                    //newRd.SetDataSource(ds);
                    //GenerateView.ViewerCore.ReportSource = newRd;

                    using (SqlDataAdapter nsda = new SqlDataAdapter("GenderCount", conn))
                    {
                        nsda.SelectCommand.CommandType = CommandType.StoredProcedure;

                        using (DataSet das = new DataSet())
                        {
                            nsda.Fill(das, "GenderData");
                            newRd.SetDataSource(das);
                            GenerateView.ViewerCore.ReportSource = newRd;
                        }
                    }
                }
            }
            catch(Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private void studentClassWiseReport()
        {
            newRd.Load(@"C:\\Program Files (x86)\CognitiveDev\TKS Management System\Reports\StudentClassWise.rpt");

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
            conn.Open();

            SqlDataAdapter sda = new SqlDataAdapter("SearchStudentInfoAll", conn);
            sda.SelectCommand.CommandType = CommandType.StoredProcedure;
            sda.SelectCommand.Parameters.AddWithValue("@Active", "True");
            
            DataSet ds = new DataSet();
            sda.Fill(ds, "StudentTable");
            newRd.SetDataSource(ds);
            GenerateView.ViewerCore.ReportSource = newRd;
        }

        private void stockReport()
        {
            newRd.Load(@"C:\\Program Files (x86)\CognitiveDev\TKS Management System\Reports\Stocks.rpt");

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
            conn.Open();

            SqlDataAdapter sda = new SqlDataAdapter("GetStockTable", conn);
            sda.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;

            DataSet ds = new DataSet();
            sda.Fill(ds, "StockTable");
            newRd.SetDataSource(ds);
            GenerateView.ViewerCore.ReportSource = newRd;
        }
    }
}


