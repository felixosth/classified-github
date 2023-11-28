using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WinservLite
{
    public class UpdateChecker
    {
        public event EventHandler<UpdateCheckEventArgs> OnUpdateFound;
        public event EventHandler<UpdateCheckEventArgs> OnUpdateError;
        public event EventHandler<UpdateCheckEventArgs> OnUpdateComplete;
        ApplicationDeployment currentDeployment;

        bool isClosing = false;

        Version newVer;

        public UpdateChecker()
        {
            if (!Debugger.IsAttached)
            {
                currentDeployment = ApplicationDeployment.CurrentDeployment;

                currentDeployment.UpdateCompleted += CurrentDeployment_UpdateCompleted;
                new Thread(CheckThread).Start();
            }
                
        }

        private void CurrentDeployment_UpdateCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //throw new NotImplementedException();
            OnUpdateComplete?.Invoke(this, new UpdateCheckEventArgs(newVer));
        }

        public void Stop()
        {
            isClosing = true;
        }

        private void CheckThread()
        {
            while (!isClosing)
            {
                try
                {
                    var update = currentDeployment.CheckForDetailedUpdate();
                    
                    if(update.UpdateAvailable)
                    {
                        if(newVer == null)
                        {
                            Update(update);
                        }
                        else if(newVer > update.AvailableVersion)
                        {
                            Update(update);
                        }
                    }
                }
                catch(Exception ex)
                {
                    OnUpdateError(this, new UpdateCheckEventArgs(ex));
                }

                var nextCheck = DateTime.Now.AddMinutes(1);
                while(DateTime.Now < nextCheck)
                {
                    if (isClosing)
                        break;
                    Thread.Sleep(10 * 1000);
                }
            }
        }

        private void Update(UpdateCheckInfo update)
        {
            OnUpdateFound?.Invoke(this, new UpdateCheckEventArgs(update.AvailableVersion));
            newVer = update.AvailableVersion;
            currentDeployment.UpdateAsync();
        }

    }

    public class UpdateCheckEventArgs : EventArgs
    {
        public Version Version { get; set; }
        public Exception Error { get; set; }
        public UpdateCheckEventArgs(Version version)
        {
            Version = version;
        }

        public UpdateCheckEventArgs(Exception ex)
        {
            Error = ex;
        }
    }
}
