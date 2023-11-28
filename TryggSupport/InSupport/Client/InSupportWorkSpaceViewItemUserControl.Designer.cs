namespace InSupport.Client
{
    partial class InSupportWorkSpaceViewItemUserControl
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
            this.components = new System.ComponentModel.Container();
            this.infoLabel = new System.Windows.Forms.Label();
            this.infoBox = new System.Windows.Forms.GroupBox();
            this.pcNameLabel = new System.Windows.Forms.Label();
            this.externalIpLabel = new System.Windows.Forms.Label();
            this.userInfoLabel = new System.Windows.Forms.Label();
            this.localIpLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.logoPicBox = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.updateBtn = new System.Windows.Forms.Button();
            this.updateLabel = new System.Windows.Forms.Label();
            this.checkForUpdateWorker = new System.ComponentModel.BackgroundWorker();
            this.newsFeedBrowser = new System.Windows.Forms.WebBrowser();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.ntpTimeLabel = new System.Windows.Forms.Label();
            this.criticalInformationLabel = new System.Windows.Forms.Label();
            this.updateTimeTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.helpBtn = new System.Windows.Forms.Button();
            this.emailBox = new System.Windows.Forms.GroupBox();
            this.attachedLabel = new System.Windows.Forms.Label();
            this.attachCamBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.emailTele = new System.Windows.Forms.TextBox();
            this.emailCompany = new System.Windows.Forms.TextBox();
            this.emailName = new System.Windows.Forms.TextBox();
            this.emailEmail = new System.Windows.Forms.TextBox();
            this.emailSendBtn = new System.Windows.Forms.Button();
            this.emailBody = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.infoBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPicBox)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.groupBox8.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.emailBox.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infoLabel.ForeColor = System.Drawing.Color.White;
            this.infoLabel.Location = new System.Drawing.Point(6, 22);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(88, 80);
            this.infoLabel.TabIndex = 2;
            this.infoLabel.Text = "Tel vx:\r\nSupport/Jour:\r\n\r\nÖppet M-F\r\nLunchstängt:";
            // 
            // infoBox
            // 
            this.infoBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.infoBox.BackColor = System.Drawing.Color.Transparent;
            this.infoBox.Controls.Add(this.pcNameLabel);
            this.infoBox.Controls.Add(this.externalIpLabel);
            this.infoBox.Controls.Add(this.userInfoLabel);
            this.infoBox.Controls.Add(this.localIpLabel);
            this.infoBox.ForeColor = System.Drawing.Color.White;
            this.infoBox.Location = new System.Drawing.Point(1238, 633);
            this.infoBox.Name = "infoBox";
            this.infoBox.Size = new System.Drawing.Size(244, 107);
            this.infoBox.TabIndex = 5;
            this.infoBox.TabStop = false;
            this.infoBox.Text = "Info";
            // 
            // pcNameLabel
            // 
            this.pcNameLabel.AutoSize = true;
            this.pcNameLabel.Location = new System.Drawing.Point(7, 84);
            this.pcNameLabel.Name = "pcNameLabel";
            this.pcNameLabel.Size = new System.Drawing.Size(57, 13);
            this.pcNameLabel.TabIndex = 4;
            this.pcNameLabel.Text = "datornamn";
            // 
            // externalIpLabel
            // 
            this.externalIpLabel.AutoSize = true;
            this.externalIpLabel.Location = new System.Drawing.Point(7, 39);
            this.externalIpLabel.Name = "externalIpLabel";
            this.externalIpLabel.Size = new System.Drawing.Size(44, 13);
            this.externalIpLabel.TabIndex = 3;
            this.externalIpLabel.Text = "external";
            // 
            // userInfoLabel
            // 
            this.userInfoLabel.AutoSize = true;
            this.userInfoLabel.Location = new System.Drawing.Point(7, 62);
            this.userInfoLabel.Name = "userInfoLabel";
            this.userInfoLabel.Size = new System.Drawing.Size(27, 13);
            this.userInfoLabel.TabIndex = 2;
            this.userInfoLabel.Text = "user";
            // 
            // localIpLabel
            // 
            this.localIpLabel.AutoSize = true;
            this.localIpLabel.Location = new System.Drawing.Point(7, 16);
            this.localIpLabel.Name = "localIpLabel";
            this.localIpLabel.Size = new System.Drawing.Size(15, 13);
            this.localIpLabel.TabIndex = 0;
            this.localIpLabel.Text = "ip";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.infoLabel);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(90, 173);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(247, 115);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Support och Service";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(125, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 80);
            this.label7.TabIndex = 3;
            this.label7.Text = "08-459 00 60\r\n08-459 00 66\r\n\r\n08:00-17:00\r\n12:00-13:00";
            // 
            // logoPicBox
            // 
            this.logoPicBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.logoPicBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.logoPicBox.Image = global::InSupport.Resources.Resource1.logowhite;
            this.logoPicBox.Location = new System.Drawing.Point(90, 9);
            this.logoPicBox.Name = "logoPicBox";
            this.logoPicBox.Size = new System.Drawing.Size(127, 60);
            this.logoPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logoPicBox.TabIndex = 0;
            this.logoPicBox.TabStop = false;
            this.logoPicBox.Click += new System.EventHandler(this.logoPicBox_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.updateBtn);
            this.groupBox2.Controls.Add(this.updateLabel);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(1238, 746);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(244, 66);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Uppdateringar";
            // 
            // updateBtn
            // 
            this.updateBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.updateBtn.Enabled = false;
            this.updateBtn.ForeColor = System.Drawing.Color.Black;
            this.updateBtn.Location = new System.Drawing.Point(163, 37);
            this.updateBtn.Name = "updateBtn";
            this.updateBtn.Size = new System.Drawing.Size(75, 23);
            this.updateBtn.TabIndex = 1;
            this.updateBtn.Text = "Uppdatera";
            this.updateBtn.UseVisualStyleBackColor = true;
            this.updateBtn.Click += new System.EventHandler(this.updateBtn_Click);
            // 
            // updateLabel
            // 
            this.updateLabel.AutoSize = true;
            this.updateLabel.Location = new System.Drawing.Point(6, 20);
            this.updateLabel.Name = "updateLabel";
            this.updateLabel.Size = new System.Drawing.Size(186, 13);
            this.updateLabel.TabIndex = 0;
            this.updateLabel.Text = "Det finns en ny uppdatering tillgänglig.";
            // 
            // checkForUpdateWorker
            // 
            this.checkForUpdateWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.checkForUpdateWorker_DoWork);
            // 
            // newsFeedBrowser
            // 
            this.newsFeedBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.newsFeedBrowser.Location = new System.Drawing.Point(3, 20);
            this.newsFeedBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.newsFeedBrowser.Name = "newsFeedBrowser";
            this.newsFeedBrowser.ScriptErrorsSuppressed = true;
            this.newsFeedBrowser.Size = new System.Drawing.Size(241, 472);
            this.newsFeedBrowser.TabIndex = 13;
            this.newsFeedBrowser.Url = new System.Uri("", System.UriKind.Relative);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.newsFeedBrowser);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.Color.White;
            this.groupBox3.Location = new System.Drawing.Point(1238, 132);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(247, 495);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Nyhetsflöde";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.DarkGray;
            this.pictureBox1.Location = new System.Drawing.Point(90, 72);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1395, 1);
            this.pictureBox1.TabIndex = 17;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Tag = "DoNotThemeMe";
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.ntpTimeLabel);
            this.groupBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox5.ForeColor = System.Drawing.Color.White;
            this.groupBox5.Location = new System.Drawing.Point(1238, 76);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(247, 50);
            this.groupBox5.TabIndex = 20;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Internet tid";
            // 
            // ntpTimeLabel
            // 
            this.ntpTimeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ntpTimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ntpTimeLabel.Location = new System.Drawing.Point(3, 18);
            this.ntpTimeLabel.Name = "ntpTimeLabel";
            this.ntpTimeLabel.Size = new System.Drawing.Size(241, 29);
            this.ntpTimeLabel.TabIndex = 1;
            this.ntpTimeLabel.Text = "ntpTime";
            this.ntpTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // criticalInformationLabel
            // 
            this.criticalInformationLabel.AutoSize = true;
            this.criticalInformationLabel.BackColor = System.Drawing.Color.Transparent;
            this.criticalInformationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.criticalInformationLabel.ForeColor = System.Drawing.Color.Red;
            this.criticalInformationLabel.Location = new System.Drawing.Point(223, 9);
            this.criticalInformationLabel.Name = "criticalInformationLabel";
            this.criticalInformationLabel.Size = new System.Drawing.Size(0, 55);
            this.criticalInformationLabel.TabIndex = 21;
            this.criticalInformationLabel.Tag = "DoNotThemeMe";
            // 
            // updateTimeTimer
            // 
            this.updateTimeTimer.Enabled = true;
            this.updateTimeTimer.Interval = 1000;
            this.updateTimeTimer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label4);
            this.groupBox6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox6.ForeColor = System.Drawing.Color.White;
            this.groupBox6.Location = new System.Drawing.Point(90, 76);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(247, 91);
            this.groupBox6.TabIndex = 23;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Trygghetscenter";
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(241, 70);
            this.label4.TabIndex = 0;
            this.label4.Text = "Här hittar du information hur du kontaktar\r\noss. Även information om support, ser" +
    "vice,\r\nfelsökning, lathundar, utbildning och,\r\növrig teknisk dokumentation.";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label6);
            this.groupBox7.Controls.Add(this.label5);
            this.groupBox7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox7.ForeColor = System.Drawing.Color.White;
            this.groupBox7.Location = new System.Drawing.Point(90, 294);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(247, 203);
            this.groupBox7.TabIndex = 24;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Företagsuppgifter";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(125, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(116, 112);
            this.label6.TabIndex = 1;
            this.label6.Text = "08-459 00 60\r\n08-459 00 00\r\n\r\nwww.insupport.se\r\ninfo@insupport.se\r\n\r\n556667-9766";
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(3, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(241, 182);
            this.label5.TabIndex = 0;
            this.label5.Text = "InSupport Nätverksvideo AB\r\nVallgatan 5B\r\n170 67 Solna\r\n\r\nTelefon vx:\r\nKoncern vx" +
    ":\r\n\r\nHemsida:\r\nE-post:\r\n\r\nOrg nr:                        \r\n";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox2.BackColor = System.Drawing.Color.DarkGray;
            this.pictureBox2.Location = new System.Drawing.Point(343, 73);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(1, 754);
            this.pictureBox2.TabIndex = 25;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Tag = "DoNotThemeMe";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox3.BackColor = System.Drawing.Color.DarkGray;
            this.pictureBox3.Location = new System.Drawing.Point(1231, 73);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(1, 754);
            this.pictureBox3.TabIndex = 26;
            this.pictureBox3.TabStop = false;
            this.pictureBox3.Tag = "DoNotThemeMe";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.linkLabel2);
            this.groupBox8.Controls.Add(this.linkLabel1);
            this.groupBox8.ForeColor = System.Drawing.Color.White;
            this.groupBox8.Location = new System.Drawing.Point(90, 503);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(247, 70);
            this.groupBox8.TabIndex = 16;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Teamviewer";
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel2.LinkColor = System.Drawing.SystemColors.Highlight;
            this.linkLabel2.Location = new System.Drawing.Point(6, 16);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(114, 16);
            this.linkLabel2.TabIndex = 2;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "Teamviewer Host";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.LinkColor = System.Drawing.SystemColors.Highlight;
            this.linkLabel1.Location = new System.Drawing.Point(6, 41);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(165, 16);
            this.linkLabel1.TabIndex = 1;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Teamviewer Quicksupport";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.helpBtn);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.ForeColor = System.Drawing.Color.White;
            this.groupBox4.Location = new System.Drawing.Point(7, 411);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(427, 63);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Vanliga frågor/problem";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(183, 16);
            this.label2.TabIndex = 12;
            this.label2.Text = "Här hittar ni svar till det mesta!";
            // 
            // helpBtn
            // 
            this.helpBtn.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.helpBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpBtn.ForeColor = System.Drawing.Color.Black;
            this.helpBtn.Location = new System.Drawing.Point(305, 24);
            this.helpBtn.Name = "helpBtn";
            this.helpBtn.Size = new System.Drawing.Size(116, 27);
            this.helpBtn.TabIndex = 11;
            this.helpBtn.Text = "Problemlösning";
            this.helpBtn.UseVisualStyleBackColor = true;
            this.helpBtn.Click += new System.EventHandler(this.button1_Click);
            // 
            // emailBox
            // 
            this.emailBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.emailBox.BackColor = System.Drawing.Color.Transparent;
            this.emailBox.Controls.Add(this.attachedLabel);
            this.emailBox.Controls.Add(this.attachCamBtn);
            this.emailBox.Controls.Add(this.label1);
            this.emailBox.Controls.Add(this.emailTele);
            this.emailBox.Controls.Add(this.emailCompany);
            this.emailBox.Controls.Add(this.emailName);
            this.emailBox.Controls.Add(this.emailEmail);
            this.emailBox.Controls.Add(this.emailSendBtn);
            this.emailBox.Controls.Add(this.emailBody);
            this.emailBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emailBox.ForeColor = System.Drawing.Color.White;
            this.emailBox.Location = new System.Drawing.Point(7, 3);
            this.emailBox.Name = "emailBox";
            this.emailBox.Size = new System.Drawing.Size(427, 402);
            this.emailBox.TabIndex = 4;
            this.emailBox.TabStop = false;
            this.emailBox.Text = "Kontaktformulär";
            this.emailBox.Click += new System.EventHandler(this.emailBox_Click);
            // 
            // attachedLabel
            // 
            this.attachedLabel.AutoSize = true;
            this.attachedLabel.Location = new System.Drawing.Point(151, 373);
            this.attachedLabel.Name = "attachedLabel";
            this.attachedLabel.Size = new System.Drawing.Size(0, 18);
            this.attachedLabel.TabIndex = 9;
            // 
            // attachCamBtn
            // 
            this.attachCamBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.attachCamBtn.ForeColor = System.Drawing.Color.Black;
            this.attachCamBtn.Location = new System.Drawing.Point(6, 371);
            this.attachCamBtn.Name = "attachCamBtn";
            this.attachCamBtn.Size = new System.Drawing.Size(139, 25);
            this.attachCamBtn.TabIndex = 8;
            this.attachCamBtn.Text = "Bifoga kamerabild";
            this.attachCamBtn.UseVisualStyleBackColor = true;
            this.attachCamBtn.Click += new System.EventHandler(this.attachCamBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(395, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Här kan du skicka ett meddelande direkt till vår supportavdelning.";
            // 
            // emailTele
            // 
            this.emailTele.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emailTele.Location = new System.Drawing.Point(209, 49);
            this.emailTele.Name = "emailTele";
            this.emailTele.Size = new System.Drawing.Size(212, 22);
            this.emailTele.TabIndex = 2;
            this.emailTele.Text = "Telefon nr";
            this.emailTele.Click += new System.EventHandler(this.emailTele_Click);
            this.emailTele.Enter += new System.EventHandler(this.emailTele_Enter);
            this.emailTele.Leave += new System.EventHandler(this.emailTele_Leave);
            // 
            // emailCompany
            // 
            this.emailCompany.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emailCompany.Location = new System.Drawing.Point(6, 49);
            this.emailCompany.Name = "emailCompany";
            this.emailCompany.Size = new System.Drawing.Size(197, 22);
            this.emailCompany.TabIndex = 1;
            this.emailCompany.Text = "Företag";
            this.emailCompany.Click += new System.EventHandler(this.emailCompany_Click);
            this.emailCompany.Enter += new System.EventHandler(this.emailCompany_Enter);
            this.emailCompany.Leave += new System.EventHandler(this.emailCompany_Leave);
            // 
            // emailName
            // 
            this.emailName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emailName.Location = new System.Drawing.Point(6, 79);
            this.emailName.Name = "emailName";
            this.emailName.Size = new System.Drawing.Size(197, 22);
            this.emailName.TabIndex = 3;
            this.emailName.Text = "Namn";
            this.emailName.Click += new System.EventHandler(this.emailName_Click);
            this.emailName.Enter += new System.EventHandler(this.emailName_Enter);
            this.emailName.Leave += new System.EventHandler(this.emailName_Leave);
            // 
            // emailEmail
            // 
            this.emailEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emailEmail.Location = new System.Drawing.Point(209, 79);
            this.emailEmail.Name = "emailEmail";
            this.emailEmail.Size = new System.Drawing.Size(212, 22);
            this.emailEmail.TabIndex = 4;
            this.emailEmail.Text = "E-Post";
            this.emailEmail.Click += new System.EventHandler(this.emailEmail_Click);
            this.emailEmail.Enter += new System.EventHandler(this.emailEmail_Enter);
            this.emailEmail.Leave += new System.EventHandler(this.emailEmail_Leave);
            // 
            // emailSendBtn
            // 
            this.emailSendBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emailSendBtn.ForeColor = System.Drawing.Color.Black;
            this.emailSendBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.emailSendBtn.Location = new System.Drawing.Point(346, 371);
            this.emailSendBtn.Name = "emailSendBtn";
            this.emailSendBtn.Size = new System.Drawing.Size(75, 25);
            this.emailSendBtn.TabIndex = 6;
            this.emailSendBtn.Text = "Skicka";
            this.emailSendBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.emailSendBtn.UseVisualStyleBackColor = true;
            this.emailSendBtn.Click += new System.EventHandler(this.emailSendBtn_Click);
            // 
            // emailBody
            // 
            this.emailBody.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emailBody.Location = new System.Drawing.Point(6, 109);
            this.emailBody.Multiline = true;
            this.emailBody.Name = "emailBody";
            this.emailBody.Size = new System.Drawing.Size(415, 256);
            this.emailBody.TabIndex = 5;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Controls.Add(this.emailBox);
            this.flowLayoutPanel1.Controls.Add(this.groupBox4);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(343, 72);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(889, 745);
            this.flowLayoutPanel1.TabIndex = 22;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(691, 848);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(190, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "©2017 InSupport | All Rights Reserved";
            // 
            // InSupportWorkSpaceViewItemUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(97)))), ((int)(((byte)(148)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.criticalInformationLabel);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.infoBox);
            this.Controls.Add(this.logoPicBox);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Transparent;
            this.MinimumSize = new System.Drawing.Size(1130, 585);
            this.Name = "InSupportWorkSpaceViewItemUserControl";
            this.Size = new System.Drawing.Size(1575, 865);
            this.Load += new System.EventHandler(this.InSupportWorkSpaceViewItemUserControl_Load);
            this.Click += new System.EventHandler(this.ViewItemUserControlClick);
            this.DoubleClick += new System.EventHandler(this.ViewItemUserControlDoubleClick);
            this.infoBox.ResumeLayout(false);
            this.infoBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPicBox)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.emailBox.ResumeLayout(false);
            this.emailBox.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox logoPicBox;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.GroupBox infoBox;
        private System.Windows.Forms.Label localIpLabel;
        private System.Windows.Forms.Label userInfoLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label updateLabel;
        private System.Windows.Forms.Button updateBtn;
        private System.ComponentModel.BackgroundWorker checkForUpdateWorker;
        private System.Windows.Forms.WebBrowser newsFeedBrowser;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label externalIpLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label ntpTimeLabel;
        private System.Windows.Forms.Label criticalInformationLabel;
        private System.Windows.Forms.Timer updateTimeTimer;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button helpBtn;
        private System.Windows.Forms.GroupBox emailBox;
        private System.Windows.Forms.Label attachedLabel;
        private System.Windows.Forms.Button attachCamBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox emailTele;
        private System.Windows.Forms.TextBox emailCompany;
        private System.Windows.Forms.TextBox emailName;
        private System.Windows.Forms.TextBox emailEmail;
        private System.Windows.Forms.Button emailSendBtn;
        private System.Windows.Forms.TextBox emailBody;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label pcNameLabel;
    }
}
