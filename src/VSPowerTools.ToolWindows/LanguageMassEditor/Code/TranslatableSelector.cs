using System.Collections.ObjectModel;

namespace VSPowerTools.ToolWindows.LanguageMassEditor.Code
{
	public class TranslatableSelector<T> : ObservableCollection<TranslatableSelection<T>>
	{
		/// <summary>
		/// Adds Selector Option
		/// </summary>
		/// <param name="value">Value of type T</param>
		/// <param name="displayName">DisplayName of Value</param>
		public void Add(T value, string displayName)
		{
			Add(new TranslatableSelection<T>(value, displayName));
		}
	}
}
