using System;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using VSPowerTools.ToolWindows.LanguageMassEditor.ViewModels;

namespace VSPowerTools.LanguageMassEditor
{
	[SolutionComponent]
	public class LanguageMassEditorManager
	{
		public LanguageMassEditorViewModel ViewModel
		{
			get;
			private set;
		}

		public LanguageMassEditorManager([NotNull] ISolution solution)
		{
			ViewModel = new LanguageMassEditorViewModel();
			ViewModel.SolutionPath = solution.SolutionFilePath.FullPath;
			ViewModel.OnCreatorNodeCreated += new ResourceCreatedEventHandler(ViewModel_OnCreatorNodeCreated);
		}

		public Action<ResourceCreatedEventArgs> Callback
		{
			get;
			set;
		}

		void ViewModel_OnCreatorNodeCreated(ResourceViewModel viewModel, ResourceCreatedEventArgs args)
		{
			if (Callback != null)
				Callback(args);
		}
	}
}