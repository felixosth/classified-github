namespace TryggRetail.Playback
{
    partial class PlaybackAlarmUserControl
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
            this.panelPlaybackControl = new System.Windows.Forms.Panel();
            this.panelVideo = new System.Windows.Forms.Panel();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // panelPlaybackControl
            // 
            this.panelPlaybackControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPlaybackControl.BackColor = System.Drawing.Color.White;
            this.panelPlaybackControl.Location = new System.Drawing.Point(0, 431);
            this.panelPlaybackControl.Name = "panelPlaybackControl";
            this.panelPlaybackControl.Size = new System.Drawing.Size(826, 50);
            this.panelPlaybackControl.TabIndex = 6;
            // 
            // panelVideo
            // 
            this.panelVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelVideo.Location = new System.Drawing.Point(0, 0);
            this.panelVideo.Name = "panelVideo";
            this.panelVideo.Size = new System.Drawing.Size(826, 481);
            this.panelVideo.TabIndex = 7;
            this.panelVideo.Paint += new System.Windows.Forms.PaintEventHandler(this.panelVideo_Paint_1);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // PlaybackAlarmUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelPlaybackControl);
            this.Controls.Add(this.panelVideo);
            this.Name = "PlaybackAlarmUserControl";
            this.Size = new System.Drawing.Size(826, 481);
            this.Load += new System.EventHandler(this.UserControl1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelPlaybackControl;
        private System.Windows.Forms.Panel panelVideo;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}
