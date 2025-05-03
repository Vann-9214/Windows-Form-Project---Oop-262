using System.Data.OleDb;
using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.VisualBasic.ApplicationServices;

namespace PortLink__Final_Project_
{
    internal class AdminMaterialCards : AdminButtonHandler, ImagePic
    {
        public bool clicked = false;
        internal List<MaterialCard> GetManageShipments(OleDbConnection myConn)
        {
            List<MaterialCard> cards = new List<MaterialCard>();
            var shipmentCards = new Dictionary<string, MaterialCard>();
            myConn.Open();
            string query = "SELECT [ShipmentID], [PickUpAddress], [DeliveryAddress], [CargoType], [ShippingSpeed], [Status], [ItemLists], [RecieverFullName], [RecieverContact], [TrackingNumber], [PackageType] FROM ShipmentItemDetails";
            using (OleDbCommand cmd = new OleDbCommand(query, myConn))
            {
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int shipmentID = Convert.ToInt32(reader["ShipmentID"]);
                        string shipmentId = reader["ShipmentID"].ToString();
                        string status = reader["Status"].ToString();
                        if (status == "Pending")
                        {
                            MaterialCard card = new MaterialCard
                            {
                                Size = new Size(580, 376),
                            };
                            MaterialLabel From = new MaterialLabel
                            {
                                Text = "From : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(48, 17),
                                Location = new Point(11, 35),
                            };
                            MaterialLabel To = new MaterialLabel
                            {
                                Text = "To : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(29, 17),
                                Location = new Point(200, 35),
                            };
                            MaterialLabel ReceiverNameT = new MaterialLabel
                            {
                                Text = "Receiver Name : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(108, 23),
                                Location = new Point(11, 119),
                            };
                            MaterialLabel ReceiverContactT = new MaterialLabel
                            {
                                Text = "Receiver Contact : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(120, 23),
                                Location = new Point(200, 119),
                            };
                            MaterialLabel ReceiverName = new MaterialLabel
                            {
                                Text = $"{reader["RecieverFullName"]}",
                                Size = new Size(170, 60),
                                Location = new Point(11, 139),
                            };
                            MaterialLabel ReceiverContact = new MaterialLabel
                            {
                                Text = $"{reader["RecieverContact"]}",
                                Size = new Size(170, 60),
                                Location = new Point(200, 139),
                            };
                            MaterialLabel PickUp = new MaterialLabel
                            {
                                Text = $"{reader["PickUpAddress"]}",
                                Size = new Size(170, 60),
                                Location = new Point(11, 56),
                            };
                            MaterialLabel Destination = new MaterialLabel
                            {
                                Text = $"{reader["DeliveryAddress"]}",
                                Size = new Size(170, 60),
                                Location = new Point(200, 55),
                            };
                            MaterialLabel CargoType = new MaterialLabel
                            {
                                Text = $"{reader["CargoType"]}",
                                TextAlign = ContentAlignment.MiddleCenter,
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Subtitle1,
                                Size = new Size(187, 24),
                                Location = new Point(376, 9),
                            };
                            PictureBox CargoPic = new PictureBox
                            {
                                Size = new Size(187, 146),
                                Location = new Point(376, 35),
                                Image = Image.FromFile(ImagePic(reader["CargoType"].ToString())),
                                SizeMode = PictureBoxSizeMode.StretchImage,
                            };
                            MaterialLabel ShipmentID = new MaterialLabel
                            {
                                Text = $"Ship ID: {shipmentID}",
                                Size = new Size(118, 17),
                                Location = new Point(368, 206),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel TrackingNumber = new MaterialLabel
                            {
                                Text = $"Tracking No. :",
                                Size = new Size(90, 21),
                                Location = new Point(244, 206),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Track = new MaterialLabel
                            {
                                Text = $"{reader["TrackingNumber"]}",
                                Size = new Size(120, 20),
                                Location = new Point(244, 226),
                            };
                            MaterialLabel ShippingSpeed = new MaterialLabel
                            {
                                Text = $"Shipping Speed :",
                                Size = new Size(111, 17),
                                Location = new Point(368, 223),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Shipping = new MaterialLabel
                            {
                                Text = $"{reader["ShippingSpeed"]}",
                                Size = new Size(191, 60),
                                Location = new Point(368, 245),
                            };
                            MaterialLabel Item = new MaterialLabel
                            {
                                Text = $"List Of Items",
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Subtitle1,
                                TextAlign = ContentAlignment.MiddleCenter,
                                Size = new Size(218, 20),
                                Location = new Point(11, 203),
                            };
                            MaterialListBox ItemList = new MaterialListBox
                            {
                                Size = new Size(218, 97),
                                Location = new Point(11, 223),
                            };
                            MaterialLabel PackageType = new MaterialLabel
                            {
                                Text = $"Package Type :",
                                Size = new Size(100, 21),
                                Location = new Point(11, 323),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Package = new MaterialLabel
                            {
                                Text = $"{reader["PackageType"]}",
                                Size = new Size(210, 20),
                                Location = new Point(11, 344),
                            };
                            MaterialButton Approve = new MaterialButton
                            {
                                Text = "Approve",
                                Tag = shipmentID,
                                Size = new Size(152, 48),
                                Location = new Point(244, 316),
                                AutoSize = false
                            };
                            MaterialButton Decline = new MaterialButton
                            {
                                Text = "Decline",
                                Tag = shipmentID,
                                Size = new Size(153, 48),
                                Location = new Point(404, 316),
                                AutoSize = false
                            };

                            Approve.Click += (sender, e) => ApproveorDecline_Click(sender, e, myConn, shipmentID);
                            Decline.Click += (sender, e) => ApproveorDecline_Click(sender, e, myConn, shipmentID);

                            string items = reader["ItemLists"].ToString();
                            string[] itemArray = items.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (string item in itemArray)
                            {
                                string cleanItem = item.Replace("[", "").Replace("]", "").Trim();
                                string[] itemParts = cleanItem.Split(',');
                                string name = itemParts[0].Trim();
                                ItemList.Items.Add(new MaterialListBoxItem(name));
                            }

                            cards.Add(card);
                            shipmentCards[shipmentId] = card;

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
                            card.Controls.Add(Approve);
                            card.Controls.Add(Decline);

                        }
                    }
                }
            }
            string query2 = "SELECT ShipmentID, TotalCost FROM ShipmentPaymentDetails";
            using (OleDbCommand cmd2 = new OleDbCommand(query2, myConn))
            {
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
                                Location = new Point(244, 258),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.H6
                            };
                            MaterialLabel Total = new MaterialLabel
                            {
                                Text = $"{reader2["TotalCost"]}",
                                Size = new Size(118, 16),
                                Location = new Point(244, 284),
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
        internal List<MaterialCard> GetQuickShipments(OleDbConnection myConn)
        {
            List<MaterialCard> cards = new List<MaterialCard>();
            myConn.Open();
            string query = "SELECT [ShipmentID], [PickUpAddress], [DeliveryAddress], [CargoType], [Status], [ShippingSpeed], [PackageType], [TrackingNumber] FROM ShipmentItemDetails";
            using (OleDbCommand cmd = new OleDbCommand(query, myConn))
            {
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int shipmentID = Convert.ToInt32(reader["ShipmentID"]);
                        string status = reader["Status"].ToString();
                        if (status.Contains("Pending"))
                        {
                            shipmentID = Convert.ToInt32(reader["ShipmentID"]);
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
                            };
                            MaterialLabel StatusText = new MaterialLabel
                            {
                                Text = $"{reader["Status"]}",
                                Size = new Size(111, 21),
                                Location = new Point(17, 41),

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
            return cards;
        }
        internal List<MaterialCard> GetHistoryShipments(OleDbConnection myConn)
        {
            List<MaterialCard> cards = new List<MaterialCard>();
            myConn.Open();
            string query = "SELECT [ShipmentID], [PickUpAddress], [DeliveryAddress], [CargoType], [ShippingSpeed], [Status], [ItemLists], [RecieverFullName], [RecieverContact], [TrackingNumber], [PackageType] FROM ShipmentItemDetails";
            var shipmentCards = new Dictionary<string, MaterialCard>();
            using (OleDbCommand cmd = new OleDbCommand(query, myConn))
            {
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string shipmentId = reader["ShipmentID"].ToString();
                        string status = reader["Status"].ToString();
                        if (status == "Delivered" || status == "Denied")
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

            string query2 = "SELECT ShipmentID, CargoFee, WeightFee, ValueFee, ShippingFee, TotalCost FROM ShipmentPaymentDetails";
            using (OleDbCommand cmd2 = new OleDbCommand(query2, myConn))
            {
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
        internal List<MaterialCard> GetManageShipmentsID(OleDbConnection myConn, string ID)
        {
            List<MaterialCard> cards = new List<MaterialCard>();
            var shipmentCards = new Dictionary<string, MaterialCard>();
            if (string.IsNullOrEmpty(ID)) return cards;

            myConn.Open();
            string query = "SELECT [ShipmentID], [PickUpAddress], [DeliveryAddress], [CargoType], [ShippingSpeed], [Status], [ItemLists], [RecieverFullName], [RecieverContact], [TrackingNumber], [PackageType] FROM ShipmentItemDetails WHERE [ShipmentID] LIKE ? OR [TrackingNumber] LIKE ?";
            using (OleDbCommand cmd = new OleDbCommand(query, myConn))
            {
                cmd.Parameters.AddWithValue("?", "%" + ID + "%");
                cmd.Parameters.AddWithValue("?", "%" + ID + "%");

                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int shipmentID = Convert.ToInt32(reader["ShipmentID"]);
                        string shipmentId = reader["ShipmentID"].ToString();
                        string status = reader["Status"].ToString();
                        if (status == "Pending")
                        {
                            MaterialCard card = new MaterialCard
                            {
                                Size = new Size(580, 376),
                            };
                            MaterialLabel From = new MaterialLabel
                            {
                                Text = "From : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(48, 17),
                                Location = new Point(11, 35),
                            };
                            MaterialLabel To = new MaterialLabel
                            {
                                Text = "To : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(29, 17),
                                Location = new Point(200, 35),
                            };
                            MaterialLabel ReceiverNameT = new MaterialLabel
                            {
                                Text = "Receiver Name : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(108, 23),
                                Location = new Point(11, 119),
                            };
                            MaterialLabel ReceiverContactT = new MaterialLabel
                            {
                                Text = "Receiver Contact : ",
                                FontType = MaterialSkinManager.fontType.Button,
                                Size = new Size(120, 23),
                                Location = new Point(200, 119),
                            };
                            MaterialLabel ReceiverName = new MaterialLabel
                            {
                                Text = $"{reader["RecieverFullName"]}",
                                Size = new Size(170, 60),
                                Location = new Point(11, 139),
                            };
                            MaterialLabel ReceiverContact = new MaterialLabel
                            {
                                Text = $"{reader["RecieverContact"]}",
                                Size = new Size(170, 60),
                                Location = new Point(200, 139),
                            };
                            MaterialLabel PickUp = new MaterialLabel
                            {
                                Text = $"{reader["PickUpAddress"]}",
                                Size = new Size(170, 60),
                                Location = new Point(11, 56),
                            };
                            MaterialLabel Destination = new MaterialLabel
                            {
                                Text = $"{reader["DeliveryAddress"]}",
                                Size = new Size(170, 60),
                                Location = new Point(200, 55),
                            };
                            MaterialLabel CargoType = new MaterialLabel
                            {
                                Text = $"{reader["CargoType"]}",
                                TextAlign = ContentAlignment.MiddleCenter,
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Subtitle1,
                                Size = new Size(187, 24),
                                Location = new Point(376, 9),
                            };
                            PictureBox CargoPic = new PictureBox
                            {
                                Size = new Size(187, 146),
                                Location = new Point(376, 35),
                                Image = Image.FromFile(ImagePic(reader["CargoType"].ToString())),
                                SizeMode = PictureBoxSizeMode.StretchImage,
                            };
                            MaterialLabel ShipmentID = new MaterialLabel
                            {
                                Text = $"Ship ID: {shipmentID}",
                                Size = new Size(118, 17),
                                Location = new Point(368, 206),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel TrackingNumber = new MaterialLabel
                            {
                                Text = $"Tracking No. :",
                                Size = new Size(90, 21),
                                Location = new Point(244, 206),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Track = new MaterialLabel
                            {
                                Text = $"{reader["TrackingNumber"]}",
                                Size = new Size(120, 20),
                                Location = new Point(244, 226),
                            };
                            MaterialLabel ShippingSpeed = new MaterialLabel
                            {
                                Text = $"Shipping Speed :",
                                Size = new Size(111, 17),
                                Location = new Point(368, 223),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Shipping = new MaterialLabel
                            {
                                Text = $"{reader["ShippingSpeed"]}",
                                Size = new Size(191, 60),
                                Location = new Point(368, 245),
                            };
                            MaterialLabel Item = new MaterialLabel
                            {
                                Text = $"List Of Items",
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Subtitle1,
                                TextAlign = ContentAlignment.MiddleCenter,
                                Size = new Size(218, 20),
                                Location = new Point(11, 203),
                            };
                            MaterialListBox ItemList = new MaterialListBox
                            {
                                Size = new Size(218, 97),
                                Location = new Point(11, 223),
                            };
                            MaterialLabel PackageType = new MaterialLabel
                            {
                                Text = $"Package Type :",
                                Size = new Size(100, 21),
                                Location = new Point(11, 323),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.Button
                            };
                            MaterialLabel Package = new MaterialLabel
                            {
                                Text = $"{reader["PackageType"]}",
                                Size = new Size(210, 20),
                                Location = new Point(11, 344),
                            };
                            MaterialButton Approve = new MaterialButton
                            {
                                Text = "Approve",
                                Tag = shipmentID,
                                Size = new Size(152, 48),
                                Location = new Point(244, 316),
                                AutoSize = false
                            };
                            MaterialButton Decline = new MaterialButton
                            {
                                Text = "Decline",
                                Tag = shipmentID,
                                Size = new Size(153, 48),
                                Location = new Point(404, 316),
                                AutoSize = false
                            };

                            Approve.Click += (sender, e) => ApproveorDecline_Click(sender, e, myConn, shipmentID);
                            Decline.Click += (sender, e) => ApproveorDecline_Click(sender, e, myConn, shipmentID);

                            string items = reader["ItemLists"].ToString();
                            string[] itemArray = items.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (string item in itemArray)
                            {
                                string cleanItem = item.Replace("[", "").Replace("]", "").Trim();
                                string[] itemParts = cleanItem.Split(',');
                                string name = itemParts[0].Trim();
                                ItemList.Items.Add(new MaterialListBoxItem(name));
                            }

                            cards.Add(card);
                            shipmentCards[shipmentId] = card;

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
                            card.Controls.Add(Approve);
                            card.Controls.Add(Decline);
                        }
                    }
                }
            }
            string query2 = "SELECT ShipmentID, TotalCost FROM ShipmentPaymentDetails WHERE [ShipmentID] LIKE ? OR [TrackingNumber] LIKE ?";
            using (OleDbCommand cmd2 = new OleDbCommand(query2, myConn))
            {
                cmd2.Parameters.AddWithValue("?", "%" + ID + "%");
                cmd2.Parameters.AddWithValue("?", "%" + ID + "%");
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
                                Location = new Point(244, 258),
                                FontType = MaterialSkin.MaterialSkinManager.fontType.H6
                            };
                            MaterialLabel Total = new MaterialLabel
                            {
                                Text = $"{reader2["TotalCost"]}",
                                Size = new Size(118, 16),
                                Location = new Point(244, 284),
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
        internal List<MaterialCard> GetHistoryShipmentsID(OleDbConnection myConn, string ID)
        {
            List<MaterialCard> cards = new List<MaterialCard>();

            if (string.IsNullOrEmpty(ID)) return cards;

            myConn.Open();
            string query = "SELECT [ShipmentID], [PickUpAddress], [DeliveryAddress], [CargoType], [ShippingSpeed], [Status], [ItemLists], [RecieverFullName], [RecieverContact], [TrackingNumber], [PackageType] FROM ShipmentItemDetails WHERE [ShipmentID] LIKE ? OR [TrackingNumber] LIKE ?";
            var shipmentCards = new Dictionary<string, MaterialCard>();
            using (OleDbCommand cmd = new OleDbCommand(query, myConn))
            {
                cmd.Parameters.AddWithValue("?", "%" + ID + "%");
                cmd.Parameters.AddWithValue("?", "%" + ID + "%");

                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string shipmentId = reader["ShipmentID"].ToString();
                        string status = reader["Status"].ToString();
                        if (status == "Delivered" || status == "Denied")
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

            string query2 = "SELECT ShipmentID, CargoFee, WeightFee, ValueFee, ShippingFee, TotalCost FROM ShipmentPaymentDetails WHERE [ShipmentID] LIKE ? OR [TrackingNumber] LIKE ?";
            using (OleDbCommand cmd2 = new OleDbCommand(query2, myConn))
            {
                cmd2.Parameters.AddWithValue("?", "%" + ID + "%");
                cmd2.Parameters.AddWithValue("?", "%" + ID + "%");
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
        internal List<MaterialCard> GetUsers(OleDbConnection myConn, Action<object, EventArgs> refreshAction, Control controls)
        {
            List<MaterialCard> cards = new List<MaterialCard>();
            myConn.Open();
            string query = "SELECT [AccountID], [FullName], [Email], [Image], [Status], [CreatedDate], [LastLogin], [LastBooked] FROM UserAccounts";
            using (OleDbCommand cmd = new OleDbCommand(query, myConn))
            {
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int accountID = Convert.ToInt32(reader["AccountID"].ToString());
                        int totalbooking = 0, shipped = 0, approval = 0;
                        bool visible;
                        if (reader["Status"].ToString() == "Active") { visible = true; } else { visible = false; }
                        string query2 = "SELECT Status, UserID FROM ShipmentItemDetails WHERE UserID = ?";

                        using (OleDbCommand cmd2 = new OleDbCommand(query2, myConn))
                        {
                            cmd2.Parameters.AddWithValue("?", accountID);
                            using (OleDbDataReader reader2 = cmd2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    string userID = reader2["UserID"].ToString();
                                    totalbooking++;
                                    if (reader2["Status"].ToString() == "Pending") { approval++; }
                                    else if (reader2["Status"].ToString() == "Delivered (Sent)") { shipped++; }
                                }
                            }
                        }
                        MaterialCard card = new MaterialCard
                        {
                            Size = new Size(557, 460),
                        };
                        PictureBox Profile = new PictureBox
                        {
                            Size = new Size(237, 192),
                            Location = new Point(17, 17),
                            Image = Image.FromFile(reader["Image"].ToString()),
                            SizeMode = PictureBoxSizeMode.StretchImage,
                        };
                        MaterialLabel Status = new MaterialLabel
                        {
                            Text = $"Account Status : {reader["Status"]}",
                            Size = new Size(359, 27),
                            FontType = MaterialSkin.MaterialSkinManager.fontType.H6,
                            Location = new Point(17, 231),
                        };
                        MaterialLabel FullName = new MaterialLabel
                        {
                            Text = $"Name : {reader["FullName"]}",
                            Size = new Size(280, 27),
                            FontType = MaterialSkin.MaterialSkinManager.fontType.H6,
                            Location = new Point(260, 17),
                        };
                        MaterialLabel Email = new MaterialLabel
                        {
                            Text = $"Email : {reader["Email"]}",
                            Size = new Size(280, 27),
                            FontType = MaterialSkin.MaterialSkinManager.fontType.H6,
                            Location = new Point(260, 58),
                        };
                        MaterialLabel CreatedDate = new MaterialLabel
                        {
                            Text = $"Account Created : {reader["CreatedDate"]}",
                            Size = new Size(359, 27),
                            FontType = MaterialSkin.MaterialSkinManager.fontType.H6,
                            Location = new Point(17, 272),
                        };
                        MaterialLabel LastLogin = new MaterialLabel
                        {
                            Text = $"Last Login : {reader["LastLogin"]}",
                            Size = new Size(359, 27),
                            FontType = MaterialSkin.MaterialSkinManager.fontType.H6,
                            Location = new Point(17, 313),
                        };
                        MaterialLabel LastBooked = new MaterialLabel
                        {
                            Text = $"Last Booked : {reader["LastBooked"]}",
                            Size = new Size(359, 27),
                            FontType = MaterialSkin.MaterialSkinManager.fontType.H6,
                            Location = new Point(17, 354),
                        };
                        MaterialButton ViewShipment = new MaterialButton
                        {
                            Text = "View Shipments",
                            Tag = accountID,
                            Size = new Size(240, 45),
                            Location = new Point(299, 396),
                            AutoSize = false
                        };
                        MaterialButton Disable = new MaterialButton
                        {
                            Text = "Disable Account",
                            Tag = accountID,
                            Size = new Size(273, 45),
                            Location = new Point(18, 396),
                            AutoSize = false,
                            Visible = visible,
                        };
                        MaterialButton Enable = new MaterialButton
                        {
                            Text = "Enable Account",
                            Tag = accountID,
                            Size = new Size(150, 45),
                            Location = new Point(18, 396),
                            AutoSize = false,
                            Visible = !visible,
                        };
                        MaterialButton Ban = new MaterialButton
                        {
                            Text = "Ban Account",
                            Tag = accountID,
                            Size = new Size(115, 45),
                            Location = new Point(176, 396),
                            AutoSize = false,
                            Visible = !visible,
                        };
                        MaterialLabel TotalBooking = new MaterialLabel
                        {
                            Text = $"Total Bookings : {totalbooking}",
                            Size = new Size(280, 27),
                            FontType = MaterialSkin.MaterialSkinManager.fontType.H6,
                            Location = new Point(260, 99),
                        };
                        MaterialLabel Shipped = new MaterialLabel
                        {
                            Text = $"Successfully Shipped : {shipped}",
                            Size = new Size(280, 27),
                            FontType = MaterialSkin.MaterialSkinManager.fontType.H6,
                            Location = new Point(260, 140),
                        };
                        MaterialLabel Approval = new MaterialLabel
                        {
                            Text = $"Awaiting Approval : {approval}",
                            Size = new Size(280, 27),
                            FontType = MaterialSkin.MaterialSkinManager.fontType.H6,
                            Location = new Point(260, 181),
                        };

                        card.Controls.Add(Status);
                        card.Controls.Add(Profile);
                        card.Controls.Add(LastBooked);
                        card.Controls.Add(LastLogin);
                        card.Controls.Add(CreatedDate);
                        card.Controls.Add(FullName);
                        card.Controls.Add(Email);
                        card.Controls.Add(TotalBooking);
                        card.Controls.Add(Shipped);
                        card.Controls.Add(Approval);
                        card.Controls.Add(Ban);
                        card.Controls.Add(Enable);
                        card.Controls.Add(Disable);
                        card.Controls.Add(ViewShipment);

                        ViewShipment.Click += (sender, e) => {
                            ViewShipment_Click(sender, e, myConn, accountID);
                            refreshAction(sender, e);

                            var userCards = GetUsersHistory(myConn, accountID);

                            foreach (var card in userCards)
                            {
                                controls.Controls.Add(card);
                            }
                        };
                        Disable.Click += (sender, e) => { Disable_Click(sender, e, myConn, accountID); refreshAction(sender, e); };
                        Enable.Click += (sender, e) => { Enable_Click(sender, e, myConn, accountID); refreshAction(sender, e); };
                        Ban.Click += (sender, e) => { Ban_Click(sender, e, myConn, accountID); refreshAction(sender, e); };

                        cards.Add(card);
                    }
                }
            }
            myConn.Close();
            return cards;
        }
        internal List<MaterialCard> GetUsersHistory(OleDbConnection myConn, int UserID)
        {
            List<MaterialCard> cards = new List<MaterialCard>();
            myConn.Open();
            string query = "SELECT [ShipmentID], [PickUpAddress], [DeliveryAddress], [CargoType], [ShippingSpeed], [Status], [ItemLists], [RecieverFullName], [RecieverContact], [TrackingNumber], [PackageType] FROM ShipmentItemDetails WHERE UserID = ?";
            var shipmentCards = new Dictionary<string, MaterialCard>();
            using (OleDbCommand cmd = new OleDbCommand(query, myConn))
            {
                cmd.Parameters.AddWithValue("?", UserID);
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

            string query2 = "SELECT ShipmentID, CargoFee, WeightFee, ValueFee, ShippingFee, TotalCost FROM ShipmentPaymentDetails";
            using (OleDbCommand cmd2 = new OleDbCommand(query2, myConn))
            {
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
