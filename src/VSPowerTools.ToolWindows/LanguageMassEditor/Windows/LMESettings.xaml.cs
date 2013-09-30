using System.Windows;
using AMLib.Wpf.Controls;
using VSPowerTools.ToolWindows.LanguageMassEditor.BusinessObjects;

namespace VSPowerTools.ToolWindows.LanguageMassEditor.Windows
{
    /// <summary>
    /// Interaktionslogik für LMESettings.xaml
    /// </summary>
    public partial class LMESettings : ThemeWindow
    {
        public static readonly DependencyProperty SettingsProperty =
            DependencyProperty.Register("Settings", typeof (Settings), typeof (LMESettings), new PropertyMetadata(default(Settings)));

        public Settings Settings
        {
            get
            {
                return (Settings) GetValue(SettingsProperty);
            }
            set
            {
                SetValue(SettingsProperty, value);
            }
        }

        public LMESettings()
        {
            InitializeComponent();
        }
    }
}
