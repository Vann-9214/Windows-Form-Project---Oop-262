using System.Data;
using System.Data.OleDb;
using System.Text;
using MaterialSkin;
using System.Security.Cryptography;
namespace PortLink__Final_Project_
{
    public partial class Login : UserControl
    {
        public int AccountID { get; private set; }
        private MainMenuPage menu;
        private bool showpass = true;
        public Login(MainMenuPage menu)
        {
            InitializeComponent();
            this.menu = menu;
            MaterialSkinManager manager = MaterialSkinManager.Instance;
            manager.Theme = MaterialSkinManager.Themes.LIGHT;
            manager.ColorScheme = new ColorScheme(Primary.Green600, Primary.Green800, Primary.Green400, Accent.LightGreen200, TextShade.WHITE);
        }
        internal string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void CreateLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            Register register = new Register(menu);
            register.Location = new Point((menu.ClientSize.Width - register.Width) / 2, (menu.ClientSize.Height - register.Height) / 2 + 30);
            menu.Controls.Add(register);
            register.BringToFront();
            register.Show();
        }       
        private void PassShow_Click(object sender, EventArgs e)
        {
            if (showpass)
            {
                Password.PasswordChar = '\0';
            }
            else
            {
                Password.PasswordChar = '●';
            }
            showpass = !showpass;
        }
        private void Username_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Password.Focus();
            }
        }
        private void Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoginBTN_Click(sender, e);
                e.SuppressKeyPress = true;
            }
        }
        private void Exit_Click(object sender, EventArgs e)
        {
            this.Hide();
            menu.Exit.Enabled = true;
            menu.Login.Enabled = true;
            menu.SignUp.Enabled = true;
        }

        private void LoginBTN_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Username.Text) || string.IsNullOrWhiteSpace(Password.Text))
            {
                MessageBox.Show("Please fill up Username and Password to continue");
                Username.Text = string.Empty;
                Password.Text = string.Empty;
                Username.Focus();
                return;
            }
            string hashpass = HashPassword(Password.Text);
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Ivan\Documents\PortLink (Final Project)\Database\PortLink Database.accdb;";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                string queryAdmin = "SELECT AccountID FROM AdminAccounts WHERE (Username = ? OR Email = ?) AND Password = ?";
                using (OleDbCommand command = new OleDbCommand(queryAdmin, connection))
                {
                    command.Parameters.AddWithValue("?", Username.Text);
                    command.Parameters.AddWithValue("?", Username.Text);
                    command.Parameters.AddWithValue("?", hashpass);
                    object adminID = command.ExecuteScalar();

                    if (adminID != null)
                    {
                        AccountID = Convert.ToInt32(adminID);
                        MessageBox.Show("Admin Login Successful!");
                        foreach (Form form in Application.OpenForms.Cast<Form>().ToList()) form.Hide();
                        AdminDashboard admindashboard = new AdminDashboard(this);
                        admindashboard.Show();
                        return;
                    }
                }
                string queryUser = "SELECT AccountID, Status FROM UserAccounts WHERE (Username = ? OR Email = ?) AND Password = ?";
                using (OleDbCommand command = new OleDbCommand(queryUser, connection))
                {
                    command.Parameters.AddWithValue("?", Username.Text);
                    command.Parameters.AddWithValue("?", Username.Text); 
                    command.Parameters.AddWithValue("?", hashpass);

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            AccountID = Convert.ToInt32(reader["AccountID"]);
                            string status = reader["Status"].ToString();

                            if (status.ToLower() != "active")
                            {
                                MessageBox.Show("Account is either banned or disabled from admin.");
                                return;
                            }
                            MessageBox.Show("User Login Successful!");
                            foreach (Form form in Application.OpenForms.Cast<Form>().ToList())
                                form.Hide();

                            UserDashboard userdashboard = new UserDashboard(this);
                            userdashboard.Show();
                            return;
                        }
                    }
                }

                MessageBox.Show("Invalid Username or Password!");
                Username.Text = string.Empty;
                Password.Text = string.Empty;
                Username.Focus();
            }
        }
    }
}
