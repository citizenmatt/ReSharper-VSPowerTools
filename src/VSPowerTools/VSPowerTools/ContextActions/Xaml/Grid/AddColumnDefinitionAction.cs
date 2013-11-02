using System;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.Xaml.Bulbs;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using VSPowerTools.ToolWindows.General;

namespace VSPowerTools.ContextActions.Xaml.Grid
{
	[ContextAction(Name = "VSPowerTools.AddColumnDefinitionAction", Description = "Add column definition.", Group = "XAML")]
	public class AddColumnDefinitionAction : ContextActionBase
	{
		private readonly XamlContextActionDataProvider _provider;
		private ITreeNode _currentTreeNode;

		public AddColumnDefinitionAction(XamlContextActionDataProvider provider)
		{
			_provider = provider;
		}

		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
		{
			_provider.GetSelectedElement<ITreeNode>(true, true);

			return null;
		}

		public override string Text
		{
			get
			{
				return "Add column definition.";
			}
		}

		public override bool IsAvailable(IUserDataHolder cache)
		{
			var node = _provider.GetSelectedElement<ITreeNode>(true, true);

			if (node != null)
			{
				_currentTreeNode = node;
				return true;
			}
			return false;
		}
	}
}