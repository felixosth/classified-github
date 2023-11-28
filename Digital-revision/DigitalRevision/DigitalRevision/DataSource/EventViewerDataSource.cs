using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static DigitalRevision.Global.Helper;

namespace DigitalRevision.DataSource
{
    class EventViewerDataSource : DataSourceBase
    {
        public override string Name => "Loggboken";

        public override double Version => 1.0;

        public override async Task CollectData(string folderDestination)
        {
            ProgressIsIndeterminate = true;

            var csvSep = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;

            ulong t = 365L * 3600 * 24 * 1000;

            string eventLogFilePath = Path.Combine(folderDestination, "event log data.csv");

            string eventLogQueryString =
                "<QueryList>" +
                "  <Query Id=\"0\" Path=\"Application\">" +
               $"    <Select Path=\"Application\">*[System[(Level=1  or Level=2) and TimeCreated[timediff(@SystemTime) &lt;= {t}]]]</Select>" +
               $"    <Select Path=\"Security\">*[System[(Level=1  or Level=2) and TimeCreated[timediff(@SystemTime) &lt;= {t}]]]</Select>" +
               $"    <Select Path=\"System\">*[System[(Level=1  or Level=2) and TimeCreated[timediff(@SystemTime) &lt;= {t}]]]</Select>" +
                "  </Query>" +
                "</QueryList>";

            using (var eventLog = new EventLogReader(new EventLogQuery("System", PathType.FilePath, eventLogQueryString)))
            using (var fileStream = new FileStream(eventLogFilePath, FileMode.Create))
            using (var writer = new StreamWriter(fileStream, Encoding.UTF8))
            {
                await writer.WriteLineAsync($"providerName{csvSep}description{csvSep}timeCreated{csvSep}recordId{csvSep}category{csvSep}logName");

                for (EventRecord eventEntry = eventLog.ReadEvent(); eventEntry != null; eventEntry = eventLog.ReadEvent())
                {
                    await writer.WriteAsync(StringToCSVCell(eventEntry.ProviderName) + csvSep);

                    await writer.WriteAsync(StringToCSVCell(eventEntry.FormatDescription()) + csvSep);

                    await writer.WriteAsync(StringToCSVCell(eventEntry.TimeCreated.Value.ToString()) + csvSep);

                    await writer.WriteAsync(StringToCSVCell(eventEntry.RecordId.ToString()) + csvSep);

                    await writer.WriteAsync(StringToCSVCell(eventEntry.LevelDisplayName) + csvSep);

                    await writer.WriteLineAsync(StringToCSVCell(eventEntry.LogName));

                    //await writer.WriteLineAsync($"{StringToCSVCell(providerName)}{csvSep}{StringToCSVCell(description)}{csvSep}{StringToCSVCell(timeCreated.Value.ToString())}{csvSep}{StringToCSVCell(recordId.ToString())}{csvSep}{StringToCSVCell(category)}{csvSep}{StringToCSVCell(logName)}");
                }
            }

            ProgressIsIndeterminate = false;
            ProgressPercentage = 100;
        }
    }
}
