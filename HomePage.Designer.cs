namespace PortLink__Final_Project_
{
    partial class MainMenuPage
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainMenuPage));
            Exit = new MaterialSkin.Controls.MaterialButton();
            Login = new MaterialSkin.Controls.MaterialButton();
            SignUp = new MaterialSkin.Controls.MaterialButton();
            TitleBar = new Panel();
            label3 = new Label();
            pictureBox3 = new PictureBox();
            label1 = new Label();
            label2 = new Label();
            TitleBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            SuspendLayout();
            // 
            // Exit
            // 
            Exit.AutoSize = false;
            Exit.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Exit.BackColor = Color.White;
            Exit.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            Exit.Depth = 0;
            Exit.ForeColor = SystemColors.ActiveCaptionText;
            Exit.HighEmphasis = true;
            Exit.Icon = null;
            Exit.Location = new Point(1246, 6);
            Exit.Margin = new Padding(4, 6, 4, 6);
            Exit.MouseState = MaterialSkin.MouseState.HOVER;
            Exit.Name = "Exit";
            Exit.NoAccentTextColor = Color.Empty;
            Exit.Size = new Size(52, 43);
            Exit.TabIndex = 8;
            Exit.TabStop = false;
            Exit.Text = "X";
            Exit.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            Exit.UseAccentColor = false;
            Exit.UseVisualStyleBackColor = false;
            Exit.Click += Exit_Click;
            // 
            // Login
            // 
            Login.AutoSize = false;
            Login.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Login.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            Login.Depth = 0;
            Login.Font = new Font("Stencil", 12F, FontStyle.Bold);
            Login.ForeColor = SystemColors.ActiveCaptionText;
            Login.HighEmphasis = true;
            Login.Icon = null;
            Login.Location = new Point(897, 6);
            Login.Margin = new Padding(4, 6, 4, 6);
            Login.MouseState = MaterialSkin.MouseState.HOVER;
            Login.Name = "Login";
            Login.NoAccentTextColor = Color.Empty;
            Login.Size = new Size(119, 43);
            Login.TabIndex = 9;
            Login.Text = "LOG IN";
            Login.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            Login.UseAccentColor = false;
            Login.UseVisualStyleBackColor = true;
            Login.Click += Login_Click;
            // 
            // SignUp
            // 
            SignUp.AutoSize = false;
            SignUp.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            SignUp.BackColor = Color.Blue;
            SignUp.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            SignUp.Depth = 0;
            SignUp.Font = new Font("Stencil", 12F, FontStyle.Bold);
            SignUp.ForeColor = SystemColors.ActiveCaptionText;
            SignUp.HighEmphasis = true;
            SignUp.Icon = null;
            SignUp.Location = new Point(1024, 6);
            SignUp.Margin = new Padding(4, 6, 4, 6);
            SignUp.MouseState = MaterialSkin.MouseState.HOVER;
            SignUp.Name = "SignUp";
            SignUp.NoAccentTextColor = Color.Empty;
            SignUp.Size = new Size(119, 43);
            SignUp.TabIndex = 10;
            SignUp.Text = "SIGN UP";
            SignUp.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            SignUp.UseAccentColor = false;
            SignUp.UseVisualStyleBackColor = false;
            SignUp.Click += SignUp_Click;
            // 
            // TitleBar
            // 
            TitleBar.BackColor = Color.RoyalBlue;
            TitleBar.BackgroundImageLayout = ImageLayout.Stretch;
            TitleBar.Controls.Add(label3);
            TitleBar.Controls.Add(pictureBox3);
            TitleBar.Controls.Add(Login);
            TitleBar.Controls.Add(SignUp);
            TitleBar.Controls.Add(Exit);
            TitleBar.Dock = DockStyle.Top;
            TitleBar.Location = new Point(0, 0);
            TitleBar.Name = "TitleBar";
            TitleBar.Size = new Size(1313, 55);
            TitleBar.TabIndex = 11;
            // 
            // label3
            // 
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Tahoma", 28.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.Silver;
            label3.Location = new Point(166, -5);
            label3.Name = "label3";
            label3.Size = new Size(281, 54);
            label3.TabIndex = 16;
            label3.Text = "PORTLINK";
            // 
            // pictureBox3
            // 
            pictureBox3.BackColor = Color.Transparent;
            pictureBox3.BackgroundImage = (Image)resources.GetObject("pictureBox3.BackgroundImage");
            pictureBox3.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox3.Location = new Point(101, 0);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(59, 55);
            pictureBox3.TabIndex = 14;
            pictureBox3.TabStop = false;
            // 
            // label1
            // 
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI Semibold", 48F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(44, 140);
            label1.Name = "label1";
            label1.Size = new Size(653, 326);
            label1.TabIndex = 14;
            label1.Text = "Secure Storage && Hassle-Free Moving";
            // 
            // label2
            // 
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Verdana", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.White;
            label2.Location = new Point(44, 475);
            label2.Name = "label2";
            label2.Size = new Size(653, 127);
            label2.TabIndex = 15;
            label2.Text = "Smart, Secure and Hassle-free logistics with seamless transportation and safe storage for every shipment.";
            // 
            // MainMenuPage
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1313, 694);
            ControlBox = false;
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(TitleBar);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MainMenuPage";
            StartPosition = FormStartPosition.CenterScreen;
            Load += MainMenuPage_Load;
            Resize += MainMenuPage_Resize;
            TitleBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ResumeLayout(false);
        }

        #endregion

        internal Button ExitMain;
        internal MaterialSkin.Controls.MaterialButton Exit;
        internal MaterialSkin.Controls.MaterialButton Login;
        internal MaterialSkin.Controls.MaterialButton SignUp;
        private Panel TitleBar;
        private PictureBox pictureBox3;
        private Label label1;
        private Label label2;
        private Label label3;
    }
}
