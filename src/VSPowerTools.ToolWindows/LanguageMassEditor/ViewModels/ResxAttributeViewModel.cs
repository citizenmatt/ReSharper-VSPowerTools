using System.Threading.Tasks;
using AMLib.Wpf.Common;

namespace VSPowerTools.ToolWindows.LanguageMassEditor.ViewModels
{
	public class ResxAttributeViewModel : SaveableViewModelBase 
	{
		public string Language
		{
			get
			{
				return GetValue(() => Language);
			}
			set
			{
				SetValue(() => Language, value);
			}
		}

		public string Value
		{
			get
			{
				return GetValue(() => Value);
			}
			set
			{
				if(SetValue(() => Value, value))
				{
					IsDirty = true;
				}
			}
		}

		public string Comment
		{
			get
			{
				return GetValue(() => Comment);
			}
			set
			{
				SetValue(() => Comment, value);
			}
		}

		public override Task Save()
		{
			return null;
		}
	}
}
