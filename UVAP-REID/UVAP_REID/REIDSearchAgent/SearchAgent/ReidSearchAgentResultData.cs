using REIDShared.NodeRED;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoOS.Platform.Search;
using VideoOS.Platform.Search.Results;

namespace REIDSearchAgent.SearchAgent
{
    public class ReidSearchAgentResultData : SearchResultData
    {
        public NodeREDPerson Person { get; set; }
        public string PersonKey { get; set; }

        //public string PersonName { get; set; }

        //public string Category { get; set; }

        public bool IsDefined { get; set; }
        //public string Firstname { get; set; }
        //public string Lastname { get; set; }

        public ReidSearchAgentResultData(Guid id) : base(id)
        {
        }

        protected override Task<ICollection<ResultProperty>> GetPropertiesAsync(CancellationToken cancellationToken)
        {
            var props = new Collection<ResultProperty>()
            {
                new ResultProperty("Key", PersonKey),
                new ResultProperty("Name", Person != null ? Person.personName : "N/A"),
                new ResultProperty("Category", Person != null ? Person.categoryName : "N/A")
            };
            return Task.FromResult<ICollection<ResultProperty>>(props);
        }
    }
}
