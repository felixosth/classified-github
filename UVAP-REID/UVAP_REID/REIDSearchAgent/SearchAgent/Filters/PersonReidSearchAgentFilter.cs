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
    public class PersonReidSearchAgentFilter : SearchFilter
    {
        private static Guid ID = new Guid("{F8CD78F2-CCAC-40EA-9239-986FC9CE6A54}");

        public PersonReidSearchAgentFilter()
        {
            this.Id = ID;
            Name = "Person key";
        }

        public override FilterValueBase CreateValue()
        {
            StringFilterValue filterValue = new StringFilterValue();
            ResetValue(filterValue);
            return filterValue;
        }

        public override FilterConfigurationBase GetFilterConfiguration()
        {
            var cfg = new TextBoxFilterConfiguration()
            {
                MaxLength = 50,
                Watermark = "Optional"
            };
            return cfg;
        }

        public override void ResetValue(FilterValueBase value)
        {
            var val = value as StringFilterValue;
            val.Text = "";
        }
    }
}
