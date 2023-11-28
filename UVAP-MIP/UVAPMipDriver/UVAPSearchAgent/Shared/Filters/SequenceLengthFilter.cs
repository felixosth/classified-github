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
    // Shared search filter that enables the user to specify the sequence length parameter The search method will skip X amount of seconds when a search result is found. 
    // Will be presented to the user in the UI.
    // All methods in this class is overridden from the SearchFilter class and will be invoked by Milestone
    public class SequenceLengthFilter : SearchFilter
    {
        public SequenceLengthFilter()
        {
            Id = new Guid("{3ACA7F0E-1F78-497D-B014-D7C5F5975CB6}");
            Name = "Sequence length";
        }

        public override FilterValueBase CreateValue()
        {
            var value = new DoubleFilterValue();
            ResetValue(value);
            return value;
        }

        public override FilterConfigurationBase GetFilterConfiguration()
        {
            var filterConfig = new SliderFilterConfiguration();
            filterConfig.Maximum = 600;
            filterConfig.Minimum = 1;
            filterConfig.StepSize = 5;
            return filterConfig;
        }

        public override void ResetValue(FilterValueBase value)
        {
            var doubleFilterVal = value as DoubleFilterValue;
            doubleFilterVal.Value = 10;
        }
    }
}
