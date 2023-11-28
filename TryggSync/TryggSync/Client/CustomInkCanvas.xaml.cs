using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VideoOS.Platform;

namespace TryggSync.Client
{
    /// <summary>
    /// Interaction logic for CustomCanvas.xaml
    /// </summary>
    public partial class CustomInkCanvas : UserControl
    {
        public event EventHandler<InkCanvasStrokeCollectedEventArgs> OnStrokeCollected;

        public CustomInkCanvas()
        {
            InitializeComponent();

            inkCanvas.StrokeCollected += InkCanvas_StrokeCollected;
        }

        public MemoryStream GetStrokesStream()
        {
            var ms = new MemoryStream();
                inkCanvas.Strokes.Save(ms);
                ms.Position = 0;
                return ms;
        }

        public void ClearStrokes()
        {
            inkCanvas.Strokes.Clear();
        }

        public void UpdateStrokes(MemoryStream strokes)
        {
            var strokeCol = new StrokeCollection(strokes);
            ClientControl.Instance.CallOnUiThread(() =>
                {
                    inkCanvas.Strokes = strokeCol;
                });
        }

        public void AddStroke(Stroke stroke)
        {
            inkCanvas.Strokes.Add(stroke);
        }

        private void InkCanvas_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            OnStrokeCollected?.Invoke(this, e);
        }
    }
}
