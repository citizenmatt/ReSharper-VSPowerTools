using System.Reflection;
using System.Windows.Forms;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;

namespace VSPowerTools
{
  [ActionHandler("VSPowerTools.About")]
  public class AboutAction : IActionHandler
  {
    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
    {
      // return true or false to enable/disable this action
      return true;
    }

    public void Execute(IDataContext context, DelegateExecute nextExecute)
    {
      MessageBox.Show(
		string.Format("VSPowerTools v{0}\nAndreas Müller\n\nResource File Management, Xaml functionality and other things", Assembly.GetAssembly(typeof(AboutAction)).GetName().Version),
        "About VSPowerTools",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information);
    }
  }
}
