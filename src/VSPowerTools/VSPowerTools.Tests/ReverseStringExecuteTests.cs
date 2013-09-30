using JetBrains.ReSharper.Feature.Services.CSharp.Bulbs;
using JetBrains.ReSharper.Intentions.CSharp.Test;
using JetBrains.ReSharper.Intentions.Extensibility;
using NUnit.Framework;

namespace VSPowerTools.Tests
{
  [TestFixture]
  public class ReverseStringExecuteTests : CSharpContextActionExecuteTestBase
  {
    protected override string ExtraPath
    {
      get { return "ReverseStringAction"; }
    }

    protected override string RelativeTestDataPath
    {
      get { return "ReverseStringAction"; }
    }

    protected override IContextAction CreateContextAction(ICSharpContextActionDataProvider dataProvider)
    {
      return new ReverseStringAction(dataProvider);
    }

    [Test]
    public void ExecuteTest()
    {
      DoTestFiles("execute01.cs");
    }
  }
}
