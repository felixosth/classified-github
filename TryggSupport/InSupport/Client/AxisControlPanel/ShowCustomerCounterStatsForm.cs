using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VideoOS.Platform;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Flurl.Http;
using System.Windows.Forms.DataVisualization.Charting;

namespace InSupport.Client
{
    public partial class ShowCustomerCounterStatsForm : Form
    {
        string counterUsername { get; set; }
        string counterPassword { get; set; }
        string counterUrl = "none";


        public ShowCustomerCounterStatsForm()
        {
            InitializeComponent();

            //ClientControl.Instance.RegisterUIControlForAutoTheming(this);

            //if(Properties.Settings.Default.CounterURL == "")
            //{
            //    using (var textInput = new UserInputTextForm()
            //    {
            //        Name = "PeopleCounter URL"
            //    })
            //    {
            //        textInput.TipLabel.Text = "URL:";
            //        if (textInput.ShowDialog() == DialogResult.OK)
            //        {
            //            Properties.Settings.Default.CounterURL = textInput.ReturnedText;
            //            Properties.Settings.Default.Save();
            //            //counterUrl = Properties.Settings.Default.CounterURL;
            //        }
            //    }
            //}
            //counterUrl = Properties.Settings.Default.CounterURL;
            using (var textInput = new UserInputTextForm(Properties.Settings.Default.CounterURL) { Text = "PeopleCounter URL" })
            {
                textInput.TipLabel.Text = "URL:";
                //textInput.Name = "Enter URL";
                if (textInput.ShowDialog() == DialogResult.OK)
                {
                    counterUrl = textInput.ReturnedText;
                    Properties.Settings.Default.CounterURL = counterUrl;
                    Properties.Settings.Default.Save();
                }
                    //counterUrl = "";
            }

            //ClientControl.Instance.RegisterUIControlForAutoTheming(this);

            if (counterUrl == "none")
                Close();
        }

        private async void ShowCustomerCounterStatsForm_Load(object sender, EventArgs e)
        {
            if (counterUrl == "")
                return;

            ShowData(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-1)); // yesterday
        }

        async void ShowData(DateTime startDate, DateTime endDate)
        {
            string url = "http://" + counterUrl + "/local/people-counter/.api?export-json&date=" + startDate.ToString("yyyyMMdd") + "-" + endDate.ToString("yyyyMMdd") + "&res=1h";
            //MessageBox.Show(url);
            var counter = await(url).GetJsonAsync<Counter>();

            foreach (KeyValuePair<double, List<double>> data in counter.Data)
            {
                //var key = data.Key.ToString();
                //var newKey = double.Parse(key.Remove(key.Length - 2, 2).Remove(0, 8));


                var date = DateTime.ParseExact(data.Key.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                //MessageBox.Show(date.ToShortTimeString());

                chart1.Series[0].Points.AddXY(date, data.Value[0]);
            }

            chart1.Series[0].Name = counter.CounterInfo.Name;
            chart1.Series[0].XValueType = ChartValueType.DateTime;

            chart1.ChartAreas[0].AxisX.Interval = 1;
            chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Hours;
            chart1.ChartAreas[0].AxisX.IntervalOffset = 1;
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "yy/MM/dd HH:mm";
            //chart1.ChartAreas[0].AxisX.Minimum = startDate.ToOADate();
            //chart1.ChartAreas[0].AxisX.Maximum = endDate.ToOADate();
        }


        Point? prevPosition = null;
        ToolTip tooltip = new ToolTip();
        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.Location;
            if (prevPosition.HasValue && pos == prevPosition.Value)
                return;
            tooltip.RemoveAll();
            prevPosition = pos;
            var results = chart1.HitTest(pos.X, pos.Y, false,
                                            ChartElementType.DataPoint);
            foreach (var result in results)
            {
                if (result.ChartElementType == ChartElementType.DataPoint)
                {
                    var prop = result.Object as DataPoint;
                    if (prop != null)
                    {
                        var pointXPixel = result.ChartArea.AxisX.ValueToPixelPosition(prop.XValue);
                        var pointYPixel = result.ChartArea.AxisY.ValueToPixelPosition(prop.YValues[0]);

                        // check if the cursor is really close to the point (2 pixels around the point)
                        if (Math.Abs(pos.X - pointXPixel) < 2 &&
                            Math.Abs(pos.Y - pointYPixel) < 2)
                        {
                            tooltip.Show("X=" + prop.XValue + ", Y=" + prop.YValues[0], this.chart1,
                                            pos.X, pos.Y - 15);
                        }
                    }
                }
            }
        }

        private async void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            chart1.Series[0].Points.Clear();
            ShowData(e.Start, e.End);
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            MessageBox.Show(unixTimeStamp.ToString());
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }

    [DataContract]
    class Counter
    {
        [DataMember(Name = "counter")]
        public Info CounterInfo { get; set; }
        [DataMember(Name = "data")]
        public Dictionary<double, List<double>> Data { get; set; }


    }
    //[DataContract(Name = "counter")]
    struct Info
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "serial")]
        public string Serial { get; set; }
    }


}
