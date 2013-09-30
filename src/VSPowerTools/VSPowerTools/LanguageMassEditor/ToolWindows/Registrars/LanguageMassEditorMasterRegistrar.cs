using System;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.ToolWindowManagement;

namespace VSPowerTools.LanguageMassEditor
{
	[SolutionComponent]
	public class LanguageMassEditorMasterRegistrar
	{
		private readonly Lifetime _lifetime;
		private readonly ToolWindowClass _toolWindowClass;
		private VSPowerTools.ToolWindows.LanguageMassEditor.LanguageMassEditor _userControl;
		public VSPowerTools.ToolWindows.LanguageMassEditor.LanguageMassEditor UserControl
		{
			get { return _userControl ?? (_userControl = new VSPowerTools.ToolWindows.LanguageMassEditor.LanguageMassEditor()); }
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
//			var solution = Shell.Instance.GetComponent<JetBrains.Util.Lazy.Lazy<IVsSolution>>().Value;
//			
//			string solutionPath;
//			string solutionFile;
//			string userOptsFile;
//			int solutionPtr = solution.GetSolutionInfo(out solutionPath, out solutionFile, out userOptsFile);
//			Instance = this;
			try
			{
//				var t2 = Shell.Instance.GetComponent<ISolution>();
				
			}
			catch (Exception ex)
			{
			}
//			var t = Shell.Instance.GetComponent<LanguageMassEditorManager>();
//		    UserControl.SolutionPath = Path.Combine(solutionPath, solutionFile);

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
		}

		public void Show()
		{
			ToolWindowInstance instance = _toolWindowClass.RegisterInstance(
				_lifetime,
				"Übersicht", // title of your window; tip: StringUtil.MakeTitle
				null, // return a System.Drawing.Image to be displayed
				(lt, twi) =>
				{
					return new EitherControl(UserControl);
				});
			instance.EnsureControlCreated().Show();
		}
	}
}