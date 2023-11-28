
namespace TryggDRIFT_Configurator
{
    partial class Installer1
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
            this.installer2 = new System.Configuration.Install.Installer();
            // 
            // installer2
            // 
            this.installer2.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.installer2_AfterInstall);
            this.installer2.BeforeUninstall += new System.Configuration.Install.InstallEventHandler(this.installer2_BeforeUninstall);
            // 
            // Installer1
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.installer2});

        }

        #endregion

        private System.Configuration.Install.Installer installer2;
    }
}