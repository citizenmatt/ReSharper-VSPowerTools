namespace VSPowerTools.ToolWindows.LanguageMassEditor.Code
{
	public class TranslatableSelection<T>
	{
		public T Value { get; set; }
		public string DisplayMember { get; set; }

		public TranslatableSelection(T value, string displayName)
		{
			Value = value;
			DisplayMember = displayName;
		}
	}
}
