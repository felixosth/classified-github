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

namespace UVAPSearchAgent.HeadSearchAgent
{
    // This class contains the properties of a search result related to the headdetection model. Derives from SearchResultData 
    public class HeadSearchResultData : SearchResultData
    {
        public RegionSelection RegionSelection;
        public List<SlimHeadDetection> Heads { get; set; }
        public double ImageWidth, ImageHeight;

        public HeadSearchResultData(Guid id) : base(id)
        {
        }

        protected override Task<ICollection<ResultProperty>> GetPropertiesAsync(CancellationToken cancellationToken)
        {
            var props = new Collection<ResultProperty>()
            {
                new ResultProperty("Headcount", Heads.Count.ToString()),
                new ResultProperty("Average confidence", (Heads.Select(h => h.Confidence).Average() * 100).ToString("0") + "%")
            };

            return Task.FromResult<ICollection<ResultProperty>>(props);
        }
    }
}
