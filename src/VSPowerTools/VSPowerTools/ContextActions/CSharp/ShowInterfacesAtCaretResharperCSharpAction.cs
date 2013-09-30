using System;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.CSharp.Bulbs;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using VSPowerTools.ToolWindows.General;

namespace VSPowerTools.ContextActions.CSharp
{
#if(DEBUG)
	[ContextAction(Name = "ShowInterfacesAtCaretCSharpAction", Description = "Shows all interfaces at caret", Group = "C#")]
	public class ShowInterfacesAtCaretResharperCSharpAction : ContextActionBase
	{
		private readonly ICSharpContextActionDataProvider _provider;
		private ITreeNode _currentTreeNode;

		public ShowInterfacesAtCaretResharperCSharpAction(ICSharpContextActionDataProvider provider)
		{
			_provider = provider;
		}

		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
		{
			if (_currentTreeNode != null)
			{
				var type = _currentTreeNode.GetType();
				var interfaces = type.GetInterfaces();

				var window = new ClipboardBox();
				window.ClipboardText = string.Join(Environment.NewLine, interfaces.Select(s => s.FullName));
				window.Show();
			}
			return null;
		}

		public override string Text
		{
			get
			{
				return "Show full type name at caret";
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
#endif
}