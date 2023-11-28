using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VideoOS.Platform;
using VideoOS.Platform.Data;
using VideoOS.Platform.Search;
using VideoOS.Platform.Search.FilterValues;

namespace UVAPSearchAgent.Shared.UserControl
{
    // Custom user control for displaying the search results to the user. Derives from SearchResultUserControl
    public partial class UvapSearchResultUsrControl : SearchResultUserControl, INotifyPropertyChanged
    {
        private ImageSource _img;

        SearchResultData _searchResult;

        SkeletonSearchAgent.SkeletonSearchResultData SkeletonResult => _searchResult as SkeletonSearchAgent.SkeletonSearchResultData;
        HeadSearchAgent.HeadSearchResultData HeadResult => _searchResult as HeadSearchAgent.HeadSearchResultData;

        private ImageSource Image
        {
            get => _img; set
            {
                _img = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        public UvapSearchResultUsrControl()
        {
            DataContext = this;
            InitializeComponent();
            IsLoading = true;
        }

        public override void Init(SearchResultData searchResultData) // Entry point
        {
            _searchResult = searchResultData; // this will be either SkeletonSearchResultData or HeadSearchResultData

            new Thread(() => // Create a new thread and fetch the closest image to the returned search result
            {
                var imageSrc = GetImage(searchResultData.TriggerTime, searchResultData.Item); // Get the image
                ClientControl.Instance.CallOnUiThread(() =>  // Call the following code on the UI thread
                {
                    image.Source = imageSrc;

                    if(_searchResult is SkeletonSearchAgent.SkeletonSearchResultData)
                    {
                        BuildRoi(SkeletonResult.RegionSelection, new System.Drawing.Size((int)image.Source.Width, (int)image.Source.Height));
                        BuildSkeleton();
                    }
                    else if(_searchResult is HeadSearchAgent.HeadSearchResultData)
                    {
                        BuildRoi(HeadResult.RegionSelection, new System.Drawing.Size((int)image.Source.Width, (int)image.Source.Height));
                        BuildHeads();
                    }

                    IsLoading = false; // We're done.
                });
            }).Start();
        }

        void BuildRoi(RegionSelection regionSelection, System.Drawing.Size imageResolution)
        {
            canvas.Width = imageResolution.Width;
            canvas.Height = imageResolution.Height;

            int blockWidth = imageResolution.Width / regionSelection.Size.Width;
            int blockHeight = imageResolution.Height / regionSelection.Size.Height;

            for (int i = 0; i < regionSelection.Mask.Length; i++)
            {
                if (regionSelection.Mask[i] == '0')
                {
                    int x = (i % regionSelection.Size.Width);
                    int y = (i / regionSelection.Size.Height);

                    int fromX = blockWidth * x;
                    int fromY = blockHeight * y;

                    Rectangle rect = new Rectangle()
                    {
                        Fill = Brushes.Red,
                        Opacity = 0.33,
                        Width = blockWidth,
                        Height = blockHeight
                    };
                    Canvas.SetZIndex(rect, canvas.Children.Count + 1);
                    canvas.Children.Add(rect);
                    Canvas.SetLeft(rect, fromX);
                    Canvas.SetTop(rect, fromY);
                }
            }
        }

        void BuildSkeleton()
        {
            double size = 80;

            Rectangle rect = new Rectangle()
            {
                Stroke = Brushes.Yellow,
                StrokeThickness = 5,
                Width = size,
                Height = size
            };

            Canvas.SetZIndex(rect, 9999);
            canvas.Children.Add(rect);
            Canvas.SetLeft(rect, SkeletonResult.X - size/2);
            Canvas.SetTop(rect, SkeletonResult.Y - size/2);
        }

        void BuildHeads()
        {
            foreach(var head in HeadResult.Heads)
            {
                Rectangle rect = new Rectangle()
                {
                    Stroke = Brushes.Yellow,
                    StrokeThickness = 5,
                    Width = head.BoundingBox.Width,
                    Height = head.BoundingBox.Height
                };

                Canvas.SetZIndex(rect, canvas.Children.Count + 100);
                canvas.Children.Add(rect);
                Canvas.SetLeft(rect, head.BoundingBox.X);
                Canvas.SetTop(rect, head.BoundingBox.Y);
            }
        }

        BitmapSource GetImage(DateTime dt, Item camera) // get the video frame data and convert it to a BitmapSource so we can display it on the Image control
        {
            JPEGVideoSource source = new JPEGVideoSource(camera);
            source.Init();
            var frame = source.GetNearest(dt) as JPEGData;

            //var dif = frame.DateTime - dt;
            //if(Math.Abs(dif.TotalSeconds) > 1)
            //{
            //    return null;
            //}

            source.Close();
            return frame.ConvertToBitmapSource();
        }
    }
}
