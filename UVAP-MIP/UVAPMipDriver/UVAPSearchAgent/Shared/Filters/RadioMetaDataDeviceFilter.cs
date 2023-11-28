using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform.Search;
using VideoOS.Platform.Search.FilterConfigurations;
using VideoOS.Platform.Search.FilterValues;
using VideoOS.Platform;
using UVAPSearchAgent.Shared.Filters.RadioList;

namespace UVAPSearchAgent.Shared.Filters
{
    // Shared search filter that enables the user to specify the metadata device to get data from in the search. This will be populated by the search definition based on the searchscope.
    // Will be presented to the user in the UI.
    // All methods in this class is overridden from the SearchFilter class and will be invoked by Milestone
    public class RadioMetaDataDeviceFilter : SearchFilter
    {
        public IEnumerable<Item> Cameras { get; set; } // Is set by the search definition based on searchscope

        public RadioMetaDataDeviceFilter()
        {
            this.Id = new Guid("{9061DC57-CDC6-43E7-9635-D11CCF5D99F2}");
            this.Name = "Metadata device";
        }

        public override FilterValueBase CreateValue()
        {
            var value = new RadioListFilterValue();
            ResetValue(value);
            return value;
        }

        public override FilterConfigurationBase GetFilterConfiguration()
        {
            List<Item> metadataItems = new List<Item>();
            
            if (Cameras != null && Cameras.Count() > 0)
            {
                foreach (var camera in Cameras)
                {
                    var metadataDevices = camera.GetRelated().Where(i => i.FQID.Kind == Kind.Metadata); // Get the related metadata devices from the configuration api
                    metadataItems.AddRange(metadataDevices);
                }
            }

            return new RadioListFilterConfiguration(metadataItems);
        }

        public override void ResetValue(FilterValueBase value)
        {
            (value as RadioListFilterValue).SelectedObjectString = null;
            (value as RadioListFilterValue).SelectedObject = null;
        }
    }
}
