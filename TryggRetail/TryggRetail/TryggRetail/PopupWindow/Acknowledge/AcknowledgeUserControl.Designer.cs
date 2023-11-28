namespace TryggRetail.PopupWindow.Acknowledge
{
    partial class AcknowledgeUserControl
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
            this.ackBtn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.loopBtn = new System.Windows.Forms.Button();
            this.replayBtn = new System.Windows.Forms.Button();
            this.actionButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ackBtn
            // 
            this.ackBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ackBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ackBtn.BackColor = System.Drawing.Color.Red;
            this.ackBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ackBtn.ForeColor = System.Drawing.Color.Black;
            this.ackBtn.Location = new System.Drawing.Point(369, 3);
            this.ackBtn.Name = "ackBtn";
            this.ackBtn.Size = new System.Drawing.Size(681, 393);
            this.ackBtn.TabIndex = 0;
            this.ackBtn.Text = "Kvittera";
            this.ackBtn.UseVisualStyleBackColor = false;
            this.ackBtn.Click += new System.EventHandler(this.ackBtn_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.loopBtn);
            this.panel1.Controls.Add(this.replayBtn);
            this.panel1.Location = new System.Drawing.Point(1056, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(398, 393);
            this.panel1.TabIndex = 1;
            this.panel1.Resize += new System.EventHandler(this.panel1_Resize);
            // 
            // loopBtn
            // 
            this.loopBtn.BackColor = System.Drawing.Color.Orange;
            this.loopBtn.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.loopBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loopBtn.Location = new System.Drawing.Point(0, 196);
            this.loopBtn.Name = "loopBtn";
            this.loopBtn.Size = new System.Drawing.Size(398, 197);
            this.loopBtn.TabIndex = 1;
            this.loopBtn.Text = "Starta\r\nLoop";
            this.loopBtn.UseVisualStyleBackColor = false;
            this.loopBtn.Click += new System.EventHandler(this.loopBtn_Click);
            // 
            // replayBtn
            // 
            this.replayBtn.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.replayBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.replayBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.replayBtn.Location = new System.Drawing.Point(0, 0);
            this.replayBtn.Name = "replayBtn";
            this.replayBtn.Size = new System.Drawing.Size(398, 197);
            this.replayBtn.TabIndex = 0;
            this.replayBtn.Text = "Återuppspela\r\nLarmhändelse";
            this.replayBtn.UseVisualStyleBackColor = false;
            this.replayBtn.Click += new System.EventHandler(this.replayBtn_Click);
            // 
            // actionButtonsPanel
            // 
            this.actionButtonsPanel.AutoSize = true;
            this.actionButtonsPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.actionButtonsPanel.Location = new System.Drawing.Point(3, 3);
            this.actionButtonsPanel.MaximumSize = new System.Drawing.Size(0, 400);
            this.actionButtonsPanel.Name = "actionButtonsPanel";
            this.actionButtonsPanel.Size = new System.Drawing.Size(0, 393);
            this.actionButtonsPanel.TabIndex = 2;
            this.actionButtonsPanel.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.actionButtonsPanel_ControlAdded);
            this.actionButtonsPanel.Resize += new System.EventHandler(this.actionButtonsPanel_Resize);
            // 
            // AcknowledgeUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.actionButtonsPanel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ackBtn);
            this.Name = "AcknowledgeUserControl";
            this.Size = new System.Drawing.Size(1457, 399);
            this.Load += new System.EventHandler(this.AcknowledgeUserControl_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ackBtn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button loopBtn;
        private System.Windows.Forms.Button replayBtn;
        private System.Windows.Forms.FlowLayoutPanel actionButtonsPanel;
    }
}
