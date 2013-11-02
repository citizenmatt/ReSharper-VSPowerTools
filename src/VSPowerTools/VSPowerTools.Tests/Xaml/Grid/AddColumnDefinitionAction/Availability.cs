using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Xaml.Bulbs;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Intentions.Test;
using JetBrains.TextControl;
using NUnit.Framework;

namespace VSPowerTools.Tests.Xaml.Grid.AddColumnDefinitionAction
{
	[TestFixture]
	[Category("XAML")]
	public class Availability :  ContextActionAvailabilityTestBase
	{
		[Test]
		public void AvailabilityTest()
		{
			DoTestFiles("availability01.cs");
		}

		protected override IContextAction CreateContextAction(ISolution solution, ITextControl textControl)
		{
			return new ContextActions.Xaml.Grid.AddColumnDefinitionAction(XamlContextActionDataProvider.Create(solution, textControl));
		}

		protected override string ExtraPath
		{
			get { return "AddColumnDefinitionAction"; }
		}

		protected override string RelativeTestDataPath
		{
			get { return "AddColumnDefinitionAction"; }
		}
	}
}
