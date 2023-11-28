using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UVAPSearchAgent.HeadSearchAgent.Filters;
using UVAPSearchAgent.Shared.Filters;
using VideoOS.Platform.Search;

namespace UVAPSearchAgent.HeadSearchAgent
{
    // Entry point for the Headdetection model search agent
    // We create and return the SearchDefinition to Milestone in this class
    // We also store the search filters here.
    public class HeadSearchAgentPlugin : SearchAgentPlugin
    {
        // Instances of the search filters
        internal static readonly RadioMetaDataDeviceFilter MetadataDeviceFilter = new RadioMetaDataDeviceFilter();
        internal static readonly UvapRegionSearchAgentFilter RegionFilter = new UvapRegionSearchAgentFilter();
        internal static readonly HeadsAmountSearchFilter HeadsAmountFilter = new HeadsAmountSearchFilter();
        internal static readonly SequenceLengthFilter SequenceLengthFilter = new SequenceLengthFilter();

        public HeadSearchAgentPlugin()
        {
        }

        public override Guid Id { get; protected set; } = new Guid("{B9DC8AED-2EDD-43CF-8F7A-40E005847A1B}");
        public override string Name { get; protected set; } = "UvapHeadSearchAgentPlugin";

        public override SearchFilterCategory SearchFilterCategory { get; protected set; } // Set in the constructor

        // Builtin method to return the searchdefinition. The actual search is done here
        public override SearchDefinition CreateSearchDefinition(SearchScope searchScope)
        {
            return new HeadSearchAgentDefinition(searchScope);
        }

        bool initialized = false;
        public override void Init()
        {
            if (!initialized)
            {
                // Initialze the SearchFilterCategory with the name, icon and our filters
                SearchFilterCategory = new VideoOS.Platform.Search.FilterCategories.OtherSearchFilterCategory("Heads", Properties.Resources.head2, new SearchFilter[]
                {
                    RegionFilter,
                    MetadataDeviceFilter,
                    HeadsAmountFilter,
                    SequenceLengthFilter
                }) ;
                initialized = true;
            }
        }
    }
}
