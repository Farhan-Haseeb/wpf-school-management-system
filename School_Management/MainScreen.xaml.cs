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
using System.Text.RegularExpressions;
using MahApps.Metro.Controls.Dialogs;
using System.Globalization;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using Microsoft.Win32;
using System.Windows.Navigation;
using System.Data;
using System.IO;
using System.Drawing.Imaging;
using FontAwesomeWPF;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace School_Management
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayTime.Content = DateTime.Now.ToLongTimeString();
            DisplayDate.Text = DateTime.Now.ToLongDateString();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += tickevent;
            timer.Start();

            loadTileData();
            icons();
        }

        private void tickevent(object sender, EventArgs e)
        {
            DisplayTime.Content = DateTime.Now.ToLongTimeString();
            DisplayDate.Text = DateTime.Now.ToLongDateString();
        }

        private void icons()
        {
            ClockIcon.Content = Fa.Clock_o;
            CalenderIcon.Content = Fa.Calendar_check_o;

            AdmissionSection.Content = Fa.Graduation_cap + "    Admission Section";
            FeeSection.Content = Fa.Bank + "    Fee Section";
            AccountSection.Content = Fa.Bar_chart + "   Accounts Section";
            StockSection.Content = Fa.Stack_exchange + "    Stock Section";
            EmployeeSection.Content = Fa.Users + "  Employee Section";
            MessageSection.Content = Fa.Send + "   SMS";
            PromptionSection.Content = Fa.Upload + "   Promption";
            ShutdownLabel.Content = Fa.Power_off + "    Close";
            //optionsTitle.Content = Fa.Compass + "   Navigation Panel";
            BackupLabel.Content = Fa.Retweet + "    Backup";
            Family.Content = Fa.Group + "   Family";
            StudentDiscount.Content = Fa.Ban + "    Discount";
            ViewSection.Content = Fa.File_picture_o + "     Views";
            hideIcon.Content = Fa.Indent;
            showIcon.Content = Fa.Outdent;
            hideIcon_Copy.Content = Fa.Indent;
            showIcon_grid.Content = Fa.Outdent;

        }
        private void MainWindowActivated(object sender, EventArgs e)
        {
            loadTileData();
        }
        public void loadTileData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
                    conn.Open();
                    string query = "SELECT COUNT(AdmissionNumber) AS Total FROM StudentTable;";
                    using (SqlCommand cmd1 = new SqlCommand(query, conn))
                    {
                        cmd1.CommandType = System.Data.CommandType.Text;
                        using (var reader = cmd1.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                studentCount.Content = reader["Total"].ToString();
                            }
                        }
                    }
                    string find = "SELECT COUNT(AdmissionNumber) AS CurrentStudent FROM StudentTable WHERE Active = 'True';";
                    using (SqlCommand cmd1 = new SqlCommand(find, conn))
                    {
                        cmd1.CommandType = System.Data.CommandType.Text;
                        using (var reader = cmd1.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                ActiveCount.Content = reader["CurrentStudent"].ToString();
                            }
                        }
                    }
                    string sumAll = "SELECT SUM(Paid) As FullInvoice FROM InvoiceTable WHERE InvoiceDate = @Date;";
                    using (SqlCommand cmd1 = new SqlCommand(sumAll, conn))
                    {
                        cmd1.CommandType = System.Data.CommandType.Text;
                        cmd1.Parameters.AddWithValue("@Date", DateTime.Now.Date);
                        using (var reader = cmd1.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                TransactionSum.Content = reader["FullInvoice"].ToString();
                            }
                        }
                    }
                    string pending = "SELECT COUNT(AdmissionNumber) AS Pending FROM FeeAssociationTable WHERE Status = 'Pending'";
                    using (SqlCommand cmd1 = new SqlCommand(pending, conn))
                    {
                        cmd1.CommandType = System.Data.CommandType.Text;
                        using (var reader = cmd1.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                PendingCount.Content = reader["Pending"].ToString();
                            }
                        }
                    }
                }
            }catch(Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }
        public void LoadIcon()
        {
            ButtonCloseMenu.Content = Fa.Outdent;
            shutdownIcon.Content = Fa.Times_circle;
        }

        private void hideMainClick(object sender, RoutedEventArgs e)
        {
            if (MainMenu.Width.ToString() == "200")
            {
                toggleAnimation(60, TimeSpan.FromSeconds(0.3), MainMenu);
                options.Visibility = Visibility.Collapsed;
                MainMenu.Background = Brushes.Black;
                image.Margin = new Thickness(0, 49, 0, 0);
            }
            else
            {
                toggleAnimation(200, TimeSpan.FromSeconds(0.3), MainMenu);
                options.Visibility = Visibility.Visible;
                var bg = new BrushConverter();
                MainMenu.Background = (Brush)bg.ConvertFrom("#B2FFFFFF");
                image.Margin = new Thickness(20, 49, 20, 0);
            }
        }
        public void toggleAnimation(int newWidth, TimeSpan duration, Grid name)
        {
            DoubleAnimation animation = new DoubleAnimation(newWidth, duration);
            name.BeginAnimation(Grid.WidthProperty, animation);
        }

        public void Toggle(System.Windows.Controls.Label cl, Grid g)
        {
            if (g.Visibility.ToString() == "Hidden")
            {
                g.Visibility = System.Windows.Visibility.Visible;
                cl.FontSize = 16.0;
            }
            else
            {
                g.Visibility = System.Windows.Visibility.Hidden;
                cl.FontSize = 14.0;
            }
        }

        public void hide(Grid g, Label cl)
        {
            if (g.Visibility.ToString() == "Visible")
            {
                g.Visibility = System.Windows.Visibility.Hidden;
                cl.FontSize = 14.0;
            }
        }
        //*********************** MAIN MENU ***********************
        private void AdmissionSectionMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            hide(EmployeeMenu, EmployeeSection);
            hide(FeeMenu, FeeSection);
            Toggle(AdmissionSection, AdmissionMenu);
        }
        private void FeeSectionMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            hide(AdmissionMenu, AdmissionSection);
            hide(EmployeeMenu, EmployeeSection);
            Toggle(FeeSection, FeeMenu);
        }
        private void EmployeeSectionMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            hide(AdmissionMenu, AdmissionSection);
            hide(FeeMenu, FeeSection);
            Toggle(EmployeeSection, EmployeeMenu);
        }

        private void MessageSectionMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SMS sms = new SMS();
            sms.Show();
        }
        //*********************** ADMISSION SECTION***********************
        private void NewAdmissionMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Toggle(AdmissionSection, AdmissionMenu);
            NewAdmission NewAdmission = new NewAdmission();
            NewAdmission.Show();
        }

        private void StudentSearchMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Toggle(AdmissionSection, AdmissionMenu);
            StudentSearch studenSearch = new StudentSearch();
            studenSearch.Show();
        }

        private void WithdrawMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Toggle(AdmissionSection, AdmissionMenu);
            Withdraw withdraw = new Withdraw();
            withdraw.Show();
        }
        private void WithdrawRegisterMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Toggle(AdmissionSection, AdmissionMenu);
            WithdrawRegister withdrawRegister = new WithdrawRegister();
            withdrawRegister.Show();
        }
        private void SchoolWiseAdmissionMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Toggle(AdmissionSection, AdmissionMenu);
            SchoolWiseAdmission SWA = new SchoolWiseAdmission();
            SWA.Show();
        }
        private void FamilyMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Family family = new Family();
            family.Show();
        }
        private void StudentDiscountMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            StudentDiscount studentDiscount = new StudentDiscount();
            studentDiscount.Show();
        }
        //*********************** FEE SECTION***********************

        private void AssignFeeMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Toggle(FeeSection, FeeMenu);
            AssignFee AssignFee = new AssignFee();
            AssignFee.Show();
        }

        private void UpdateCreateMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Toggle(FeeSection, FeeMenu);
            UpdateORCreateFee UOP = new UpdateORCreateFee();
            UOP.Show();
        }

        private void DeopsitFeeMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Toggle(FeeSection, FeeMenu);
            DepositFee Deopsit = new DepositFee();
            Deopsit.Show();

        }

        private void ViewSectionMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Views views = new Views();
            views.Show();
        }
        private void FeeSlipMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Toggle(FeeSection, FeeMenu);
            FeeSlip feeSlip = new FeeSlip();
            feeSlip.Show();
        }
        //*********************** EMPLOYEE SECTION***********************
        private void manageEmployeeMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Toggle(EmployeeSection, EmployeeMenu);
            Employee employee = new Employee();
            employee.Show();
        }

        private void employeePaymentMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Toggle(EmployeeSection, EmployeeMenu);
            Payment payment = new Payment();
            payment.Show();
        }
        //*****************************PROMOTION SECTION****************************
        private void PromptionSectionMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Promotion promotion = new Promotion();
            promotion.Show();
        }
        //*****************************STOCK SECTION****************************
        private void StockSectionMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Stocks stock = new Stocks();
            stock.Show();
        }
        //******************************Tile Mouse Down **************************
        private void ActiveCountMouseDown(object sender, RoutedEventArgs e)
        {
            Withdraw withdraw = new Withdraw();
            withdraw.Show();
        }

        private void studentCountMouseDown(object sender, RoutedEventArgs e)
        {
            StudentSearch ss = new StudentSearch();
            ss.Show();
        }

        private void PendingCountMouseDown(object sender, RoutedEventArgs e)
        {
            Views view = new Views();
            view.Show();
        }

        private void AccountSectionMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Accounts accounts = new Accounts();
            accounts.Show();
        }

        private void CloseMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            close();
        }
        private void BackupMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Backup backup = new Backup();
            backup.Show();
        }

        private void shutdownIconClick(object sender, RoutedEventArgs e)
        {
            close();
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            close();
        }
        protected void close()
        {
            Application.Current.Shutdown();
        }
        private void AnimationMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Label el = sender as Label;
            if (el != null)
            {
                var bg = new BrushConverter();
                el.Background = (Brush)bg.ConvertFrom("#FF6E798B");
                el.Foreground = Brushes.White;
                el.FontSize = 16.0;
            }
        }

        private void AnimationMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Label el = sender as Label;
            if (el != null)
            {
                var bg = new BrushConverter();
                el.Background = Brushes.Transparent;
                el.Foreground = (Brush)bg.ConvertFrom("#FFB6B6B6");
                el.FontSize = 14.0;
            }
        }
        private void MenuAnimationMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Label el = sender as Label;
            if (el != null)
            {
                var bg = new BrushConverter();
                el.Background = (Brush)bg.ConvertFrom("#FF6E798B");
                el.Foreground = Brushes.White;
            }
        }

        private void MenuAnimationMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Label el = sender as Label;
            if (el != null)
            {
                var bg = new BrushConverter();
                el.Background = Brushes.Transparent;
                el.Foreground = (Brush)bg.ConvertFrom("#FFB6B6B6");
            }
        }

            private void hideIcon_GridMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //DoubleAnimation animation = new DoubleAnimation();

            //animation.From = counter_grid.ActualWidth;
            //animation.To = 0;
            //animation.Duration = (Duration)TimeSpan.FromSeconds(0.3);
            //counter_grid.BeginAnimation(WidthProperty, animation);


            //showIcon_grid.Visibility = Visibility.Visible;
        }
        private void showIcon_gridClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //DoubleAnimation animation = new DoubleAnimation();

            //animation.From = counter_grid.ActualWidth;
            //animation.To = 580;
            //animation.Duration = (Duration)TimeSpan.FromSeconds(0.3);
            //showIcon_grid.BeginAnimation(WidthProperty, animation);
            //showIcon_grid.Visibility = Visibility.Hidden;
        }

        private void hideIcon_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation();

            animation.From = MainMenu.ActualWidth;
            animation.To = 0;
            animation.Duration = (Duration)TimeSpan.FromSeconds(0.3);
            MainMenu.BeginAnimation(WidthProperty, animation);

            titleGrid.Margin = new Thickness(0);
            time_bar.Margin = new Thickness(0,190,0,0);



            showIcon.Visibility = Visibility.Visible;
        }
        private void showIconClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            showIcon.Visibility = Visibility.Hidden;

            DoubleAnimation animation = new DoubleAnimation();

            animation.From = MainMenu.ActualWidth;
            animation.To = 300;
            animation.Duration = (Duration)TimeSpan.FromSeconds(0.3);
            MainMenu.BeginAnimation(WidthProperty, animation);

            titleGrid.Margin = new Thickness(300,0,0,0);
            time_bar.Margin = new Thickness(300,190,0,0);
        }
    }
}