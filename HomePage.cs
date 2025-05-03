using System.Data.OleDb;
using System.Data;
using MaterialSkin;
namespace PortLink__Final_Project_
{
    public partial class MainMenuPage : Form
    {
        OleDbConnection? myConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Ivan\Documents\PortLink (Final Project)\Database\PortLink Database.accdb;");
        OleDbDataAdapter? da;
        OleDbCommand? cmd;
        DataSet? ds;
        int indexRow;
        public MainMenuPage()
        {
            InitializeComponent();
            MaterialSkinManager manager = MaterialSkinManager.Instance;
            manager.Theme = MaterialSkinManager.Themes.LIGHT;
            manager.ColorScheme = new ColorScheme(Primary.Green600, Primary.Green800, Primary.Green400, Accent.LightGreen200, TextShade.WHITE);
        }
        private void MainMenuPage_Resize(object sender, EventArgs e)
        {
            TitleBar.Left = TitleBar.ClientSize.Width - TitleBar.Width - 10;
            Exit.Left = TitleBar.ClientSize.Width - Exit.Width - 10;
        }
        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void SignUp_Click(object sender, EventArgs e)
        {
            Register register = new Register(this);
            register.Location = new Point((this.ClientSize.Width - register.Width) / 2, (this.ClientSize.Height - register.Height) / 2 + 30);
            this.Controls.Add(register);
            register.BringToFront();
            register.Show();
            Exit.Enabled = false;
            Login.Enabled = false;
            SignUp.Enabled = false;
            register.Email.Focus();
        }
        private void Login_Click(object sender, EventArgs e)
        {
            Login login = new Login(this);
            login.Location = new Point((this.ClientSize.Width - login.Width) / 2 + 350, (this.ClientSize.Height - login.Height) / 2 + 40);
            this.Controls.Add(login);
            login.BringToFront();
            login.Show();
            Exit.Enabled = false;
            Login.Enabled = false;
            SignUp.Enabled = false;
        }
        private void MainMenuPage_Load(object sender, EventArgs e)
        {
            
            MaterialSkinManager manager = MaterialSkinManager.Instance;
            manager.Theme = MaterialSkinManager.Themes.LIGHT;
        }        
    }
}
