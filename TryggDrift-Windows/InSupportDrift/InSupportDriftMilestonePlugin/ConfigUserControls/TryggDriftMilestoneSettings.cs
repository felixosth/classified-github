using System;
using System.Net;
using System.Windows.Forms;
using VideoOS.Platform.SDK.Platform;

namespace InSupport.Drift.Plugins
{
    public partial class TryggDriftMilestoneSettings : BasePluginConfig
    {
        bool testResult = false;

        public TryggDriftMilestoneSettings()
        {
            InitializeComponent();
            srvBox.Text = "http://localhost";
        }

        private void TryggDriftMilestoneSettings_Load(object sender, EventArgs e)
        {
            VideoOS.Platform.SDK.Environment.Initialize();
        }

        public override bool ValidateForm()
        {
            if (!testResult)
            {
                MessageBox.Show("Please establish a successful connection before you continue.", "Configurator");
            }
            return testResult;
        }

        public override string[] GetSettings()
        {
            string server = "MilestoneServer=" + srvBox.Text;

            if (useWindowsUsrChkBox.Checked)
                return new string[]
                {
                    server,
                    "MilestoneUseCurrent=True",
                    "MilestoneNotResThreshold=3",
                    "MilestoneAlarmList=" + sendAlarmsCheckBox.Checked.ToString()
                };
            else
                return new string[]
                {
                    server,
                    "MilestoneUsername=" + usrBox.Text,
                    "MilestonePassword=" + passBox.Text,
                    "MilestoneNotResThreshold=3",
                    "MilestoneAlarmList=" + sendAlarmsCheckBox.Checked.ToString()
                };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var uri = new Uri(srvBox.Text);

                if (useWindowsUsrChkBox.Checked)
                {
                    MessageBox.Show("When using current Windows user, the user must be assigned to the service 'TryggDrift' manually after installation.", "Please note");
                    testResult = true;
                    return;
                }
                else
                {
                    CredentialCache cc = VideoOS.Platform.Login.Util.BuildCredentialCache(uri, usrBox.Text, passBox.Text, "Basic");
                    VideoOS.Platform.SDK.Environment.AddServer(secureOnly: false, uri, cc);
                }

                VideoOS.Platform.SDK.Environment.Login(uri, false);

                testResult = VideoOS.Platform.SDK.Environment.IsLoggedIn(uri);


                if (!testResult)
                {
                    MessageBox.Show("Connection failed!", "Failure");
                }
                else
                {
                    MessageBox.Show("Connection established!", "Success");
                }
            }
            catch (InvalidCredentialsMIPException)
            {
                MessageBox.Show("Invalid login credentials", "Error");
            }
            catch (ServerNotFoundMIPException)
            {
                MessageBox.Show("Server not found", "Error");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString());
            }
            VideoOS.Platform.SDK.Environment.Logout();
            VideoOS.Platform.SDK.Environment.RemoveAllServers();

        }

        private void ChangeTestResult(object sender, EventArgs e)
        {
            testResult = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            passBox.Enabled = !useWindowsUsrChkBox.Checked;
            usrBox.Enabled = !useWindowsUsrChkBox.Checked;
        }
    }
}
