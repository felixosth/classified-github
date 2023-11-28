using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform.Search;
using VideoOS.Platform.Search.FilterConfigurations;
using UVAPSearchAgent.Shared.Filters.RadioList;
using UVAPSearchAgent.Shared.UserControl;

namespace UVAPSearchAgent.Shared
{
    // Entry point of the search result user control plugin. We define what kind of custom user controls we want to include in the plugin here.
    // All methods are overridden and will be invoked by Milestone
    public class UvapSearchUserControlsPlugin : SearchUserControlsPlugin
    {
        public static Guid UvapSearchResultUserControlID = new Guid("{24A48AE2-F92D-4DFE-9080-406C2D251817}");

        public override Guid Id { get; protected set; } = new Guid("{C60DF07D-59A5-4675-8FB4-C8C16146A0FA}");
        public override string Name { get; protected set; } = nameof(UvapSearchUserControlsPlugin);
        public override IEnumerable<Guid> SearchResultUserControlTypes { get; protected set; } = new[] { UvapSearchResultUserControlID };

        public override void Init()
        {
            base.Init();
        }

        public override SearchResultUserControl CreateSearchResultUserControl(Guid searchResultUserControlType)
        {
            // We know that all our kinds of search results will be used for the same user control so we don't bother checking what kind of result we got
            return new UvapSearchResultUsrControl();
        }

        public override IEnumerable<Type> SearchFilterConfigurationTypes { get; protected set; } = new List<Type>()
        {
            typeof(RadioListFilterConfiguration)
        };

        public override SearchFilterEditControl CreateSearchFilterEditControl(FilterConfigurationBase filterConfiguration)
        {
            if (filterConfiguration is RadioListFilterConfiguration)
                return new RadioListFilterUserControl(filterConfiguration as RadioListFilterConfiguration);

            return base.CreateSearchFilterEditControl(filterConfiguration);
        }
    }
}
