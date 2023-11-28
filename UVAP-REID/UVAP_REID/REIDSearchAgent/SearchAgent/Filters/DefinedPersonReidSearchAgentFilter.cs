using Newtonsoft.Json;
using REIDShared.NodeRED;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform.Search;
using VideoOS.Platform.Search.FilterConfigurations;
using VideoOS.Platform.Search.FilterValues;

namespace REIDSearchAgent.SearchAgent
{
    public class DefinedPersonReidSearchAgentFilter : SearchFilter
    {
        public Dictionary<Guid, NodeREDPerson> DefinedPersons { get; set; }

        SelectionFilterValue value;
        ListSelectionFilterConfiguration cfg;

        bool nodeRedWasNull = true;

        public DefinedPersonReidSearchAgentFilter()
        {
            this.Name = "Defined persons";

            value = new SelectionFilterValue();
            cfg = new ListSelectionFilterConfiguration();
        }

        public override FilterValueBase CreateValue()
        {
            ResetValue(value);
            return value;
        }

        public void Refresh(IEnumerable<NodeREDPerson> personsToAdd = null, IEnumerable<string> keysToRemove = null)
        {
            if (DefinedPersons == null || nodeRedWasNull)
            {
                DefinedPersons = new Dictionary<Guid, NodeREDPerson>();

                //DefinedPersons.Add(Guid.NewGuid(), new NodeREDPerson() { key = "Unrecognized", personName = "Unrecognized" });

                if (ReidPluginSearchAgentDefinition.NodeRED != null)
                {
                    nodeRedWasNull = false;

                    if (ReidPluginSearchAgentDefinition.NodeRED.Persons == null)
                        ReidPluginSearchAgentDefinition.NodeRED.Update();

                    foreach (var person in ReidPluginSearchAgentDefinition.NodeRED.Persons.OrderBy(p => p.personName))
                    {
                        DefinedPersons.Add(Guid.NewGuid(), person);
                    }
                }
            }
            else if (personsToAdd != null)
            {
                if (keysToRemove != null && keysToRemove.Count() > 0)
                {
                    List<Guid> guidsToRemove = new List<Guid>();
                    foreach (var kvp in DefinedPersons)
                    {
                        if (keysToRemove.Contains(kvp.Value.key))
                        {
                            guidsToRemove.Add(kvp.Key);
                        }
                    }
                    foreach (var k in guidsToRemove)
                    {
                        DefinedPersons.Remove(k);
                    }
                }

                if(personsToAdd != null)
                {
                    foreach(var person in personsToAdd)
                    {
                        // Todo: Add person in internal list
                        //ReidPluginSearchAgentDefinition.NodeRED.Persons.Add?
                        DefinedPersons.Add(Guid.NewGuid(), person);
                    }
                }
            }

            cfg.Items.Clear();
            foreach (var p in DefinedPersons)
            {
                cfg.Items.Add(p.Key, p.Value.ToString());
            }
        }

        public override FilterConfigurationBase GetFilterConfiguration()
        {
            if (DefinedPersons == null)
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
