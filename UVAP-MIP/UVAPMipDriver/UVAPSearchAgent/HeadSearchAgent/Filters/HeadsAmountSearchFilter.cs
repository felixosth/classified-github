using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform.Search;
using VideoOS.Platform.Search.FilterConfigurations;
using VideoOS.Platform.Search.FilterValues;

namespace UVAPSearchAgent.HeadSearchAgent.Filters
{
    // Search filter to enable the user to specify how many present heads will be in the selected camera region. Will be presented to the user in the UI.
    // All methods in this class is overridden from the SearchFilter class and will be invoked by Milestone
    public class HeadsAmountSearchFilter : SearchFilter
    {
        public HeadsAmountSearchFilter()
        {
            this.Name = "Detections";
            this.Id = new Guid("{2B36221C-B1A5-4EF6-913E-B4FD4C03EA15}");
        }

        public override FilterValueBase CreateValue()
        {
            var value = new DoubleRangeFilterValue();
            ResetValue(value);
            return value;
        }

        public override FilterConfigurationBase GetFilterConfiguration()
        {
            return new RangeSliderFilterConfiguration()
            {
                Minimum = 1, Maximum = 150,
                RangeMin = 1, RangeMax = 150,
                StepSize = 1,
                DisplayMode = EditControlDisplayMode.SnapToParentWidth
            };
        }

        public override void ResetValue(FilterValueBase value)
        {
            (value as DoubleRangeFilterValue).High = 150;
            (value as DoubleRangeFilterValue).Low = 1;
        }
    }
}
