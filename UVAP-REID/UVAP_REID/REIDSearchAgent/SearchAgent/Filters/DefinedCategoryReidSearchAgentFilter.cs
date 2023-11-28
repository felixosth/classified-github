using REIDShared.NodeRED;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform.Search;
using VideoOS.Platform.Search.FilterConfigurations;
using VideoOS.Platform.Search.FilterValues;

namespace REIDSearchAgent.SearchAgent.Filters
{
    public class DefinedCategoryReidSearchAgentFilter : SearchFilter
    {
        public Dictionary<Guid, NodeREDCategory> Categories { get; set; }

        SelectionFilterValue value;
        ListSelectionFilterConfiguration cfg;

        bool nodeRedWasNull = true;

        public DefinedCategoryReidSearchAgentFilter()
        {
            this.Name = "Categories";

            value = new SelectionFilterValue();
            cfg = new ListSelectionFilterConfiguration();
        }


        public override FilterValueBase CreateValue()
        {
            ResetValue(value);
            return value;
        }

        public void Refresh()
        {
            if (Categories == null || nodeRedWasNull)
            {
                Categories = new Dictionary<Guid, NodeREDCategory>();

                Categories.Add(Guid.NewGuid(), new NodeREDCategory() { id = null, name = "Uncategorized" }) ;

                if (ReidPluginSearchAgentDefinition.NodeRED != null)
                {
                    nodeRedWasNull = false;

                    if (ReidPluginSearchAgentDefinition.NodeRED.Categories == null)
                        ReidPluginSearchAgentDefinition.NodeRED.Update();

                    foreach (var category in ReidPluginSearchAgentDefinition.NodeRED.Categories.OrderBy(c => c.name))
                    {
                        Categories.Add(Guid.NewGuid(), category);
                    }
                }
            }

            cfg.Items.Clear();
            foreach (var c in Categories)
            {
                cfg.Items.Add(c.Key, c.Value.name);
            }
        }

        public override FilterConfigurationBase GetFilterConfiguration()
        {
            if (Categories == null)
                Refresh();

            return cfg;
        }

        public override void ResetValue(FilterValueBase value)
        {
            var selectionValue = value as SelectionFilterValue;
            if (selectionValue == null)
                throw new ArgumentException();
            selectionValue.SetSelectedIds(Enumerable.Empty<Guid>());
        }
    }
}
