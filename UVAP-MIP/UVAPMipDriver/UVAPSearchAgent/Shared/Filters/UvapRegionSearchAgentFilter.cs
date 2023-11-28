using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform.Search;
using VideoOS.Platform.Search.FilterConfigurations;
using VideoOS.Platform.Search.FilterValues;

namespace UVAPSearchAgent.Shared.Filters
{
    // Shared search filter that enables the user to specify a region in the camera and a confidence threshold. This user control is builtin the SDK. Will be presented to the user in the UI.
    // All methods in this class is overridden from the SearchFilter class and will be invoked by Milestone
    public class UvapRegionSearchAgentFilter : SearchFilter
    {
        private static Guid _id = new Guid("{136C468D-D793-4DE8-A378-1EAE9F420BF5}");
        public UvapRegionSearchAgentFilter()
        {
            this.Id = _id;
            Name = "Camera region";
        }

        public override FilterValueBase CreateValue()
        {
            RegionSelectionFilterValue value = new RegionSelectionFilterValue();
            ResetValue(value);
            return value;
        }

        public override FilterConfigurationBase GetFilterConfiguration()
        {
            return new RegionSelectionFilterConfiguration() { };
        }

        public override void ResetValue(FilterValueBase value)
        {
            RegionSelectionFilterValue regionValue = value as RegionSelectionFilterValue;
            if(regionValue == null)
            {
                throw new ArgumentException("Does not match expected type: " +
                            typeof(RegionSelectionFilterValue).Name,
                            nameof(value));
            }

            regionValue = new RegionSelectionFilterValue();
        }

        public override string GetDisplayValue(FilterValueBase value)
        {
            int regionsWithSelection = 0;

            var selections = (value as RegionSelectionFilterValue).Selections;
            foreach (var selection in selections)
            {
                if (selection.Mask.Contains("0"))
                    regionsWithSelection++;
            }

            return $"{regionsWithSelection} cameras";
        }
    }
}
