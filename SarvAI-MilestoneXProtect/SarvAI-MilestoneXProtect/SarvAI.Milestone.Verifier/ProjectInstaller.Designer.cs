
namespace SarvAI.Milestone.Verifier
{
    partial class ProjectInstaller
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
            this.installer = new System.Configuration.Install.Installer();
            this.serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // installer
            // 
            this.installer.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.installer_AfterInstall);
            // 
            // serviceProcessInstaller
            // 
            this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.NetworkService;
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;
            // 
            // serviceInstaller
            // 
            this.serviceInstaller.DelayedAutoStart = true;
            this.serviceInstaller.Description = "Prototype by InSupport Nätverksvideo AB";
            this.serviceInstaller.DisplayName = "SarvAI Milestone Verifier";
            this.serviceInstaller.ServiceName = "SarvAIVerifier";
            this.serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.serviceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstaller_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.installer,
            this.serviceProcessInstaller,
            this.serviceInstaller});

        }

        #endregion

        private System.Configuration.Install.Installer installer;
        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller serviceInstaller;
    }
}