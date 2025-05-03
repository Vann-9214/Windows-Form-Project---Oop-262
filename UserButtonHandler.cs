using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortLink__Final_Project_
{
    internal class UserButtonHandler
    {
        public void ReadyorCancel_Click(object sender, EventArgs e, OleDbConnection myConn, int shipmentID)
        {
            try
            {
                myConn.Open();

                if (sender is MaterialSkin.Controls.MaterialButton clickedButton)
                {
                    if (clickedButton.Text == "Ready")
                    {
                        string updateQuery = "UPDATE ShipmentItems SET Status = ? WHERE ShipmentID = ?";
                        using (OleDbCommand updateCmd = new OleDbCommand(updateQuery, myConn))
                        {
                            updateCmd.Parameters.AddWithValue("?", "Preparing for Pickup");
                            updateCmd.Parameters.AddWithValue("?", shipmentID);
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                    else if (clickedButton.Text == "Cancel")
                    {
                        string query = "DELETE FROM ShipmentItems WHERE ShipmentID = ?";
                        using (OleDbCommand cmd = new OleDbCommand(query, myConn))
                        {
                            cmd.Parameters.AddWithValue("?", shipmentID);
                            cmd.ExecuteNonQuery();
                        }
                        string query2 = "DELETE FROM AllCosts WHERE ShipmentID = ?";
                        using (OleDbCommand cmd2 = new OleDbCommand(query2, myConn))
                        {
                            cmd2.Parameters.AddWithValue("?", shipmentID);
                            cmd2.ExecuteNonQuery();
                        }
                        MessageBox.Show("Shipment Canceled...");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            myConn.Close();
        }

    }
}
