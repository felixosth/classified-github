using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoOS.Platform.Search;
using VideoOS.Platform;
using System.Security.Cryptography;
using VideoOS.Platform.Search.FilterValues;
using System.Web;
using System.Diagnostics;
using System.Windows;
using static REIDSearchAgent.ReidPluginSearchAgentDefinition;

namespace REIDSearchAgent.SearchAgent
{
    public class ReidSearchAgentDefinition : SearchDefinition
    {
        public ReidSearchAgentDefinition(SearchScope searchScope) : base(searchScope)
        {
        }

        protected override void Search(SearchInput searchInput, CancellationToken cancellationToken)
        {
            if(NodeRED == null)
            {
                MessageBox.Show("Please configure the Reid settings in the settings tab (top right cogwheel).", "Error");
                return;
            }


            List<int?> categories = new List<int?>();

            var definedCategoriesFilter = (SelectionFilterValue)SearchCriteria.GetFilterValues(SearchAgent.ReidSearchAgentPlugin.DefinedCategoriesFilter).FirstOrDefault();
            if (definedCategoriesFilter != null)
            {
                categories = ReidSearchAgentPlugin.DefinedCategoriesFilter.Categories.Where(dp => definedCategoriesFilter.SelectedIds.Contains(dp.Key)).Select(p => p.Value.id).ToList();
            }

            string categoriesQuery = "";
            if(categories.Count > 0)
            {
                categoriesQuery = "&";
                for (int i = 0; i < categories.Count; i++)
                {
                    categoriesQuery += "category=" + categories[i];
                    if (i + 1 != categories.Count) 
                        categoriesQuery += "&";
                }
            }

            List<string> persons = new List<string>();

            var definedPersonFilter = (SelectionFilterValue)SearchCriteria.GetFilterValues(SearchAgent.ReidSearchAgentPlugin.DefinedPersonFilter).FirstOrDefault();
            if(definedPersonFilter != null)
            {
                persons = ReidSearchAgentPlugin.DefinedPersonFilter.DefinedPersons.Where(dp => definedPersonFilter.SelectedIds.Contains(dp.Key)).Select(p => p.Value.key).ToList();
            }

            var personFilter = (StringFilterValue)SearchCriteria.GetFilterValues(SearchAgent.ReidSearchAgentPlugin.PersonFilter).FirstOrDefault();
            if(personFilter != null && !string.IsNullOrEmpty(personFilter.Text) && !string.IsNullOrWhiteSpace(personFilter.Text))
            {
                persons.Add(personFilter.Text);
            }

            string personQuery = "";
            if(persons.Count > 0)
            {
                personQuery = "&";
                for (int i = 0; i < persons.Count; i++)
                {
                    personQuery += "person=" + HttpUtility.UrlEncode(persons[i]);
                    if (i + 1 != persons.Count)
                        personQuery += "&";
                }
            }

            try
            {
                var recognitions = ReidPluginSearchAgentDefinition.NodeRED.Search(searchInput.From, searchInput.To, personQuery, categoriesQuery);

                foreach (var item in searchInput.Items)
                {
                    List<ReidSearchAgentResultData> results = new List<ReidSearchAgentResultData>();
                    foreach (var recognition in recognitions.Where(r => r.stream == item.Name))
                    {
                        recognition.time = DateTime.SpecifyKind(recognition.time, DateTimeKind.Utc);
                        string title = recognition.personName;

                        DateTime beginTime = DateTime.SpecifyKind(recognition.time.AddSeconds(-10), DateTimeKind.Utc);
                        DateTime endTime = DateTime.SpecifyKind(recognition.time.AddSeconds(10), DateTimeKind.Utc);


                        // Filter out close entries
                        if (results.FirstOrDefault(r => (recognition.time - r.TriggerTime).TotalSeconds <= 8 && r.Item == item && r.PersonKey == recognition.person) != null)
                            continue;

                        var person = NodeRED.Persons.FirstOrDefault(p => p.id == recognition.personId);

                        var resultData = new ReidSearchAgentResultData(MakeId(title, item, recognition.time, endTime, recognition.person))
                        {
                            Title = title,
                            BeginTime = beginTime,
                            TriggerTime = recognition.time,
                            EndTime = endTime,
                            Item = item,
                            Person = person,
                            PersonKey = recognition.person,
                            //PersonName = recognition.personName,
                            //Category = recognition.categoryName,
                            IsDefined = recognition.personId != null
                        };
                        results.Add(resultData);
                    }

                    FireSearchResultsReadyEvent(searchInput.SessionId, results);
                }
            }
            catch
            {
                if (Debugger.IsAttached)
                    throw;
            }
        }

        Guid MakeId(string title, Item item, DateTime triggerTime, DateTime endTime, string customParam)
        {
            string stringId = $"{title}:{item.FQID.ObjectId}:{triggerTime:s}:{endTime:s}:{customParam}";
            // generate id using MD5 hash function
            var provider = new MD5CryptoServiceProvider();
            byte[] input = Encoding.Default.GetBytes(stringId);
            byte[] hashBytes = provider.ComputeHash(input);
            return new Guid(hashBytes);
        }
    }



}
