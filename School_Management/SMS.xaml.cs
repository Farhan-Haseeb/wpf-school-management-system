using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using Jitbit.Utils;
using System.Data.SqlClient;

namespace School_Management
{
    /// <summary>
    /// Interaction logic for SMS.xaml
    /// </summary>
    public partial class SMS
    {
        public SMS()
        {
            InitializeComponent();
            newText();
            textTitle.Focus();
        }
        SchoolEntities context = new SchoolEntities();

        MessageTable messageTable;

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadData();
        }

        void loadData()
        {
            var messages = context.GetMessageTable();
            foreach(var item in messages)
            {
                messagesTitle.Items.Add(item.Title);
            }
        }

        int selected;
        string selectedMessage;

        private void messagesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var messageTitle = context.GetMessageTable();
                var selectedName = messagesTitle.SelectedItem.ToString();

                foreach (var title in messageTitle)
                {
                    if (selectedName == title.Title)
                    {
                        selected = title.MessageId;
                        selectedMessage = title.Body;

                        Console.WriteLine(messagesTitle.SelectedItem.ToString());
                        Console.WriteLine(selected + " " + selectedName);
                    }
                }
            }
            catch (Exception err)
            {
                if (err is NullReferenceException)
                    Debug.Write("Err", err.ToString());
                else
                    this.ShowMessageAsync("Err", err.ToString());
            }
        }

        private void exportContactsClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (gradeRadioButton.IsChecked == true)
                    toGrade();

                else if (ToAllRadioButton.IsChecked == true)
                    toAllStudents();

                else if (DefaultersRadioButton.IsChecked == true)
                {
                    toDefaulters();
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
            }
        }
        
        private void toGrade()
        {
            ProcessStartInfo smsCaster = new ProcessStartInfo("smscaster.exe");
            Process startCaster = new Process();
            startCaster.StartInfo = smsCaster;
            startCaster.Start();

            var selectedItem = new List<SearchStudentInfoByGradeResult>();

            ProcessStartInfo cmdStartInfo = new ProcessStartInfo("cmd");

            cmdStartInfo.FileName = @"C:\Windows\System32\cmd.exe";
            cmdStartInfo.UseShellExecute = false;
            cmdStartInfo.CreateNoWindow = true;

            var myExport = new CsvExport();

            foreach (SearchStudentInfoByGradeResult item in SMSSearchGrid.ItemsSource)
            {
                //string command = $"/C smscaster.exe -Compose {item.Emergency} \"{selectedMessage}\" -Long";
                //cmdStartInfo.Arguments = command;
                //Process cmdProcess = new Process();
                //cmdProcess.StartInfo = cmdStartInfo;
                //cmdProcess.EnableRaisingEvents = true;
                //cmdProcess.Start();
                //cmdProcess.WaitForExit();

                myExport.AddRow();
                myExport["Name"] = item.Name;
                myExport["Mobile"] = item.Emergency;

            }

            myExport.ExportToFile("C:\\Program Files (x86)\\CognitiveDev\\TKS Management System\\contacts.csv");
            myExport.ExportToBytes();

            string command = $"/C smscaster.exe 'C:\\Users\\Haseeb\\Desktop\\contacts.csv'";
            cmdStartInfo.Arguments = command;
            Process cmdProcess = new Process();
            cmdProcess.StartInfo = cmdStartInfo;
            cmdProcess.EnableRaisingEvents = true;
            cmdProcess.Start();
            cmdProcess.WaitForExit();

        }

        private void toDefaulters()
        {
            ProcessStartInfo smsCaster = new ProcessStartInfo("smscaster.exe");
            Process startCaster = new Process();
            startCaster.StartInfo = smsCaster;
            startCaster.Start();

            var selectedItem = new List<DefaultersListResult>();

            ProcessStartInfo cmdStartInfo = new ProcessStartInfo("cmd");
            cmdStartInfo.FileName = @"C:\Windows\System32\cmd.exe";
            cmdStartInfo.UseShellExecute = false;
            cmdStartInfo.CreateNoWindow = true;

            var myExport = new CsvExport();


            foreach (DefaultersListResult item in SMSSearchGrid.ItemsSource)
            {
                //string command = $"/C smscaster.exe -Compose {item.Emergency} \"{selectedMessage}\" -Long";
                //cmdStartInfo.Arguments = command;
                //Process cmdProcess = new Process();
                //cmdProcess.StartInfo = cmdStartInfo;
                //cmdProcess.EnableRaisingEvents = true;
                //cmdProcess.Start();
                //cmdProcess.WaitForExit();

                myExport.AddRow();
                myExport["Name"] = item.Name;
                myExport["Mobile"] = item.Emergency;
            }

            ///ASP.NET MVC action example
            myExport.ExportToFile("C:\\Program Files (x86)\\CognitiveDev\\TKS Management System\\contacts.csv");
            myExport.ExportToBytes();

            string command = $"/C smscaster.exe 'C:\\Users\\Haseeb\\Desktop\\contacts.csv'";
            cmdStartInfo.Arguments = command;
            Process cmdProcess = new Process();
            cmdProcess.StartInfo = cmdStartInfo;
            cmdProcess.EnableRaisingEvents = true;
            cmdProcess.Start();
            cmdProcess.WaitForExit();
        }

        private void toAllStudents()
        {
            ProcessStartInfo smsCaster = new ProcessStartInfo("smscaster.exe");
            Process startCaster = new Process();
            startCaster.StartInfo = smsCaster;
            startCaster.Start();
            var selectedItem = new List<SearchStudentInfoAllResult>();

            ProcessStartInfo cmdStartInfo = new ProcessStartInfo("cmd");
            cmdStartInfo.FileName = @"C:\Windows\System32\cmd.exe";
            cmdStartInfo.UseShellExecute = false;
            cmdStartInfo.CreateNoWindow = true;

            var myExport = new CsvExport();

            foreach (SearchStudentInfoAllResult item in SMSSearchGrid.ItemsSource)
            {
                //string command = $"/C smscaster.exe -Compose {item.Emergency} \"{selectedMessage}\" -Long";
                //cmdStartInfo.Arguments = command;
                //Process cmdProcess = new Process();
                //cmdProcess.StartInfo = cmdStartInfo;
                //cmdProcess.EnableRaisingEvents = true;
                //cmdProcess.Start();
                //cmdProcess.WaitForExit();

                myExport.AddRow();
                myExport["Name"] = item.Name;
                myExport["Mobile"] = item.Emergency;
            }

            myExport.ExportToFile("C:\\Program Files (x86)\\CognitiveDev\\TKS Management System\\contacts.csv");
            myExport.ExportToBytes();

            string command = $"/C smscaster.exe 'C:\\Users\\Haseeb\\Desktop\\contacts.csv'";
            cmdStartInfo.Arguments = command;
            Process cmdProcess = new Process();
            cmdProcess.StartInfo = cmdStartInfo;
            cmdProcess.EnableRaisingEvents = true;
            cmdProcess.Start();
            cmdProcess.WaitForExit();
        }

        // ================================================================================================
        private void searchContactsClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DefaultersRadioButton.IsChecked == true)
                {
                    var D_result = context.DefaultersList(00).ToList();
                    SMSSearchGrid.ItemsSource = D_result;
                }
                else if (ToAllRadioButton.IsChecked == true)
                {
                    var toAll = context.SearchStudentInfoAll(active: "True").ToList();
                    SMSSearchGrid.ItemsSource = toAll;
                }
                else
                {
                    var result = context.SearchStudentInfoByGrade(active: "True", grade: ((ComboBoxItem)Grade.SelectedItem).Tag.ToString()).ToList();
                    SMSSearchGrid.ItemsSource = result;
                }
            }
            catch(Exception err)
            {
                Console.WriteLine(err.ToString());
                MessageBox.Show("Please select any field then press search");
            }
        }
        // COPIED FROM STUDENT DISCOUNT

        int serachID;

        public void newText()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
            conn.Open();

            string query = "SELECT IDENT_CURRENT('MessageTable')+1 AS MessageId";
            var command = new SqlCommand(query, conn);

            using (var Reader = command.ExecuteReader())
            {
                Reader.Read();
                newTextId.Text = Reader["MessageId"].ToString();
            }
        }

        private void ResetOnLostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (newTextId.Text != "")
                {
                    UpdateTextButton.Visibility = Visibility.Visible;
                    CreateTextButton.Visibility = Visibility.Hidden;

                    int.TryParse(newTextId.Text.ToString(), out serachID);

                    messageTable = context.MessageTables.Find(serachID);

                    textTitle.Text = messageTable.Title;
                    textBody.Text = messageTable.Body.ToString();
                }
                else
                {
                    UpdateTextButton.Visibility = Visibility.Hidden;
                    CreateTextButton.Visibility = Visibility.Visible;
                    newText();
                }
            }
            catch (Exception err)
            {
                if(err is NullReferenceException)
                    this.ShowMessageAsync("Invalid", "The Id box is empty or the entered Id doesn't exists");
                    Debug.WriteLine(err.ToString());
                this.ShowMessageAsync("Error", err.ToString());
            }
        }

        private void createButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageTable Modal = new MessageTable();

                Modal.Body = textBody.Text.ToString();
                Modal.Title = textTitle.Text.ToString();

                context.MessageTables.Add(Modal);
                context.SaveChanges();
                this.ShowMessageAsync("Created", "New Discount created successfully!");
                newText();
            }
            catch (Exception ex)
            {
                if(ex is FormatException)
                    this.ShowMessageAsync("Format Exception", "The fields weren't filled properly");
                this.ShowMessageAsync("Exception", ex.ToString());
            }
            clearAll();
            messagesTitle.Items.Clear();
            loadData();
        }
        private void updateButtonClick(object sender, RoutedEventArgs e)
        {
            forUpdate(messageTable);
            clearAll();
        }

        public void populateFeeBox(ComboBox c)
        {
            var feeTable = context.FeeTables;

            foreach (var fee in feeTable)
            {
                c.Items.Add(fee.FeeTitle);
            }
        }

        protected void forUpdate(MessageTable dt)
        {
            try
            {
                dt.Title = textTitle.Text.ToString();
                dt.Body =  textBody.Text.ToString();

                context.Entry(dt).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                this.ShowMessageAsync("Done", "Text Updated");

                UpdateTextButton.Visibility = Visibility.Hidden;
                CreateTextButton.Visibility = Visibility.Visible;
            }
            catch (NullReferenceException)
            {
                this.ShowMessageAsync("Invalid", "Some values are invalid");
            }

        }
        public void clearAll()
        {
            newTextId.Clear();
            textTitle.Clear();
            textBody.Clear();
        }
        private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex Numeric = new Regex("[^0-9/]+"); e.Handled = Numeric.IsMatch(e.Text);
        }
        private void AlphabetOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex Alphabet = new Regex("[^A-Za-z ]+$"); e.Handled = Alphabet.IsMatch(e.Text);
        }
        private void AlphaNumeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex CharAndDigit = new Regex("[^0-9A-Za-z ]+$"); e.Handled = CharAndDigit.IsMatch(e.Text);
        }
    }
}