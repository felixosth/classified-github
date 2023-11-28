using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;

namespace TryggDRIFT_Configurator
{
    /// <summary>
    /// Interaction logic for LogViewerWindow.xaml
    /// </summary>
    public partial class LogViewerWindow : Window
    {
        EventLogQuery eventLogQuery;
        EventLogReader eventLogReader;
        EventLogWatcher eventLogWatcher;

        public LogViewerWindow()
        {
            InitializeComponent();

            var query = "*[System[Provider[@Name='TryggDrift']]]";
            //var query = "*[System[Provider[@Name = 'TryggDrift'] and TimeCreated[timediff(@SystemTime) & lt;= 604800000]]]";
            //var query = $"*[System[Provider[@Name = 'TryggDrift'] and TimeCreated[timediff(@SystemTime) & lt;= {TimeSpan.FromDays(30).TotalMilliseconds}]]]";
            eventLogQuery = new EventLogQuery("Application", PathType.LogName, query) { ReverseDirection = false };

            eventLogReader = new EventLogReader(eventLogQuery);
            eventLogWatcher = new EventLogWatcher(eventLogQuery);
            eventLogWatcher.EventRecordWritten += EventLogWatcher_EventRecordWritten;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            EventRecord eventRecord = eventLogReader.ReadEvent();
            while (eventRecord != null)
            {
                AddTextToLog(eventRecord);
                eventRecord = eventLogReader.ReadEvent();
            }

            logTextblock.ScrollToEnd();

            eventLogWatcher.Enabled = true;

        }

        private void EventLogWatcher_EventRecordWritten(object sender, EventRecordWrittenEventArgs e)
        {
            if (e.EventRecord != null)
            {
                AddTextToLog(e.EventRecord);
            }
        }

        private void AddTextToLog(EventRecord eventRecord)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => AddTextToLog(eventRecord));
            }
            else
            {
                string eventData = "N/A";
                try
                {
                    eventData = XDocument.Parse(eventRecord.ToXml()).Descendants("{http://schemas.microsoft.com/win/2004/08/events/event}Data").First().Value;
                }
                catch { eventData = eventRecord.FormatDescription() ?? "N/A"; }
                logTextblock.AppendText($"[{eventRecord.TimeCreated}] {eventData}\r", eventRecord.Level < 4 ? "Red" : "Black");

            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            eventLogWatcher.Enabled = false;
            eventLogReader.CancelReading();
            eventLogReader.Dispose();
            eventLogWatcher.EventRecordWritten -= EventLogWatcher_EventRecordWritten;
            eventLogWatcher.Dispose();
        }
    }

    static class Ext
    {
        public static void AppendText(this RichTextBox box, string text, string color)
        {
            BrushConverter bc = new BrushConverter();
            TextRange tr = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
            tr.Text = text;
            try
            {
                tr.ApplyPropertyValue(TextElement.ForegroundProperty,
                    bc.ConvertFromString(color));
            }
            catch (FormatException) { }
        }
    }
}
