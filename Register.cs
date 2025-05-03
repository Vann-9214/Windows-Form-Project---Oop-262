using MaterialSkin;
using System.Data;
using System.Data.OleDb;
using System.Text;
using System.Security.Cryptography;
namespace PortLink__Final_Project_
{
    public partial class Register : UserControl
    {
        private MainMenuPage menu;
        OleDbConnection? myConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Ivan\Documents\PortLink (Final Project)\Database\PortLink Database.accdb;");
        OleDbDataAdapter? da;
        OleDbCommand? cmd;
        DataSet? ds;
        int indexRow;
        public Register(MainMenuPage menu)
        {
            InitializeComponent();
            Email.Focus();
            MaterialSkinManager manager = MaterialSkinManager.Instance;
            manager.Theme = MaterialSkinManager.Themes.LIGHT;
            manager.ColorScheme = new ColorScheme(Primary.Green600, Primary.Green800, Primary.Green400, Accent.LightGreen200, TextShade.WHITE);
            this.menu = menu;
            Email.Focus();
        }
        private string HashPassword(string password)
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
        private void CreateUser_Click(object sender, EventArgs e)
        {
            if (Username.Text == "" || Password.Text == "" || ConfirmPassword.Text == "" || Email.Text == "" || FirstName.Text == "" || LastName.Text == "")
            {
                MessageBox.Show("Please fill in all the forms.");
                Username.Text = string.Empty;
                Password.Text = string.Empty;
                ConfirmPassword.Text = string.Empty;
                Email.Text = string.Empty;
                FirstName.Text = string.Empty;
                LastName.Text = string.Empty;
                return;
            }
            if (Password.Text != ConfirmPassword.Text)
            {
                MessageBox.Show("Password and Confirm Password do not match. Please try again.");
                Password.Text = string.Empty;
                ConfirmPassword.Text = string.Empty;
                return;
            }
            string hashpass = HashPassword(Password.Text);
            string query = "Insert into UserAccounts ([Email], [FullName], [FirstName], [LastName], [Username], [Password], [Image], [Status], [CreatedDate], [LastLogin], [LastBooked]) values ( @Mail, @FullName, @FirstName, @LastName, @User, @Pass, @Images, @status, @created, @login, @booked)";
            cmd = new OleDbCommand(query, myConn);
            cmd.Parameters.AddWithValue("@Mail", Email.Text);
            cmd.Parameters.AddWithValue("@FullName", FirstName.Text + " " + LastName.Text);
            cmd.Parameters.AddWithValue("@FirstName", FirstName.Text);
            cmd.Parameters.AddWithValue("@LastName", LastName.Text);
            cmd.Parameters.AddWithValue("@User", Username.Text);
            cmd.Parameters.AddWithValue("@Pass", hashpass);
            cmd.Parameters.AddWithValue("@Images", @"C:\Users\Ivan\Documents\PortLink (Final Project)\Images\Default Profile Icon.png");
            cmd.Parameters.AddWithValue("@status", "Active");
            cmd.Parameters.AddWithValue("@created", DateTime.Now.ToString("dd / MM / yyyy"));
            cmd.Parameters.AddWithValue("@login", DateTime.Now.ToString("dd / MM / yyyy"));
            cmd.Parameters.AddWithValue("@booked", "None");

            myConn.Open();
            cmd.ExecuteNonQuery();
            myConn.Close();
            MessageBox.Show("Account Created Successfully...");
            this.Hide();
            menu.Exit.Enabled = true;
            menu.Login.Enabled = true;
            menu.SignUp.Enabled = true;
            return;
        }
        private void SignInLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            Login login = new Login(menu);
            login.Location = new Point((menu.ClientSize.Width - login.Width) / 2 + 350, (menu.ClientSize.Height - login.Height) / 2 + 40);
            menu.Controls.Add(login);
            login.BringToFront();
            login.Show();
        }
        private void Exit_Click(object sender, EventArgs e)
        {
            this.Hide();
            menu.Exit.Enabled = true;
            menu.Login.Enabled = true;
            menu.SignUp.Enabled = true;
        }
        private void Pressed(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                if (sender == Email)
                {
                    FirstName.Focus();
                }
                else if (sender == FirstName)
                {
                    LastName.Focus();
                }
                else if (sender == LastName)
                {
                    Username.Focus();
                }
                else if (sender == Username)
                {
                    Password.Focus();
                }
                else if (sender == Password)
                {
                    ConfirmPassword.Focus();
                }
                else if (sender == ConfirmPassword)
                {
                    CreateUser.PerformClick();
                }
            }
        }

        private void CreateAdmin_Click(object sender, EventArgs e)
        {
            if (Username.Text == "" || Password.Text == "" || ConfirmPassword.Text == "" || Email.Text == "" || FirstName.Text == "" || LastName.Text == "")
            {
                MessageBox.Show("Please fill in all the forms.");
                Username.Text = string.Empty;
                Password.Text = string.Empty;
                ConfirmPassword.Text = string.Empty;
                Email.Text = string.Empty;
                FirstName.Text = string.Empty;
                LastName.Text = string.Empty;
                return;
            }
            if (Password.Text != ConfirmPassword.Text)
            {
                MessageBox.Show("Password and Confirm Password do not match. Please try again.");
                Password.Text = string.Empty;
                ConfirmPassword.Text = string.Empty;
                return;
            }
            string hashpass = HashPassword(Password.Text);
            string query = "Insert into AdminAccounts ([Email], [FullName], [FirstName], [LastName], [Username], [Password], [Image], [CreatedDated]) values ( @Mail, @FullName, @FirstName, @LastName, @User, @Pass, @Images, @created)";
            cmd = new OleDbCommand(query, myConn);
            cmd.Parameters.AddWithValue("@Mail", Email.Text);
            cmd.Parameters.AddWithValue("@FullName", FirstName.Text + " " + LastName.Text);
            cmd.Parameters.AddWithValue("@FirstName", FirstName.Text);
            cmd.Parameters.AddWithValue("@LastName", LastName.Text);
            cmd.Parameters.AddWithValue("@User", Username.Text);
            cmd.Parameters.AddWithValue("@Pass", hashpass);
            cmd.Parameters.AddWithValue("@Images", @"C:\Users\Ivan\Documents\PortLink (Final Project)\Images\Default Profile Icon.png");
            cmd.Parameters.AddWithValue("@created", DateTime.Now.ToString("dd / MM / yyyy"));
            myConn.Open();
            cmd.ExecuteNonQuery();
            myConn.Close();
            MessageBox.Show("Account Created Successfully...");
            this.Hide();
            menu.Exit.Enabled = true;
            menu.Login.Enabled = true;
            menu.SignUp.Enabled = true;
            return;
        }
    }
}
