using REIDSearchAgent.SearchToolbar.Add;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VideoOS.Platform.Search;

namespace REIDSearchAgent.SearchToolbar.Remove
{
    public class ReidRemoveToolbarInstance : SearchToolbarPluginInstance
    {
        public ReidRemoveToolbarInstance()
        {
            this.Title = "Remove from db";
            this.Tooltip = "Click to remove person from db";

            this.Icon = Properties.Resources.minus;
        }

        public override ActivateResult Activate(IEnumerable<SearchResultData> searchResults)
        {
            //var actualSearchResults = searchResults.Cast<REIDSearchAgent.SearchAgent.ReidSearchAgentResultData>().Where(sr => sr.IsDefined).ToArray();
            var actualSearchResults = searchResults.Cast<REIDSearchAgent.SearchAgent.ReidSearchAgentResultData>().Where(sr => sr.IsDefined).GroupBy(sr => sr.PersonKey).Select(grp => grp.ToList().FirstOrDefault()).ToArray();

            if (actualSearchResults.Length == 0)
            {
                MessageBox.Show("You can only delete defined persons!");
            }
            else
            {
                var keys = actualSearchResults.Select(sr => sr.PersonKey);
                string definitions = string.Join("\r\n", actualSearchResults.Select(sr => sr.Person.personName));
                if(MessageBox.Show("You're about to delete the following definitions:\r\n" + definitions + "\r\n\r\nDo you want to continue?", "Are you sure?", 
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    foreach (var result in actualSearchResults)
                    {
                        ReidPluginSearchAgentDefinition.NodeRED.RemovePerson(new REIDShared.NodeRED.NodeREDPerson() { key = result.PersonKey });
                    }
                }

                SearchAgent.ReidSearchAgentPlugin.DefinedPersonFilter.Refresh(keysToRemove: keys);
            }

            return ActivateResult.Default;
        }
    }
}
