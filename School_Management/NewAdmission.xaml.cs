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

namespace School_Management
{
    /// <summary>
    /// Interaction logic for NewAdmission.xaml
    /// </summary>
    public partial class NewAdmission
    {

  

        SchoolEntities context = new SchoolEntities();
        StudentTable Modal = new StudentTable();
        StudentTable search;

        string stdImage, BCImage, FBImage;

        public NewAdmission()
        {
            InitializeComponent();
        }
        private void LoadingWindow(object sender, RoutedEventArgs e)
        {
            onLoad();
        }
        public const string DOB_Format = "dd/MM/yyyy";
        //==================================================================================================================================================
        public BitmapImage BitmapImageFromBytes(byte[] bytes)
        {
            BitmapImage image = null;
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(bytes);
                stream.Seek(0, SeekOrigin.Begin);
                System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                image = new BitmapImage();
                image.BeginInit();
                MemoryStream ms = new MemoryStream();
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                image.StreamSource = ms;
                image.StreamSource.Seek(0, SeekOrigin.Begin);
                image.EndInit();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
            return image;
        }

        public void browseImage()
        {

        }
        private void BrowseButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                FileDialog fldlg = new OpenFileDialog();
                fldlg.InitialDirectory = Environment.SpecialFolder.MyPictures.ToString();
                fldlg.Filter = "Image File (*.jpg;*.bmp;*.gif)|*.jpg;*.bmp;*.gif";
                fldlg.ShowDialog();
                {
                    string strName = fldlg.SafeFileName;
                    stdImage = fldlg.FileName;
                    ImageSourceConverter isc = new ImageSourceConverter();
                    studentImage.SetValue(Image.SourceProperty, isc.ConvertFromString(stdImage));
                }
                fldlg = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
            }
        }
        private void browseBCButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                FileDialog fldlg = new OpenFileDialog();
                fldlg.InitialDirectory = Environment.SpecialFolder.MyPictures.ToString();
                fldlg.Filter = "Image File (*.jpg;*.bmp;*.gif)|*.jpg;*.bmp;*.gif";
                fldlg.ShowDialog();
                {
                    string strName = fldlg.SafeFileName;
                    BCImage = fldlg.FileName;
                    ImageSourceConverter isc = new ImageSourceConverter();
                    birthCertificate.SetValue(Image.SourceProperty, isc.ConvertFromString(BCImage));
                }
                fldlg = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
            }
        }
        private void browseFBButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                FileDialog fldlg = new OpenFileDialog();
                fldlg.InitialDirectory = Environment.SpecialFolder.MyPictures.ToString();
                fldlg.Filter = "Image File (*.jpg;*.bmp;*.gif)|*.jpg;*.bmp;*.gif";
                fldlg.ShowDialog();
                {
                    string strName = fldlg.SafeFileName;
                    FBImage = fldlg.FileName;
                    ImageSourceConverter isc = new ImageSourceConverter();
                    formBay.SetValue(Image.SourceProperty, isc.ConvertFromString(FBImage));
                }
                fldlg = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
            }
        }
        private byte[] insertImageData(string img)
        {
            byte[] imgByteArr = null;
            try
            {
                if (img != "")
                {
                    //Initialize a file stream to read the image file
                    FileStream fs = new FileStream(img, FileMode.Open, FileAccess.Read);

                    //Initialize a byte array with size of stream
                    imgByteArr = new byte[fs.Length];

                    //Read data from the file stream and put into the byte array
                    fs.Read(imgByteArr, 0, Convert.ToInt32(fs.Length));

                    //Close a file stream
                    fs.Close();

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return imgByteArr;
        }

        private void BindImageList()
        { }
        //***************************************************************************************************************************************************
        private void SubmitButtonClick(object sender, RoutedEventArgs e)
        {
            checkEmptyAndSave();
        }

        public void saveNewStudent()
        {
            try
            {
                string currentTime = DateTime.Now.ToString("hh:mm:tt");
                string Gender = null;
                if (MaleRadioButton.IsChecked == true)
                {
                    Gender = "Male";
                }
                else
                {
                    Gender = "Female";
                }

                var SelectedClass = ((ComboBoxItem)Grade.SelectedItem).Tag.ToString();
                var SelectedReligion = ((ComboBoxItem)ReligionBox.SelectedItem).Tag.ToString();
                var SelectedNationality = ((ComboBoxItem)NationalityBox.SelectedItem).Tag.ToString();

                DateTime DOB;

                if (DOBText.Text.Contains("/"))
                {
                    DOB = DateTime.ParseExact(DOBText.Text.ToString(), DOB_Format, CultureInfo.InvariantCulture);
                }
                else
                {
                    DOB = DateTime.ParseExact(DOBText.Text.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }

                byte[] TimeOfAdmission = Encoding.ASCII.GetBytes(currentTime);

                Modal.StudentImage = insertImageData(stdImage);
                Modal.StudentBC = insertImageData(BCImage);
                Modal.StudentFormBay = insertImageData(FBImage);

                Modal.AcademicYear = AcademicYearDisplay.Text.ToString();
                Modal.Active = "True";
                Modal.DateOfAdmission = DateTime.Now.Date;
                Modal.TimeOfAdmission = TimeOfAdmission;
                Modal.DOB = DOB;
                Modal.Emergency = EmergencyText.Text.ToString();
                Modal.FatherName = FatherNameText.Text.ToString();
                Modal.FatherCNIC = FatherCNICText.Text.ToString();
                Modal.FirstLanguage = FirstLanguageText.Text.ToString();
                Modal.Gender = Gender;
                Modal.Grade = SelectedClass;
                Modal.ClassAdmitedIn = SelectedClass;

                Modal.HomeAddress = HomeAddressText.Text.ToString();
                Modal.Mobile = MobileText.Text.ToString();
                Modal.MotherCNIC = MotherCNICText.Text.ToString();
                Modal.MotherName = MotherNameText.Text.ToString();
                Modal.Name = StudentNameText.Text.ToString();
                Modal.Nationality = SelectedNationality;
                Modal.Phone = PhoneText.Text.ToString();
                Modal.PlaceOfBirth = PlaceOfBirthText.Text.ToString();
                Modal.Religion = SelectedReligion;

                //***************************PARENTS INFORMATION GRID***********************
                Modal.FarherOccupation = FatherOccupationText.Text.ToString();
                Modal.FatherDesignation = FatherDesignation.Text.ToString();
                Modal.FatherWorkAddress = FatherAdress.Text.ToString();
                Modal.MotherOccupation = MotherOccupationText.Text.ToString();
                Modal.MotherDesignation = MotherDesignation.Text.ToString();
                Modal.MotherWorkAddress = MotherAddress.Text.ToString();
                Modal.GuardianName = GuardianNameText.Text.ToString();
                Modal.GuardianRelation = RelationText.Text.ToString();
                Modal.GuardianAddress = GuardianAddress.Text.ToString();

                //***************************PREVIOUS ACADEMIC INFORMATION GRID***********************
                Modal.PreviousSchoolName = PreviousSchoolText.Text.ToString();
                Modal.Studied = FromToText.Text.ToString();
                Modal.ReasonOfLeaving = ReasonText.Text.ToString();

                try
                {
                    context.StudentTables.Add(Modal);
                    context.SaveChanges();
                    this.ShowMessageAsync("Student Added", "Student admission was successful");
                    clearAll();
                    nextAdmission();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private void ResetOnLostFocus(object sender, RoutedEventArgs e)
        {
            if (AdmissionNumberDisplay.Text == "")
            {
                nextAdmission();
                clearAll();
                UpdateButton.Visibility = Visibility.Hidden;
                SubmitButton.Visibility = Visibility.Visible;
                StudentNameText.Focus();
            }
            else if (AdmissionNumberDisplay.Text != "")
            {
                UpdateButton.Visibility = Visibility.Visible;
                SubmitButton.Visibility = Visibility.Collapsed;
                int entered;
                Int32.TryParse(AdmissionNumberDisplay.Text.ToString(), out entered);
                forUpdate(entered);
                StudentNameText.Focus();
            }
        }
        public void forUpdate(int val)
        {
            try
            {
                search = context.StudentTables.Find(val);

                if (search.Gender == "Male")
                {
                    MaleRadioButton.IsChecked = true;
                }
                else
                {
                    FemaleRadioButton.IsChecked = true;
                }

                Grade.SelectedValue = search.Grade.ToString();
                NationalityBox.SelectedValue = search.Nationality.ToString();
                ReligionBox.SelectedValue = search.Religion.ToString();

                //***************************PERSONAL INFORMATION GRID***********************

                //              STUDENT IMAGE
                if (search.StudentImage == null)
                {
                    studentImage.Source = null;
                }
                else
                {
                    studentImage.Source = BitmapImageFromBytes(search.StudentImage);
                }
                //              STUDENT FROM BAY
                if (search.StudentFormBay == null)
                {
                    formBay.Source = null;
                }
                else
                {
                    formBay.Source = BitmapImageFromBytes(search.StudentFormBay);
                }
                //              STUDENT Birth Certificate
                if (search.StudentBC == null)
                {
                    birthCertificate.Source = null;
                }
                else
                {
                    birthCertificate.Source = BitmapImageFromBytes(search.StudentBC);
                }

                DOBText.Text = search.DOB.ToString("dd/MM/yyyy");
                StudentNameText.Text = search.Name;
                PlaceOfBirthText.Text = search.PlaceOfBirth;
                FirstLanguageText.Text = search.FirstLanguage;
                FatherNameText.Text = search.FatherName;
                FatherCNICText.Text = search.FatherCNIC;
                MotherNameText.Text = search.MotherName;
                MotherCNICText.Text = search.MotherCNIC;
                HomeAddressText.Text = search.HomeAddress;
                PhoneText.Text = search.Phone;
                EmergencyText.Text = search.Emergency;
                MobileText.Text = search.Mobile;
                //***************************PARENTS INFORMATION GRID***********************
                FatherOccupationText.Text = search.FarherOccupation;
                FatherDesignation.Text = search.FatherDesignation;
                FatherAdress.Text = search.FatherWorkAddress;
                MotherOccupationText.Text = search.MotherOccupation;
                MotherDesignation.Text = search.MotherDesignation;
                MotherAddress.Text = search.MotherWorkAddress;
                GuardianNameText.Text = search.GuardianName;
                RelationText.Text = search.GuardianRelation;
                GuardianAddress.Text = search.GuardianAddress;

                //***************************PREVIOUS ACADEMIC INFORMATION GRID***********************
                PreviousSchoolText.Text = search.PreviousSchoolName;
                FromToText.Text = search.Studied;
                ReasonText.Text = search.ReasonOfLeaving;
            }
            catch (Exception err)
            {
                this.ShowMessageAsync("Doesn't not Exits!", "The Student does not exits in the database with this Admission ID "
                + val + " . Please enter a valid Admission ID.");
                clearAll();
                nextAdmission();
                UpdateButton.Visibility = Visibility.Hidden;
                SubmitButton.Visibility = Visibility.Visible;
                Console.WriteLine(err.ToString());
            }
        }
        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string currentTime = DateTime.Now.ToString("hh:mm:tt");
                string Gender = null;
                if (MaleRadioButton.IsChecked == true)
                {
                    Gender = "Male";
                }
                else
                {
                    Gender = "Female";
                }

                var SelectedClass = ((ComboBoxItem)Grade.SelectedItem).Tag.ToString();
                var SelectedNationality = ((ComboBoxItem)NationalityBox.SelectedItem).Tag.ToString();
                var SelectedReligion = ((ComboBoxItem)ReligionBox.SelectedItem).Tag.ToString();
                DateTime DOB;

                if (DOBText.Text.Contains("/"))
                {
                    DOB = DateTime.ParseExact(DOBText.Text.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                else
                {
                    DOB = DateTime.ParseExact(DOBText.Text.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }

                DateTime DateOfAdmission = DateTime.Now.Date;
                byte[] TimeOfAdmission = Encoding.ASCII.GetBytes(currentTime);

                //                      STUDENT IMAGE
                if (studentImage.Source == null)
                {
                    search.StudentImage = search.StudentImage;
                }
                else
                {
                    search.StudentImage = insertImageData(stdImage);
                }
                //                       STUDENT FORM BAY
                if (formBay.Source == null)
                {
                    search.StudentFormBay = search.StudentFormBay;
                }
                else
                {
                    search.StudentFormBay = insertImageData(FBImage);
                }
                //                       STUDENT Birth Certificate
                if (birthCertificate.Source == null)
                {
                    search.StudentBC = search.StudentBC;
                }
                else
                {
                    search.StudentBC = insertImageData(BCImage);
                }

                search.AcademicYear = AcademicYearDisplay.Text.ToString();
                search.DateOfAdmission = DateOfAdmission;
                search.TimeOfAdmission = TimeOfAdmission;
                search.DOB = DOB;
                search.Emergency = EmergencyText.Text.ToString();
                search.FatherName = FatherNameText.Text.ToString();
                search.FatherCNIC = FatherCNICText.Text.ToString();
                search.FirstLanguage = FirstLanguageText.Text.ToString();
                search.Gender = Gender;
                search.Grade = SelectedClass;
                search.HomeAddress = HomeAddressText.Text.ToString();
                search.Mobile = MobileText.Text.ToString();
                search.MotherCNIC = MotherCNICText.Text.ToString();
                search.MotherName = MotherNameText.Text.ToString();
                search.Name = StudentNameText.Text.ToString();
                search.Phone = PhoneText.Text.ToString();
                search.PlaceOfBirth = PlaceOfBirthText.Text.ToString();
                //***************************PARENTS INFORMATION GRID***********************
                search.FarherOccupation = FatherOccupationText.Text.ToString();
                search.FatherDesignation = FatherDesignation.Text.ToString();
                search.FatherWorkAddress = FatherAdress.Text.ToString();
                search.MotherOccupation = MotherOccupationText.Text.ToString();
                search.MotherDesignation = MotherDesignation.Text.ToString();
                search.MotherWorkAddress = MotherAddress.Text.ToString();
                search.GuardianName = GuardianNameText.Text.ToString();
                search.GuardianRelation = RelationText.Text.ToString();
                search.GuardianAddress = GuardianAddress.Text.ToString();

                //***************************PREVIOUS ACADEMIC INFORMATION GRID***********************
                search.PreviousSchoolName = PreviousSchoolText.Text.ToString();
                search.Studied = FromToText.Text.ToString();
                search.ReasonOfLeaving = ReasonText.Text.ToString();
                search.Updated = DateTime.Now.Date;

                emptyBoxes();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private void nextButtonClick(object sender, RoutedEventArgs e)
        {
            form.SelectedIndex = form.SelectedIndex + 1;
        }
        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            form.SelectedIndex = form.SelectedIndex - 1;
        }

        private void PIMouseDown(object sender, MouseButtonEventArgs e)
        {
            Label lab = sender as Label;
            form.SelectedIndex = 0;
        }
        private void FIMouseDown(object sender, MouseButtonEventArgs e)
        {
            Label lab = sender as Label;
            form.SelectedIndex = 1;
        }
        private void PAMouseDown(object sender, MouseButtonEventArgs e)
        {
            Label lab = sender as Label;
            form.SelectedIndex = 2;
        }
        private void LabelMouseEnter(object sender, MouseEventArgs e)
        {
            Label lab = sender as Label;

            lab.FontSize = 16.0;
            var bd = new BrushConverter();
            lab.Background = (Brush)bd.ConvertFrom("#4CFBFBFB");
        }

        private void LabelMouseLeave(object sender, MouseEventArgs e)
        {
            Label lab = sender as Label;
            lab.FontSize = 14.0;
            var bd = new BrushConverter();
            lab.Background = Brushes.Transparent;
        }
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            clearAll();
        }
        private void BlankButtonClick(object sender, RoutedEventArgs e)
        {
            Affidavit aff = new Affidavit(0, "True");
            aff.Show();
            
        }
        private void FilledButtonClick(object sender, RoutedEventArgs e)
        {
            search = context.StudentTables.Find(int.Parse(AdmissionNumberDisplay.Text.ToString()));
           
            Affidavit aff = new Affidavit(search.AdmissionNumber, search.Active);
            aff.Show();
        }
        private void clearFBButtonClick(object sender, RoutedEventArgs e)
        {
            formBay.Source = null;
        }
        private void clearBCButtonClick(object sender, RoutedEventArgs e)
        {
            birthCertificate.Source = null;
        }
        private void stdClearImage(object sender, RoutedEventArgs e)
        {
            studentImage.Source = null;
        }
        public void clearAll()
        {
            formBay.Source = birthCertificate.Source = studentImage.Source = null;
            totalKids.Text = "";
            StudentNameText.Clear();
            PlaceOfBirthText.Clear();
            FirstLanguageText.Clear();
            FatherNameText.Clear();
            FatherCNICText.Clear();
            MotherNameText.Clear();
            MotherCNICText.Clear();
            HomeAddressText.Clear();
            PhoneText.Clear();
            EmergencyText.Clear();
            MobileText.Clear();
            FatherOccupationText.Clear();
            FatherDesignation.Clear();
            FatherAdress.Clear();
            MotherOccupationText.Clear();
            MotherDesignation.Clear();
            MotherAddress.Clear();
            GuardianNameText.Clear();
            RelationText.Clear();
            GuardianAddress.Clear();
            PreviousSchoolText.Clear();
            FromToText.Clear();
            ReasonText.Clear();
        }
        public void onLoad()
        {

            PINextButton.Content = Fa.Arrow_right;
            FIBackButton.Content = Fa.Arrow_left;
            FInextButton.Content = Fa.Arrow_right;
            RBackButton.Content = Fa.Arrow_left;

            StudentNameText.Focus();
            DateDisplay.Content = DateTime.Now.ToString("dd-MM-yyyy");
            //DOBText.Text = "DD/MM/YYYY";
            string format = "yy";
            var CurrentYear = DateTime.Now.ToString(format);
            var NextYear = DateTime.Now.AddYears(1);

            AcademicYearDisplay.Text = CurrentYear + "-" + NextYear.ToString(format);
            nextAdmission();
        }
        public void nextAdmission()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
            conn.Open();

            string query = "SELECT IDENT_CURRENT('StudentTable')+1 AS AdmissionNumber";
            var command = new SqlCommand(query, conn);

            using (var Reader = command.ExecuteReader())
            {
                Reader.Read();
                AdmissionNumberDisplay.Text = Reader["AdmissionNumber"].ToString();
            }
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
        private void DOBText_KeyDown(object sender, KeyEventArgs e)
        {
            //if (DOBText.Text.Length == 2)
            //{
            //    DOBText.AppendText("/");
            //    DOBText.Focus();
            //    //DOBText.SelectionStart = DOBText.Text.Length;

            //    DOBText.CaretIndex = DOBText.Text.Length;

            //}
            //else if (DOBText.Text.Length == 5)
            //{
            //    DOBText.AppendText("/");
            //    DOBText.Focus();
            //    //DOBText.SelectionStart = DOBText.Text.Length;

            //    DOBText.CaretIndex = DOBText.Text.Length;
            //}
        }
        private void DOBText_KeyUp(object sender, KeyEventArgs e)
        {
            if (DOBText.Text.Length == 2)
            {
                DOBText.AppendText("/");
                DOBText.Focus();
                //DOBText.SelectionStart = DOBText.Text.Length;

                DOBText.CaretIndex = DOBText.Text.Length;

            }
            else if (DOBText.Text.Length == 5)
            {
                DOBText.AppendText("/");
                DOBText.Focus();
                //DOBText.SelectionStart = DOBText.Text.Length;

                DOBText.CaretIndex = DOBText.Text.Length;
            }
        }
        private void FatherCNICText_KeyUp(object sender, KeyEventArgs e)
        {
            if (FatherCNICText.Text.Length == 5)
            {
                FatherCNICText.AppendText("-");
                FatherCNICText.Focus();
                //FatherCNICText.SelectionStart = FatherCNICText.Text.Length;

                FatherCNICText.CaretIndex = FatherCNICText.Text.Length;

            }
            else if (FatherCNICText.Text.Length == 13)
            {
                FatherCNICText.AppendText("-");
                FatherCNICText.Focus();
                //FatherCNICText.SelectionStart = FatherCNICText.Text.Length;

                FatherCNICText.CaretIndex = FatherCNICText.Text.Length;
            }
        }
        private void MotherCNICText_KeyUp(object sender, KeyEventArgs e)
        {
            if (MotherCNICText.Text.Length == 5)
            {
                MotherCNICText.AppendText("-");
                MotherCNICText.Focus();
                //MotherCNICText.SelectionStart = MotherCNICText.Text.Length;

                MotherCNICText.CaretIndex = MotherCNICText.Text.Length;

            }
            else if (MotherCNICText.Text.Length == 13)
            {
                MotherCNICText.AppendText("-");
                MotherCNICText.Focus();
                //MotherCNICText.SelectionStart = MotherCNICText.Text.Length;

                MotherCNICText.CaretIndex = MotherCNICText.Text.Length;
            }
        }

        private void FatherCNICText_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (StudentNameText.Text.ToString() == "" && FatherNameText.Text.ToString() == "")
                {
                    using (SqlConnection conn = new SqlConnection())
                    {
                        conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
                        conn.Open();
                        using (SqlCommand cmd1 = new SqlCommand("SearchStudentInfoByCNIC", conn))
                        {
                            cmd1.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd1.Parameters.AddWithValue("@CNIC", FatherCNICText.Text.ToString());
                            cmd1.ExecuteNonQuery();

                            using (var reader = cmd1.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    reader.Read();
                                    HomeAddressText.Text = reader["HomeAddress"].ToString();
                                    FatherNameText.Text = reader["FatherName"].ToString();
                                    MotherNameText.Text = reader["MotherName"].ToString();
                                    MobileText.Text = reader["Mobile"].ToString();
                                    EmergencyText.Text = reader["Emergency"].ToString();
                                    PhoneText.Text = reader["Phone"].ToString();
                                    FirstLanguageText.Text = reader["FirstLanguage"].ToString();

                                    var tk = reader["TotalKids"].ToString();
                                    totalKids.Text = (tk.ToString() + " students registered on this CNIC");

                                }
                            }
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection())
                    {
                        conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Other"].ConnectionString;
                        conn.Open();
                        using (SqlCommand cmd1 = new SqlCommand("SearchStudentInfoByCNIC", conn))
                        {
                            cmd1.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd1.Parameters.AddWithValue("@CNIC", FatherCNICText.Text.ToString());
                            cmd1.ExecuteNonQuery();

                            using (var reader = cmd1.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    reader.Read();
                                    var tk = reader["TotalKids"].ToString();
                                    totalKids.Text = (tk.ToString() + " students registered on this CNIC");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
        }
        // FOR SUBMIT BUTTON
        public void checkEmptyAndSave()
        {
            if (StudentNameText.Text.Length <= 2)
            {
                this.ShowMessageAsync("Empty", "Student Name Box is empty!");
                StudentNameText.Focus();
            }
            else if (DOBText.Text.Length < 8)
            {
                this.ShowMessageAsync("Empty or Missing", "DOB is empty!");
                DOBText.Focus();
            }
            else if (FatherCNICText.Text.Length <= 2)
            {
                this.ShowMessageAsync("Empty", "Father CNIC Box is empty!");
                FatherCNICText.Focus();
            }
            //else if (HomeAddressText.Text.Length <= 2)
            //{
            //    this.ShowMessageAsync("Empty", "Address Box is empty!");
            //    HomeAddressText.Focus();
            //}
            else if (EmergencyText.Text.Length <= 2)
            {
                this.ShowMessageAsync("Empty", "Mobile Number Box is empty!");
                EmergencyText.Focus();
            }
            else
            {
                saveNewStudent();
            }
        }

        // FOR UPDATE BUTTON
        public void emptyBoxes()
        {
            if (StudentNameText.Text.Length <= 2)
            {
                this.ShowMessageAsync("Empty", "Student Name Box is empty!");
                StudentNameText.Focus();
            }
            else if (DOBText.Text.Length < 8)
            {
                this.ShowMessageAsync("Empty or Missing", "DOB is empty!");
                DOBText.Focus();
            }
            else if (FatherCNICText.Text.Length <= 2)
            {
                this.ShowMessageAsync("Empty", "Father CNIC Box is empty!");
                FatherCNICText.Focus();
            }
            //else if (HomeAddressText.Text.Length <= 2)
            //{
            //    this.ShowMessageAsync("Empty", "Address Box is empty!");
            //    HomeAddressText.Focus();
            //}
            else if (EmergencyText.Text.Length <= 2)
            {
                this.ShowMessageAsync("Empty", "Mobile Number Box is empty!");
                EmergencyText.Focus();
            }
            else
            {
                try
                {
                    //3. Mark entity as modified
                    context.Entry(search).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    this.ShowMessageAsync("Student Updated", "Student updated successfully");
                    clearAll();
                    nextAdmission();
                    UpdateButton.Visibility = Visibility.Hidden;
                    SubmitButton.Visibility = Visibility.Visible;
                    Grade.SelectedIndex = 0;
                    onLoad();
                }
                catch (DbEntityValidationException err)
                {
                    foreach (var eve in err.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                }
            }
        }
    }
}