using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InSupport.Client
{
    partial class InSupportSidePanelUserControl
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
            this.buttonMax = new System.Windows.Forms.Button();
            this.buttonMin = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonMax
            // 
            this.buttonMax.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMax.Location = new System.Drawing.Point(15, 11);
            this.buttonMax.Name = "buttonMax";
            this.buttonMax.Size = new System.Drawing.Size(211, 23);
            this.buttonMax.TabIndex = 0;
            this.buttonMax.Text = "Maximize ViewItem";
            this.buttonMax.UseVisualStyleBackColor = true;
            this.buttonMax.Click += new System.EventHandler(this.OnMaximizeClick);
            // 
            // buttonMin
            // 
            this.buttonMin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMin.Location = new System.Drawing.Point(15, 39);
            this.buttonMin.Name = "buttonMin";
            this.buttonMin.Size = new System.Drawing.Size(211, 23);
            this.buttonMin.TabIndex = 1;
            this.buttonMin.Text = "Restore ViewItem";
            this.buttonMin.UseVisualStyleBackColor = true;
            this.buttonMin.Click += new System.EventHandler(this.OnRestoreClick);
            // 
            // InSupportSidePanelUserControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.buttonMin);
            this.Controls.Add(this.buttonMax);
            this.Font = new System.Drawing.Font("Arial", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "InSupportSidePanelUserControl";
            this.Size = new System.Drawing.Size(248, 78);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.Button buttonMax;
        private System.Windows.Forms.Button buttonMin;
    }
}
