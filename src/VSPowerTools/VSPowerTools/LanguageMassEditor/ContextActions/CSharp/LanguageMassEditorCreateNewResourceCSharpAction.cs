using System;
using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.CSharp.Bulbs;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Transactions;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using VSPowerTools.Extensions.Generic;
using VSPowerTools.ToolWindows.LanguageMassEditor.ViewModels;

namespace VSPowerTools.LanguageMassEditor
{
	[ContextAction(Name = "LanguageMassEditor Eintrag anlegen", Description = "LanguageMassEditor Eintrag anlegen", Group = "C#")]
	public class LanguageMassEditorCreateNewResourceCSharpAction : ContextActionBase
	{
		private readonly ICSharpContextActionDataProvider _provider;
		private ICSharpLiteralExpression _currentTreeNode;
		
		public LanguageMassEditorCreateNewResourceCSharpAction(ICSharpContextActionDataProvider provider)
		{
			_provider = provider;
		}

		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
		{
			var manager = solution.GetComponent<LanguageMassEditorManager>();
			var window = solution.GetComponent<LanguageMassEditorNewItemRegistrar>();
			window.ViewModel = manager.ViewModel;
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

						//"Properties.Settings.Default.teststring"
						var factory = CSharpElementFactory.GetInstance(_currentTreeNode.GetPsiModule(), true, true);
						var expression = factory.CreateReferenceExpression(string.Format("{0}.{1}.{2}", args.Namespace, args.ClassName, args.TokenName));

						ModificationUtil.ReplaceChild(_currentTreeNode, expression);

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
				return "LanguageMassEditor Eintrag anlegen";
			}
		}

		public override bool IsAvailable(IUserDataHolder cache)
		{
			var node = _provider.GetSelectedElement<ICSharpLiteralExpression>(true, true);

			if (node != null)
			{
				_currentTreeNode = node;
				return true;
			}
			return false;
		}
	}
}