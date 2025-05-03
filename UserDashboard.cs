using System.Data.OleDb;
using System.Data;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Text.RegularExpressions;
using NanoidDotNet;
using AForge.Video;
using AForge.Video.DirectShow;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using System.Windows.Forms;
using System.Net.Mail;
using LiveChartsCore.Themes;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using Microsoft.VisualBasic.ApplicationServices;
using System.Security.Policy;
namespace PortLink__Final_Project_
{
    public partial class UserDashboard : MaterialForm
    {
        OleDbConnection? myConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Ivan\Documents\PortLink (Final Project)\Database\PortLink Database.accdb;");
        OleDbDataAdapter? da;
        OleDbCommand? cmd;
        DataSet? ds;

        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private Bitmap currentFrame;
        private bool isCameraRunning = false;

        // Class Variable
        string cellNumber = "COD", eWallet = "COD";
        public int indexRow, maxWeight = 1, totalWeight;

        private int ID, shipmentID;
        internal double totalPrice, weightFee, valueFee, packageFee, shippingFee;

        // Other Forms or UserControl
        private UserMaterialCards user;
        private Login login;
        private MainMenuPage main = new MainMenuPage();
        public UserDashboard(Login loginInstance)
        {
            InitializeComponent();

            user = new UserMaterialCards();
            login = loginInstance;
            ID = Convert.ToInt32(login.AccountID);
            LoadUserData();
            PieChart();
            AddQuickShipment();
            AddMyShipment();
            AddHistoryShipment();
            timerclock.Start();

            PaymentMethod.SelectedIndex = 0;
            EWalletName.SelectedIndex = 0;
            Value.SelectedIndex = 0;
            ShippingSpeed.SelectedIndex = 0;
            CargoType.SelectedIndex = 0;
            PackageType.SelectedIndex = 0;
            CalculatePrice();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Green600, Primary.Green800, Primary.Green400, Accent.LightGreen200, TextShade.WHITE);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;

        }
        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x112;
            const int SC_SIZE = 0xF000;

            if (m.Msg == WM_SYSCOMMAND && (m.WParam.ToInt32() & 0xFFF0) == SC_SIZE)
                return;

            base.WndProc(ref m);
        }
        private void timerclock_Tick(object sender, EventArgs e)
        {
            Time.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
            List<int> deliveredShipments = new();
            using (OleDbConnection myConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Ivan\Documents\PortLink (Final Project)\Database\PortLink Database.accdb;"))
            {
                myConn.Open();
                string query = "SELECT ShipmentID, TimeElapsed, Status, ShippingSpeed FROM ShipmentItems";
                using (OleDbCommand command = new OleDbCommand(query, myConn))
                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int shipmentId = reader.GetInt32(0);
                        int currentSeconds = reader.GetInt32(1);
                        string currentStatus = reader.GetString(2);
                        string shippingSpeed = reader.GetString(3);

                        if (currentStatus == "Delivered (Sent)" || currentStatus == "Pending" || currentStatus == "Approved" || currentStatus == "Denied")
                        {
                            continue;
                        }

                        int pickup = 0, warehouse = 0, processing = 0, transit = 0, delivery = 0, approaching = 0;

                        int newElapsed = currentSeconds + 1;

                        DeliveryTime(ref pickup, ref warehouse, ref processing, ref transit, ref delivery, ref approaching, shippingSpeed);

                        if (newElapsed < pickup)
                            currentStatus = "Pickup in Progress";
                        else if (newElapsed < warehouse)
                            currentStatus = "In Warehouse";
                        else if (newElapsed < processing)
                            currentStatus = "Being Processed";
                        else if (newElapsed < transit)
                            currentStatus = "In Transit";
                        else if (newElapsed < delivery)
                            currentStatus = "Out for Delivery";
                        else if (newElapsed < approaching)
                            currentStatus = "Approaching Destination";
                        else { currentStatus = "Delivered"; deliveredShipments.Add(shipmentId); }

                        string updateQuery = "UPDATE ShipmentItems SET TimeElapsed = ?, Status = ? WHERE ShipmentID = ?";
                        using (OleDbCommand updateCmd = new OleDbCommand(updateQuery, myConn))
                        {
                            updateCmd.Parameters.AddWithValue("?", newElapsed);
                            updateCmd.Parameters.AddWithValue("?", currentStatus);
                            updateCmd.Parameters.AddWithValue("?", shipmentId);
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                }

                foreach (var shipmentId in deliveredShipments)
                {
                    string emailQuery = "SELECT RecieverContact FROM ShipmentItemDetails WHERE ShipmentID = ?";
                    using (OleDbCommand emailCmd = new OleDbCommand(emailQuery, myConn))
                    {
                        emailCmd.Parameters.AddWithValue("?", shipmentId);
                        using (OleDbDataReader emailReader = emailCmd.ExecuteReader())
                        {
                            if (emailReader.Read())
                            {
                                string email = emailReader["RecieverContact"]?.ToString() ?? "";

                                string sentUpdateQuery = "UPDATE ShipmentItems SET Status = ? WHERE ShipmentID = ?";
                                using (OleDbCommand sentCmd = new OleDbCommand(sentUpdateQuery, myConn))
                                {
                                    sentCmd.Parameters.AddWithValue("?", "Delivered (Sent)");
                                    sentCmd.Parameters.AddWithValue("?", shipmentId);
                                    sentCmd.ExecuteNonQuery();
                                }
                                MaterialCard card = new MaterialCard
                                {
                                    Size = new Size(334, 50),
                                };
                                MaterialLabel text = new MaterialLabel
                                {
                                    Size = new Size(302, 19),
                                    Location = new Point(17, 14),
                                    Text = $"Shipment {shipmentId} Has Arrived...",
                                };
                                card.Controls.Add(text);
                                NotificationFlow.Controls.Add(card);
                                SendEmail(email, "Delivery Completed", $"Your package Shipment ID {shipmentId} has been successfully delivered. Thank you for using PortLink!");
                                System.Media.SystemSounds.Asterisk.Play();
                            }
                        }
                    }
                }
            }
        }
        private void DeliveryTime(ref int pickup, ref int warehouse, ref int processing, ref int transit, ref int delivery, ref int approaching, string shippingSpeed)
        {
            if (shippingSpeed.Contains("Standard Shipping")) { pickup = 30; warehouse = 60; processing = 120; transit = 180; delivery = 240; approaching = 300; return; }
            else if (shippingSpeed.Contains("Express Shipping")) { pickup = 20; warehouse = 40; processing = 60; transit = 80; delivery = 100; approaching = 120; return; }
            else if (shippingSpeed.Contains("Standard Shipping")) { pickup = 10; warehouse = 30; processing = 40; transit = 50; delivery = 70; approaching = 80; return; }
            else if (shippingSpeed.Contains("Economy Shipping")) { pickup = 30; warehouse = 60; processing = 120; transit = 180; delivery = 240; approaching = 490; return; }
        }
        private void AddQuickShipment()
        {
            List<MaterialCard> shipmentCards = user.GetQuickShipments(ID, myConn);

            foreach (var card in shipmentCards)
            {
                DashboardLayout.Controls.Add(card);
            }
        } // Dashboard Card
        private void AddMyShipment()
        {
            List<MaterialCard> shipmentCards = user.GetDetailedShipments(ID, myConn, RefreshShipment_Click);
            foreach (var card in shipmentCards)
            {
                MyShipmentLayout.Controls.Add(card);
            }
        } // My Shipment Card
        private void AddHistoryShipment()
        {
            List<MaterialCard> shipmentCards = user.GetHistoryShipments(ID, myConn);
            foreach (var card in shipmentCards)
            {
                HistoryLayout.Controls.Add(card);
            }
        } // History Shipment Card
        private void SearchMyShipID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                MyShipmentLayout.Controls.Clear();
                List<MaterialCard> shipmentCards = user.GetDetailedShipmentsID(ID, myConn, SearchMyShipID.Text, RefreshShipment_Click);
                foreach (var card in shipmentCards)
                {
                    MyShipmentLayout.Controls.Add(card);
                }
            }
        } // Search My Shipment
        private void SearchIDHistory_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                HistoryLayout.Controls.Clear();
                List<MaterialCard> shipmentCards = user.GetHistoryShipmentsID(ID, myConn, SearchHistoryID.Text);
                foreach (var card in shipmentCards)
                {
                    HistoryLayout.Controls.Add(card);
                }
            }
        } // Search History
        private void Logout_Click(object sender, EventArgs e)
        {
            foreach (Form form in Application.OpenForms.Cast<Form>().ToList()) form.Hide();
            main.Show();
        } // Logout
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
            string query = "UPDATE UserAccounts SET [Image] = @Image WHERE AccountID = @AccountID";

            using (OleDbCommand cmd = new OleDbCommand(query, myConn))
            {
                cmd.Parameters.AddWithValue("@Image", newImagePath);
                cmd.Parameters.AddWithValue("@AccountID", ID);
                cmd.ExecuteNonQuery();
            }
            myConn.Close();
        } // Change Photo
        private void RemovePhoto_Click(object sender, EventArgs e)
        {
            Profile.BackgroundImage = Image.FromFile(@"C:\Users\Ivan\Documents\PortLink (Final Project)\Images\Default Profile Icon.png");
            string query = "UPDATE UserAccounts SET [Image] = @Image WHERE AccountID = @AccountID";
            myConn.Open();
            using (OleDbCommand cmd = new OleDbCommand(query, myConn))
            {
                cmd.Parameters.AddWithValue("@Image", @"C:\Users\Ivan\Documents\PortLink (Final Project)\Images\Default Profile Icon.png");
                cmd.Parameters.AddWithValue("@AccountID", ID);
                cmd.ExecuteNonQuery();
            }
            myConn.Close();
        } // Remove Photo
        private void FirstNameButtons(object sender, EventArgs e)
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
                    string query = "UPDATE UserAccounts SET [FirstName] = @FirstName WHERE AccountID = @AccountID";
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
        } // buttons
        private void EmailButtons(object sender, EventArgs e)
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
                    string query = "UPDATE UserAccounts SET [Email] = @Email WHERE AccountID = @AccountID";
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
        private void UsernameButtons(object sender, EventArgs e)
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
                    string query = "UPDATE UserAccounts SET [Username] = @Username WHERE AccountID = @AccountID";
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
        private void LastNameButtons(object sender, EventArgs e)
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
                    string query = "UPDATE UserAccounts SET [LastName] = @LastName WHERE AccountID = @AccountID";
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
        private void SenderButtons(object sender, EventArgs e)
        {
            MaterialSkin.Controls.MaterialButton clickedButton = sender as MaterialSkin.Controls.MaterialButton;

            if (clickedButton == null)
                return;
            // First Name
            if (clickedButton == SBEFN) // Edit
            {
                STFN.Visible = SBCFN.Visible = SBSFN.Visible = true;
                SFN.Visible = SBEFN.Visible = false;
                if (SFN.Text == "First Name (Input Field)") { STFN.Text = ""; return; }
                STFN.Text = SFN.Text;
            }
            else if (clickedButton == SBSFN) // Save
            {
                SFN.Visible = SBEFN.Visible = true;
                SBSFN.Visible = STFN.Visible = SBCFN.Visible = false;
                if (STFN.Text == "") { SFN.Text = "First Name (Input Field)"; return; }
                SFN.Text = STFN.Text;
            }
            else if (clickedButton == SBCFN) // Cancel
            {
                SFN.Visible = SBEFN.Visible = true;
                SBSFN.Visible = SBCFN.Visible = STFN.Visible = false;
                STFN.Text = SFN.Text;
            }
            // Last Name
            if (clickedButton == SBELN) // Edit
            {
                STLN.Visible = SBCLN.Visible = SBSLN.Visible = true;
                SLN.Visible = SBELN.Visible = false;
                if (SLN.Text == "Last Name (Input Field)") { STLN.Text = ""; return; }
                STLN.Text = SLN.Text;
            }
            else if (clickedButton == SBSLN) // Save
            {
                SLN.Visible = SBELN.Visible = true;
                SBSLN.Visible = SBCLN.Visible = STLN.Visible = false;
                if (STLN.Text == "") { SLN.Text = "Last Name (Input Field)"; return; }
                SLN.Text = STLN.Text;
            }
            else if (clickedButton == SBCLN) // Cancel
            {
                SLN.Visible = SBELN.Visible = true;
                SBSLN.Visible = SBCLN.Visible = STLN.Visible = false;
                STLN.Text = SLN.Text;
            }
            // Contact Info
            if (clickedButton == SBECI) // Edit
            {
                STCI.Visible = SBCCI.Visible = SBSCI.Visible = true;
                SCI.Visible = SBECI.Visible = false;
                if (SCI.Text == "Email (Input Field)") { STCI.Text = ""; return; }
                STCI.Text = SCI.Text;
            }
            else if (clickedButton == SBSCI) // Save
            {
                SBECI.Visible = SCI.Visible = true;
                SBSCI.Visible = SBCCI.Visible = STCI.Visible = false;
                if (STCI.Text == "") { SCI.Text = "Email (Input Field)"; return; }
                SCI.Text = STCI.Text;
            }
            else if (clickedButton == SBCCI) //Cancel
            {
                SBECI.Visible = SCI.Visible = true;
                SBSCI.Visible = SBCCI.Visible = STCI.Visible = false;
                STCI.Text = SCI.Text;
            }
            // Pick up Location
            if (clickedButton == SBEPL) // Edit
            {
                STPL.Visible = SBCPL.Visible = SBSPL.Visible = true;
                SPL.Visible = SBEPL.Visible = false;
                if (SPL.Text == "Pick Up Address (Input Field)") { STPL.Text = ""; return; }
                STPL.Text = SPL.Text;
            }
            else if (clickedButton == SBSPL) // Save
            {
                SBEPL.Visible = SPL.Visible = true;
                SBSPL.Visible = SBCPL.Visible = STPL.Visible = false;
                if (STPL.Text == "") { SPL.Text = "Pick Up Address (Input Field)"; return; }
                SPL.Text = STPL.Text;
                PickAddress.Text = $"Pick Up Address : {SPL.Text} ({PickUpRegion.SelectedItem})";
            }
            else if (clickedButton == SBCPL) // Cancel
            {
                SBEPL.Visible = SPL.Visible = true;
                SBSPL.Visible = SBCPL.Visible = STPL.Visible = false;
                STPL.Text = SPL.Text;
            }
        }
        private void RecieverButtons(object sender, EventArgs e)
        {
            MaterialButton clickedButton = sender as MaterialButton;
            if (clickedButton == null)
                return;
            // First Name
            if (clickedButton == RBEFN) // Edit
            {
                RTFN.Visible = RBSFN.Visible = RBCFN.Visible = true;
                RFN.Visible = RBEFN.Visible = false;
                if (RFN.Text == "First Name (Input Field)") { RTFN.Text = ""; return; }
                RTFN.Text = RFN.Text;
            }
            else if (clickedButton == RBSFN) // Save
            {
                RTFN.Visible = RBCFN.Visible = RBSFN.Visible = false;
                RFN.Visible = RBEFN.Visible = true;
                if (RTFN.Text == "") { RFN.Text = "First Name (Input Field)"; return; }
                RFN.Text = RTFN.Text;
            }
            else if (clickedButton == RBCFN) // Cancel
            {
                RTFN.Visible = RBCFN.Visible = RBSFN.Visible = false;
                RFN.Visible = RBEFN.Visible = true;
                RTFN.Text = RFN.Text;
            }
            // Last Name
            if (clickedButton == RBELN) // Edit
            {
                RTLN.Visible = RBCLN.Visible = RBSLN.Visible = true;
                RLN.Visible = RBELN.Visible = false;
                if (RLN.Text == "Last Name (Input Field)") { RTLN.Text = ""; return; }
                RTLN.Text = RLN.Text;
            }
            else if (clickedButton == RBSLN) // Save
            {
                RTLN.Visible = RBCLN.Visible = RBSLN.Visible = false;
                RLN.Visible = RBELN.Visible = true;
                if (RTLN.Text == "") { RLN.Text = "Last Name (Input Field)"; return; }
                RLN.Text = RTLN.Text;
            }
            else if (clickedButton == RBCLN) // Cancel
            {
                RTLN.Visible = RBCLN.Visible = RBSLN.Visible = false;
                RLN.Visible = RBELN.Visible = true;
                RTLN.Text = RLN.Text;
            }
            // Contact Info
            if (clickedButton == RBECI) // Edit
            {
                RTCI.Visible = RBCCI.Visible = RBSCI.Visible = true;
                RCI.Visible = RBECI.Visible = false;
                if (RCI.Text == "Email (Input Field)") { RTCI.Text = ""; return; }
                RTCI.Text = RCI.Text;
            }
            else if (clickedButton == RBSCI) // Save
            {
                RTCI.Visible = RBCCI.Visible = RBSCI.Visible = false;
                RCI.Visible = RBECI.Visible = true;
                if (RTCI.Text == "") { RCI.Text = "Email (Input Field)"; return; }
                RCI.Text = RTCI.Text;
            }
            else if (clickedButton == RBCCI) // Cancel
            {
                RTCI.Visible = RBCCI.Visible = RBSCI.Visible = false;
                RCI.Visible = RBECI.Visible = true;
                RTCI.Text = RCI.Text;
            }
            // Delivery Location
            if (clickedButton == RBEPL) // Edit
            {
                RTPL.Visible = RBCPL.Visible = RBSPL.Visible = true;
                RPL.Visible = RBEPL.Visible = false;
                if (RPL.Text == "Delivery Address (Input Field)") { RTPL.Text = ""; return; }
                RTPL.Text = RPL.Text;
            }
            else if (clickedButton == RBSPL) // Save
            {
                RTPL.Visible = RBCPL.Visible = RBSPL.Visible = false;
                RPL.Visible = RBEPL.Visible = true;
                if (RTPL.Text == "") { RPL.Text = "Delivery Address (Input Field)"; return; }
                RPL.Text = RTPL.Text;
                DeliveryAddress.Text = $"Delivery Address : {RPL.Text} ({DeliveryRegion.SelectedItem})";
            }
            else if (clickedButton == RBCPL) // Cancel
            {
                RTPL.Visible = RBCPL.Visible = RBSPL.Visible = false;
                RPL.Visible = RBEPL.Visible = true;
                RTPL.Text = RPL.Text;
            }
        }
        private void AddItemButtons(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton == null) { return; }
            if (clickedButton == AddItem)
            {
                AddItem.Visible = DeleteItem.Visible = false;
                TextItem.Visible = CancelItem.Visible = SaveItem.Visible = true;
            }
            else if (clickedButton == CancelItem)
            {
                TextItem.Text = "";
                AddItem.Visible = DeleteItem.Visible = true;
                TextItem.Visible = CancelItem.Visible = SaveItem.Visible = false;
            }
            else if (clickedButton == SaveItem)
            {
                if (string.IsNullOrEmpty(TextItem.Text) || Amount.ValueNumber == 0) { MessageBox.Show("Please fill in the Item Name."); return; }

                if (maxWeight * 1000 < totalWeight + (int)Weight.ValueNumber) { MessageBox.Show($"You can only add {maxWeight} KG of items."); return; }
                string formattedText = $"{TextItem.Text} , Amount : {Amount.ValueNumber} , Weight : {(Amount.ValueNumber * Weight.ValueNumber).ToString("N0")} Gram";
                MaterialListBoxItem newItem = new MaterialListBoxItem { Text = formattedText };
                ItemList.Items.Add(newItem);
                TextItem.Text = "";
                AddItem.Visible = DeleteItem.Visible = true;
                TextItem.Visible = CancelItem.Visible = SaveItem.Visible = false;
                Weight.ValueNumber = 1;
                Amount.ValueNumber = 1;
                CalculatePrice();
            }
            else if (clickedButton == DeleteItem)
            {
                if (ItemList.SelectedItem != null) { ItemList.Items.Remove(ItemList.SelectedItem); }
                CalculatePrice();
            }
        } // buttons
        private void TextBoxItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrEmpty(TextItem.Text) || Amount.ValueNumber == 0) { MessageBox.Show("Please fill in the Item Name."); return; }
                string formattedText = $"{TextItem.Text}, Amount : {Amount.ValueNumber}, Weight : {Weight.ValueNumber} KG";
                MaterialListBoxItem newItem = new MaterialListBoxItem { Text = formattedText };
                ItemList.Items.Add(newItem);
                TextItem.Text = "";
                AddItem.Visible = DeleteItem.Visible = true;
                TextItem.Visible = CancelItem.Visible = SaveItem.Visible = false;
                Weight.ValueNumber = 1;
                Amount.ValueNumber = 1;
                CalculatePrice();
            }
        } // keydown
        private void Weight_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { this.ActiveControl = null; e.SuppressKeyPress = true; }
        }
        private void Amount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { this.ActiveControl = null; e.SuppressKeyPress = true; }
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
        } // keydown
        private void ShippingSpeed_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculatePrice();
        } // change
        private void CargoType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CargoType.SelectedItem == null) return;

            string selectedType = CargoType.SelectedItem.ToString();
            PackageType.Items.Clear();

            if (selectedType == "Parcel")
            {
                PackageType.Items.AddRange(new string[] { "Small Box - 1 kg", "Medium Box - 3 kg", "Large Box - 5 kg", "XL Box - 10 kg" });
            }
            else if (selectedType == "Perishable Goods")
            {
                PackageType.Items.AddRange(new string[] { "Small Cooler Box - 2 kg", "Medium Cooler Box - 5 kg", "Large Cooler Box - 10 kg" });
            }
            else if (selectedType == "Fragile Items")
            {
                PackageType.Items.AddRange(new string[] { "Fragile Small Box - 1 kg", "Fragile Medium Box - 3 kg", "Fragile Crate - 5 – 10 kg" });
            }
            else if (selectedType == "Bulk Cargo")
            {
                PackageType.Items.AddRange(new string[] { "Half Pallet - 20 – 50 kg", "Full Pallet - 51 – 100 kg", "Oversized Crate - 100 kg+" });
            }
            else if (selectedType == "Hazardous Materials")
            {
                PackageType.Items.AddRange(new string[] { "Type A Flammable - 10 L", "Type B Compressed Gases - 20 L", "Type C Corrosives - 10 L" });
            }
            if (PackageType.SelectedIndex == -1) { PackageType.SelectedIndex = 0; }
            CalculatePrice();
        }
        private void PackageType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedPackage = PackageType.SelectedItem.ToString();
            if (selectedPackage == null) return;

            if (selectedPackage.Contains("Small Box"))
            { maxWeight = 1; packageFee = 60; }
            else if (selectedPackage.Contains("Medium Box"))
            { maxWeight = 3; packageFee = 90; }
            else if (selectedPackage.Contains("Large Box"))
            { maxWeight = 5; packageFee = 120; }
            else if (selectedPackage.Contains("XL Box"))
            { maxWeight = 10; packageFee = 180; }

            else if (selectedPackage.Contains("Small Cooler Box"))
            { maxWeight = 2; packageFee = 120; }
            else if (selectedPackage.Contains("Medium Cooler Box"))
            { maxWeight = 5; packageFee = 200; }
            else if (selectedPackage.Contains("Large Cooler Box"))
            { maxWeight = 10; packageFee = 300; }

            else if (selectedPackage.Contains("Fragile Small Box"))
            { maxWeight = 1; packageFee = 80; }
            else if (selectedPackage.Contains("Fragile Medium Box"))
            { maxWeight = 3; packageFee = 140; }
            else if (selectedPackage.Contains("Fragile Crate"))
            { maxWeight = 10; packageFee = 200; }

            else if (selectedPackage.Contains("Half Pallet"))
            { maxWeight = 50; packageFee = 600; }
            else if (selectedPackage.Contains("Full Pallet"))
            { maxWeight = 100; packageFee = 1000; }
            else if (selectedPackage.Contains("Oversized Crate"))
            { maxWeight = 200; packageFee = 1500; }

            else if (selectedPackage.Contains("Type A"))
            { maxWeight = 10; packageFee = 350; }
            else if (selectedPackage.Contains("Type B"))
            { maxWeight = 20; packageFee = 500; }
            else if (selectedPackage.Contains("Type C"))
            { maxWeight = 10; packageFee = 700; }

        }
        private void PaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PaymentMethod.SelectedItem == null) { return; }
            else if (PaymentMethod.SelectedItem.ToString() == "Cash On Delivery") { COD.Visible = true; EWallet.Visible = false; }
            else if (PaymentMethod.SelectedItem.ToString() == "E-Wallet") { EWallet.Visible = true; COD.Visible = false; }
        } // change       
        private void CalculatePrice()
        {
            totalWeight = 0;
            string cargoType = CargoType.SelectedItem.ToString();
            string shippingSpeed = ShippingSpeed.SelectedItem.ToString();
            string value = Value.SelectedItem.ToString();
            foreach (MaterialListBoxItem item in ItemList.Items)
            {
                string text = item.Text;
                string[] parts = text.Split(new char[] { ',' }, 3);

                int weight = int.Parse(Regex.Match(parts[2], @"\d[\d,]*").Value.Replace(",", ""));
                totalWeight += weight;
            }
            valueFee = ValueFeeConverter(value);
            weightFee = (totalWeight / 1000) * 20;
            shippingFee = shippingSpeed switch
            {
                "Standard Shipping (3-7 days, affordable option)" => 0,
                "Express Shipping (1-3 days, faster but higher cost)" => 300,
                "Overnight Shipping (Next-day delivery, for urgent shipments)" => 500,
                "Economy Shipping (5-10 days, cheapest but slowest option)" => -50,
                _ => 0
            };
            if (PickUpRegion.SelectedIndex != DeliveryRegion.SelectedIndex) { shippingFee += 100; }
            totalPrice = packageFee + weightFee + valueFee + shippingFee;
            PackageFee.Text = $"₱ {packageFee:N2}";
            WeightCost.Text = $"₱ {weightFee:N2}";
            ValueFee.Text = $"₱ {valueFee:N2}";
            DeliveryFee.Text = $"₱ {shippingFee:N2}";
            TotalCost.Text = $"₱ {totalPrice:N2}";
        } // Calculate Price
        private double ValueFeeConverter(string text)
        {
            double fee = 0;
            if (text.Contains("(Low Value)")) { fee = 30; }
            else if (text.Contains("(Basic Value)")) { fee = 50; }
            else if (text.Contains("(Standard Value)")) { fee = 150; }
            else if (text.Contains("(Medium Value)")) { fee = 250; }
            else if (text.Contains("(High Value)")) { fee = 500; }
            else if (text.Contains("(Premium Value)")) { fee = 1000; }
            return fee;
        }
        private void LoadUserData()
        {
            string query = "SELECT FullName, FirstName, LastName, Email, Username, Image, CreatedDate FROM UserAccounts WHERE AccountID = ?";
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
                            AccFN.Text = SFN.Text = reader["FirstName"].ToString();
                            AccLN.Text = SLN.Text = reader["LastName"].ToString();
                            AccEA.Text = SCI.Text = reader["Email"].ToString();
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
        } // Load User Data
        private void BookaShipment_Click(object sender, EventArgs e)
        {
            CalculatePrice();
            if (PaymentMethod.SelectedItem.ToString() == "E-Wallet")
            {
                if (CellNumber.TextLength < 11)
                {
                    MessageBox.Show("Enter Valid Number");
                    CellNumber.Text = "";
                    return;
                }
                cellNumber = CellNumber.Text;
                CellNumber.Text = "";
                eWallet = EWalletName.Text;
            }
            if (ItemList.Items.Count == 0) { MessageBox.Show("You Must Add an Item to Proceed!"); return; }
            if (maxWeight * 1000 < totalWeight) { MessageBox.Show($"You can only add {maxWeight} KG of items."); return; }
            if (string.IsNullOrWhiteSpace(SFN.Text) || SFN.Text.Contains("First Name (Input Field)") ||
            string.IsNullOrWhiteSpace(SLN.Text) || SLN.Text.Contains("Last Name (Input Field)") ||
            string.IsNullOrWhiteSpace(SCI.Text) || SCI.Text.Contains("Email (Input Field)") ||
            string.IsNullOrWhiteSpace(SPL.Text) || SPL.Text.Contains("Pick Up Address (Input Field)") ||
            string.IsNullOrWhiteSpace(RFN.Text) || RFN.Text.Contains("First Name (Input Field)") ||
            string.IsNullOrWhiteSpace(RLN.Text) || RLN.Text.Contains("Last Name (Input Field)") ||
            string.IsNullOrWhiteSpace(RCI.Text) || RCI.Text.Contains("Email (Input Field)") ||
            string.IsNullOrWhiteSpace(RPL.Text) || RPL.Text.Contains("Delivery Address (Input Field)"))
            {
                MessageBox.Show("Please fill in the required fields!", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ConfirmationBook.Visible = true;
            ConfirmationBook.Location = new Point(85, 80);
            BookShipment.Enabled = false;
            ConfirmationBook.BringToFront();
            // Sender Cancel's the buttons
            STPL.Visible = SBSPL.Visible = SBCFN.Visible = SBSFN.Visible = STFN.Visible = SBCPL.Visible = STCI.Visible = SBSCI.Visible = SBCCI.Visible = STLN.Visible = SBSLN.Visible = SBCLN.Visible = false;
            SPL.Visible = SBEPL.Visible = SBEFN.Visible = SFN.Visible = SCI.Visible = SBECI.Visible = SLN.Visible = SBELN.Visible = true;
            STFN.Text = SFN.Text;
            STLN.Text = SLN.Text;
            STCI.Text = SCI.Text;
            STPL.Text = SPL.Text;
            //Reciever Cancel's the buttons
            RTPL.Visible = RBCFN.Visible = RBSFN.Visible = RTFN.Visible = RBCLN.Visible = RBSLN.Visible = RTLN.Visible = RBCCI.Visible = RBSCI.Visible = RTCI.Visible = RBCPL.Visible = RBSPL.Visible = false;
            RPL.Visible = RBEFN.Visible = RFN.Visible = RBELN.Visible = RLN.Visible = RBECI.Visible = RCI.Visible = RBEPL.Visible = true;
            RTFN.Text = SFN.Text;
            RTLN.Text = SLN.Text;
            RTCI.Text = SCI.Text;
            RTPL.Text = SPL.Text;

            CSFN.Text = SFN.Text;
            CSLN.Text = SLN.Text;
            CSCI.Text = SCI.Text;
            CSPL.Text = SPL.Text;
            CRFN.Text = RFN.Text;
            CRLN.Text = RLN.Text;
            CRPL.Text = RPL.Text;
            CRCI.Text = RCI.Text;

            CCargo.Text = CargoType.SelectedItem.ToString();
            CPackage.Text = PackageType.SelectedItem.ToString();
            CShipping.Text = ShippingSpeed.SelectedItem.ToString();
            CPaymentMethod.Text = PaymentMethod.SelectedItem.ToString();

            CPackageFee.Text = $"₱ {packageFee:N2}";
            CWeightCost.Text = $"₱ {weightFee:N2}";
            CValueFee.Text = $"₱ {valueFee:N2}";
            CDeliveryFee.Text = $"₱ {shippingFee:N2}";
            CTotalCost.Text = $"₱ {totalPrice:N2}";

            CItemList.Items.Clear();
            foreach (var item in ItemList.Items) { CItemList.Items.Add(item); }
        }
        private void RefreshShipment_Click(object sender, EventArgs e) { MyShipmentLayout.Controls.Clear(); AddMyShipment(); }
        private void RefreshHistory_Click(object sender, EventArgs e) { HistoryLayout.Controls.Clear(); AddHistoryShipment(); }
        private void RefreshDashboard_Click(object sender, EventArgs e) { DashboardLayout.Controls.Clear(); AddQuickShipment(); PieChart(); }
        private void UpdatePass_Click(object sender, EventArgs e)
        {
            string confirmPass = login.HashPassword(CurrentPass.Text);
            string query = "SELECT Password FROM UserAccounts WHERE AccountID = ?";
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
                            string updateQuery = "UPDATE UserAccounts SET [Password] = ? WHERE AccountID = ?";
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
        private void PickUpRegion_SelectedIndexChanged(object sender, EventArgs e) { CalculatePrice(); PickAddress.Text = $"Pick Up Address : {SPL.Text} ({PickUpRegion.SelectedItem})"; }
        private void DeliveryRegion_SelectedIndexChanged(object sender, EventArgs e) { CalculatePrice(); DeliveryAddress.Text = $"Delivery Address : {RPL.Text} ({DeliveryRegion.SelectedItem})"; }
        private void ExitBooking_Click(object sender, EventArgs e) { ConfirmationBook.Visible = false; BookShipment.Enabled = true; }
        private void AddBook()
        {
            string trackingnumber = Nanoid.Generate(size: 10);
            myConn.Open();

            // 1. Insert into ShipmentItems
            string query1 = "INSERT INTO ShipmentItems ([UserID], [CargoType], [ItemLists], [ShippingSpeed], [Status], [PackageType]) VALUES (@UserID, @Cargo, @Items, @Speed, @status, @Package)";
            using (OleDbCommand cmd1 = new OleDbCommand(query1, myConn))
            {
                cmd1.Parameters.AddWithValue("@UserID", ID);
                cmd1.Parameters.AddWithValue("@Cargo", CargoType.SelectedItem.ToString());

                var items = new List<string>();
                foreach (MaterialListBoxItem item in ItemList.Items) { items.Add("[ " + item.Text + " ] "); }

                cmd1.Parameters.AddWithValue("@Items", string.Join("\n", items));
                cmd1.Parameters.AddWithValue("@Speed", ShippingSpeed.SelectedItem.ToString());
                cmd1.Parameters.AddWithValue("@status", "Pending");
                cmd1.Parameters.AddWithValue("@Package", PackageType.SelectedItem.ToString());

                cmd1.ExecuteNonQuery();
            }

            // 2. Insert into ShipmentInformation
            int shipmentID = 0;
            string query2 = "INSERT INTO ShipmentInformation ([UserID], [PickUpAddress], [RecieverFullName], [RecieverContact], [DeliveryAddress], [SenderFullName], [SenderContact], [TrackingNumber]) VALUES (@UserID, @PL, @FullName, @CI, @Delivery, @SFL, @SCI, @Track)";
            using (OleDbCommand cmd2 = new OleDbCommand(query2, myConn))
            {
                cmd2.Parameters.AddWithValue("@UserID", ID);
                cmd2.Parameters.AddWithValue("@PL", SPL.Text + " " + PickUpRegion.SelectedItem);
                cmd2.Parameters.AddWithValue("@FullName", RFN.Text + " " + RLN.Text);
                cmd2.Parameters.AddWithValue("@CI", RCI.Text);
                cmd2.Parameters.AddWithValue("@Delivery", RPL.Text + " " + DeliveryRegion.SelectedItem);
                cmd2.Parameters.AddWithValue("@SFL", SFN.Text + " " + SLN.Text);
                cmd2.Parameters.AddWithValue("@SCI", SCI.Text);
                cmd2.Parameters.AddWithValue("@Track", trackingnumber);
                cmd2.ExecuteNonQuery();

                // GET LAST INSERTED SHIPMENTID
                cmd2.CommandText = "SELECT @@IDENTITY";
                cmd2.Parameters.Clear();
                shipmentID = Convert.ToInt32(cmd2.ExecuteScalar());
            }

            // 3. Insert into AllCosts using shipmentID
            string query4 = "INSERT INTO AllCosts ([UserID], [ShipmentID], [CargoFee], [WeightFee], [ValueFee], [ShippingFee], [TotalCost]) VALUES (@UserID, @SID, @CFee, @WFee, @AFee, @SFee, @totalcost)";
            using (OleDbCommand cmd4 = new OleDbCommand(query4, myConn))
            {
                cmd4.Parameters.AddWithValue("@UserID", ID);
                cmd4.Parameters.AddWithValue("@SID", shipmentID);
                cmd4.Parameters.AddWithValue("@CFee", "₱ " + packageFee.ToString());
                cmd4.Parameters.AddWithValue("@WFee", "₱ " + weightFee.ToString());
                cmd4.Parameters.AddWithValue("@AFee", "₱ " + valueFee.ToString());
                cmd4.Parameters.AddWithValue("@SFee", "₱ " + shippingFee.ToString());
                cmd4.Parameters.AddWithValue("@totalcost", "₱ " + totalPrice.ToString());
                cmd4.ExecuteNonQuery();
            }

            // 4. Insert into PaymentInfo using shipmentID
            string query3 = "INSERT INTO PaymentInfo ([UserID], [ShipmentID], [TypeOfPayment], [EWalletName], [CPNumber]) VALUES (@UserID, @SID, @TypePay, @WalletName, @CellNo)";
            using (OleDbCommand cmd3 = new OleDbCommand(query3, myConn))
            {
                cmd3.Parameters.AddWithValue("@UserID", ID);
                cmd3.Parameters.AddWithValue("@SID", shipmentID);
                cmd3.Parameters.AddWithValue("@TypePay", PaymentMethod.SelectedItem.ToString());
                cmd3.Parameters.AddWithValue("@WalletName", eWallet);
                cmd3.Parameters.AddWithValue("@CellNo", cellNumber);
                cmd3.ExecuteNonQuery();
            }
            MessageBox.Show("Shipment Booked Successfully!");
            ItemList.Items.Clear();
            myConn.Close();
        }
        private void ConfirmBooking_Click(object sender, EventArgs e)
        {
            if (PaymentMethod.SelectedItem.ToString() == "E-Wallet")
            {
                PayQR.Visible = true;
                PayQR.BringToFront();
                PayQR.Location = new Point(485, 180);
                ConfirmationBook.Visible = false;
                return;
            }
            ConfirmationBook.Visible = false;
            BookShipment.Enabled = true;
            AddBook();
        }
        private void Value_SelectedIndexChanged(object sender, EventArgs e) { CalculatePrice(); }
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs) { currentFrame = (Bitmap)eventArgs.Frame.Clone(); Profile.BackgroundImage = (Bitmap)currentFrame.Clone(); }
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

                        string query = "UPDATE UserAccounts SET [Image] = @Image WHERE AccountID = @AccountID";
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
        private void ConfirmPay_Click(object sender, EventArgs e) 
        { 
            AddBook();
            PayQR.Visible = false; 
            BookShipment.Enabled = true;
            SendReceipt();
        }
        private void SendEmail(string recipientEmail, string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("caneteivan9214@gmail.com");
                mail.To.Add(recipientEmail);
                mail.Subject = subject;
                mail.Body = body;

                smtpServer.Port = 587;
                smtpServer.Credentials = new System.Net.NetworkCredential("caneteivan9214@gmail.com", "rtsv mvfj qbwo vzfa");
                smtpServer.EnableSsl = true;

                smtpServer.Send(mail);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine("SMTP Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error: " + ex.Message);
            }
        }
        private void Notification_Click(object sender, EventArgs e)
        {
            NotificationFlow.Location = new Point(930, 130);
            NotificationFlow.Visible = !NotificationFlow.Visible;
        }
        private void PieChart()
        {
            myConn.Open();
            int totalbooking = 0, approval = 0, shipped = 0, denied = 0, ongoing = 0;

            string query2 = "SELECT Status FROM ShipmentItemDetails WHERE UserID = ?";

            using (OleDbCommand cmd2 = new OleDbCommand(query2, myConn))
            {
                cmd2.Parameters.AddWithValue("?", ID);
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
        private void SendReceipt()
        {
            string email = string.Empty, fullname = "";
            string paymentMethod = PaymentMethod.SelectedItem.ToString(); 
            DateTime orderDate = DateTime.Now;

            // Shipment details
            string receiverName = "", cargoType = "", shippingSpeed = "",
                   pickupAddress = "", deliveryAddress = "", packageType = "", trackingNumber = "";

            // Step 1: Get shipment item details
            myConn.Open();
            string query2 = "SELECT RecieverFullName, CargoType, ShippingSpeed, PickUpAddress, DeliveryAddress, PackageType, TrackingNumber FROM ShipmentItemDetails WHERE UserID = ?";
            using (OleDbCommand cmd2 = new OleDbCommand(query2, myConn))
            {
                cmd2.Parameters.AddWithValue("?", ID);
                using (OleDbDataReader reader2 = cmd2.ExecuteReader())
                {
                    if (reader2.Read())
                    {
                        receiverName = reader2["RecieverFullName"].ToString();
                        cargoType = reader2["CargoType"].ToString();
                        shippingSpeed = reader2["ShippingSpeed"].ToString();
                        pickupAddress = reader2["PickUpAddress"].ToString();
                        deliveryAddress = reader2["DeliveryAddress"].ToString();
                        packageType = reader2["PackageType"].ToString();
                        trackingNumber = reader2["TrackingNumber"].ToString();
                    }
                }
            }
            myConn.Close();

            // Step 2: Get user email
            myConn.Open();
            string query = "SELECT Email, FullName FROM UserAccounts WHERE AccountID = ?";
            using (OleDbCommand cmd = new OleDbCommand(query, myConn))
            {
                cmd.Parameters.AddWithValue("?", ID);
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        email = reader["Email"].ToString();
                        fullname = reader["FullName"].ToString();
                    }
                }
            }
            myConn.Close();

            // Step 3: Compose the email
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("caneteivan9214@gmail.com");
            mail.To.Add(email);
            mail.Subject = "Portlink Shipment Confirmation";

            string htmlBody = $@"
<html>
<body>
    <div style='max-width: 600px; margin: auto; padding: 20px; border: 2px solid #4CAF50; font-family: Arial, sans-serif;'>
        <div style='text-align: center; margin-bottom: 20px;'>
            <img src='cid:LogoImageCid' width='150' />
        </div>
        <h2 style='text-align: center;'>Order Confirmation</h2>
        <p style='text-align: center;'>Hello <strong>{fullname}</strong>,</p>
        <p style='text-align: center;'>Thank you for your order! Here are your shipment details:</p>
        <table style='margin: auto;'>
            <tr><td><strong>Receiver Name:</strong></td><td>{receiverName}</td></tr>
            <tr><td><strong>Cargo Type:</strong></td><td>{cargoType}</td></tr>
            <tr><td><strong>Shipping Speed:</strong></td><td>{shippingSpeed}</td></tr>
            <tr><td><strong>Pick-up Address:</strong></td><td>{pickupAddress}</td></tr>
            <tr><td><strong>Delivery Address:</strong></td><td>{deliveryAddress}</td></tr>
            <tr><td><strong>Package Type:</strong></td><td>{packageType}</td></tr>
            <tr><td><strong>Tracking Number:</strong></td><td>{trackingNumber}</td></tr>
            <tr><td><strong>Payment Method:</strong></td><td>{paymentMethod}</td></tr>
            <tr><td><strong>Status:</strong></td><td>Pending - waiting for confirmation</td></tr>
        </table>
        <br/>
        <br/>
        <p style='text-align: center;'>Regards,<br/>Portlink Team</p>
    </div>
</body>
</html>";

            AlternateView altView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            mail.AlternateViews.Add(altView);

            // Step 4: Send the email
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new System.Net.NetworkCredential("caneteivan9214@gmail.com", "rtsv mvfj qbwo vzfa"); // Use App Password
            smtp.EnableSsl = true;
            smtp.Send(mail);

            MessageBox.Show("Order placed successfully!\nA confirmation receipt has been sent to your email.",
                            "Order Placed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

