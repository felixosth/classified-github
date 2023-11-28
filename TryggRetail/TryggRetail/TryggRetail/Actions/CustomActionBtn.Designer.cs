namespace TryggRetail.Actions
{
    partial class CustomActionBtn
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
            this.actionButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // actionButton
            // 
            this.actionButton.BackColor = System.Drawing.Color.Violet;
            this.actionButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.actionButton.Location = new System.Drawing.Point(0, 0);
            this.actionButton.Name = "actionButton";
            this.actionButton.Size = new System.Drawing.Size(312, 153);
            this.actionButton.TabIndex = 0;
            this.actionButton.Text = "CustomAction";
            this.actionButton.UseVisualStyleBackColor = false;
            this.actionButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // CustomActionBtn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.actionButton);
            this.Name = "CustomActionBtn";
            this.Size = new System.Drawing.Size(312, 153);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button actionButton;
    }
}
