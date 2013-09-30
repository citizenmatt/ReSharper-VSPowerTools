using System;
using System.ComponentModel;

namespace VSPowerTools.ToolWindows.LanguageMassEditor.BusinessObjects
{
	public abstract class InterfaceObject : INotifyPropertyChanged 
	{
		[field:NonSerialized]
		public event PropertyChangedEventHandler PropertyChanged;

		protected void RaisePropertyChanged(string propertyName)
		{
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
