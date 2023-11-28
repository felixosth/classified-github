using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TryggSTORE.Http
{
    internal class NotifyManager
    {
        private NotifyProfile[] NotifyProfiles;

        private Dictionary<NotifyProfile, DateTime> notifyProfilesDateTimes = new Dictionary<NotifyProfile, DateTime>();
        private Dictionary<NotifyProfile, bool> notifyProfilesCanNotify = new Dictionary<NotifyProfile, bool>();

        public event EventHandler<string> OnError;

        private const int _MINUTE_COOLDOWN = 20;
        //private const int _MINUTE_COOLDOWN = 5;

        public NotifyManager(NotifyProfile[] notifyProfiles)
        {
            this.NotifyProfiles = notifyProfiles;

            if (NotifyProfiles == null)
                NotifyProfiles = new NotifyProfile[0];

            foreach(var notifyProfile in notifyProfiles)
            {
                if(notifyProfile.UseDefaultSubjectAndMessage)
                {
                    notifyProfile.EmailSubject = (ConfigFile.Instance.SiteName ?? "TryggSTORE ") + $": %CURCOUNT% besökare";
                    notifyProfile.EmailMessage = $"Hej, \r\n\r\nAntalet besökare är just nu %CURCOUNT%. \r\n\r\nMvh TryggSTORE";
                }

                notifyProfilesDateTimes[notifyProfile] = DateTime.Now;
                notifyProfilesCanNotify[notifyProfile] = true;
            }

        }

        public void Start()
        {
            ConfigFile.Instance.CurrentCountChanged += CurrentCountChanged;
        }

        public void Stop()
        {
            ConfigFile.Instance.CurrentCountChanged -= CurrentCountChanged;
        }

        private void CurrentCountChanged(object sender, int count)
        {
            foreach(var notifyProfile in NotifyProfiles.Where(np => np.Enabled))
            {
                if(count >= notifyProfile.NotifyAtCount)
                {
                    if (notifyProfilesCanNotify.ContainsKey(notifyProfile) && notifyProfilesCanNotify[notifyProfile] == true)
                    {
                        if (notifyProfilesDateTimes.ContainsKey(notifyProfile))
                        {
                            if (notifyProfilesDateTimes[notifyProfile].AddMinutes(_MINUTE_COOLDOWN) < DateTime.Now)
                                Notify(notifyProfile);
                        }
                        else // We couldnt find the profile in our dict so we add it and don't notify
                        {
                            notifyProfilesDateTimes[notifyProfile] = DateTime.Now;
                        }
                    }
                }
                else
                {
                    notifyProfilesCanNotify[notifyProfile] = true;
                }
            }
        }

        private void Notify(NotifyProfile notifyProfile)
        {
            notifyProfilesDateTimes[notifyProfile] = DateTime.Now;
            notifyProfilesCanNotify[notifyProfile] = false;

            Notify(notifyProfile.EmailAddresses, AddVariables(notifyProfile.EmailSubject), AddVariables(notifyProfile.EmailMessage));
        }
        
        private string AddVariables(string str)
        {
            return str.Replace("%CURCOUNT%", ConfigFile.Instance.CurrentCount.ToString());
        }


        private void Notify(string[] emailAddresses, string subject, string message)
        {
            if (emailAddresses.Length == 0)
                return;

            string emails = "recipient[]=" + string.Join("&recipient[]=", emailAddresses);
            const string license = "0FAD3732-FDDC-4D97-BEE4-CE5BBA1B6487";

            var url = string.Format("https://portal.tryggconnect.se/api/email.php?{0}&from=TryggSTORE&subject={1}&message={2}&license={3}", emails, subject, message, license);

            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    using (var wc = new WebClient())
                    {
                        wc.DownloadString(url);
                    }
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(this, ex.ToString());
                }
            });
        }
    }
}
