namespace PortLink__Final_Project_
{
    partial class Login
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            panel1 = new Panel();
            Username = new TextBox();
            pictureBox1 = new PictureBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            Password = new TextBox();
            panel2 = new Panel();
            PassShow = new Button();
            CreateLink = new LinkLabel();
            label8 = new Label();
            Exit = new MaterialSkin.Controls.MaterialButton();
            LoginBTN = new MaterialSkin.Controls.MaterialButton();
            pictureBox3 = new PictureBox();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.Gainsboro;
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(Username);
            panel1.Controls.Add(pictureBox1);
            panel1.Location = new Point(67, 119);
            panel1.Name = "panel1";
            panel1.Size = new Size(387, 46);
            panel1.TabIndex = 0;
            // 
            // Username
            // 
            Username.BackColor = Color.Gainsboro;
            Username.BorderStyle = BorderStyle.None;
            Username.Font = new Font("Tahoma", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Username.Location = new Point(57, 8);
            Username.Name = "Username";
            Username.Size = new Size(325, 28);
            Username.TabIndex = 1;
            Username.KeyDown += Username_KeyDown;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Location = new Point(0, -1);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(51, 46);
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Rockwell", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(199, 32);
            label1.Name = "label1";
            label1.Size = new Size(124, 48);
            label1.TabIndex = 3;
            label1.Text = "Log In";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(67, 88);
            label2.Name = "label2";
            label2.Size = new Size(172, 28);
            label2.TabIndex = 4;
            label2.Text = "Username / Email";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(68, 168);
            label3.Name = "label3";
            label3.Size = new Size(103, 28);
            label3.TabIndex = 5;
            label3.Text = "Password ";
            // 
            // Password
            // 
            Password.BackColor = Color.Gainsboro;
            Password.BorderStyle = BorderStyle.None;
            Password.Font = new Font("Tahoma", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Password.Location = new Point(56, 8);
            Password.Name = "Password";
            Password.PasswordChar = '●';
            Password.Size = new Size(326, 28);
            Password.TabIndex = 1;
            Password.KeyDown += Password_KeyDown;
            // 
            // panel2
            // 
            panel2.BackColor = Color.Gainsboro;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(PassShow);
            panel2.Controls.Add(Password);
            panel2.Location = new Point(68, 199);
            panel2.Name = "panel2";
            panel2.Size = new Size(387, 46);
            panel2.TabIndex = 2;
            // 
            // PassShow
            // 
            PassShow.BackgroundImage = (Image)resources.GetObject("PassShow.BackgroundImage");
            PassShow.BackgroundImageLayout = ImageLayout.Stretch;
            PassShow.FlatAppearance.BorderSize = 0;
            PassShow.FlatAppearance.MouseDownBackColor = Color.Gainsboro;
            PassShow.FlatAppearance.MouseOverBackColor = Color.WhiteSmoke;
            PassShow.FlatStyle = FlatStyle.Popup;
            PassShow.Location = new Point(0, -1);
            PassShow.Name = "PassShow";
            PassShow.Size = new Size(50, 46);
            PassShow.TabIndex = 3;
            PassShow.UseVisualStyleBackColor = true;
            PassShow.Click += PassShow_Click;
            // 
            // CreateLink
            // 
            CreateLink.ActiveLinkColor = Color.FromArgb(102, 200, 106);
            CreateLink.BackColor = Color.Transparent;
            CreateLink.Font = new Font("Calibri", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            CreateLink.LinkBehavior = LinkBehavior.NeverUnderline;
            CreateLink.LinkColor = Color.FromArgb(0, 192, 0);
            CreateLink.Location = new Point(262, 318);
            CreateLink.Name = "CreateLink";
            CreateLink.Size = new Size(172, 21);
            CreateLink.TabIndex = 15;
            CreateLink.TabStop = true;
            CreateLink.Text = "Create an account";
            CreateLink.LinkClicked += CreateLink_LinkClicked;
            // 
            // label8
            // 
            label8.BackColor = Color.Transparent;
            label8.Font = new Font("Calibri", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label8.Location = new Point(86, 318);
            label8.Name = "label8";
            label8.Size = new Size(178, 25);
            label8.TabIndex = 23;
            label8.Text = "Dont have an account?";
            // 
            // Exit
            // 
            Exit.AutoSize = false;
            Exit.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Exit.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            Exit.Depth = 0;
            Exit.HighEmphasis = false;
            Exit.Icon = null;
            Exit.Location = new Point(470, 6);
            Exit.Margin = new Padding(4, 6, 4, 6);
            Exit.MouseState = MaterialSkin.MouseState.HOVER;
            Exit.Name = "Exit";
            Exit.NoAccentTextColor = Color.Empty;
            Exit.Size = new Size(56, 46);
            Exit.TabIndex = 24;
            Exit.Text = "X";
            Exit.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            Exit.UseAccentColor = false;
            Exit.UseVisualStyleBackColor = true;
            Exit.Click += Exit_Click;
            // 
            // LoginBTN
            // 
            LoginBTN.AutoSize = false;
            LoginBTN.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            LoginBTN.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            LoginBTN.Depth = 0;
            LoginBTN.HighEmphasis = true;
            LoginBTN.Icon = null;
            LoginBTN.Location = new Point(153, 267);
            LoginBTN.Margin = new Padding(4, 6, 4, 6);
            LoginBTN.MouseState = MaterialSkin.MouseState.HOVER;
            LoginBTN.Name = "LoginBTN";
            LoginBTN.NoAccentTextColor = Color.Empty;
            LoginBTN.Size = new Size(198, 45);
            LoginBTN.TabIndex = 25;
            LoginBTN.Text = "Login";
            LoginBTN.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            LoginBTN.UseAccentColor = false;
            LoginBTN.UseVisualStyleBackColor = true;
            LoginBTN.Click += LoginBTN_Click;
            // 
            // pictureBox3
            // 
            pictureBox3.BackColor = Color.Transparent;
            pictureBox3.BackgroundImage = (Image)resources.GetObject("pictureBox3.BackgroundImage");
            pictureBox3.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox3.Location = new Point(134, 25);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(59, 55);
            pictureBox3.TabIndex = 26;
            pictureBox3.TabStop = false;
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(pictureBox3);
            Controls.Add(LoginBTN);
            Controls.Add(Exit);
            Controls.Add(label8);
            Controls.Add(CreateLink);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Name = "Login";
            Size = new Size(533, 385);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private PictureBox pictureBox1;
        private Label label1;
        private Label label2;
        private Label label3;       
        private TextBox Password;
        private Panel panel2;
        private Button PassShow;
        private LinkLabel CreateLink;
        internal TextBox Username;
        private Label label8;
        private MaterialSkin.Controls.MaterialButton Exit;
        private MaterialSkin.Controls.MaterialButton LoginBTN;
        private PictureBox pictureBox3;
    }
}
