using AForge.Video.DirectShow;
using AForge.Video;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Data;
using System.Data.OleDb;
using System.Net.Mail;
using AForge.Video;
using AForge.Video.DirectShow;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using SkiaSharp;
namespace PortLink__Final_Project_
{
    public partial class AdminDashboard : MaterialForm
    {
        OleDbConnection? myConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Ivan\Documents\PortLink (Final Project)\Database\PortLink Database.accdb;");
        OleDbDataAdapter? da;
        OleDbCommand? cmd;
        DataSet? ds;
        private int indexRow;

        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private Bitmap currentFrame;
        private bool isCameraRunning = false;

        private int shipmentID, ID;
        private Login login;

        private AdminMaterialCards admin;
        private MainMenuPage main = new MainMenuPage();
        public AdminDashboard(Login loginInstance)
        {
            InitializeComponent();

            admin = new AdminMaterialCards();
            login = loginInstance;
            ID = Convert.ToInt32(login.AccountID);

            AddQuickShipment();
            AddUsers();
            AddManageShipment();
            AddHistoryShipment();
            LoadUserData();
            PieChart();

            timerclock.Start();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Green600, Primary.Green800, Primary.Green400, Accent.LightGreen200, TextShade.WHITE);
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x112;
            const int SC_SIZE = 0xF000;

            if (m.Msg == WM_SYSCOMMAND && (m.WParam.ToInt32() & 0xFFF0) == SC_SIZE)
                return;

            base.WndProc(ref m);
        }
        private void AddQuickShipment()
        {
            List<MaterialCard> shipmentCards = admin.GetQuickShipments(myConn);

            foreach (var card in shipmentCards)
            {
                PendingLayout.Controls.Add(card);
            }
        }
        private void AddUsers()
        {
            List<MaterialCard> shipmentCards = admin.GetUsers(myConn, RefreshUsers_Click, AllShipments);
            foreach (var card in shipmentCards)
            {
                Users.Controls.Add(card);
            }
        }
        private void AddManageShipment()
        {
            List<MaterialCard> shipmentCards = admin.GetManageShipments(myConn);
            foreach (var card in shipmentCards)
            {
                ManageShipmentLayout.Controls.Add(card);
            }
        }
        private void AddHistoryShipment()
        {
            List<MaterialCard> shipmentCards = admin.GetHistoryShipments(myConn);

            foreach (var card in shipmentCards)
            {
                HistoryLayout.Controls.Add(card);
            }
        }
        private void FirstNameBtn(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                if (clickedButton == AccBEFN) // Edit
                {
                    AccTFN.Visible = AccBSFN.Visible = AccBCFN.Visible = true;
                    AccFN.Visible = AccBEFN.Visible = false;
                    AccTFN.Text = AccFN.Text;
                }
                else if (clickedButton == AccBSFN) // Save
                {
                    AccTFN.Visible = AccBSFN.Visible = AccBCFN.Visible = false;
                    AccFN.Visible = AccBEFN.Visible = true;
                    string query = "UPDATE AdminAccounts SET [FirstName] = @FirstName WHERE AccountID = @AccountID";
                    myConn.Open();
                    using (OleDbCommand cmd = new OleDbCommand(query, myConn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", AccTFN.Text);
                        cmd.Parameters.AddWithValue("@AccountID", ID);
                        cmd.ExecuteNonQuery();
                    }
                    myConn.Close();
                    AccFN.Text = AccTFN.Text;
                }
                else if (clickedButton == AccBCFN) // Cancel
                {
                    AccTFN.Visible = AccBSFN.Visible = AccBCFN.Visible = false;
                    AccFN.Visible = AccBEFN.Visible = true;
                    AccTFN.Text = AccFN.Text;
                }
            }
        }
        private void LastNameBtn(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton != null)
            {
                if (clickedButton == AccBELN) // Edit
                {
                    AccTLN.Visible = AccBSLN.Visible = AccBCLN.Visible = true;
                    AccLN.Visible = AccBELN.Visible = false;
                    AccTLN.Text = AccLN.Text;
                }
                else if (clickedButton == AccBSLN) // Save
                {
                    AccTLN.Visible = AccBSLN.Visible = AccBCLN.Visible = false;
                    AccLN.Visible = AccBELN.Visible = true;
                    AccLN.Text = AccTLN.Text;
                    string query = "UPDATE AdminAccounts SET [LastName] = @LastName WHERE AccountID = @AccountID";
                    myConn.Open();
                    using (OleDbCommand cmd = new OleDbCommand(query, myConn))
                    {
                        cmd.Parameters.AddWithValue("@LastName", AccTLN.Text);
                        cmd.Parameters.AddWithValue("@AccountID", ID);
                        cmd.ExecuteNonQuery();
                    }
                    myConn.Close();
                }
                else if (clickedButton == AccBCLN) // Cancel
                {
                    AccTLN.Visible = AccBSLN.Visible = AccBCLN.Visible = false;
                    AccLN.Visible = AccBELN.Visible = true;
                    AccLN.Text = AccTLN.Text;
                }
            }
        }
        private void Username(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton != null)
            {
                if (clickedButton == AccBEU) // Edit
                {
                    AccTU.Visible = AccBSU.Visible = AccBCU.Visible = true;
                    AccU.Visible = AccBEU.Visible = false;
                    AccTU.Text = AccU.Text;
                }
                else if (clickedButton == AccBSU) // Save
                {
                    AccTU.Visible = AccBSU.Visible = AccBCU.Visible = false;
                    AccU.Visible = AccBEU.Visible = true;
                    AccU.Text = AccTU.Text;
                    string query = "UPDATE AdminAccounts SET [Username] = @Username WHERE AccountID = @AccountID";
                    myConn.Open();
                    using (OleDbCommand cmd = new OleDbCommand(query, myConn))
                    {
                        cmd.Parameters.AddWithValue("@Username", AccTU.Text);
                        cmd.Parameters.AddWithValue("@AccountID", ID);
                        cmd.ExecuteNonQuery();
                    }
                    myConn.Close();
                }
                else if (clickedButton == AccBCU) // Cancel
                {
                    AccTU.Visible = AccBSU.Visible = AccBCU.Visible = false;
                    AccU.Visible = AccBEU.Visible = true;
                    AccU.Text = AccTU.Text;
                }
            }
        }
        private void EmailBtn(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton != null)
            {
                if (clickedButton == AccBEEA) // Edit
                {
                    AccTEA.Visible = AccBSEA.Visible = AccBCEA.Visible = true;
                    AccEA.Visible = AccBEEA.Visible = false;
                    AccTEA.Text = AccEA.Text;
                }
                else if (clickedButton == AccBSEA) // Save
                {
                    AccTEA.Visible = AccBSEA.Visible = AccBCEA.Visible = false;
                    AccEA.Visible = AccBEEA.Visible = true;
                    AccEA.Text = AccTEA.Text;
                    string query = "UPDATE AdminAccounts SET [Email] = @Email WHERE AccountID = @AccountID";
                    myConn.Open();
                    using (OleDbCommand cmd = new OleDbCommand(query, myConn))
                    {
                        cmd.Parameters.AddWithValue("@Email", AccTEA.Text);
                        cmd.Parameters.AddWithValue("@AccountID", ID);
                        cmd.ExecuteNonQuery();
                    }
                    myConn.Close();

                }
                else if (clickedButton == AccBCEA) // Cancel
                {
                    AccTEA.Visible = AccBSEA.Visible = AccBCEA.Visible = false;
                    AccEA.Visible = AccBEEA.Visible = true;
                    AccEA.Text = AccTEA.Text;
                }
            }
        }
        private void ChangePhoto_Click(object sender, EventArgs e)
        {
            string newImagePath = "";
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.png;*.bmp;*.gif";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Profile.BackgroundImage = Image.FromFile(ofd.FileName);
                }
                newImagePath = ofd.FileName;
            }
            myConn.Open();
            string query = "UPDATE AdminAccounts SET [Image] = @Image WHERE AccountID = @AccountID";

            using (OleDbCommand cmd = new OleDbCommand(query, myConn))
            {
                cmd.Parameters.AddWithValue("@Image", newImagePath);
                cmd.Parameters.AddWithValue("@AccountID", ID);
                cmd.ExecuteNonQuery();
            }
            myConn.Close();
        }
        private void RemovePhoto_Click(object sender, EventArgs e)
        {
            Profile.BackgroundImage = Image.FromFile(@"C:\Users\Ivan\Documents\PortLink (Final Project)\Images\Default Profile Icon.png");
            string query = "UPDATE AdminAccounts SET [Image] = @Image WHERE AccountID = @AccountID";
            myConn.Open();
            using (OleDbCommand cmd = new OleDbCommand(query, myConn))
            {
                cmd.Parameters.AddWithValue("@Image", @"C:\Users\Ivan\Documents\PortLink (Final Project)\Images\Default Profile Icon.png");
                cmd.Parameters.AddWithValue("@AccountID", ID);
                cmd.ExecuteNonQuery();
            }
            myConn.Close();
        }
        private void Logout_Click(object sender, EventArgs e)
        {
            foreach (Form form in Application.OpenForms.Cast<Form>().ToList()) form.Hide();
            main.Show();
        }
        private void RefreshManageShipment_Click(object sender, EventArgs e)
        {
            ManageShipmentLayout.Controls.Clear();
            AddManageShipment();
        } //Refresh
        private void RefreshDashboard_Click(object sender, EventArgs e)
        {
            PendingLayout.Controls.Clear();
            AddQuickShipment();
            PieChart();
        }
        private void RefreshHistory_Click(object sender, EventArgs e)
        {
            HistoryLayout.Controls.Clear();
            AddHistoryShipment();
        }
        private void SearchManageID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ManageShipmentLayout.Controls.Clear();
                List<MaterialCard> shipmentCards = admin.GetManageShipmentsID(myConn, SearchManageID.Text);
                foreach (var card in shipmentCards)
                {
                    ManageShipmentLayout.Controls.Add(card);
                }
            }
        }
        private void SearchHistoryID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                HistoryLayout.Controls.Clear();
                List<MaterialCard> shipmentCards = admin.GetHistoryShipmentsID(myConn, SearchHistoryID.Text);
                foreach (var card in shipmentCards)
                {
                    HistoryLayout.Controls.Add(card);
                }
            }
        }
        private void LoadUserData()
        {
            string query = "SELECT FullName, FirstName, LastName, Email, Username, Image, CreatedDate FROM AdminAccounts WHERE AccountID = ?";
            using (OleDbCommand cmd = new OleDbCommand(query, myConn))
            {
                cmd.Parameters.AddWithValue("?", ID);
                try
                {
                    myConn.Open();
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            FullName.Text = reader["FullName"].ToString();
                            AccFN.Text = reader["FirstName"].ToString();
                            AccLN.Text = reader["LastName"].ToString();
                            AccEA.Text = reader["Email"].ToString();
                            AccU.Text = reader["Username"].ToString();
                            Profile.BackgroundImage = Image.FromFile(reader["Image"].ToString());
                            AccountCreated.Text = reader["CreatedDate"].ToString();
                        }
                    }
                    myConn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }
        private void timerclock_Tick(object sender, EventArgs e)
        {
            Time.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        }

        private void UpdatePass_Click(object sender, EventArgs e)
        {
            string confirmPass = login.HashPassword(CurrentPass.Text);
            string query = "SELECT Password FROM AdminAccounts WHERE AccountID = ?";
            using (OleDbCommand cmd = new OleDbCommand(query, myConn))
            {
                cmd.Parameters.AddWithValue("?", ID);
                myConn.Open();
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (confirmPass == reader["Password"].ToString() && NewPass.Text == ConfirmPass.Text)
                        {
                            string updateQuery = "UPDATE AdminAccounts SET [Password] = ? WHERE AccountID = ?";
                            using (OleDbCommand updateCmd = new OleDbCommand(updateQuery, myConn))
                            {
                                updateCmd.Parameters.AddWithValue("?", login.HashPassword(NewPass.Text));
                                updateCmd.Parameters.AddWithValue("?", ID);
                                updateCmd.ExecuteNonQuery();
                                MessageBox.Show("Password updated successfully!");
                                CurrentPass.Text = ConfirmPass.Text = NewPass.Text = string.Empty;
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Password confirmation failed or passwords do not match.");
                            CurrentPass.Text = ConfirmPass.Text = NewPass.Text = string.Empty;
                            CurrentPass.Focus();
                        }
                    }
                }
                myConn.Close();
            }
        }
        private void CurrentPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                NewPass.Focus();
            }
        }
        private void NewPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ConfirmPass.Focus();
            }
        }
        private void ConfirmPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                UpdatePass_Click(sender, e);
            }
        }
        private void ForgotPass_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            EmailVerifyCard.Visible = true;
            EmailVerifyCard.BringToFront();
            EmailVerifyCard.Location = new Point(393, 190);
            AdminDash.Enabled = false;
        }
        private void ExitVerify_Click(object sender, EventArgs e)
        {
            EmailVerifyCard.Visible = false;
            AdminDash.Enabled = true;
        }
        private void Verify_Click(object sender, EventArgs e)
        {
            EmailVerifyCard.Visible = false;
            ChangePassCard.Visible = true;
            ChangePassCard.BringToFront();
            ChangePassCard.Location = new Point(393, 190);
        }
        private void ChangePass_Click(object sender, EventArgs e)
        {
            ChangePassCard.Visible = false;
            AdminDash.Enabled = true;
        }

        private void RefreshUsers_Click(object sender, EventArgs e)
        {
            AllShipments.Controls.Clear();
            Users.Controls.Clear();
            AddUsers();
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            currentFrame = (Bitmap)eventArgs.Frame.Clone();
            Profile.BackgroundImage = (Bitmap)currentFrame.Clone();
        }
        private void Camera_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isCameraRunning)
                {
                    videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                    if (videoDevices.Count == 0)
                    {
                        MessageBox.Show("No camera found!");
                        return;
                    }

                    videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                    videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                    videoSource.Start();
                    isCameraRunning = true;
                }
                else
                {
                    if (currentFrame != null)
                    {
                        Bitmap capturedImage = (Bitmap)currentFrame.Clone();

                        string imagePath = Path.Combine(Application.StartupPath, "CapturedProfile_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".jpg");

                        capturedImage.Save(imagePath, System.Drawing.Imaging.ImageFormat.Jpeg);

                        Profile.BackgroundImage = capturedImage;

                        string query = "UPDATE AdminAccounts SET [Image] = @Image WHERE AccountID = @AccountID";
                        myConn.Open();
                        using (OleDbCommand cmd = new OleDbCommand(query, myConn))
                        {
                            cmd.Parameters.AddWithValue("@Image", imagePath);
                            cmd.Parameters.AddWithValue("@AccountID", ID);
                            cmd.ExecuteNonQuery();
                        }
                        myConn.Close();
                    }

                    if (videoSource != null && videoSource.IsRunning)
                    {
                        videoSource.SignalToStop();
                        videoSource.NewFrame -= video_NewFrame;
                    }

                    isCameraRunning = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Camera error: " + ex.Message);
            }
        }
        private void PieChart()
        {
            myConn.Open();
            int totalbooking = 0, approval = 0, shipped = 0, denied = 0, ongoing = 0;

            string query2 = "SELECT Status FROM ShipmentItemDetails";

            using (OleDbCommand cmd2 = new OleDbCommand(query2, myConn))
            {
                using (OleDbDataReader reader2 = cmd2.ExecuteReader())
                {
                    while (reader2.Read())
                    {
                        string status = reader2["Status"].ToString();
                        totalbooking++;
                        if (status == "Pending") { approval++; }
                        else if (status == "Delivered (Sent)") { shipped++; }
                        else if (status == "Denied") { denied++; }
                        else if (status != "Pending" && status != "Delivered (Sent)" && status != "Denied") { ongoing++; }
                    }
                }
            }

            myConn.Close();
            pieChart1.Series = new ISeries[]
        {
            new PieSeries<double>
            {
                Values = new double[] { totalbooking },
                Name = "Total Booked",
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsSize = 16
            },
            new PieSeries<double>
            {
                Values = new double[] { approval },
                Name = "Awaiting Approval",
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsSize = 16
            },
            new PieSeries<double>
            {
                Values = new double[] { shipped },
                Name = "Shipment Successful",
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsSize = 16
            },
            new PieSeries<double>
            {
                Values = new double[] { ongoing },
                Name = "Ongoing Shipment",
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsSize = 16
            },
            new PieSeries<double>
            {
                Values = new double[] { denied },
                Name = "Denied Booking",
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsSize = 16
            }
        };
            pieChart1.LegendPosition = LiveChartsCore.Measure.LegendPosition.Right;
        }

        
    }
}
