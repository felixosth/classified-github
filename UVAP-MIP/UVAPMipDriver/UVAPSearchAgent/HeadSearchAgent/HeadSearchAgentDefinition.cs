using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UVAPSearchAgent.Shared;
using UVAPSearchAgent.Shared.Filters.RadioList;
using UvapShared.Objects;
using VideoOS.Platform;
using VideoOS.Platform.Data;
using VideoOS.Platform.Search;
using VideoOS.Platform.Search.FilterValues;

namespace UVAPSearchAgent.HeadSearchAgent
{
    // This is the main class of a search agent. This class overrides the Search method from the SearchDefinition class. The actual search is performed in this method.
    public class HeadSearchAgentDefinition : SearchDefinition
    {
        public HeadSearchAgentDefinition(SearchScope searchScope) : base(searchScope) // We get the search scope by Milestone in the constructor
        {
            HeadSearchAgentPlugin.MetadataDeviceFilter.Cameras = searchScope.Items;
        }

        
        // Do the actual search and return the results to Milestone
        protected override void Search(Guid sessionId, DateTime from, DateTime to, IEnumerable<Item> items, CancellationToken cancellationToken)
        {
            var cameraItem = items.FirstOrDefault();

            var sequenceLengthFilter = (DoubleFilterValue)SearchCriteria.GetFilterValues(HeadSearchAgentPlugin.SequenceLengthFilter).FirstOrDefault();
            int sequenceLength = 10;

            if(sequenceLengthFilter != null)
            {
                sequenceLength = (int)sequenceLengthFilter.Value;
            }

            var headsAmountFilter = (DoubleRangeFilterValue)SearchCriteria.GetFilterValues(HeadSearchAgentPlugin.HeadsAmountFilter).FirstOrDefault();
            int headsAmountHigh = 150;
            int headsAmountLow = 1;

            if (headsAmountFilter != null)
            {
                headsAmountLow = (int)headsAmountFilter.Low;
                headsAmountHigh = (int)headsAmountFilter.High;
            }

            Item metadataDevice = null;
            var metadataDeviceFilter = (RadioListFilterValue)SearchCriteria.GetFilterValues(HeadSearchAgentPlugin.MetadataDeviceFilter).FirstOrDefault();
            if(metadataDeviceFilter != null)
            {
                metadataDevice = metadataDeviceFilter.SelectedObject as Item;
            }

            var regions = (RegionSelectionFilterValue)SearchCriteria.GetFilterValues(HeadSearchAgentPlugin.RegionFilter).FirstOrDefault();
            RegionSelection regionSelection = null;
            float confidenceThreshold = 1;
            if (regions != null && regions.Selections.Count > 0)
            {
                regionSelection = regions.Selections.FirstOrDefault();
                confidenceThreshold = regionSelection.Threshold / 100f;
                if (!regionSelection.Mask.Contains("0"))
                    regionSelection = null;
            }

            if (regionSelection == null || metadataDevice == null)
                return;

            MetadataPlaybackSource playbackSource = new MetadataPlaybackSource(metadataDevice);
            playbackSource.Init();

            playbackSource.GoTo(from, "Custom");
            var cameraResolution = SearchAgentsHelper.GetResolution(cameraItem, from);

            int gotNulls = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                var frame = playbackSource.GetNext();

                if(frame != null)
                {
                    if (frame.DateTime > to)
                        break;

                    string content = frame.Content.GetMetadataString();

                    if (content == "[]") // Don't even bother deserializing empty arrays
                        continue;

                    SlimHeadDetection[] heads;

                    try
                    {
                        heads = JsonConvert.DeserializeObject<SlimHeadDetection[]>(content);
                    }
                    catch
                    {
                        continue;
                    }

                    if(heads != null && heads.Length > 0)
                    {
                        List<SlimHeadDetection> validHeads = new List<SlimHeadDetection>();

                        foreach(var head in heads)
                        {
                            if(head.Confidence >= confidenceThreshold && SearchAgentsHelper.IsWithinMask(regionSelection, cameraResolution, head.BoundingBox.CenterX, head.BoundingBox.CenterY))
                            {
                                validHeads.Add(head);
                            }
                        }

                        if(headsAmountLow <= validHeads.Count && headsAmountHigh >= validHeads.Count)
                        {
                            var result = BuildSearchResult(validHeads, cameraItem, metadataDevice, frame.DateTime, to, cameraResolution, regionSelection);
                            FireSearchResultsReadyEvent(sessionId, new List<SearchResultData>() { result });
                            playbackSource.GoTo(frame.DateTime.AddSeconds(sequenceLength), "Custom"); // Skip X amount of seconds depending on the sequence length setting
                            continue;
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

            playbackSource.Close();
        }

        // Build the search result
        HeadSearchResultData BuildSearchResult(List<SlimHeadDetection> heads, Item camera, Item metadataItem, DateTime triggerTime, DateTime endTime, Size imageResolution, RegionSelection regionSelection)
        {
            string title = $"{heads.Count} heads";
            var searchResult = new HeadSearchResultData(SearchAgentsHelper.MakeId(title, camera, triggerTime, endTime, heads.Count.ToString()))
            {
                Title = title,
                Item = camera,
                RelatedItems = new[] { metadataItem },
                BeginTime = triggerTime.AddSeconds(-2),
                TriggerTime = triggerTime,
                EndTime = triggerTime.AddSeconds(5),
                Heads = heads,
                ImageWidth = imageResolution.Width,
                ImageHeight = imageResolution.Height,
                RegionSelection = regionSelection,
                SearchResultUserControlType = Shared.UvapSearchUserControlsPlugin.UvapSearchResultUserControlID
            };

            return searchResult;
        }
    }
}
