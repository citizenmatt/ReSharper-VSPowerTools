using System;
using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.Xaml.Bulbs;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Psi.Transactions;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xaml.Impl.Util;
using JetBrains.ReSharper.Psi.Xaml.Tree;
using JetBrains.ReSharper.Psi.Xml.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using VSPowerTools.Extensions.Generic;
using VSPowerTools.Extensions.Xaml;
using VSPowerTools.ToolWindows.LanguageMassEditor.ViewModels;

namespace VSPowerTools.LanguageMassEditor
{
	[ContextAction(Name = "LanguageMassEditor - New *.resx-entry", Description = "LanguageMassEditor - New *.resx-entry", Group = "XAML")]
	public class LanguageMassEditorCreateNewResourceXamlAction : ContextActionBase
	{
		private readonly XamlContextActionDataProvider _provider;
		private ITreeNode _currentTreeNode;

		public LanguageMassEditorCreateNewResourceXamlAction(XamlContextActionDataProvider provider)
		{
			_provider = provider;
		}

		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
		{
			var manager = solution.GetComponent<LanguageMassEditorManager>();
			var window = solution.GetComponent<LanguageMassEditorNewItemRegistrar>();
			window.ViewModel = manager.ViewModel;
			window.ViewModel.AssemblyShortNameFilter = _provider.Project.Name;

			manager.Callback = new Action<ResourceCreatedEventArgs>(args =>
			{
				var locker = solution.GetComponent<IShellLocks>();
				locker.ReentrancyGuard.ExecuteOrQueue("replaceReferences", delegate
				{
					using (PsiTransactionCookie.CreateAutoCommitCookieWithCachesUpdate(_currentTreeNode.GetPsiServices(), "updateXamlReferences"))
					{
						var file = _currentTreeNode.GetContainingFile();
						// xmlns:Converters1="clr-namespace:AMLib.Wpf.Converters;assembly=AMLib.Wpf"
						file.AddNamespaceOnce(args.ClassName, args.Namespace, args.AssemblyName, true);
						
						//	{x:Static Properties:Resources.teststring}
						if (_propertyAttribute != null)
						{
							_propertyAttribute.SetStringValue(string.Format("{{x:Static {0}:{1}.{2}}}", args.ClassName, args.ClassName, args.TokenName), true);
						}

						window.Hide();
					}
				});
			});

			window.Show();
			
			return null;
		}

		public override string Text
		{
			get
			{
				return "LanguageMassEditor - New *.resx-entry";
			}
		}

		private IPropertyAttribute _propertyAttribute;
		public override bool IsAvailable(IUserDataHolder cache)
		{
			var node = _provider.FindNodeAtCaret<ITreeNode>();


			//        XmlTagHeaderNode
			//          XmlTagStartToken(384,1)(type:TAG_START, text:<)
			//          XamlIdentifier(385,9)(type:IDENTIFIER, text:TextBlock)
			//          PropertyAttribute
			//            XamlIdentifier(395,4)(type:IDENTIFIER, text:Text)
			//            XmlEqToken(399,1)(type:EQ, text:=)
			//            PropertyAttributeValue
			//              XamlToken(400,1)(type:MARKUP_QUOTE, text:")
			//              XamlToken(401,2)(type:MARKUP_TEXT, text:hi) <!--
			//              XamlToken(403,1)(type:MARKUP_QUOTE, text:")

			IXmlToken token;
			if (node != null && node.IsChildOfPropertyAttributeValue(out token) && token != null)
			{
				var s = token.GetText();
				if (!string.IsNullOrEmpty(s))
				{
					_currentTreeNode = token;
					if (_currentTreeNode.Parent != null && _currentTreeNode.Parent.Parent != null)
					{
						_propertyAttribute = (IPropertyAttribute)_currentTreeNode.Parent.Parent;
					}
					return true;
				}
			}
			return false;
		}
	}
}