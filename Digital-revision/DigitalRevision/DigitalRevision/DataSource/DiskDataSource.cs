using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace DigitalRevision.DataSource
{
    enum TextHighlight { Normal, Danger, None }

    class DiskDataSource : DataSourceBase
    {
        private const int IndentStep = 20;

        public override string Name => "Disk Info";
        public override double Version => 1.0;

        private bool errors;
        private Document document;
        private Section section;

        public override async Task CollectData(string folderDestination)
        {
            await Task.Run(() =>
            {
                ProgressIsIndeterminate = true;

                CreatePdf();
                AddHeading1("Disk Info report");
                string path = Path.Combine(folderDestination, "disks.pdf");
                var volumes = DriveInfo.GetDrives();

                foreach (var volume in volumes)
                {
                    if (volume.DriveType == DriveType.CDRom)
                        continue;
                    AddHeading2(0, volume.Name);
                    int indentSteps = 1;
                    int usedPercentage = (int)(volume.TotalFreeSpace * 100 / volume.TotalSize);
                    AddKeyValue(indentSteps, "Used space: ", usedPercentage + "%", usedPercentage >= 90 ? TextHighlight.Danger : TextHighlight.Normal);
                    double oneTerabyte = Math.Pow(1024, 4);
                    string size = (volume.TotalSize / oneTerabyte).ToString(
                        "n3",
                        new NumberFormatInfo()
                        {
                            NumberDecimalSeparator = ".", //Use . for consistency with storcli
                        }
                    ) + " TB";
                    AddKeyValue(indentSteps, "Size: ", size);
                    AddKeyValue(indentSteps, "Drive type: ", volume.DriveType);
                    AddKeyValue(indentSteps, "IsReady: ", volume.IsReady);
                }
                section.AddParagraph();

                AddStorcliOutput();
                ProgressPercentage = 100;
                ProgressIsIndeterminate = false;
                SavePdf(path);
            });
        }

        private void AddStorcliOutput()
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "storcli64.exe";
            p.StartInfo.Arguments = "/call /sall show all j";
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            //output = File.ReadAllText("callsall.txt"); //For debugging

            errors = false;
            var summaryHeading = AddHeading2(0);
            var summary = section.AddParagraph();
            section.AddParagraph();
            var errorDrives = new List<string>();
            bool controllersFound = false;

            var controllers = JsonConvert.DeserializeObject<dynamic>(output)["Controllers"].ToObject(typeof(dynamic[]));
            foreach (var controller in controllers)
            {
                var commandStatus = controller["Command Status"];
                if (commandStatus["Status"] == "Failure")
                {
                    section.AddParagraph((string)commandStatus["Description"]);
                    continue;
                }
                controllersFound = true;
                var controllerFt = AddHeading2(0, "Controller " + (string)commandStatus["Controller"]);
                controllerFt.Color = Colors.Green;

                var data = (Dictionary<string, dynamic>)controller["Response Data"].ToObject(typeof(Dictionary<string, dynamic>));
                FormattedText driveFt = null;

                foreach (var dataKvp in data)
                {
                    string key = dataKvp.Key;
                    //Drive name is everything before second ' '
                    int driveNameEndIndex = key.IndexOf(' ', key.IndexOf(' ') + 1);
                    string drive = driveNameEndIndex == -1 ? key : key.Substring(0, driveNameEndIndex);
                    if (key == drive)
                    {
                        driveFt = AddHeading3(1, key);
                        driveFt.Color = Colors.Green;
                    }
                    else if (key.Contains("Detailed Information"))
                    {
                        var detailedInfo = (Dictionary<string, dynamic>)dataKvp.Value.ToObject(typeof(Dictionary<string, dynamic>));
                        foreach (var detailedInfoKvp in detailedInfo)
                        {
                            AddDetailedInfo(detailedInfoKvp);
                            if (errors)
                            {
                                errorDrives.Add(drive);
                                // If one or more attributes of a drive indicate an error, make drive name red
                                // If one or more drives in a controller has an error, make controller name red
                                controllerFt.Color = driveFt.Color = Colors.Red;
                                errors = false;
                            }
                        }

                        section.AddParagraph();
                    }
                }
            }
            if (controllersFound)
            {
                summaryHeading.AddText("StorCLI summary");

                if (errorDrives.Count == 0)
                    summary.AddText("No errors reported.");
                else
                {
                    summary.AddText("The following drives reported errors:");
                    summary.AddLineBreak();
                    foreach (var drive in errorDrives)
                    {
                        var ft = summary.AddFormattedText(drive);
                        ft.Font.Name = "Consolas";
                        summary.AddLineBreak();
                    }
                }
            }
        }

        private void AddDetailedInfo(KeyValuePair<string, dynamic> kvp)
        {
            int indentSteps = 2;
            if (kvp.Key.Contains("State"))
            {
                var state = (Dictionary<string, object>)kvp.Value.ToObject(typeof(Dictionary<string, object>));

                //Text is highlighted red if error, otherwise green
                // Shield counter = the number of times a drive error has been automatically fixed
                // A value of > 0 is probably not a big deal and is not highlighted
                AddKeyValue(indentSteps, state, "Shield Counter");
                AddKeyValue(indentSteps, state, "Media Error Count", 0);
                AddKeyValue(indentSteps, state, "Other Error Count", 0);
                AddKeyValue(indentSteps, state, "BBM Error Count", 0);
                AddKeyValue(indentSteps, state, "Predictive Failure Count", 0);
                AddKeyValue(indentSteps, state, "S.M.A.R.T alert flagged by drive", "No");
                AddKeyValue(indentSteps, state, "Drive Temperature");
            }
            else if (kvp.Key.Contains("Device attributes"))
            {
                var attributes = (Dictionary<string, object>)kvp.Value.ToObject(typeof(Dictionary<string, object>));
                AddKeyValue(indentSteps, attributes, "SN");
                AddKeyValue(indentSteps, attributes, "Model Number");
                AddKeyValue(indentSteps, attributes, "Raw size");
                AddKeyValue(indentSteps, attributes, "Coerced size");
            }
        }

        private void CreatePdf()
        {
            document = new Document();
            section = document.AddSection();
        }

        private void SavePdf(string path)
        {
            document.UseCmykColor = true;
            var pdfRenderer = new PdfDocumentRenderer(true);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();
            pdfRenderer.PdfDocument.Save(path);
        }

        private FormattedText AddHeading1(string text = "")
        {
            var p = section.AddParagraph();
            p.Format.SpaceAfter = 15;
            p.Format.Alignment = ParagraphAlignment.Center;
            var ft = p.AddFormattedText(text, TextFormat.Underline);
            ft.Font.Size = 20;
            ft.Color = Colors.DarkBlue;
            return ft;
        }

        private FormattedText AddHeading2(int indentSteps, string text = "")
        {
            var p = section.AddParagraph();
            p.Format.SpaceAfter = 5;
            p.Format.LeftIndent = IndentStep * indentSteps;
            var ft = p.AddFormattedText(text);
            ft.Color = Colors.DarkBlue;
            ft.Font.Size = 14;
            return ft;
        }

        private FormattedText AddHeading3(int indentSteps, string text = "")
        {
            var p = section.AddParagraph();
            p.Format.SpaceAfter = 5;
            p.Format.LeftIndent = IndentStep * indentSteps;
            var ft = p.AddFormattedText(text);
            ft.Font.Size = 12;
            ft.Color = Colors.DarkBlue;
            return ft;
        }

        private void AddKeyValue(int indentSteps, Dictionary<string, object> dictionary, string key, string normalValue = null)
        {
            var value = dictionary[key].ToString();
            var highlight = TextHighlight.None;
            if (normalValue != null)
                highlight = value == normalValue ? TextHighlight.Normal : TextHighlight.Danger;
            AddKeyValue(indentSteps, key + ": ", value, highlight);
        }

        private void AddKeyValue(int indentSteps, Dictionary<string, object> dictionary, string key, int maxNormalValue)
        {
            AddKeyValue(indentSteps, key + ": ", dictionary[key], maxNormalValue);
        }

        private void AddKeyValue(int indentSteps, string key, object value, int maxNormalValue)
        {
            var highlight = Convert.ToInt32(value) > maxNormalValue ? TextHighlight.Danger : TextHighlight.Normal;
            AddKeyValue(indentSteps, key, value, highlight);
        }

        private void AddKeyValue(int indentSteps, string key, object value, TextHighlight highlight = TextHighlight.None)
        {
            var p = section.AddParagraph();
            p.Format.Font.Name = "Consolas";
            p.Format.LeftIndent = IndentStep * indentSteps;

            var keyFt = p.AddFormattedText(key, TextFormat.Bold);
            var valueFt = p.AddFormattedText(value.ToString());
            if (highlight == TextHighlight.Normal)
                valueFt.Color = Colors.Green;
            else if (highlight == TextHighlight.Danger)
            {
                keyFt.Color = Colors.Red;
                valueFt.Color = Colors.Red;
                errors = true;
            }
        }
    }
}