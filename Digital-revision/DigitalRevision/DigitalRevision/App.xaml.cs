using DigitalRevision.Global;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;

namespace DigitalRevision
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            // -silent -emailto dev@insupport.se -name "Felix Dev"

            var argsParser = new ArgsParser(new[]
            {
                new ArgsParser.Arg("silent", true),
                new ArgsParser.Arg("emailto", false),
                new ArgsParser.Arg("name", false)
            });

            var parsedArgs = argsParser.Parse(e.Args);

            var silentFlag = parsedArgs["silent"];
            var emailTo = parsedArgs["emailto"];
            var machineName = parsedArgs["name"]?.Value ?? Dns.GetHostName();

            if (silentFlag != null)
            {
                var dataSourceManager = new DataSourceManager();
                string zipFilePath = DataSourceManager.DefaultZipPath();

                await dataSourceManager.CollectAllData(zipFilePath);

                if (emailTo != null)
                {
                    await EmailSender.SendEmailAsync(emailTo.Value, $"Digital Revision på {machineName}", $"Din digitala revision på {machineName} är klar.\r\n\r\n{zipFilePath}", "Digital Revision");
                }

                Shutdown();
            }
            else
            {
                MainWindow mw = new MainWindow();
                this.MainWindow = mw;
                mw.Show();
            }
        }
    }

    internal class ArgsParser
    {
        private readonly Arg[] existingArgs;
        internal ArgsParser(Arg[] existingArgs)
        {
            this.existingArgs = existingArgs;
        }

        public Args Parse(string[] cmdArgs)
        {
            var parsedArgs = new List<Arg>();
            for (int i = 0; i < cmdArgs.Length; i++)
            {
                cmdArgs[i] = cmdArgs[i].ToLower();
                if (cmdArgs[i][0] == '/' || cmdArgs[i][0] == '-')
                    cmdArgs[i] = cmdArgs[i].Remove(0, 1);

                foreach (var arg in existingArgs)
                {
                    if (cmdArgs[i] == arg.Command && arg.IsFlag)
                    {
                        parsedArgs.Add(arg.Clone());
                        break;
                    }
                    else if (cmdArgs[i] == arg.Command &&
                        !arg.IsFlag &&
                        i + 1 < cmdArgs.Length)
                    {
                        i++;
                        parsedArgs.Add(arg.CloneWithValue(cmdArgs[i]));
                        break;
                    }
                }
            }
            return new Args(parsedArgs);
        }

        internal class Args
        {
            internal IEnumerable<Arg> Arguments { get; set; }

            internal Args(IEnumerable<Arg> args)
            {
                Arguments = args;
            }

            internal Arg this[string command]
            {
                get => Arguments.FirstOrDefault(arg => arg.Command == command.ToLower());
            }
        }

        internal class Arg
        {
            public string Command { get; private set; }
            public string Value { get; private set; }
            public bool IsFlag { get; private set; }

            public Arg(string command, bool isFlag)
            {
                this.Command = command.ToLower();
                this.IsFlag = isFlag;
            }

            //public Arg(string command, string value, bool isFlag)
            //{
            //    this.Command = command.ToLower();
            //    this.Value = value;
            //    this.IsFlag = isFlag;
            //}

            public void SetValue(string value) => this.Value = value;

            public Arg Clone()
            {
                return new Arg(this.Command, this.IsFlag)
                {
                    Value = this.Value
                };
            }

            public Arg CloneWithValue(string value)
            {
                this.SetValue(value);
                return Clone();
            }

            public override string ToString()
            {
                return $"{Command} = {(IsFlag ? "<Flag>" : Value)}";
            }
        }
    }
}
