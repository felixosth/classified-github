using REIDSearchAgent.SearchAgent.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using VideoOS.Platform.Search;

namespace REIDSearchAgent.SearchAgent
{
    public class ReidSearchAgentPlugin : SearchAgentPlugin
    {
        internal static readonly PersonReidSearchAgentFilter PersonFilter = new PersonReidSearchAgentFilter();
        internal static readonly DefinedPersonReidSearchAgentFilter DefinedPersonFilter = new DefinedPersonReidSearchAgentFilter();
        internal static readonly DefinedCategoryReidSearchAgentFilter DefinedCategoriesFilter = new DefinedCategoryReidSearchAgentFilter();
        //internal static readonly OnlyUnrecognizedReidSearchAgentFilter OnlyUnrecognizedFilter = new OnlyUnrecognizedReidSearchAgentFilter();

        public static Guid _ID = new Guid("{0C1B2F23-B966-402D-ABF9-13B4480AD9CC}");

        public override Guid Id { get => _ID; protected set => _ID = value; }

        public override string Name { get; protected set; } = "ReidSearchAgent";
        public override SearchFilterCategory SearchFilterCategory { get; protected set; }

        public override SearchDefinition CreateSearchDefinition(SearchScope searchScope)
        {
            return new ReidSearchAgentDefinition(searchScope);
        }

        public override void Init()
        {
            System.Xml.XmlNode result = VideoOS.Platform.Configuration.Instance.GetOptionsConfiguration(REIDShared.Settings.ClientSettingsID, false);

            if (result != null)
            {
                foreach (XmlNode node in result.ChildNodes)
                {
                    switch (node.Attributes["name"].Value)
                    {
                        case Settings.ReidSettingsPlugin.NodeREDUrl:
                            ReidPluginSearchAgentDefinition.NodeRED = new REIDShared.NodeREDConnection(new Uri(node.Attributes["value"].Value));
                            ReidPluginSearchAgentDefinition.NodeRED.StartPoll();
                            break;
                    }
                }
            }

            SearchFilterCategory = new VideoOS.Platform.Search.FilterCategories.OtherSearchFilterCategory("Reid", Properties.Resources.eh2, new SearchFilter[]
            {
                PersonFilter,
                DefinedPersonFilter,
                DefinedCategoriesFilter
            });
        }

        public override void Close()
        {
            DefinedPersonFilter.DefinedPersons = null;
        }
    }
}
