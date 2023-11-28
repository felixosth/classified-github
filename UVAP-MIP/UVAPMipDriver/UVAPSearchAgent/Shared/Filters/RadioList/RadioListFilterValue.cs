using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform.Search.FilterValues;
using VideoOS.Platform;
using Newtonsoft.Json;

namespace UVAPSearchAgent.Shared.Filters.RadioList
{
    // Custom search filter value. Consider looking over this class for potential improvements.
    // This class is created because the API lacks a radio list user control
    public class RadioListFilterValue : FilterValueBase
    {
        [JsonIgnore]
        private string _selectedObjectString;

        public string SelectedObjectString { get => _selectedObjectString;
            set
            {
                _selectedObjectString = value;
                FireChangedEvent();
            }
        }

        [JsonIgnore]
        public object SelectedObject { get; set; }

        FQID _mipItem;
        [JsonIgnore]
        public FQID SelectedMIPItem { get => _mipItem; set
            {
                _mipItem = value;
                mipItemString = _mipItem.ToXmlNode().OuterXml;
            }
        }

        private string mipItemString { get; set; }

        public RadioListFilterValue()
        {
        }

        public override bool Deserialize(string data)
        {
            try
            {
                var radioListVal = JsonConvert.DeserializeObject<RadioListFilterValue>(data);

                SelectedMIPItem = new FQID(radioListVal.mipItemString);
                SelectedObjectString = radioListVal.SelectedObjectString;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override string FallbackDisplayValue()
        {
            return SelectedObjectString ?? "N/A";
        }

        public override string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
