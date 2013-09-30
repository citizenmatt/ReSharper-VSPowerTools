using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using AMLib.Wpf.Helpers;
using VSPowerTools.ToolWindows.LanguageMassEditor.ViewModels;

namespace VSPowerTools.ToolWindows.LanguageMassEditor.UserControls
{
	/// <summary>
	/// Interaktionslogik für LanguageControl.xaml
	/// </summary>
	public partial class LanguageControl : DockPanel, INotifyPropertyChanged
	{
		#region DependencyProperty "DisplayNode"

		public static readonly DependencyProperty DisplayNodeProperty =
			DependencyProperty.Register("DisplayNode", typeof(ResxNodeViewModel), typeof(LanguageControl), new FrameworkPropertyMetadata(default(ResxNodeViewModel), DisplayNodeChanged));

		private static void DisplayNodeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
		{
			LanguageControl ctrl = dependencyObject as LanguageControl;
			ResxNodeViewModel rNode = args.NewValue as ResxNodeViewModel;
			if (rNode == null) 
                return;
			if (ctrl == null) 
                return;

            if(ctrl.PropertyChanged != null)
            {
                ctrl.PropertyChanged(ctrl, new PropertyChangedEventArgs("DisplayNodeView"));
            }
		}

		public ResxNodeViewModel DisplayNodeView
	    {
	        get
	        {
                if (DisplayNode == null)
                    return null;

	            var view = CollectionViewSource.GetDefaultView(DisplayNode.Attributes);
                view.SortDescriptions.Add(new SortDescription("Language.Length", ListSortDirection.Descending));

				return DisplayNode;
	        }
	    }

		public ResxNodeViewModel DisplayNode
		{
			get { return (ResxNodeViewModel)GetValue(DisplayNodeProperty); }
			set { SetValue(DisplayNodeProperty, value); }
		}

		#endregion DependencyProperty "DisplayNode"

		public LanguageControl()
		{
			InitializeComponent();
		}

	    public void FocusDefault()
	    {
	        var firstTextBox = icRoot.GetChildOfType<TextBox>();
            if(firstTextBox != null)
            {
                firstTextBox.Focus();
            }
	    }

	    public event PropertyChangedEventHandler PropertyChanged;
	}
}
