﻿namespace TryggRetail.Actions
{
    partial class TriggerEvent_Editor
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
            this.selectedEventLabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // selectedEventLabel
            // 
            this.selectedEventLabel.AutoSize = true;
            this.selectedEventLabel.Location = new System.Drawing.Point(3, 14);
            this.selectedEventLabel.Name = "selectedEventLabel";
            this.selectedEventLabel.Size = new System.Drawing.Size(94, 13);
            this.selectedEventLabel.TabIndex = 0;
            this.selectedEventLabel.Text = "No event selected";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 30);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(91, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Select Event";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // TriggerEvent_Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.selectedEventLabel);
            this.Name = "TriggerEvent_Editor";
            this.Size = new System.Drawing.Size(142, 82);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label selectedEventLabel;
        private System.Windows.Forms.Button button1;
    }
}
