using System.Data.OleDb;
using MaterialSkin;
using MaterialSkin.Controls;

namespace PortLink__Final_Project_
{
    internal class AdminButtonHandler
    {
        public void ApproveorDecline_Click(object sender, EventArgs e, OleDbConnection myConn, int shipmentID)
        {
            string query1 = "UPDATE ShipmentItems SET [Status] = @status WHERE ShipmentID = @ShipmentID";
            myConn.Open();
            using (OleDbCommand cmd1 = new OleDbCommand(query1, myConn))
            {
                if (sender is MaterialSkin.Controls.MaterialButton clickedButton)
                {
                    if (clickedButton.Text == "Approve")
                    {
                        cmd1.Parameters.AddWithValue("@status", "Approved");
                        MessageBox.Show("Shipment Approved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (clickedButton.Text == "Decline")
                    {
                        cmd1.Parameters.AddWithValue("@status", "Denied");
                        MessageBox.Show("Shipment Declined!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                cmd1.Parameters.AddWithValue("@ShipmentID", shipmentID);
                cmd1.ExecuteNonQuery();
            }
            myConn.Close();
        }
        public void ViewShipment_Click(object sender, EventArgs e, OleDbConnection myConn, int accountID)
        {
            
        }
        public void Enable_Click(object sender, EventArgs e, OleDbConnection myConn, int accountID)
        {
            string query2 = "UPDATE UserAccounts SET Status = ? WHERE AccountID = ?";
            myConn.Open();
            using (OleDbCommand cmd2 = new OleDbCommand(query2, myConn))
            {
                cmd2.Parameters.AddWithValue("?", "Active");
                cmd2.Parameters.AddWithValue("?", accountID);
                cmd2.ExecuteNonQuery();
            }
            myConn.Close();
        }
        public void Disable_Click(object sender, EventArgs e, OleDbConnection myConn, int accountID)
        {
            string query2 = "UPDATE UserAccounts SET Status = ? WHERE AccountID = ?";
            myConn.Open();
            using (OleDbCommand cmd2 = new OleDbCommand(query2, myConn))
            {
                cmd2.Parameters.AddWithValue("?", "Disabled");
                cmd2.Parameters.AddWithValue("?", accountID);
                cmd2.ExecuteNonQuery();
            }
            myConn.Close();
        }
        public void Ban_Click(object sender, EventArgs e, OleDbConnection myConn, int accountID)
        {
            string query = "DELETE FROM UserAccounts WHERE AccountID = ?";
            myConn.Open();
            using (OleDbCommand cmd = new OleDbCommand(query, myConn))
            {
                cmd.Parameters.AddWithValue("?", accountID);
                cmd.ExecuteNonQuery();
            }
            myConn.Close();
        }
    }
}
