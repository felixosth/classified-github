using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinServLib;
using WinServLib.Objects;

namespace WSLiteConsole
{
    class Program
    {
        public static ConsoleOptionMenu CurrentOption = null;

        static void Main(string[] args)
        {
            string db = Debugger.IsAttached ? "wsdb_dev2" : "wsdb";
            Console.Title = "WinservULTRALITE [" + db + "]";


            WinServ.Initialize($"Data Source=192.168.2.127\\WINSERV,1436;Initial Catalog={db};User ID=wslite;Password=wslite;");

            WinServ.GetTechnicians();

            ConsoleOptionMenu co = new ConsoleOptionMenu(null);

            var options = new List<ConsoleOption>()
            {
                new ConsoleOption( "Search jobs", () =>
                    {
                        Console.Clear();
                        var jobs = WinServ.JobManager.SearchJobs(GetUserTextInput("Search criteria: "), Job.JobStatusType.Active);
                        if(jobs.Count == 0)
                        {
                            Console.WriteLine("No jobs found.");
                            Console.WriteLine("Press any key to return.");
                            Console.ReadKey();
                            throw new ReturnToParentException();
                        }
                        else
                            DisplayJobsWithMenu(jobs, co);
                    }
                ),
                new ConsoleOption( "Exit", () => { Environment.Exit(0); } )
            };

            co.SetOptions(options).GetSelection();

            Console.ReadKey();
        }

        static void ColorWrite(string text, ConsoleColor color)
        {
            var prevCol = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = prevCol;
        }

        static string GetUserTextInput(string label)
        {
            Console.Write(label);
            Console.CursorVisible = true;
            var text = Console.ReadLine();
            Console.CursorVisible = false;
            return text;
        }

        static void DisplayJobsWithMenu(List<Job> jobs, ConsoleOptionMenu parent)
        {
            ConsoleOptionMenu consoleOption = new ConsoleOptionMenu(null, parent);
            var jobsWithOptions = new List<ConsoleOption>()
            {
                new ConsoleOption("Go back", () => { throw new ReturnToParentException(); }),
            };

            foreach(var job in jobs)
            {
                jobsWithOptions.Add(new ConsoleOption(string.Format("[{0}] {1}", job.JobID, job.SiteName), () => { DisplayJob(job, consoleOption); }, job.CompleteJobDescription));
            }

            consoleOption.SetOptions(jobsWithOptions).GetSelection();
        }

        static void DisplayJob(Job job, ConsoleOptionMenu parent)
        {
            var co = new ConsoleOptionMenu(null, parent: parent, returnToTop: true);

            var options = new List<ConsoleOption>()
            {
                new ConsoleOption("Go back", null ),
                new ConsoleOption("Time reports", () => { DisplayTimeReports(job, co); }),
                new ConsoleOption("Description: \r\n" + job.CompleteJobDescription, null) { Enabled = false }
            };


            co.SetOptions(options).Render();

            co.GetSelection(false);
        }

        static void DisplayTimeReports(Job job, ConsoleOptionMenu parent)
        {
            var co = new ConsoleOptionMenu(null, parent);

            var options = new List<ConsoleOption>()
            {
                new ConsoleOption("Go back", null),
                new ConsoleOption("Add new", null)
            };

            foreach(TimeReport tr in job.GetTimeReports())
            {
                string opt = string.Format("[{0}, {1}] {2}", tr.Technician, tr.Date.ToShortDateString(), tr.Comment);
                options.Add(new ConsoleOption(opt, null));
            }

            co.SetOptions(options).GetSelection();
        }
    }

    public class ConsoleOptionMenu
    {
        public ConsoleColor SelectedColor { get; set; }
        private int LastSelectedIndex { get; set; }
        private int SelectedIndex { get; set; }

        List<ConsoleOption> consoleOptions;
        public ConsoleOptionMenu Parent { get; set; }

        private bool ReturnToTop { get; set; }

        public ConsoleOptionMenu(List<ConsoleOption> consoleOptions, ConsoleOptionMenu parent = null, bool returnToTop = false)
        {
            ReturnToTop = returnToTop;
            SelectedColor = ConsoleColor.Red;
            Parent = parent;
            this.consoleOptions = consoleOptions;
            SelectedIndex = 1;
        }

        public ConsoleOptionMenu SetOptions(List<ConsoleOption> options)
        {
            this.consoleOptions = options;
            return this;
        }

        public ConsoleOptionMenu GetSelection(bool render = true)
        {
            Program.CurrentOption = this;

            Console.CursorVisible = false;

            if(render)
                Render();

            ConsoleKey inputKey = ConsoleKey.Z;

            while(inputKey != ConsoleKey.Enter)
            {
                inputKey = Console.ReadKey(true).Key;
                LastSelectedIndex = SelectedIndex;
                switch(inputKey)
                {
                    case ConsoleKey.DownArrow:
                        if (SelectedIndex != consoleOptions.Count)
                            SelectedIndex++;
                        break;
                    case ConsoleKey.UpArrow:
                        if (SelectedIndex != 1)
                            SelectedIndex--;
                        break;
                    case ConsoleKey.Enter:
                        try
                        {
                            if(!consoleOptions[SelectedIndex - 1].Enabled)
                            {
                                inputKey = ConsoleKey.A;
                                break;
                            }

                            if (consoleOptions[SelectedIndex - 1].Action == null)
                            {
                                throw new ReturnToParentException();
                            }
                            else
                                consoleOptions[SelectedIndex - 1].Action();
                        }
                        catch(ReturnToParentException)
                        {
                            ShowParent();
                        }
                        break;
                }

                if (SelectedIndex != LastSelectedIndex)
                    ReRenderSelection(true);
            }
            return this;
        }

        public void ShowParent()
        {
            if (Parent != null)
            {
                if(Parent.ReturnToTop)
                    Parent.SelectedIndex = 1;

                Parent.GetSelection();
            }
            else
            {
                if(ReturnToTop)
                    SelectedIndex = 1;

                GetSelection();
            }
        }

        public void ReRenderSelection(bool renderExtra = false)
        {
            Console.SetCursorPosition(0, GetAbsoluteOptionPos(LastSelectedIndex - 1));
            Console.Write("{0}. {1}", LastSelectedIndex, consoleOptions[LastSelectedIndex - 1].Text);

            if (renderExtra)
            {
                Console.SetCursorPosition(0, Console.CursorTop + 1);
                Console.Write(consoleOptions[LastSelectedIndex - 1].ExtraText);
            }

            Console.SetCursorPosition(0, GetAbsoluteOptionPos(SelectedIndex - 1));
            Console.ForegroundColor = SelectedColor;
            Console.Write("{0}. {1}", SelectedIndex, consoleOptions[SelectedIndex - 1].Text);

            if(renderExtra)
            {
                Console.SetCursorPosition(0, Console.CursorTop + 1);
                Console.Write(consoleOptions[SelectedIndex - 1].ExtraText);
            }


            Console.ForegroundColor = ConsoleColor.Gray;
        }

        int GetAbsoluteOptionPos(int index)
        {
            int rowCount = 0;

            for (int i = 0; i < consoleOptions.Count; i++)
            {
                if (i == index)
                {
                    break;
                }
                rowCount += consoleOptions[i].ExtraRows;
            }

            return index + rowCount;
        }

        public ConsoleOptionMenu Render()
        {
            Console.Clear();

            Console.SetCursorPosition(0, 0);
            //Console.SetWindowPosition(0, 0);
            int count = 1;
            foreach(var option in consoleOptions)
            {
                if(count == SelectedIndex)
                    Console.ForegroundColor = SelectedColor;

                Console.WriteLine("{0}. {1}", count, option.Text);

                if (option.ExtraText != null)
                    Console.WriteLine(option.ExtraText);

                Console.ForegroundColor = ConsoleColor.Gray;

                count++;
            }

            Console.SetWindowPosition(0, GetAbsoluteOptionPos(SelectedIndex - 1));

            return this;
        }
    }

    public class ConsoleOption
    {
        public Action Action { get; set; }
        public string Text { get; set; }

        public string ExtraText { get; set; }

        public int ExtraRows { get; set; }

        public bool Enabled { get; set; }

        public ConsoleOption(string text, Action action, string extra = null)
        {
            this.Text = text;
            Enabled = true;
            ExtraRows = text.Length / Console.BufferWidth;
            //ExtraRows = text.Length / Console.BufferWidth;

            if (extra != null)
            {
                ExtraRows += 1;
                ExtraRows += TextTool.CountStringOccurrences(extra, "\r\n");
                foreach(var line in extra.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    ExtraRows += line.Length / Console.BufferWidth;
                }
            }


            this.Action = action;
            this.ExtraText = extra;
        }
    }

    public static class TextTool
    {
        /// <summary>
        /// Count occurrences of strings.
        /// </summary>
        public static int CountStringOccurrences(string text, string pattern)
        {
            // Loop through all instances of the string 'text'.
            int count = 0;
            int i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }
    }

    class ReturnToParentException : Exception
    {

    }
}
