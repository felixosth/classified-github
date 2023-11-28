using System;
using VideoOS.Platform.Client;

namespace TryggSupport.Client
{
    /// <summary>
    /// Interaction logic for TryggSupportWorkSpaceViewItemWpfUserControl.xaml
    /// </summary>
    public partial class TryggSupportWorkSpaceViewItemWpfUserControl : ViewItemWpfUserControl
    {
        public TryggSupportWorkSpaceViewItemWpfUserControl()
        {
            InitializeComponent();
        }

        public override void Init()
        {
        }

        public override void Close()
        {
        }

        /// <summary>
        /// Do not show the sliding toolbar!
        /// </summary>
        public override bool ShowToolbar
        {
            get { return false; }
        }

        private void ViewItemWpfUserControl_ClickEvent(object sender, EventArgs e)
        {
            FireClickEvent();
        }

        private void ViewItemWpfUserControl_DoubleClickEvent(object sender, EventArgs e)
        {
            FireDoubleClickEvent();
        }
    }
}
