using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Asp.CustomReferences;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.ToolWindowManagement;
using VSPowerTools.ToolWindows.LanguageMassEditor.UserControls;
using VSPowerTools.ToolWindows.LanguageMassEditor.ViewModels;

namespace VSPowerTools.LanguageMassEditor
{
	[SolutionComponent]
	public class LanguageMassEditorNewItemRegistrar
	{
		private readonly Lifetime _lifetime;
		private readonly ToolWindowClass _toolWindowClass;
		private LanguageMassEditorNewItemControl _userControl;
		public LanguageMassEditorNewItemControl UserControl
		{
			get { return _userControl ?? (_userControl = new LanguageMassEditorNewItemControl()); }
			set { _userControl = value; }
		}

		public LanguageMassEditorViewModel ViewModel
		{
			get
			{
				return UserControl.DataContext as LanguageMassEditorViewModel;
			}
			set { UserControl.DataContext = value; }
		}

		public LanguageMassEditorNewItemRegistrar(Lifetime lifetime, ToolWindowManager toolWindowManager,
											LanguageMassEditorNewEntryDescriptor descriptor)
		{
			_lifetime = lifetime;

			_toolWindowClass = toolWindowManager.Classes[descriptor];
			_toolWindowClass.RegisterEmptyContent(
			  lifetime,
			  lt =>
			  {
				  return new EitherControl(UserControl);
			  });
		}

		private ToolWindowInstance _instance;

		public void Show()
		{
			_instance = _toolWindowClass.RegisterInstance(
			  _lifetime,
			  "New entry", // title of your window; tip: StringUtil.MakeTitle
			  null, // return a System.Drawing.Image to be displayed
			  (lt, twi) =>
			  {
				  return new EitherControl(UserControl);
			  });
			_instance.EnsureControlCreated().Show();
		}

		public void Hide()
		{
			_instance.EnsureControlCreated().Close();
		}
	}
}
