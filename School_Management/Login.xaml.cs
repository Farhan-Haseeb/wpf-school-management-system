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
using MahApps.Metro;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace School_Management
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void TitleGridMouseDown(object sender, MouseButtonEventArgs e)
        {
            password.Focus();
        }

        private void resetButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (checkBox.IsChecked == true)
                {
                    if (!string.IsNullOrWhiteSpace(resetUsername.Text) && !string.IsNullOrWhiteSpace(OldPassword.Password))
                    {
                        string oldPassword = OldPassword.Password.ToString();
                        string userToReset = resetUsername.Text.ToString();
                        string newPassword = NewPassword.Password.ToString();

                        SqlConnection Aconn = new SqlConnection();
                        Aconn.ConnectionString = ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
                        
                        string Newquery = "UPDATE UserTable SET [password]=@newPass WHERE [username]=@user AND [password]=@pass;";

                        SqlCommand cmd = new SqlCommand(Newquery, Aconn);
                        cmd.Parameters.AddWithValue("@newPass", newPassword);
                        cmd.Parameters.AddWithValue("@user", userToReset);
                        cmd.Parameters.AddWithValue("@pass", oldPassword);
                        Aconn.Open();
                        cmd.ExecuteNonQuery();
                        Aconn.Close();

                        OldPassword.Clear();
                        resetUsername.Clear();
                        NewPassword.Clear();
                        MessageBox.Show("Password Change successful");
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private void loginButtonClick(object sender, RoutedEventArgs e)
        {
            try {
                string EnteredUsername = username.Text;
                string EnteredPassword = password.Password.ToString();

                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
                conn.Open();

                string query = "SELECT * FROM UserTable WHERE [username]=@user AND [password]=@pass;";
                SqlDataAdapter sda = new SqlDataAdapter(query, conn);
                sda.SelectCommand.CommandType = System.Data.CommandType.Text;
                sda.SelectCommand.Parameters.AddWithValue("@user", EnteredUsername);
                sda.SelectCommand.Parameters.AddWithValue("@pass", EnteredPassword);

                DataSet ds = new DataSet();
                sda.Fill(ds);
                conn.Close();

                int count = ds.Tables[0].Rows.Count;

                if (!string.IsNullOrWhiteSpace(EnteredUsername) && !string.IsNullOrWhiteSpace(EnteredPassword))
                {
                    if (count == 1)
                    {
                        MainWindow goToMenu = new MainWindow();
                        goToMenu.Show();
                        this.Hide();
                    }
                    else
                    {
                        status.Content = "*Invalid Username/Password";
                    }
                }
                else
                {
                    status.Content = "*Enter both Username and Password";
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }
        private void closeClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        
        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            OldPassword.Visibility = Visibility.Visible;
            NewPassword.Visibility = Visibility.Visible;
            resetUsername.Visibility = Visibility.Visible;
            reset.Visibility = Visibility.Visible;
            username.IsEnabled = false;
            password.IsEnabled = false;
            loginButton.IsEnabled = false;
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            OldPassword.Visibility = Visibility.Hidden;
            NewPassword.Visibility = Visibility.Hidden;
            resetUsername.Visibility = Visibility.Hidden;
            reset.Visibility = Visibility.Hidden;
            username.IsEnabled = true;
            password.IsEnabled = true;
            loginButton.IsEnabled = true;
        }
    }
}