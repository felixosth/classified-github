using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform.Search;
using VideoOS.Platform.Search.FilterConfigurations;
using VideoOS.Platform.Search.FilterValues;

namespace UVAPSearchAgent.SkeletonSearchAgent.Filters
{
    // Search filter to enable the user to select multiple skeleton points. Will be presented to the user in the UI.
    // All methods in this class is overridden from the SearchFilter class and will be invoked by Milestone
    public class SkeletonTypeSearchAgentFilter : SearchFilter
    {
        private static Guid _id = new Guid("{537C413A-247F-4D2B-B15C-74747CC8C51D}");

        public SkeletonTypeSearchAgentFilter()
        {
            Id = _id;
            Name = "Skeleton type";

            this.MultipleValuesAllowed = true;
        }

        public override FilterValueBase CreateValue()
        {
            SelectionFilterValue value = new SelectionFilterValue();
            ResetValue(value);
            return value;
        }

        public override FilterConfigurationBase GetFilterConfiguration()
        {
            var cfg = new ListSelectionFilterConfiguration();
            
            foreach(var item in SkeletonPointType.PrettySkeletonPointTypes)
            {
                cfg.Items.Add(item.Key, item.Value);
            }

            return cfg;
        }

        public override void ResetValue(FilterValueBase value)
        {
            var selectionValue =  value as SelectionFilterValue;
            if (selectionValue == null)
                throw new ArgumentException();
            selectionValue.SetSelectedIds(Enumerable.Empty<Guid>());
        }

    }

    // Static dictionaries to convert the skeleton point type to a pretty string
    internal static class SkeletonPointType
    {
        internal static Dictionary<Guid, string> PrettySkeletonPointTypes { get; } = new Dictionary<Guid, string>()
        {
            { new Guid("{D64BB8F7-489D-4473-B0C6-2317156B2758}"), "Nose" },
            { new Guid("{3A27D08A-5397-4870-96BD-F8089AECFA1D}"), "Neck" },
            { new Guid("{1751A2AC-D3C1-4623-9D2F-CEA0866803B5}"), "Right shoulder"},
            { new Guid("{DF9F778F-56F4-47C9-A11A-C2BFA2EEC752}"), "Right elbow" },
            { new Guid("{AA447CEC-71D3-4BC3-95F9-D60012308F9D}"), "Right wrist" },
            { new Guid("{7CD1440F-3F64-4D80-9022-6A27C4C606FB}"), "Left shoulder" },
            { new Guid("{D0A7E1C9-C57A-46B9-9715-9E10B408F198}"), "Left elbow" },
            { new Guid("{417AC925-DE87-4C25-836F-83C8259BC5AF}"), "Left wrist" },
            { new Guid("{7A7C20CF-9885-45CA-A9F3-A2793A83EB01}"), "Right hip" },
            { new Guid("{3CBD6FF8-B9EB-4E47-A142-EA00D0389F21}"), "Right knee" },
            { new Guid("{A3F510A1-CAC4-4FD1-AF6F-5F569F225DDD}"), "Right ankle" },
            { new Guid("{CE85AEDD-AFE5-4CC8-BBE1-5700B4AEDEEA}"), "Left hip" },
            { new Guid("{BA818E77-9B4A-450E-99E9-E78E67B4C998}"), "Left knee" },
            { new Guid("{A8A6B364-9678-47A9-AB9C-E36E87E2787C}"), "Left ankle" },
            { new Guid("{9AA5A0FF-4785-40E7-A497-1E414804E793}"), "Right eye" },
            { new Guid("{BA6B90F9-1D58-4ADD-900D-ED1D5FED7BD6}"), "Left eye" },
            { new Guid("{E84C348D-2129-4A48-B364-6BAEAB4AD72D}"), "Right ear" },
            { new Guid("{353EBF14-4503-41CC-8DF9-4C99DB52E991}"), "Left ear" }
        };

        internal static Dictionary<Guid, string> SkeletonPointTypes { get; } = new Dictionary<Guid, string>()
        {
            { new Guid("{D64BB8F7-489D-4473-B0C6-2317156B2758}"), "NOSE" },
            { new Guid("{3A27D08A-5397-4870-96BD-F8089AECFA1D}"), "NECK" },
            { new Guid("{1751A2AC-D3C1-4623-9D2F-CEA0866803B5}"), "RIGHT_SHOULDER"},
            { new Guid("{DF9F778F-56F4-47C9-A11A-C2BFA2EEC752}"), "RIGHT_ELBOW" },
            { new Guid("{AA447CEC-71D3-4BC3-95F9-D60012308F9D}"), "RIGHT_WRIST" },
            { new Guid("{7CD1440F-3F64-4D80-9022-6A27C4C606FB}"), "LEFT_SHOULDER" },
            { new Guid("{D0A7E1C9-C57A-46B9-9715-9E10B408F198}"), "LEFT_ELBOW" },
            { new Guid("{417AC925-DE87-4C25-836F-83C8259BC5AF}"), "LEFT_WRIST" },
            { new Guid("{7A7C20CF-9885-45CA-A9F3-A2793A83EB01}"), "RIGHT_HIP" },
            { new Guid("{3CBD6FF8-B9EB-4E47-A142-EA00D0389F21}"), "RIGHT_KNEE" },
            { new Guid("{A3F510A1-CAC4-4FD1-AF6F-5F569F225DDD}"), "RIGHT_ANKLE" },
            { new Guid("{CE85AEDD-AFE5-4CC8-BBE1-5700B4AEDEEA}"), "LEFT_HIP" },
            { new Guid("{BA818E77-9B4A-450E-99E9-E78E67B4C998}"), "LEFT_KNEE" },
            { new Guid("{A8A6B364-9678-47A9-AB9C-E36E87E2787C}"), "LEFT_ANKLE" },
            { new Guid("{9AA5A0FF-4785-40E7-A497-1E414804E793}"), "RIGHT_EYE" },
            { new Guid("{BA6B90F9-1D58-4ADD-900D-ED1D5FED7BD6}"), "LEFT_EYE" },
            { new Guid("{E84C348D-2129-4A48-B364-6BAEAB4AD72D}"), "RIGHT_EAR" },
            { new Guid("{353EBF14-4503-41CC-8DF9-4C99DB52E991}"), "LEFT_EAR" }
        };
    }
}
