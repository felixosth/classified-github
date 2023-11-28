using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WinservLite
{
    public static class ThemeManager
    {
        private static List<Window> windows = new List<Window>();
        private static Theme _currentTheme = Theme.Default;


        //public ThemeManager(Window window)
        //{
        //    //windows.Add(window);
        //    _currentTheme = Theme.Default;
        //}

        public static void AddWindow(Window window)
        {
            windows.Add(window);
            SetTheme(window, _currentTheme);
        }

        public static void RemoveWindow(Window window)
        {
            windows.Remove(window);
        }

        public static void ChangeTheme(Theme theme)
        {
            if(_currentTheme != theme)
            {
                _currentTheme = theme;

                foreach (Window window in windows)
                {
                    SetTheme(window, theme);
                }
            }
        }

        private static void SetTheme(Window window, Theme theme)
        {
            window.Resources.MergedDictionaries.Clear();
            switch (theme)
            {
                default:
                case Theme.Default:
                    AddResourceDictionary(window, "DefaultTheme");
                    break;

                case Theme.Dark:
                    AddResourceDictionary(window, "DarkTheme");
                    break;
            }
        }

        public static Theme CurrentTheme
        {
            get { return _currentTheme; }
        }

        private static void AddResourceDictionary(Window window, string source)
        {
            ResourceDictionary resourceDictionary = Application.LoadComponent(new Uri("Themes\\" + source + ".xaml", UriKind.Relative)) as ResourceDictionary;
            window.Resources.MergedDictionaries.Add(resourceDictionary);
        }

    }
    public enum Theme
    {
        Default = 0,
        Dark = 1
    }
}
