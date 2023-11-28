using REIDShared.NodeRED;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VideoOS.Platform.Search;

namespace REIDSearchAgent.SearchToolbar.Add
{
    public class ReidEditToolbarInstance : SearchToolbarPluginInstance
    {
        public ReidEditToolbarInstance()
        {
            Title = "Edit db";
            Tooltip = "Click to edit defined person in db";
            Icon = Properties.Resources.edit;
        }

        public override ActivateResult Activate(IEnumerable<SearchResultData> searchResults)
        {
            var actualSearchResults = searchResults.Cast<REIDSearchAgent.SearchAgent.ReidSearchAgentResultData>().Where(sr => sr.IsDefined).GroupBy(sr => sr.PersonKey).Select(grp => grp.ToList().FirstOrDefault()).ToArray();

            if (actualSearchResults.Length == 0)
            {
                MessageBox.Show("You can only edit defined persons!");
            }
            else
            {
                var addUsrControl = new AddUsrMainUsrControl(actualSearchResults, true);
                var w = new System.Windows.Window()
                {
                    Width = addUsrControl.Width + 80,
                    Height = addUsrControl.Height + 50,
                    WindowStyle = System.Windows.WindowStyle.ThreeDBorderWindow,
                    Background = System.Windows.Media.Brushes.Black,
                    Title = "Edit persons in db...",
                    WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner
                };

                w.Content = addUsrControl;

                List<NodeREDPerson> personsToEdit = null;

                if (w.ShowDialog() == true)
                {
                    personsToEdit = addUsrControl.GetInput();

                    ReidPluginSearchAgentDefinition.NodeRED.UpdatePersons(personsToEdit);

                    SearchAgent.ReidSearchAgentPlugin.DefinedPersonFilter.Refresh();
                }
            }
            return ActivateResult.Default;
        }
    }
}
