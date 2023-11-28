using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace XProtectWebStream.Shared
{
    [Serializable]
    public class SharedAccessGroup : INotifyPropertyChanged
    {
        public int Id { get; set; }
        private string name;
        public string Name { get => name; 
            set
            {
                if(name != value)
                {
                    name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool isChecked = false;
        public bool IsChecked { get => isChecked; 
            set
            {
                if(isChecked != value)
                {
                    isChecked = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool CanCheck { get; set; } = true;

        public SharedAccessGroup(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString() => Name;
    }
}
