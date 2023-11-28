namespace TryggDriftPingMonitorPlugin
{
    partial class PingPluginConfiguration
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
            this.pingCountNum = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pingItemsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.addPingItemBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pingCountNum)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pingCountNum
            // 
            this.pingCountNum.Location = new System.Drawing.Point(6, 16);
            this.pingCountNum.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.pingCountNum.Name = "pingCountNum";
            this.pingCountNum.Size = new System.Drawing.Size(66, 20);
            this.pingCountNum.TabIndex = 1;
            this.pingCountNum.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Ping count";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.addPingItemBtn);
            this.groupBox1.Controls.Add(this.pingItemsPanel);
            this.groupBox1.Location = new System.Drawing.Point(6, 42);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(521, 412);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "IP\'s to ping";
            // 
            // pingItemsPanel
            // 
            this.pingItemsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pingItemsPanel.AutoScroll = true;
            this.pingItemsPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pingItemsPanel.Location = new System.Drawing.Point(6, 19);
            this.pingItemsPanel.Name = "pingItemsPanel";
            this.pingItemsPanel.Size = new System.Drawing.Size(509, 358);
            this.pingItemsPanel.TabIndex = 0;
            this.pingItemsPanel.WrapContents = false;
            // 
            // addPingItemBtn
            // 
            this.addPingItemBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addPingItemBtn.Location = new System.Drawing.Point(440, 383);
            this.addPingItemBtn.Name = "addPingItemBtn";
            this.addPingItemBtn.Size = new System.Drawing.Size(75, 23);
            this.addPingItemBtn.TabIndex = 1;
            this.addPingItemBtn.Text = "Add";
            this.addPingItemBtn.UseVisualStyleBackColor = true;
            this.addPingItemBtn.Click += new System.EventHandler(this.addPingItemBtn_Click);
            // 
            // PingPluginConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pingCountNum);
            this.Name = "PingPluginConfiguration";
            this.Size = new System.Drawing.Size(542, 457);
            ((System.ComponentModel.ISupportInitialize)(this.pingCountNum)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.NumericUpDown pingCountNum;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.FlowLayoutPanel pingItemsPanel;
        private System.Windows.Forms.Button addPingItemBtn;
    }
}
