using JetBrains.Application;
using JetBrains.UI.ToolWindowManagement;

namespace VSPowerTools.LanguageMassEditor
{
	[ToolWindowDescriptor(
	Text = "Language Mass Editor",
	ProductNeutralId = "LanguageMassEditorMainWindow",
	VisibilityPersistenceScope = ToolWindowVisibilityPersistenceScope.Solution,
	InitialDocking = ToolWindowInitialDocking.Floating,
	Icon = typeof(VSPowerToolsThemedIcons.Icon016),
	Type = ToolWindowType.SingleInstance)]
	public class LanguageMassEditorDescriptor : ToolWindowDescriptor
	{
		public LanguageMassEditorDescriptor(IApplicationDescriptor applicationDescriptor) : base(applicationDescriptor)
		{
		}
	}
}