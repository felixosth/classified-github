using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform.Search;
using VideoOS.Platform.Search.FilterConfigurations;
using VideoOS.Platform.Search.FilterValues;

namespace REIDSearchAgent.SearchAgent
{
    public class OnlyUnrecognizedReidSearchAgentFilter : SearchFilter
    {
        public OnlyUnrecognizedReidSearchAgentFilter()
        {
            this.MultipleValuesAllowed = false;
            //Name = "Only show unrecognized people";
        }

        public override FilterValueBase CreateValue()
        {
            var value = new BoolFilterValue();
            ResetValue(value);
            return value;
        }

        public override FilterConfigurationBase GetFilterConfiguration()
        {
            var cfg = new CheckBoxFilterConfiguration();
            cfg.CheckBoxTextValue = "Only unrecognized people";
            return cfg;
        }

        public override void ResetValue(FilterValueBase value)
        {
            var val = value as BoolFilterValue;
            if(val != null)
                val.Value = false;
        }
    }
}
