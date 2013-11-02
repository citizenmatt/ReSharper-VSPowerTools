using System;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.ToolWindowManagement;
using JetBrains.Util;

namespace VSPowerTools.LanguageMassEditor
{
	[SolutionComponent]
	public class LanguageMassEditorMasterRegistrar
	{
		private readonly Lifetime _lifetime;
		private readonly ToolWindowClass _toolWindowClass;
		private readonly ToolWindowInstance _toolWindowInstance;
		private ToolWindows.LanguageMassEditor.UserControls.LanguageMassEditor _userControl;
		public ToolWindows.LanguageMassEditor.UserControls.LanguageMassEditor UserControl
		{
			get { return _userControl ?? (_userControl = new ToolWindows.LanguageMassEditor.UserControls.LanguageMassEditor()); }
			set { _userControl = value; }
		}

		public ISolution Solution
		{
			get;
			private set;
		}

		public static LanguageMassEditorMasterRegistrar Instance
		{
			get;
			private set;
		}

		public LanguageMassEditorMasterRegistrar(ISolution solution, Lifetime lifetime, ToolWindowManager toolWindowManager,
			LanguageMassEditorDescriptor descriptor)
		{
			var manager = solution.GetComponent<LanguageMassEditorManager>();
			UserControl.DataContext = manager.ViewModel;

			_lifetime = lifetime;
			
			_toolWindowClass = toolWindowManager.Classes[descriptor];
			_toolWindowClass.RegisterEmptyContent(
				lifetime,
				lt =>
				{
					return new EitherControl(UserControl);
				});
			_toolWindowInstance = _toolWindowClass.RegisterInstance(
				_lifetime,
				"Overview", // title of your window; tip: StringUtil.MakeTitle
				null, // return a System.Drawing.Image to be displayed
				(lt, twi) =>
				{
					return new EitherControl(UserControl);
				});
		}

		public void Show()
		{
			_toolWindowInstance.EnsureControlCreated().Show();
		}
	}
}