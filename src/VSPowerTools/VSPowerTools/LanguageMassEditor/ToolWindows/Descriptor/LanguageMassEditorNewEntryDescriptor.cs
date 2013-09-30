using JetBrains.Application;
using JetBrains.UI.ToolWindowManagement;

namespace VSPowerTools.LanguageMassEditor
{
	[ToolWindowDescriptor(
		Text = "Language Mass Editor",
		ProductNeutralId = "LanguageMassEditorNewEntryWindow",
		InitialWidth = 500,
		InitialHeight = 300,
		VisibilityPersistenceScope = ToolWindowVisibilityPersistenceScope.Solution,
		InitialDocking = ToolWindowInitialDocking.Floating,
		Icon = typeof(VSPowerToolsThemedIcons.Icon016),
		Type = ToolWindowType.SingleInstance)]
	public class LanguageMassEditorNewEntryDescriptor : ToolWindowDescriptor
	{
		public LanguageMassEditorNewEntryDescriptor(IApplicationDescriptor applicationDescriptor) : base(applicationDescriptor)
		{
		}
	}
}