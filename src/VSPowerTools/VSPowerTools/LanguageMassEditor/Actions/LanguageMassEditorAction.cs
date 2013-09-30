using JetBrains.ActionManagement;
using JetBrains.UI.ToolWindowManagement;

namespace VSPowerTools.LanguageMassEditor
{
	[ActionHandler("VSPowerTools.LanguageMassEditorMainWindow")]
	public class LanguageMassEditorAction : ActivateToolWindowActionHandler<LanguageMassEditorDescriptor>
	{
	}
}