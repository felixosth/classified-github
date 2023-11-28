using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UVAPSearchAgent.Shared;
using UVAPSearchAgent.Shared.Filters;
using UVAPSearchAgent.Shared.Filters.RadioList;
using UVAPSearchAgent.SkeletonSearchAgent.Filters;
using UvapShared.Objects;
using VideoOS.Platform;
using VideoOS.Platform.Data;
using VideoOS.Platform.Search;
using VideoOS.Platform.Search.FilterConfigurations;
using VideoOS.Platform.Search.FilterValues;

namespace UVAPSearchAgent.SkeletonSearchAgent
{
    // This is the main class of a search agent. This class overrides the Search method from the SearchDefinition class. The actual search is performed in this method.
    public class SkeletonSearchAgentDefinition : SearchDefinition
    {
        public SkeletonSearchAgentDefinition(SearchScope searchScope) : base(searchScope) // We get the search scope by Milestone in the constructor
        {
            SkeletonSearchAgentPlugin.MetadataDeviceFilter.Cameras = searchScope.Items;
        }

        // Do the actual search and return the results to Milestone
        protected override void Search(Guid sessionId, DateTime from, DateTime to, IEnumerable<Item> items, CancellationToken cancellationToken)
        {
            var item = items.FirstOrDefault();

            var sequenceLengthFilter = (DoubleFilterValue)SearchCriteria.GetFilterValues(SkeletonSearchAgentPlugin.SequenceLengthFilter).FirstOrDefault();
            int sequenceLength = 10;

            if (sequenceLengthFilter != null)
            {
                sequenceLength = (int)sequenceLengthFilter.Value;
            }

            var selectedSkeletonPointTypes = (SelectionFilterValue)SearchCriteria.GetFilterValues(SkeletonSearchAgentPlugin.SkeletonTypeFilter).FirstOrDefault();
            List<string> skeletonPointTypes = new List<string>();

            if (selectedSkeletonPointTypes != null)
            {
                foreach (var ptype in selectedSkeletonPointTypes.SelectedIds)
                {
                    skeletonPointTypes.Add(SkeletonPointType.SkeletonPointTypes[ptype]);
                }
            }

            Item metadataDevice = null;
            var metadataDeviceFilter = (RadioListFilterValue)SearchCriteria.GetFilterValues(SkeletonSearchAgentPlugin.MetadataDeviceFilter).FirstOrDefault();
            if (metadataDeviceFilter != null)
            {
                metadataDevice = metadataDeviceFilter.SelectedObject as Item;
            }

            var regions = (RegionSelectionFilterValue)SearchCriteria.GetFilterValues(SkeletonSearchAgentPlugin.SkeletonRegionFilter).FirstOrDefault();
            RegionSelection regionSelection = null;
            float threshold = 1;
            if(regions != null && regions.Selections.Count > 0)
            {
                regionSelection = regions.Selections.FirstOrDefault();
                threshold = regionSelection.Threshold / 100f;
                if (!regionSelection.Mask.Contains("0"))
                    regionSelection = null;
            }


            if (skeletonPointTypes.Count == 0 || metadataDevice == null || regionSelection == null)
                return;

            MetadataPlaybackSource playback = new MetadataPlaybackSource(metadataDevice);
            playback.Init();

            playback.GoTo(from, "Custom");

            var resolution = SearchAgentsHelper.GetResolution(item, from);

            List<SearchResultData> searchResults = new List<SearchResultData>();
            int gotNulls = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                var frame = playback.GetNext();

                if (frame != null)
                {
                    if (frame.DateTime > to)
                        break;

                    var content = frame.Content.GetMetadataString();

                    if (content == "[]") // Don't even bother deserializing empty arrays
                        continue;

                    SlimSkeleton[] bodies;
                    try
                    {
                        bodies = SkeletonBodies(content);
                    }
                    catch
                    {
                        break;
                    }

                    if (bodies != null && bodies.Length > 0 && bodies.First().Points != null)
                    {
                        foreach (var body in bodies)
                        {
                            if (body.Points.Length == 0)
                                continue;

                            foreach (var point in body.Points)
                            {
                                foreach (var desiredPoint in skeletonPointTypes)
                                {
                                    if (point.Type == desiredPoint && point.Confidence >= threshold)
                                    {
                                        if (SearchAgentsHelper.IsWithinMask(regionSelection, resolution, point.X, point.Y))
                                        {
                                            var pointGuid = SkeletonPointType.SkeletonPointTypes.Single(p => p.Value == desiredPoint).Key;
                                            string title = SkeletonPointType.PrettySkeletonPointTypes[pointGuid];

                                            var searchResult = new SkeletonSearchResultData(SearchAgentsHelper.MakeId(title, item, frame.DateTime, to, point.Type))
                                            {
                                                Title = title,
                                                Item = item,
                                                RelatedItems = new[] { metadataDevice },
                                                BeginTime = frame.DateTime.AddSeconds(-4),
                                                TriggerTime = frame.DateTime,
                                                EndTime = frame.DateTime.AddSeconds(5),
                                                PointType = title,
                                                Confidence = point.Confidence,
                                                X = point.X,
                                                Y = point.Y,
                                                ImageWidth = resolution.Width,
                                                ImageHeight = resolution.Height,
                                                RegionSelection = regionSelection,
                                                SearchResultUserControlType = Shared.UvapSearchUserControlsPlugin.UvapSearchResultUserControlID
                                            };

                                            searchResults.Add(searchResult);

                                            FireSearchResultsReadyEvent(sessionId, new List<SearchResultData>() { searchResult });
                                            playback.GoTo(frame.DateTime.AddSeconds(sequenceLength), "Custom"); // Skip X amount of seconds depending on the sequence length setting

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (gotNulls > 3)
                        break;
                    else
                        gotNulls++;
                }
            }
            playback.Close();
        }

        SlimSkeleton[] SkeletonBodies(string content)
        {
            return JsonConvert.DeserializeObject<SlimSkeleton[]>(content);
        }
    }
}
