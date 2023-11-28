using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UVAPSearchAgent.Shared.Filters;
using UVAPSearchAgent.SkeletonSearchAgent.Filters;
using VideoOS.Platform.Search;
using VideoOS.Platform.Search.FilterCategories;

namespace UVAPSearchAgent.SkeletonSearchAgent
{
    // Entry point for the Skeleton model search agent
    // We create and return the SearchDefinition to Milestone in this class
    // We also store the search filters here.
    public class SkeletonSearchAgentPlugin : SearchAgentPlugin
    {
        // Instances of the search filters
        internal static readonly UvapRegionSearchAgentFilter SkeletonRegionFilter = new UvapRegionSearchAgentFilter();
        internal static readonly SkeletonTypeSearchAgentFilter SkeletonTypeFilter = new SkeletonTypeSearchAgentFilter();
        //internal static readonly MetaDataDeviceFilter MetadataDeviceFilter = new MetaDataDeviceFilter();
        internal static readonly RadioMetaDataDeviceFilter MetadataDeviceFilter = new RadioMetaDataDeviceFilter();
        internal static readonly SequenceLengthFilter SequenceLengthFilter = new SequenceLengthFilter();

        public override Guid Id { get; protected set; } = new Guid("{95A17EED-61AD-45B6-B691-63566544A424}");
        public override string Name { get; protected set; } = "UvapSkeletonSearchAgentPlugin";
        public override SearchFilterCategory SearchFilterCategory { get; protected set; }

        // Builtin method to return the searchdefinition. The actual search is done here
        public override SearchDefinition CreateSearchDefinition(SearchScope searchScope)
        {
            return new SkeletonSearchAgentDefinition(searchScope);
        }

        bool initialized = false;
        public override void Init()
        {
            if(!initialized)
            {
                // Initialze the SearchFilterCategory with the name, icon and our filters
                SearchFilterCategory = new VideoOS.Platform.Search.FilterCategories.OtherSearchFilterCategory("Skeleton", Properties.Resources.bone, new SearchFilter[]
                {
                    SkeletonRegionFilter,
                    SkeletonTypeFilter,
                    MetadataDeviceFilter,
                    SequenceLengthFilter
                });
                initialized = true;
            }
        }
    }
}
