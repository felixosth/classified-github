using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace DigitalRevision.DataSource
{
    public abstract class DataSourceBase : INotifyPropertyChanged
    {
        /// <summary>
        /// PropertyChanged event (INotifyPropertyChanged)
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Name of the data collector. The data will be placed in a folder using this name as foldername. Will also be visible to the user in the UI.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Internal version of the data collector.
        /// </summary>
        public abstract double Version { get; }


        private bool _isEnabled = true;
        /// <summary>
        /// Default is true. Only changed by the user, CollectData will be called if this is true.
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _allowUserToEnable = true;
        /// <summary>
        /// If false, disable the checkbox. Default is true.
        /// </summary>
        public bool AllowUserToEnable
        {
            get => _allowUserToEnable;
            set
            {
                if (_allowUserToEnable != value)
                {
                    _allowUserToEnable = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _progress;
        /// <summary>
        /// For user feedback, progress percentage 0-100
        /// </summary>
        public int ProgressPercentage
        {
            get => _progress;
            set
            {
                if (_progress != value)
                {
                    if (_progress < 0 || _progress > 100)
                    {
                        throw new ArgumentOutOfRangeException(nameof(ProgressPercentage));
                    }

                    _progress = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _progressIsIndeterminate;
        /// <summary>
        /// If we don't know the progress value, we can set the progressbar to indeterminate.
        /// </summary>
        public bool ProgressIsIndeterminate
        {
            get => _progressIsIndeterminate;
            set
            {
                if (_progressIsIndeterminate != value)
                {
                    _progressIsIndeterminate = value;
                    NotifyPropertyChanged();
                }
            }
        }


        /// <summary>
        /// Start the data collection. All produced files must be saved in the specified folder
        /// </summary>
        /// <param name="folderDestination">The folder to save data in</param>
        public abstract Task CollectData(string folderDestination);

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString() => Name;

        protected void ShowError(string message)
        {
            MessageBox.Show(message, Name, MessageBoxButton.OK, MessageBoxImage.Error);
            ProgressIsIndeterminate = false;
            ProgressPercentage = 0;
        }
    }
}
