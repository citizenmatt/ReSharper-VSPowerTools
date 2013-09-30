using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using AMLib.Collections;
using AMLib.VisualStudio;
using VSPowerTools.ToolWindows.LanguageMassEditor.BusinessObjects;

namespace VSPowerTools.ToolWindows.LanguageMassEditor.ViewModels
{
	public class RessourceCollection : NotifyCollection<ResourceViewModel>
	{
		private readonly List<ResxFile> _files = new List<ResxFile>();
		public void AddFile(params ResxFile[] files)
		{
			foreach (ResxFile resxFile in files)
			{
				_files.Add(resxFile);
			}
		}

		private void BuildInternal()
		{
			Items.Clear();

			var groups = _files.ToLookup(file => string.Join(",", Ressource.GetRessourceName(file.ResourceFilePath)));
			foreach (IGrouping<string, ResxFile> grouping in groups)
			{
				var ressource = new Ressource(grouping.Key);
				foreach (ResxFile file in grouping)
				{
					if (ResxFile.GetLanguageFromPath(file.ResourceFilePath) != ResxFile.InvalidLanguage)
						ressource.AddFile(file);
				}
				var viewModel = new ResourceViewModel(ressource);
				viewModel.Build();
				Items.Add(viewModel);
			}

			ICollectionView source = CollectionViewSource.GetDefaultView(this);
			source.Refresh();
			source.MoveCurrentToFirst();
		}

		public void Build()
		{
			BuildInternal();
		}

		public Task BuildAsync()
		{
			return Task.Factory.StartNew(BuildInternal);
		}
	}
}
