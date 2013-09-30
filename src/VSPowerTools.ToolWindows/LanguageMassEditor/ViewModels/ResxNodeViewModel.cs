using System;
using System.Threading.Tasks;
using System.Windows.Input;
using AMLib.Wpf.Common;

namespace VSPowerTools.ToolWindows.LanguageMassEditor.ViewModels
{
	public class ResxNodeViewModel : SaveableViewModelBase
	{
		public ResxNodeViewModel() : base()
		{
			Attributes = new ResxAttributeCollection();
		}

		public ResxNodeViewModel(string tag) : this()
		{
			Tag = tag;
		}

		public string Tag
		{
			get
			{
				return GetValue(() => Tag);
			}
			set
			{
				if(SetValue(() => Tag, value))
				{
					CommandManager.InvalidateRequerySuggested();
				}
			}
		}

		public ResxAttributeCollection Attributes
		{
			get
			{
				return GetValue(() => Attributes);
			}
			set
			{
				if(SetValue(() => Attributes, value, callback =>
				                                     {
														 callback.OnInserted -= OnAttributeInserted;
														 callback.OnRemoved -= OnAttributeRemoved;
				                                     }))
				{
					if(value != null)
					{
						value.OnInserted += OnAttributeInserted;
						value.OnRemoved += OnAttributeRemoved;
					}
				}
			}
		}

		void OnAttributeRemoved(AMLib.Collections.ChangeCollection<ResxAttributeViewModel> sender, AMLib.Collections.ChangeCollectionItemEventArgs<ResxAttributeViewModel> item)
		{
			item.Item.OnIsDirty -= OnAttributeIsDirty;
		}

		void OnAttributeInserted(AMLib.Collections.ChangeCollection<ResxAttributeViewModel> sender, AMLib.Collections.ChangeCollectionItemEventArgs<ResxAttributeViewModel> item)
		{
			item.Item.OnIsDirty += OnAttributeIsDirty;
		}

		void OnAttributeIsDirty(object sender, EventArgs e)
		{
			IsDirty = true;
		}

		public static ResxNodeViewModel GetResxNodeViewModel()
		{
			ResxNodeViewModel result = new ResxNodeViewModel("test");
			result.Attributes.Add(new ResxAttributeViewModel() { Language = "de", Value = "a" });
			result.Attributes.Add(new ResxAttributeViewModel() { Language = "en", Value = "b" });
			result.Attributes.Add(new ResxAttributeViewModel() { Language = "fr", Value = "c" });
			result.Attributes.Add(new ResxAttributeViewModel() { Language = "default", Value = "d" });

			return result;
		}

		public override Task Save()
		{
			return null;
		}
	}
}
