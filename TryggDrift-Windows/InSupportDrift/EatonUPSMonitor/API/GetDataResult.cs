namespace EatonUPSMonitor.API
{
    public class GetDataResult
    {
        public Data data { get; set; }
    }

    public class Data
    {
        private string _product { get; set; }
        public string product { set { _product = value; } } // set this locally but hide from serialization

        private int _model { get; set; }
        public int model { set { _model = value; } } // set this locally but hide from serialization

        public string productName => $"{_product} {_model}";

        /// <summary>
        /// commLost = UPS not reachable
        /// acpresent = on utility
        /// onbatXXX = on batter, XXX = battery level
        /// failure
        /// </summary>
        public string activeState { get; set; }
        public string power { get; set; }
        public int loadLevel { get; set; }
        /// <summary>
        /// /UPSStatus/RunningStatusOnBattery = battery
        /// /UPSStatus/RunningStatusOnUtility = on utility
        /// </summary>
        public string runningStatus { get; set; }
        public string batteryCap { get; set; }
        public string batteryRunTime { get; set; }
        public string lastNotification { get; set; }
    }
}
