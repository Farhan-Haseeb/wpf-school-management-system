using System;
using MahApps;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Windows;
using System.Data.SqlClient;
using System.Windows.Forms;
using MahApps.Metro.Controls;

namespace School_Management
{
    /// <summary>
    /// Interaction logic for Backup.xaml
    /// </summary>
    public partial class Backup
    {
        public Backup()
        {
            InitializeComponent();
        }
        
        private void DetachDatabase()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                try
                {
                    conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
                    conn.Open();
                    
                    string commandString = "ALTER DATABASE School SET OFFLINE WITH ROLLBACK IMMEDIATE ALTER DATABASE School SET SINGLE_USER EXEC sp_detach_db 'School'";
                    SqlCommand sqlDatabaseCommand = new SqlCommand(commandString, conn);
                    sqlDatabaseCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }

        private void RestoreButtonClick(object sender, RoutedEventArgs e)
        {
            var address = chooseRestoreFile();
            DetachDatabase();
            try
            {
                Server dbServer = new Server(new ServerConnection(System.Environment.MachineName));
                Microsoft.SqlServer.Management.Smo.Restore dbRestore = new Microsoft.SqlServer.Management.Smo.Restore() { Action = RestoreActionType.Database, Database = "School" };
                dbRestore.Devices.AddDevice(@address, DeviceType.File);
                dbRestore.ReplaceDatabase = true;

                dbRestore.SqlRestore(dbServer);
                this.ShowMessageAsync("Restored", "Database restored successfully.");
            }
            catch(Exception err)
            {
                System.Windows.MessageBox.Show(address);
                System.Windows.MessageBox.Show(err.ToString());
            }
            
        }

        private void BackupButtonClick(object sender, RoutedEventArgs e)
        {
            string fileName = string.Format("SchoolBackup_{0}.bak", DateTime.Now.ToString("yyyy_MM_dd_h_mm_tt"));
            makeVisible();
            progressBarr.Value = 0;
            try
            {
                Server dbServer = new Server (new ServerConnection(System.Environment.MachineName));
                Microsoft.SqlServer.Management.Smo.Backup dbBackup = new Microsoft.SqlServer.Management.Smo.Backup(){Action = BackupActionType.Database, Database = "School"};
                dbBackup.Devices.AddDevice(@backupDirectory() +"\\"+ fileName, DeviceType.File);
                dbBackup.Initialize = true;
                dbBackup.SqlBackupAsync(dbServer);

                progressBarr.Value = 100;
                this.ShowMessageAsync("Backup", "Backup Completed!");
                lastBackup.Content = DateTime.Now.ToString("yyyy-MM-dd h:mm tt");
            }
            catch(Exception err)
            {
                System.Windows.MessageBox.Show(err.ToString());
            }
        }
        
        public void makeVisible()
        {
            progressBarr.Visibility = Visibility.Visible;
            title.Visibility = Visibility.Visible;
        }
        public string backupDirectory()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();
                return dialog.SelectedPath;
            }
        }

        public string chooseRestoreFile()
        {
            using (var dialog = new OpenFileDialog())
            {   
                dialog.Filter = "bak files (*.bak)|*.bak";
                dialog.InitialDirectory = "C:\\";
                dialog.Title = "Select a Backup";
                dialog.ShowDialog();
                var result = dialog.FileName;
                return result;
            }
        }
    }
}