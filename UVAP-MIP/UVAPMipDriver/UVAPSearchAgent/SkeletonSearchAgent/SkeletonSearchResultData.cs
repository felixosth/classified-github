using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UvapShared.Objects;
using VideoOS.Platform.Search;
using VideoOS.Platform.Search.FilterValues;
using VideoOS.Platform.Search.Results;

namespace UVAPSearchAgent.SkeletonSearchAgent
{
    // This class contains the properties of a search result related to the skeleton model. Derives from SearchResultData 
    public class SkeletonSearchResultData : SearchResultData
    {
        public static Guid UvapSearchResultType = new Guid("{FDD59E41-6ECC-45CE-A525-3A9523E848CC}");

        public string PointType { get; set; }
        public float Confidence { get; set; }

        public double X;
        public double Y;

        public double ImageWidth, ImageHeight;

        public RegionSelection RegionSelection;

        public SkeletonSearchResultData(Guid id) : base(id)
        {
        }

        protected override Task<ICollection<ResultProperty>> GetPropertiesAsync(CancellationToken cancellationToken)
        {
            var props = new Collection<ResultProperty>()
            {
                new ResultProperty(nameof(PointType), PointType),
                new ResultProperty(nameof(Confidence), $"{(Confidence*100).ToString("0")}%")
            };
            return Task.FromResult<ICollection<ResultProperty>>(props);
        }
    }
}
