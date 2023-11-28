using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform.Search.FilterConfigurations;

namespace UVAPSearchAgent.Shared.Filters.RadioList
{
    // Custom search filter configuration.
    // This class is created because the API lacks a radio list user control
    public class RadioListFilterConfiguration : FilterConfigurationBase
    {
        public IEnumerable Items { get; private set; }
        public RadioListFilterConfiguration(IEnumerable items)
        {
            this.Items = items;
        }
    }
}
