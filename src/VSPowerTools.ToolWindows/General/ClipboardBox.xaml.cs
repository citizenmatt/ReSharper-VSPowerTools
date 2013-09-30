using System.Windows;
using System.Windows.Input;
using AMLib.Wpf.Common;

namespace VSPowerTools.ToolWindows.General
{
	/// <summary>
	/// Interaktionslogik für ClipboardBox.xaml
	/// </summary>
	public partial class ClipboardBox : Window
	{
		public ClipboardBox()
		{
			InitializeComponent();
			CopyCloseCommand = new RelayCommand(CopyCloseExecute);
		}

		private void CopyCloseExecute(object o)
		{
			Clipboard.SetText(ClipboardText);
			this.Close();
		}

		public static readonly DependencyProperty ClipboardTextProperty =
			DependencyProperty.Register("ClipboardText", typeof (string), typeof (ClipboardBox), new PropertyMetadata(default(string)));

		public string ClipboardText
		{
			get { return (string) GetValue(ClipboardTextProperty); }
			set { SetValue(ClipboardTextProperty, value); }
		}

		public static readonly DependencyProperty CopyCloseCommandProperty =
			DependencyProperty.Register("CopyCloseCommand", typeof (ICommand), typeof (ClipboardBox), new PropertyMetadata(default(ICommand)));

		public ICommand CopyCloseCommand
		{
			get { return (ICommand) GetValue(CopyCloseCommandProperty); }
			set { SetValue(CopyCloseCommandProperty, value); }
		}
	}
}
