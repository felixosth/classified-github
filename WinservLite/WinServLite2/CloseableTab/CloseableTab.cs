using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WinServLite2.CloseableTab
{
    public class CloseableTab : TabItem
    {
        private string SetTitle { get; set; }
        CloseableHeader closableTabHeader;

        public CloseableTab()
        {
            // Create an instance of the usercontrol
            closableTabHeader = new CloseableHeader();
            // Assign the usercontrol to the tab header

            closableTabHeader.button_close.MouseEnter +=
   new MouseEventHandler(button_close_MouseEnter);
            closableTabHeader.button_close.MouseLeave +=
               new MouseEventHandler(button_close_MouseLeave);
            closableTabHeader.button_close.Click +=
               new RoutedEventHandler(button_close_Click);
            closableTabHeader.label_TabTitle.SizeChanged +=
               new SizeChangedEventHandler(label_TabTitle_SizeChanged);

            closableTabHeader.MouseDown += ClosableTabHeader_MouseDown;

            var contextMenu = new ContextMenu();
            var menuItem = new MenuItem()
            {
                Header = "Flytta till eget fönster",
            };
            menuItem.Click += MakeTabWindow_Click;

            contextMenu.Items.Add(menuItem);
            closableTabHeader.ContextMenu = contextMenu;
            this.Header = closableTabHeader;
        }

        public void SetContent(object content)
        {
            if (content is DynamicUserControl)
            {
                (content as DynamicUserControl).CloseRequested += (s, e) =>
                    {
                        Close();
                    };
            }
            this.Content = content;
        }

        private void MakeTabWindow_Click(object sender, RoutedEventArgs e)
        {
            ContainerWindow containerWindow = new ContainerWindow()
            {
                Title = SetTitle,
                Content = this.Content,
                Tag = this.Tag
            };
            
            containerWindow.Show();
            Close(true);
            containerWindow.Focus();
        }

        private void ClosableTabHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.MiddleButton == MouseButtonState.Pressed)
            {
                Close();
            }
        }

        public string Title
        {
            set
            {
                SetTitle = value;
                ((CloseableHeader)this.Header).label_TabTitle.Content = value;
            }
            get { return SetTitle; }
        }

        public void IndicateChanges(bool show)
        {
            //((CloseableHeader)this.Header).label_TabTitle.Content = (show ? "*" : "") + SetTitle;
            ((CloseableHeader)this.Header).label_TabTitle.Foreground = !show ? Brushes.Black : Brushes.Red;
        }

        public void SetToolTip(string tooltip)
        {
            ((CloseableHeader)this.Header).label_TabTitle.ToolTip = tooltip;
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            ((CloseableHeader)this.Header).button_close.Visibility = Visibility.Visible;
        }

        protected override void OnUnselected(RoutedEventArgs e)
        {
            base.OnUnselected(e);
            ((CloseableHeader)this.Header).button_close.Visibility = Visibility.Hidden;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            ((CloseableHeader)this.Header).button_close.Visibility = Visibility.Visible;
        }

        public void ResetMouseLeave()
        {
            ((CloseableHeader)this.Header).button_close.Visibility = Visibility.Visible;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (!this.IsSelected)
            {
                ((CloseableHeader)this.Header).button_close.Visibility = Visibility.Hidden;
            }
        }

        void button_close_MouseEnter(object sender, MouseEventArgs e)
        {
            ((CloseableHeader)this.Header).button_close.Foreground = Brushes.Red;
        }
        // Button MouseLeave - When mouse is no longer over button - change color back to black
        void button_close_MouseLeave(object sender, MouseEventArgs e)
        {
            ((CloseableHeader)this.Header).button_close.Foreground = Brushes.Black;
        }

        void Close(bool ignoreEvent = false)
        {
            var content = this.Content as DynamicUserControl;

            if(content != null)
            {
                bool cancel = false;
                content.OnClosing(out cancel);
                if (!cancel)
                    ((TabControl)this.Parent).Items.Remove(this);
            }
        }


        // Button Close Click - Remove the Tab - (or raise
        // an event indicating a "CloseTab" event has occurred)
        void button_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        // Label SizeChanged - When the Size of the Label changes
        // (due to setting the Title) set position of button properly
        void label_TabTitle_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ((CloseableHeader)this.Header).button_close.Margin = new Thickness(
               ((CloseableHeader)this.Header).label_TabTitle.ActualWidth + 5, 3, 4, 0);
        }
    }
}
