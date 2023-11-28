using System.IO;
using System.Threading.Tasks;

namespace DigitalRevision.DataSource
{
    class TestDataSource : DataSourceBase
    {
        public override string Name => "Test";

        public override double Version => 0.1;

        public override async Task CollectData(string folderDestination)
        {
            var testFilePath = Path.Combine(folderDestination, "test.txt");

            using (var fs = new FileStream(testFilePath, FileMode.OpenOrCreate, FileAccess.Write))
            using (var writer = new StreamWriter(fs))
            {
                for (int i = 0; i < 100; i++)
                {
                    // write some data to a file
                    await writer.WriteLineAsync("Test " + i);

                    // Fake some processing time
                    //await Task.Delay(300);

                    // Show our progress for the user
                    this.ProgressPercentage = i;
                }
            }
        }
    }
}
