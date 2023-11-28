using REIDShared.NodeRED;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VideoOS.Platform.Search;

namespace REIDSearchAgent.SearchToolbar.Add
{
    public class ReidAddToolbarInstance : SearchToolbarPluginInstance
    {
        public ReidAddToolbarInstance()
        {
            this.Title = "Add to db";
            this.Tooltip = "Click to add person to db";

            this.Icon = Properties.Resources.plus;
        }


        public override ActivateResult Activate(IEnumerable<SearchResultData> searchResults)
        {
            var actualSearchResults = searchResults.Cast<REIDSearchAgent.SearchAgent.ReidSearchAgentResultData>().Where(sr => !sr.IsDefined).GroupBy(sr => sr.PersonKey).Select(grp => grp.ToList().FirstOrDefault()).ToArray();

            if (actualSearchResults.Length == 0)
            {
                MessageBox.Show("You can only add undefined persons!");
            }
            else
            {
                List<NodeREDPerson> personsToAdd = null;

                var addUsrControl = new AddUsrMainUsrControl(actualSearchResults); // contains smartClient:ImageViewerWpfControl & smartClient:PlaybackWpfUserControl 
                var w = new System.Windows.Window()
                {
                    Width = addUsrControl.Width + 80,
                    Height = addUsrControl.Height + 50,
                    WindowStyle = System.Windows.WindowStyle.ThreeDBorderWindow,
                    Background = System.Windows.Media.Brushes.Black,
                    Title = "Add new user to db...",
                    WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner,
                    Content = addUsrControl
                };

                if (w.ShowDialog() == true)
                {
                    personsToAdd = addUsrControl.GetInput();

                    foreach (var person in personsToAdd)
                    {
                        ReidPluginSearchAgentDefinition.NodeRED.AddPerson(person);
                    }
                }

                SearchAgent.ReidSearchAgentPlugin.DefinedPersonFilter.Refresh(personsToAdd: personsToAdd);
            }

            return ActivateResult.Default;
        }
    }
}
