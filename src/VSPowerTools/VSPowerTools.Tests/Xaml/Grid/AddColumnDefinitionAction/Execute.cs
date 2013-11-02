using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CSharp.Bulbs;
using JetBrains.ReSharper.Feature.Services.Xaml.Bulbs;
using JetBrains.ReSharper.Intentions.CSharp.Test;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.IntentionsTests.Xaml;
using JetBrains.TextControl;
using NUnit.Framework;

namespace VSPowerTools.Tests.Xaml.Grid.AddColumnDefinitionAction
{
	[TestFixture]
	public class Execute : XamlContextActionExecuteTestBase
	{
		protected override string ExtraPath
		{
			get { return "AddColumnDefinitionAction"; }
		}

		protected override string RelativeTestDataPath
		{
			get { return "AddColumnDefinitionAction"; }
		}

		protected override IContextAction CreateContextAction(ISolution solution, ITextControl textControl)
		{
			return new ContextActions.Xaml.Grid.AddColumnDefinitionAction(XamlContextActionDataProvider.Create(solution, textControl));
		}

		[Test]
		public void ExecuteTest()
		{
			DoTestFiles("execute01.cs");
		}
	}
}
