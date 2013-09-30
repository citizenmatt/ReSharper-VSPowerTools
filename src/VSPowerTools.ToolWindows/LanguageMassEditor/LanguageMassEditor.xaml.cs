using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AMLib.Wpf.Common;
using VSPowerTools.ToolWindows.LanguageMassEditor.ViewModels;

namespace VSPowerTools.ToolWindows.LanguageMassEditor
{
	/// <summary>
	/// Interaktionslogik für LanguageMassEditor.xaml
	/// </summary>
	public partial class LanguageMassEditor : UserControl
	{
		public LanguageMassEditor()
		{
			InitializeComponent();
            FocusDefaultCommand = new RelayCommand(o => FocusDefault());
		}

	    private void FocusDefault()
	    {
	        LanguageControl.FocusDefault();
	    }

	    public static readonly DependencyProperty SolutionPathProperty =
	        DependencyProperty.Register("SolutionPath", typeof (string), typeof (LanguageMassEditor), new PropertyMetadata(default(string), SolutionPathChanged));

	    private static void SolutionPathChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
	    {
	        LanguageMassEditor ctrl = dependencyObject as LanguageMassEditor;
            if (ctrl == null)
                return;

	        var viewModel = ctrl.DataContext as LanguageMassEditorViewModel;
            if (viewModel == null)
                return;

	        viewModel.SolutionPath = dependencyPropertyChangedEventArgs.NewValue as string;
	    }

	    public string SolutionPath
	    {
	        get
	        {
	            return (string) GetValue(SolutionPathProperty);
	        }
	        set
	        {
	            SetValue(SolutionPathProperty, value);
	        }
	    }

	    public static readonly DependencyProperty FocusDefaultCommandProperty =
	        DependencyProperty.Register("FocusDefaultCommand", typeof (ICommand), typeof (LanguageMassEditor), new PropertyMetadata(default(ICommand)));

	    public ICommand FocusDefaultCommand
	    {
	        get
	        {
	            return (ICommand) GetValue(FocusDefaultCommandProperty);
	        }
	        set
	        {
	            SetValue(FocusDefaultCommandProperty, value);
	        }
	    }

		private void TextBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			TextBox tb = sender as TextBox;
			if (tb == null) return;
			tb.SelectAll();
			e.Handled = true;
		}
	}
}
