using System.Data.OleDb;
using System.IO.Packaging;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace PortLink__Final_Project_
{
    internal class UserMaterialCards : UserButtonHandler, ImagePic
    {
        internal List<MaterialCard> GetQuickShipments(int userID, OleDbConnection myConn)
        {
            List<MaterialCard> cards = new List<MaterialCard>();

            try
            {
                myConn.Open();
                string query = "SELECT ShipmentID, PickUpAddress, DeliveryAddress, CargoType, Status, PackageType, ShippingSpeed, TrackingNumber FROM ShipmentItemDetails WHERE [UserID] = ?";
                using (OleDbCommand cmd = new OleDbCommand(query, myConn))
                {
                    cmd.Parameters.AddWithValue("?", userID);
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string status = reader["Status"].ToString();
                            if (status != "Delivered (Sent)" && status != "Denied")
                            {
                                MaterialCard card = new MaterialCard
                                {
                                    Size = new Size(320, 366),
                                };
                                MaterialLabel From = new MaterialLabel
                                {
                                    Text = $"From :",
                                    Size = new Size(286, 37),
                                    Location = new Point(143, 178),
                                    FontType = MaterialSkinManager.fontType.Button,
                                };
                                MaterialLabel PickUp = new MaterialLabel
                                {
                                    Text = $"{reader["PickUpAddress"]}",
                                    Size = new Size(160, 60),
                                    Location = new Point(143, 199),
                                    FontType = MaterialSkinManager.fontType.Body1,
                                };
                                MaterialLabel To = new MaterialLabel
                                {
                                    Text = $"To :",
                                    Size = new Size(29, 17),
                                    Location = new Point(143, 268),
                                };
                                MaterialLabel Destination = new MaterialLabel
                                {
                                    Text = $"{reader["DeliveryAddress"]}",
                                    Size = new Size(160, 60),
                                    Location = new Point(143, 288),
                                    FontType = MaterialSkinManager.fontType.Body1,
                                };
                                MaterialLabel Status = new MaterialLabel
                                {
                                    Text = "Status",
                                    Size = new Size(111, 23),
                                    Location = new Point(17, 14),
                                    FontType = MaterialSkinManager.fontType.H6,
                                    TextAlign = ContentAlignment.MiddleCenter,
                                };
                                MaterialLabel StatusText = new MaterialLabel
                                {
                                    Text = $"{reader["Status"]}",
                                    Size = new Size(137, 21),
                                    Location = new Point(1, 41),
                                    TextAlign = ContentAlignment.MiddleCenter,
                                };
                                MaterialLabel ShippingSpeed = new MaterialLabel
                                {
                                    Text = $"Shipping Speed :",
                                    Size = new Size(111, 20),
                                    Location = new Point(17, 70),
                                    FontType = MaterialSkinManager.fontType.Button,
                                };
                                MaterialLabel Speed = new MaterialLabel
                                {
                                    Text = $"{reader["ShippingSpeed"]}",
                                    Size = new Size(120, 96),
                                    Location = new Point(17, 90),
                                };
                                MaterialLabel PackageType = new MaterialLabel
                                {
                                    Text = $"Package Type :",
                                    Size = new Size(99, 21),
                                    Location = new Point(17, 198),
                                    FontType = MaterialSkinManager.fontType.Button,
                                };
                                MaterialLabel Package = new MaterialLabel
                                {
                                    Text = $"{reader["PackageType"]}",
                                    Size = new Size(111, 40),
                                    Location = new Point(17, 219),
                                };
                                MaterialLabel ShipmentID = new MaterialLabel
                                {
                                    Text = $"Ship ID: {reader["ShipmentID"]}",
                                    Size = new Size(111, 20),
                                    Location = new Point(17, 275),
                                    FontType = MaterialSkinManager.fontType.Button,
                                };
                                MaterialLabel CargoType = new MaterialLabel
                                {
                                    Text = $"{reader["CargoType"]}",
                                    TextAlign = ContentAlignment.MiddleCenter,
                                    Size = new Size(160, 24),
                                    Location = new Point(143, 14),
                                };
                                MaterialLabel TrackingNumber = new MaterialLabel
                                {
                                    Text = $"Tracking No. :",
                                    Size = new Size(90, 21),
                                    Location = new Point(17, 298),
                                    FontType = MaterialSkinManager.fontType.Button,
                                };
                                MaterialLabel Track = new MaterialLabel
                                {
                                    Text = $"{reader["TrackingNumber"]}",
                                    Size = new Size(120, 20),
                                    Location = new Point(17, 319),
                                };
                                PictureBox CargoPic = new PictureBox
                                {
                                    Size = new Size(160, 125),
                                    Location = new Point(143, 41),
                                    Image = Image.FromFile(ImagePic(reader["CargoType"].ToString())),
                                    SizeMode = PictureBoxSizeMode.StretchImage,
                                };

                                card.Controls.Add(PickUp);
                                card.Controls.Add(Destination);
                                card.Controls.Add(ShipmentID);
                                card.Controls.Add(CargoType);
                                card.Controls.Add(Status);
                                card.Controls.Add(CargoPic);
                                card.Controls.Add(StatusText);
                                card.Controls.Add(From);
                                card.Controls.Add(To);
                                card.Controls.Add(ShippingSpeed);
                                card.Controls.Add(Speed);
                                card.Controls.Add(PackageType);
                                card.Controls.Add(Package);
                                card.Controls.Add(TrackingNumber);
                                card.Controls.Add(Track);
                                cards.Add(card);
                            }
                        }
                    }
                }
                myConn.Close();
            }
            catch (Exception ex)
            {
                myConn.Close();
                MessageBox.Show("Error: " + ex.Message, "Debug - Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return cards;
        }
        internal List<MaterialCard> GetDetailedShipments(int userID, OleDbConnection myConn, Action<object, EventArgs> refreshAction)
        {
            bool visible = false;
            List<MaterialCard> cards = new List<MaterialCard>();
            var shipmentCards = new Dictionary<string, MaterialCard>();
            myConn.Open();
            string query = "SELECT ShipmentID, PickUpAddress, DeliveryAddress, CargoType, ShippingSpeed, Status, ItemLists, RecieverFullName, RecieverContact, TrackingNumber, PackageType FROM ShipmentItemDetails WHERE [UserID] = ?";
            using (OleDbCommand cmd = new OleDbCommand(query, myConn))
            {
                cmd.Parameters.AddWithValue("?", userID);
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int shipmentID = Convert.ToInt32(reader["ShipmentID"].ToString());
                        string shipmentId = reader["ShipmentID"].ToString();
                        string status = reader["Status"].ToString(), statustext = "Pending";
                        if (status == "Pending") { visible = true; }
                        else if (status == "Approved") { visible = false; }
                        else if (status != "Pending" || status != "Approved") { visible = false; }
                        if (status != "Pending") { statustext = "Approved"; }
                        if (status != "Delivered (Sent)" && status != "Denied")
                        {
                            MaterialLabel Title = new MaterialLabel
                            {
                                Text = $"{statustext}",
                                Size = new Size(578, 29),
                                Location = new Point(1, 1),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.H6,
                                TextAlign = ContentAlignment.MiddleCenter
                            };
                            MaterialCard card = new MaterialCard
                            {
                                Size = new Size(580, 433),
                            };
                            MaterialLabel Status = new MaterialLabel
                            {
                                Text = $"Status : {reader["Status"]}",
                                Size = new Size(546, 29),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.H5,
                                Location = new Point(17, 390),
                            };
                            MaterialLabel From = new MaterialLabel
                            {
                                Text = "From : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(48, 17),
                                Location = new Point(17, 55),
                            };
                            MaterialLabel To = new MaterialLabel
                            {
                                Text = "To : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(29, 17),
                                Location = new Point(206, 55),
                            };
                            MaterialLabel ReceiverNameT = new MaterialLabel
                            {
                                Text = "Receiver Name : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(108, 23),
                                Location = new Point(17, 139),
                            };
                            MaterialLabel ReceiverContactT = new MaterialLabel
                            {
                                Text = "Receiver Contact : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(120, 23),
                                Location = new Point(206, 139),
                            };
                            MaterialLabel ReceiverName = new MaterialLabel
                            {
                                Text = $"{reader["RecieverFullName"]}",
                                Size = new Size(170, 60),
                                Location = new Point(17, 159),
                            };
                            MaterialLabel ReceiverContact = new MaterialLabel
                            {
                                Text = $"{reader["RecieverContact"]}",
                                Size = new Size(170, 60),
                                Location = new Point(206, 159),
                            };
                            MaterialLabel PickUp = new MaterialLabel
                            {
                                Text = $"{reader["PickUpAddress"]}",
                                Size = new Size(170, 60),
                                Location = new Point(17, 76),
                            };
                            MaterialLabel Destination = new MaterialLabel
                            {
                                Text = $"{reader["DeliveryAddress"]}",
                                Size = new Size(170, 60),
                                Location = new Point(206, 75),
                            };
                            MaterialLabel CargoType = new MaterialLabel
                            {
                                Text = $"{reader["CargoType"]}",
                                TextAlign = ContentAlignment.MiddleCenter,
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Subtitle1,
                                Size = new Size(187, 24),
                                Location = new Point(382, 29),
                            };
                            PictureBox CargoPic = new PictureBox
                            {
                                Size = new Size(187, 146),
                                Location = new Point(382, 55),
                                Image = Image.FromFile(ImagePic(reader["CargoType"].ToString())),
                                SizeMode = PictureBoxSizeMode.StretchImage,
                            };
                            MaterialLabel ShipmentID = new MaterialLabel
                            {
                                Text = $"Ship ID: {shipmentID}",
                                Size = new Size(118, 17),
                                Location = new Point(374, 226),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel TrackingNumber = new MaterialLabel
                            {
                                Text = $"Tracking No. :",
                                Size = new Size(90, 21),
                                Location = new Point(250, 226),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Track = new MaterialLabel
                            {
                                Text = $"{reader["TrackingNumber"]}",
                                Size = new Size(120, 20),
                                Location = new Point(250, 246),
                            };
                            MaterialLabel ShippingSpeed = new MaterialLabel
                            {
                                Text = $"Shipping Speed :",
                                Size = new Size(111, 17),
                                Location = new Point(374, 243),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Shipping = new MaterialLabel
                            {
                                Text = $"{reader["ShippingSpeed"]}",
                                Size = new Size(191, 60),
                                Location = new Point(374, 265),
                            };
                            MaterialLabel Item = new MaterialLabel
                            {
                                Text = $"List Of Items",
                                TextAlign = ContentAlignment.MiddleCenter,
                                Size = new Size(218, 20),
                                Location = new Point(17, 223),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Subtitle1,
                            };
                            MaterialListBox ItemList = new MaterialListBox
                            {
                                Size = new Size(218, 97),
                                Location = new Point(17, 243),
                            };
                            MaterialLabel PackageType = new MaterialLabel
                            {
                                Text = $"Package Type :",
                                Size = new Size(100, 21),
                                Location = new Point(17, 343),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Package = new MaterialLabel
                            {
                                Text = $"{reader["PackageType"]}",
                                Size = new Size(210, 20),
                                Location = new Point(17, 364),
                            };
                            MaterialButton Ready = new MaterialButton
                            {
                                Text = "Ready",
                                Tag = shipmentID,
                                Size = new Size(152, 48),
                                Location = new Point(250, 336),
                                Visible = !visible,
                                AutoSize = false,
                            };
                            MaterialButton Cancel = new MaterialButton
                            {
                                Text = "Cancel",
                                Tag = shipmentID,
                                Size = new Size(313, 48),
                                Location = new Point(250, 336),
                                AutoSize = false,
                                Visible = visible,
                            };
                            MaterialButton MiniCancel = new MaterialButton
                            {
                                Text = "Cancel",
                                Tag = shipmentID,
                                Size = new Size(153, 48),
                                Location = new Point(410, 336),
                                Visible = !visible,
                                AutoSize = false
                            };

                            if (status == "Pickup in Progress" || status == "In Warehouse" || status == "Being Processed" || status == "In Transit" || status == "Out for Delivery" || status == "Approaching Destination") { Cancel.Visible = false; Ready.Visible = false; MiniCancel.Visible = false; }
                            Ready.Click += (sender, e) => { ReadyorCancel_Click(sender, e, myConn, shipmentID); refreshAction(sender, e); };
                            Cancel.Click += (sender, e) => { ReadyorCancel_Click(sender, e, myConn, shipmentID); refreshAction(sender, e); };
                            MiniCancel.Click += (sender, e) => { ReadyorCancel_Click(sender, e, myConn, shipmentID); refreshAction(sender, e); };

                            string items = reader["ItemLists"].ToString();
                            string[] itemArray = items.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string item in itemArray)
                            {
                                string cleanItem = item.Replace("[", "").Replace("]", "").Trim();
                                string[] itemParts = cleanItem.Split(',');
                                string name = itemParts[0].Trim();
                                ItemList.Items.Add(new MaterialListBoxItem(name));
                            }

                            card.Controls.Add(PackageType);
                            card.Controls.Add(Package);
                            card.Controls.Add(From);
                            card.Controls.Add(To);
                            card.Controls.Add(ReceiverNameT);
                            card.Controls.Add(ReceiverContactT);
                            card.Controls.Add(ReceiverName);
                            card.Controls.Add(ReceiverContact);
                            card.Controls.Add(PickUp);
                            card.Controls.Add(Destination);
                            card.Controls.Add(ShipmentID);
                            card.Controls.Add(CargoType);
                            card.Controls.Add(ShippingSpeed);
                            card.Controls.Add(Shipping);
                            card.Controls.Add(TrackingNumber);
                            card.Controls.Add(Track);
                            card.Controls.Add(CargoPic);
                            card.Controls.Add(Item);
                            card.Controls.Add(ItemList);
                            card.Controls.Add(Status);
                            card.Controls.Add(Title);
                            card.Controls.Add(Ready);
                            card.Controls.Add(Cancel);
                            card.Controls.Add(MiniCancel);
                            shipmentCards[shipmentId] = card;

                            cards.Add(card);
                        }
                    }
                }
            }
            string query2 = "SELECT ShipmentID, TotalCost FROM ShipmentPaymentDetails WHERE [UserID] = ?";
            using (OleDbCommand cmd2 = new OleDbCommand(query2, myConn))
            {
                cmd2.Parameters.AddWithValue("?", userID);
                using (OleDbDataReader reader2 = cmd2.ExecuteReader())
                {
                    while (reader2.Read())
                    {
                        string shipmentId = reader2["ShipmentID"].ToString();
                        if (shipmentCards.ContainsKey(shipmentId))
                        {
                            MaterialCard card = shipmentCards[shipmentId];


                            MaterialLabel TotalCost = new MaterialLabel
                            {
                                Text = $"Total Cost :",
                                Size = new Size(104, 24),
                                Location = new Point(250, 278),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.H6
                            };
                            MaterialLabel Total = new MaterialLabel
                            {
                                Text = $"{reader2["TotalCost"]}",
                                Size = new Size(118, 16),
                                Location = new Point(250, 304),
                            };

                            card.Controls.Add(TotalCost);
                            card.Controls.Add(Total);
                            cards.Add(card);
                        }
                    }
                }
            }
            myConn.Close();
            return cards;
        }
        internal List<MaterialCard> GetHistoryShipments(int userID, OleDbConnection myConn)
        {
            List<MaterialCard> cards = new List<MaterialCard>();
            myConn.Open();
            string query = "SELECT ShipmentID, PickUpAddress, DeliveryAddress, CargoType, ShippingSpeed, Status, ItemLists, RecieverFullName, RecieverContact, TrackingNumber, PackageType FROM ShipmentItemDetails WHERE [UserID] = ?";
            var shipmentCards = new Dictionary<string, MaterialCard>();

            using (OleDbCommand cmd = new OleDbCommand(query, myConn))
            {
                cmd.Parameters.AddWithValue("?", userID);
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string shipmentId = reader["ShipmentID"].ToString();
                        string status = reader["Status"].ToString();
                        if (status == "Delivered (Sent)" || status == "Denied")
                        {
                            MaterialCard card = new MaterialCard
                            {
                                Size = new Size(580, 410),
                            };
                            MaterialLabel Status = new MaterialLabel
                            {
                                Text = $"{reader["Status"]}",
                                Size = new Size(578, 29),
                                TextAlign = ContentAlignment.MiddleCenter,
                                FontType = MaterialSkin.MaterialSkinManager.fontType.H6,
                                Location = new Point(1, 1),
                            };
                            MaterialLabel From = new MaterialLabel
                            {
                                Text = "From : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(48, 17),
                                Location = new Point(15, 37),
                            };
                            MaterialLabel To = new MaterialLabel
                            {
                                Text = "To : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(29, 17),
                                Location = new Point(204, 37),
                            };
                            MaterialLabel ReceiverNameT = new MaterialLabel
                            {
                                Text = "Receiver Name : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(108, 23),
                                Location = new Point(15, 121),
                            };
                            MaterialLabel ReceiverContactT = new MaterialLabel
                            {
                                Text = "Receiver Contact : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(120, 23),
                                Location = new Point(204, 121),
                            };
                            MaterialLabel ReceiverName = new MaterialLabel
                            {
                                Text = $"{reader["RecieverFullName"]}",
                                Size = new Size(170, 60),
                                Location = new Point(15, 141),
                            };
                            MaterialLabel ReceiverContact = new MaterialLabel
                            {
                                Text = $"{reader["RecieverContact"]}",
                                Size = new Size(170, 60),
                                Location = new Point(204, 141),
                            };
                            MaterialLabel PickUp = new MaterialLabel
                            {
                                Text = $"{reader["PickUpAddress"]}",
                                Size = new Size(170, 60),
                                Location = new Point(15, 58),
                            };
                            MaterialLabel Destination = new MaterialLabel
                            {
                                Text = $"{reader["DeliveryAddress"]}",
                                Size = new Size(170, 60),
                                Location = new Point(204, 57),
                            };
                            MaterialLabel CargoType = new MaterialLabel
                            {
                                Text = $"{reader["CargoType"]}",
                                TextAlign = ContentAlignment.MiddleCenter,
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Subtitle1,
                                Size = new Size(187, 24),
                                Location = new Point(380, 28),
                            };
                            PictureBox CargoPic = new PictureBox
                            {
                                Size = new Size(187, 146),
                                Location = new Point(380, 55),
                                Image = Image.FromFile(ImagePic(reader["CargoType"].ToString())),
                                SizeMode = PictureBoxSizeMode.StretchImage,
                            };
                            MaterialLabel ShipmentID = new MaterialLabel
                            {
                                Text = $"Ship ID: {shipmentId}",
                                Size = new Size(104, 19),
                                Location = new Point(160, 211),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel TrackingNumber = new MaterialLabel
                            {
                                Text = $"Tracking No. :",
                                Size = new Size(90, 21),
                                Location = new Point(160, 230),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Track = new MaterialLabel
                            {
                                Text = $"{reader["TrackingNumber"]}",
                                Size = new Size(120, 20),
                                Location = new Point(160, 248),
                            };
                            MaterialLabel ShippingSpeed = new MaterialLabel
                            {
                                Text = $"Shipping Speed :",
                                Size = new Size(111, 17),
                                Location = new Point(160, 270),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Shipping = new MaterialLabel
                            {
                                Text = $"{reader["ShippingSpeed"]}",
                                Size = new Size(191, 60),
                                Location = new Point(160, 288),
                            };
                            MaterialLabel Item = new MaterialLabel
                            {
                                Text = $"List Of Items",
                                TextAlign = ContentAlignment.MiddleCenter,
                                Size = new Size(210, 22),
                                Location = new Point(357, 207),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Subtitle1,
                            };
                            MaterialListBox ItemList = new MaterialListBox
                            {
                                Size = new Size(210, 128),
                                Location = new Point(357, 230),
                            };
                            MaterialLabel PackageType = new MaterialLabel
                            {
                                Text = $"Package Type :",
                                Size = new Size(100, 21),
                                Location = new Point(357, 361),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Package = new MaterialLabel
                            {
                                Text = $"{reader["PackageType"]}",
                                Size = new Size(210, 20),
                                Location = new Point(357, 382),
                            };

                            card.Controls.Add(PackageType);
                            card.Controls.Add(Package);
                            card.Controls.Add(From);
                            card.Controls.Add(To);
                            card.Controls.Add(ReceiverNameT);
                            card.Controls.Add(ReceiverContactT);
                            card.Controls.Add(ReceiverName);
                            card.Controls.Add(ReceiverContact);
                            card.Controls.Add(PickUp);
                            card.Controls.Add(Destination);
                            card.Controls.Add(ShipmentID);
                            card.Controls.Add(CargoType);
                            card.Controls.Add(ShippingSpeed);
                            card.Controls.Add(Shipping);
                            card.Controls.Add(TrackingNumber);
                            card.Controls.Add(Track);
                            card.Controls.Add(CargoPic);
                            card.Controls.Add(Item);
                            card.Controls.Add(Status);
                            card.Controls.Add(ItemList);

                            string items = reader["ItemLists"].ToString();
                            string[] itemArray = items.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string item in itemArray)
                            {
                                string cleanItem = item.Replace("[", "").Replace("]", "").Trim();
                                string[] itemParts = cleanItem.Split(',');
                                string name = itemParts[0].Trim();
                                ItemList.Items.Add(new MaterialListBoxItem(name));
                            }
                            shipmentCards[shipmentId] = card;
                            cards.Add(card);
                        }
                    }
                }
            }

            string query2 = "SELECT ShipmentID, CargoFee, WeightFee, ValueFee, ShippingFee, TotalCost FROM ShipmentPaymentDetails WHERE [UserID] = ?";
            using (OleDbCommand cmd2 = new OleDbCommand(query2, myConn))
            {
                cmd2.Parameters.AddWithValue("?", userID);
                using (OleDbDataReader reader2 = cmd2.ExecuteReader())
                {
                    while (reader2.Read())
                    {
                        string shipmentId = reader2["ShipmentID"].ToString();
                        if (shipmentCards.ContainsKey(shipmentId))
                        {
                            MaterialCard card = shipmentCards[shipmentId];

                            MaterialLabel ShippingFee = new MaterialLabel
                            {
                                Text = $"Shipping Fee : ",
                                Size = new Size(90, 18),
                                Location = new Point(15, 212),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Shipping = new MaterialLabel
                            {
                                Text = $"{reader2["ShippingFee"]}",
                                Size = new Size(130, 16),
                                Location = new Point(15, 236),
                            };
                            MaterialLabel PackageFee = new MaterialLabel
                            {
                                Text = $"Package Fee : ",
                                Size = new Size(90, 18),
                                Location = new Point(15, 257),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Package = new MaterialLabel
                            {
                                Text = $"{reader2["CargoFee"]}",
                                Size = new Size(130, 16),
                                Location = new Point(15, 282),
                            };
                            MaterialLabel WeightFee = new MaterialLabel
                            {
                                Text = $"Weight Fee :",
                                Size = new Size(80, 18),
                                Location = new Point(15, 307),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Weight = new MaterialLabel
                            {
                                Text = $"{reader2["WeightFee"]}",
                                Size = new Size(130, 16),
                                Location = new Point(15, 331),
                            };
                            MaterialLabel ValueFee = new MaterialLabel
                            {
                                Text = $"Value Fee :",
                                Size = new Size(85, 18),
                                Location = new Point(15, 355),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Value = new MaterialLabel
                            {
                                Text = $"{reader2["ValueFee"]}",
                                Size = new Size(130, 16),
                                Location = new Point(15, 378),
                            };
                            MaterialLabel TotalCost = new MaterialLabel
                            {
                                Text = $"Total Cost :",
                                Size = new Size(104, 24),
                                Location = new Point(160, 355),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.H6
                            };
                            MaterialLabel Total = new MaterialLabel
                            {
                                Text = $"{reader2["TotalCost"]}",
                                Size = new Size(130, 16),
                                Location = new Point(160, 380),
                            };

                            card.Controls.Add(ShippingFee);
                            card.Controls.Add(PackageFee);
                            card.Controls.Add(WeightFee);
                            card.Controls.Add(ValueFee);
                            card.Controls.Add(TotalCost);
                            card.Controls.Add(Weight);
                            card.Controls.Add(Value);
                            card.Controls.Add(Shipping);
                            card.Controls.Add(Package);
                            card.Controls.Add(Total);
                            cards.Add(card);
                        }
                    }
                }
            }
            myConn.Close();
            return cards;
        }
        internal List<MaterialCard> GetHistoryShipmentsID(int userID, OleDbConnection myConn, string ID)
        {
            List<MaterialCard> cards = new List<MaterialCard>();
            if (string.IsNullOrEmpty(ID)) return cards;
            myConn.Open();
            string query = "SELECT ShipmentID, PickUpAddress, DeliveryAddress, CargoType, ShippingSpeed, Status, ItemLists, RecieverFullName, TrackingNumber, RecieverContact, PackageType FROM ShipmentItemDetails WHERE ([ShipmentID] LIKE ? OR [TrackingNumber] LIKE ?) AND [UserID] = ?";
            var shipmentCards = new Dictionary<string, MaterialCard>();

            using (OleDbCommand cmd = new OleDbCommand(query, myConn))
            {
                cmd.Parameters.AddWithValue("?", "%" + ID + "%");
                cmd.Parameters.AddWithValue("?", "%" + ID + "%");
                cmd.Parameters.AddWithValue("?", userID);
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string shipmentId = reader["ShipmentID"].ToString();
                        string status = reader["Status"].ToString();

                        if (status == "Delivered (Sent)" || status == "Denied")
                        {
                            MaterialCard card = new MaterialCard
                            {
                                Size = new Size(580, 410),
                            };
                            MaterialLabel Status = new MaterialLabel
                            {
                                Text = $"{reader["Status"]}",
                                Size = new Size(578, 29),
                                TextAlign = ContentAlignment.MiddleCenter,
                                FontType = MaterialSkin.MaterialSkinManager.fontType.H6,
                                Location = new Point(1, 1),
                            };
                            MaterialLabel From = new MaterialLabel
                            {
                                Text = "From : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(48, 17),
                                Location = new Point(15, 37),
                            };
                            MaterialLabel To = new MaterialLabel
                            {
                                Text = "To : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(29, 17),
                                Location = new Point(204, 37),
                            };
                            MaterialLabel ReceiverNameT = new MaterialLabel
                            {
                                Text = "Receiver Name : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(108, 23),
                                Location = new Point(15, 121),
                            };
                            MaterialLabel ReceiverContactT = new MaterialLabel
                            {
                                Text = "Receiver Contact : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(120, 23),
                                Location = new Point(204, 121),
                            };
                            MaterialLabel ReceiverName = new MaterialLabel
                            {
                                Text = $"{reader["RecieverFullName"]}",
                                Size = new Size(170, 60),
                                Location = new Point(15, 141),
                            };
                            MaterialLabel ReceiverContact = new MaterialLabel
                            {
                                Text = $"{reader["RecieverContact"]}",
                                Size = new Size(170, 60),
                                Location = new Point(204, 141),
                            };
                            MaterialLabel PickUp = new MaterialLabel
                            {
                                Text = $"{reader["PickUpAddress"]}",
                                Size = new Size(170, 60),
                                Location = new Point(15, 58),
                            };
                            MaterialLabel Destination = new MaterialLabel
                            {
                                Text = $"{reader["DeliveryAddress"]}",
                                Size = new Size(170, 60),
                                Location = new Point(204, 57),
                            };
                            MaterialLabel CargoType = new MaterialLabel
                            {
                                Text = $"{reader["CargoType"]}",
                                TextAlign = ContentAlignment.MiddleCenter,
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Subtitle1,
                                Size = new Size(187, 24),
                                Location = new Point(380, 28),
                            };
                            PictureBox CargoPic = new PictureBox
                            {
                                Size = new Size(187, 146),
                                Location = new Point(380, 55),
                                Image = Image.FromFile(ImagePic(reader["CargoType"].ToString())),
                                SizeMode = PictureBoxSizeMode.StretchImage,
                            };
                            MaterialLabel ShipmentID = new MaterialLabel
                            {
                                Text = $"Ship ID: {shipmentId}",
                                Size = new Size(104, 19),
                                Location = new Point(160, 211),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel TrackingNumber = new MaterialLabel
                            {
                                Text = $"Tracking No. :",
                                Size = new Size(90, 21),
                                Location = new Point(160, 230),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Track = new MaterialLabel
                            {
                                Text = $"{reader["TrackingNumber"]}",
                                Size = new Size(120, 20),
                                Location = new Point(160, 248),
                            };
                            MaterialLabel ShippingSpeed = new MaterialLabel
                            {
                                Text = $"Shipping Speed :",
                                Size = new Size(111, 17),
                                Location = new Point(160, 270),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Shipping = new MaterialLabel
                            {
                                Text = $"{reader["ShippingSpeed"]}",
                                Size = new Size(191, 60),
                                Location = new Point(160, 288),
                            };
                            MaterialLabel Item = new MaterialLabel
                            {
                                Text = $"List Of Items",
                                TextAlign = ContentAlignment.MiddleCenter,
                                Size = new Size(210, 22),
                                Location = new Point(357, 207),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Subtitle1,
                            };
                            MaterialListBox ItemList = new MaterialListBox
                            {
                                Size = new Size(210, 128),
                                Location = new Point(357, 230),
                            };
                            MaterialLabel PackageType = new MaterialLabel
                            {
                                Text = $"Package Type :",
                                Size = new Size(100, 21),
                                Location = new Point(357, 361),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Package = new MaterialLabel
                            {
                                Text = $"{reader["PackageType"]}",
                                Size = new Size(210, 20),
                                Location = new Point(357, 382),
                            };

                            card.Controls.Add(PackageType);
                            card.Controls.Add(Package);
                            card.Controls.Add(From);
                            card.Controls.Add(To);
                            card.Controls.Add(ReceiverNameT);
                            card.Controls.Add(ReceiverContactT);
                            card.Controls.Add(ReceiverName);
                            card.Controls.Add(ReceiverContact);
                            card.Controls.Add(PickUp);
                            card.Controls.Add(Destination);
                            card.Controls.Add(ShipmentID);
                            card.Controls.Add(CargoType);
                            card.Controls.Add(ShippingSpeed);
                            card.Controls.Add(Shipping);
                            card.Controls.Add(TrackingNumber);
                            card.Controls.Add(Track);
                            card.Controls.Add(CargoPic);
                            card.Controls.Add(Item);
                            card.Controls.Add(Status);
                            card.Controls.Add(ItemList);

                            string items = reader["ItemLists"].ToString();
                            string[] itemArray = items.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string item in itemArray)
                            {
                                string cleanItem = item.Replace("[", "").Replace("]", "").Trim();
                                string[] itemParts = cleanItem.Split(',');
                                string name = itemParts[0].Trim();
                                ItemList.Items.Add(new MaterialListBoxItem(name));
                            }
                            shipmentCards[shipmentId] = card;
                            cards.Add(card);
                        }
                    }
                }
            }

            string query2 = "SELECT ShipmentID, CargoFee, WeightFee, ValueFee, ShippingFee, TotalCost FROM ShipmentPaymentDetails WHERE ([ShipmentID] LIKE ? OR [TrackingNumber] LIKE ?) AND [UserID] = ?";
            using (OleDbCommand cmd2 = new OleDbCommand(query2, myConn))
            {
                cmd2.Parameters.AddWithValue("?", "%" + ID + "%");
                cmd2.Parameters.AddWithValue("?", "%" + ID + "%");
                cmd2.Parameters.AddWithValue("?", userID);
                using (OleDbDataReader reader2 = cmd2.ExecuteReader())
                {
                    while (reader2.Read())
                    {
                        string shipmentId = reader2["ShipmentID"].ToString();
                        if (shipmentCards.ContainsKey(shipmentId))
                        {
                            MaterialCard card = shipmentCards[shipmentId];

                            MaterialLabel ShippingFee = new MaterialLabel
                            {
                                Text = $"Shipping Fee : ",
                                Size = new Size(90, 18),
                                Location = new Point(15, 212),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Shipping = new MaterialLabel
                            {
                                Text = $"{reader2["ShippingFee"]}",
                                Size = new Size(130, 16),
                                Location = new Point(15, 236),
                            };
                            MaterialLabel CargoFee = new MaterialLabel
                            {
                                Text = $"Package Fee : ",
                                Size = new Size(90, 18),
                                Location = new Point(15, 257),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Cargo = new MaterialLabel
                            {
                                Text = $"{reader2["CargoFee"]}",
                                Size = new Size(130, 16),
                                Location = new Point(15, 282),
                            };
                            MaterialLabel WeightFee = new MaterialLabel
                            {
                                Text = $"Weight Fee :",
                                Size = new Size(80, 18),
                                Location = new Point(15, 307),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Weight = new MaterialLabel
                            {
                                Text = $"{reader2["WeightFee"]}",
                                Size = new Size(130, 16),
                                Location = new Point(15, 331),
                            };
                            MaterialLabel AmountFee = new MaterialLabel
                            {
                                Text = $"Value Fee :",
                                Size = new Size(85, 18),
                                Location = new Point(15, 355),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Amount = new MaterialLabel
                            {
                                Text = $"{reader2["ValueFee"]}",
                                Size = new Size(130, 16),
                                Location = new Point(15, 378),
                            };
                            MaterialLabel TotalCost = new MaterialLabel
                            {
                                Text = $"Total Cost :",
                                Size = new Size(104, 24),
                                Location = new Point(160, 355),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.H6
                            };
                            MaterialLabel Total = new MaterialLabel
                            {
                                Text = $"{reader2["TotalCost"]}",
                                Size = new Size(130, 16),
                                Location = new Point(160, 380),
                            };

                            card.Controls.Add(ShippingFee);
                            card.Controls.Add(CargoFee);
                            card.Controls.Add(WeightFee);
                            card.Controls.Add(AmountFee);
                            card.Controls.Add(TotalCost);
                            card.Controls.Add(Weight);
                            card.Controls.Add(Amount);
                            card.Controls.Add(Shipping);
                            card.Controls.Add(Cargo);
                            card.Controls.Add(Total);
                            cards.Add(card);
                        }
                    }
                }
            }

            myConn.Close();
            return cards;
        }
        internal List<MaterialCard> GetDetailedShipmentsID(int userID, OleDbConnection myConn, string ID, Action<object, EventArgs> refreshAction)
        {
            List<MaterialCard> cards = new List<MaterialCard>();
            var shipmentCards = new Dictionary<string, MaterialCard>();
            if (string.IsNullOrEmpty(ID)) return cards;

            myConn.Open();
            string query = "SELECT ShipmentID, PickUpAddress, DeliveryAddress, CargoType, ShippingSpeed, Status, ItemLists, RecieverFullName, RecieverContact, TrackingNumber, PackageType FROM ShipmentItemDetails WHERE ([ShipmentID] LIKE ? OR [TrackingNumber] LIKE ?) AND [UserID] = ?";
            using (OleDbCommand cmd = new OleDbCommand(query, myConn))
            {
                cmd.Parameters.AddWithValue("?", "%" + ID + "%");
                cmd.Parameters.AddWithValue("?", "%" + ID + "%");
                cmd.Parameters.AddWithValue("?", userID);
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int shipmentID = Convert.ToInt32(reader["ShipmentID"].ToString());
                        string shipmentId = reader["ShipmentID"].ToString();
                        string status = reader["Status"].ToString();
                        if (status != "Pending") { status = "Approved"; }
                        if (status != "Delivered (Sent)" || status != "Denied")
                        {
                            MaterialLabel Title = new MaterialLabel
                            {
                                Text = $"{status}",
                                Size = new Size(578, 29),
                                Location = new Point(1, 1),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.H6,
                                TextAlign = ContentAlignment.MiddleCenter
                            };
                            MaterialCard card = new MaterialCard
                            {
                                Size = new Size(580, 433),
                            };
                            MaterialLabel Status = new MaterialLabel
                            {
                                Text = $"Status : {reader["Status"]}",
                                Size = new Size(546, 29),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.H5,
                                Location = new Point(17, 390),
                            };
                            MaterialLabel From = new MaterialLabel
                            {
                                Text = "From : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(48, 17),
                                Location = new Point(17, 55),
                            };
                            MaterialLabel To = new MaterialLabel
                            {
                                Text = "To : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(29, 17),
                                Location = new Point(206, 55),
                            };
                            MaterialLabel ReceiverNameT = new MaterialLabel
                            {
                                Text = "Receiver Name : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(108, 23),
                                Location = new Point(17, 139),
                            };
                            MaterialLabel ReceiverContactT = new MaterialLabel
                            {
                                Text = "Receiver Contact : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(120, 23),
                                Location = new Point(206, 139),
                            };
                            MaterialLabel ReceiverName = new MaterialLabel
                            {
                                Text = $"{reader["RecieverFullName"]}",
                                Size = new Size(170, 60),
                                Location = new Point(17, 159),
                            };
                            MaterialLabel ReceiverContact = new MaterialLabel
                            {
                                Text = $"{reader["RecieverContact"]}",
                                Size = new Size(170, 60),
                                Location = new Point(206, 159),
                            };
                            MaterialLabel PickUp = new MaterialLabel
                            {
                                Text = $"{reader["PickUpAddress"]}",
                                Size = new Size(170, 60),
                                Location = new Point(17, 76),
                            };
                            MaterialLabel Destination = new MaterialLabel
                            {
                                Text = $"{reader["DeliveryAddress"]}",
                                Size = new Size(170, 60),
                                Location = new Point(206, 75),
                            };
                            MaterialLabel CargoType = new MaterialLabel
                            {
                                Text = $"{reader["CargoType"]}",
                                TextAlign = ContentAlignment.MiddleCenter,
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Subtitle1,
                                Size = new Size(187, 24),
                                Location = new Point(382, 29),
                            };
                            PictureBox CargoPic = new PictureBox
                            {
                                Size = new Size(187, 146),
                                Location = new Point(382, 55),
                                Image = Image.FromFile(ImagePic(reader["CargoType"].ToString())),
                                SizeMode = PictureBoxSizeMode.StretchImage,
                            };
                            MaterialLabel ShipmentID = new MaterialLabel
                            {
                                Text = $"Ship ID: {shipmentID}",
                                Size = new Size(118, 17),
                                Location = new Point(374, 226),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel TrackingNumber = new MaterialLabel
                            {
                                Text = $"Tracking No. :",
                                Size = new Size(90, 21),
                                Location = new Point(250, 226),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Track = new MaterialLabel
                            {
                                Text = $"{reader["TrackingNumber"]}",
                                Size = new Size(120, 20),
                                Location = new Point(250, 246),
                            };
                            MaterialLabel ShippingSpeed = new MaterialLabel
                            {
                                Text = $"Shipping Speed :",
                                Size = new Size(111, 17),
                                Location = new Point(374, 243),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Shipping = new MaterialLabel
                            {
                                Text = $"{reader["ShippingSpeed"]}",
                                Size = new Size(191, 60),
                                Location = new Point(374, 265),
                            };
                            MaterialLabel Item = new MaterialLabel
                            {
                                Text = $"List Of Items",
                                TextAlign = ContentAlignment.MiddleCenter,
                                Size = new Size(218, 20),
                                Location = new Point(17, 223),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Subtitle1,
                            };
                            MaterialListBox ItemList = new MaterialListBox
                            {
                                Size = new Size(218, 97),
                                Location = new Point(17, 243),
                            };
                            MaterialLabel PackageType = new MaterialLabel
                            {
                                Text = $"Package Type :",
                                Size = new Size(100, 21),
                                Location = new Point(17, 343),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Package = new MaterialLabel
                            {
                                Text = $"{reader["PackageType"]}",
                                Size = new Size(210, 20),
                                Location = new Point(17, 364),
                            };
                            MaterialButton Ready = new MaterialButton
                            {
                                Text = "Ready",
                                Tag = shipmentID,
                                Size = new Size(152, 48),
                                Location = new Point(250, 336),
                                Visible = !reader["Status"].ToString().Contains("Pending"),
                                AutoSize = false,
                            };
                            MaterialButton Cancel = new MaterialButton
                            {
                                Text = "Cancel",
                                Tag = shipmentID,
                                Size = new Size(313, 48),
                                Location = new Point(250, 336),
                                AutoSize = false,
                                Visible = reader["Status"].ToString().Contains("Pending")
                            };
                            MaterialButton MiniCancel = new MaterialButton
                            {
                                Text = "Cancel",
                                Tag = shipmentID,
                                Size = new Size(153, 48),
                                Location = new Point(410, 336),
                                Visible = !reader["Status"].ToString().Contains("Pending"),
                                AutoSize = false
                            };

                            Ready.Click += (sender, e) => { ReadyorCancel_Click(sender, e, myConn, shipmentID); refreshAction(sender, e); };
                            Cancel.Click += (sender, e) => { ReadyorCancel_Click(sender, e, myConn, shipmentID); refreshAction(sender, e); };
                            MiniCancel.Click += (sender, e) => { ReadyorCancel_Click(sender, e, myConn, shipmentID); refreshAction(sender, e); };

                            string items = reader["ItemLists"].ToString();
                            string[] itemArray = items.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string item in itemArray)
                            {
                                string cleanItem = item.Replace("[", "").Replace("]", "").Trim();
                                string[] itemParts = cleanItem.Split(',');
                                string name = itemParts[0].Trim();
                                ItemList.Items.Add(new MaterialListBoxItem(name));
                            }

                            card.Controls.Add(PackageType);
                            card.Controls.Add(Package);
                            card.Controls.Add(From);
                            card.Controls.Add(To);
                            card.Controls.Add(ReceiverNameT);
                            card.Controls.Add(ReceiverContactT);
                            card.Controls.Add(ReceiverName);
                            card.Controls.Add(ReceiverContact);
                            card.Controls.Add(PickUp);
                            card.Controls.Add(Destination);
                            card.Controls.Add(ShipmentID);
                            card.Controls.Add(CargoType);
                            card.Controls.Add(ShippingSpeed);
                            card.Controls.Add(Shipping);
                            card.Controls.Add(TrackingNumber);
                            card.Controls.Add(Track);
                            card.Controls.Add(CargoPic);
                            card.Controls.Add(Item);
                            card.Controls.Add(Status);
                            card.Controls.Add(ItemList);
                            card.Controls.Add(Title);
                            card.Controls.Add(Ready);
                            card.Controls.Add(Cancel);
                            card.Controls.Add(MiniCancel);
                            shipmentCards[shipmentId] = card;

                            cards.Add(card);
                        }
                    }
                }
            }
            string query2 = "SELECT ShipmentID, TotalCost, TrackingNumber FROM ShipmentPaymentDetails WHERE ([ShipmentID] LIKE ? OR [TrackingNumber] LIKE ?) AND [UserID] = ?";
            using (OleDbCommand cmd2 = new OleDbCommand(query2, myConn))
            {
                cmd2.Parameters.AddWithValue("?", "%" + ID + "%");
                cmd2.Parameters.AddWithValue("?", "%" + ID + "%");
                cmd2.Parameters.AddWithValue("?", userID);
                using (OleDbDataReader reader2 = cmd2.ExecuteReader())
                {
                    while (reader2.Read())
                    {
                        string shipmentId = reader2["ShipmentID"].ToString();
                        if (shipmentCards.ContainsKey(shipmentId))
                        {
                            MaterialCard card = shipmentCards[shipmentId];


                            MaterialLabel TotalCost = new MaterialLabel
                            {
                                Text = $"Total Cost :",
                                Size = new Size(104, 24),
                                Location = new Point(250, 278),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.H6
                            };
                            MaterialLabel Total = new MaterialLabel
                            {
                                Text = $"{reader2["TotalCost"]}",
                                Size = new Size(118, 16),
                                Location = new Point(250, 304),
                            };

                            card.Controls.Add(TotalCost);
                            card.Controls.Add(Total);
                            cards.Add(card);
                        }
                    }
                }
            }
            myConn.Close();


            return cards;
        }
        public string ImagePic(string CargoType)
        {
            string parcelPic = @"C:\Users\Ivan\Documents\PortLink (Final Project)\Images\Parcel.png",
                perishablePic = @"C:\Users\Ivan\Documents\PortLink (Final Project)\Images\Perishable Items.png",
                cargoBulkPic = @"C:\Users\Ivan\Documents\PortLink (Final Project)\Images\Cargo Bulk Pic.png",
                fragilePic = @"C:\Users\Ivan\Documents\PortLink (Final Project)\Images\Fragile Pic.png",
                hazardousPic = @"C:\Users\Ivan\Documents\PortLink (Final Project)\Images\Hazardous Pic.png";

            if (CargoType == "Parcel") { return parcelPic; }
            else if (CargoType == "Perishable Goods") { return perishablePic; }
            else if (CargoType == "Bulk Cargo") { return cargoBulkPic; }
            else if (CargoType == "Fragile Items") { return fragilePic; }
            else if (CargoType == "Hazardous Materials") { return hazardousPic; }
            return string.Empty;
        }
    }
}
