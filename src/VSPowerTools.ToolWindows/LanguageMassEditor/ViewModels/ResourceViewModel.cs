using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using AMLib.Utility;
using AMLib.VisualStudio;
using AMLib.Wpf.Common;
using VSPowerTools.ToolWindows.LanguageMassEditor.BusinessObjects;

namespace VSPowerTools.ToolWindows.LanguageMassEditor.ViewModels
{
	public class ResourceCreatedEventArgs : EventArgs
	{
		public ResourceCreatedEventArgs(string tokenName, string className, string @namespace, string assemblyName)
		{
			ClassName = className;
			Namespace = @namespace;
			TokenName = tokenName;
			AssemblyName = assemblyName;
		}

		public string AssemblyName
		{
			get;
			private set;
		}

		public string ClassName
		{
			get;
			private set;
		}

		public string Namespace
		{
			private set;
			get;
		}

		public string TokenName
		{
			private set;
			get;
		}
	}

	public delegate void ResourceCreatedEventHandler(ResourceViewModel viewModel, ResourceCreatedEventArgs args);

	public class ResourceViewModel : SaveableViewModelBase
	{
		#region Fields

		private readonly Ressource _resource;

		#endregion

		public event ResourceCreatedEventHandler OnCreatorNodeCreated;

		#region Properties

		public ResxNodeCollection Nodes
		{
			get
			{
				return GetValue(() => Nodes);
			}
			set
			{
				if(SetValue(() => Nodes, value, collection =>
				                             {
												 collection.OnInserted -= OnNodeInserted;
												 collection.OnRemoved -= OnNodeRemoved;
				                             }))
				{
					if(value != null)
					{
						value.OnInserted += OnNodeInserted;
						value.OnRemoved += OnNodeRemoved;
					}
				}
			}
		}

		public ResxNodeViewModel CreatorNode
		{
			get { return GetValue(() => CreatorNode); }
			set { SetValue(() => CreatorNode, value); }
		}

		void OnNodeRemoved(AMLib.Collections.ChangeCollection<ResxNodeViewModel> sender, AMLib.Collections.ChangeCollectionItemEventArgs<ResxNodeViewModel> item)
		{
			item.Item.OnIsDirty -= OnNodeIsDirty;
			IsDirty = true;
			RaisePropertyChanged("Nodes");
		}

		void OnNodeIsDirty(object sender, EventArgs e)
		{
			IsDirty = true;
		}

		void OnNodeInserted(AMLib.Collections.ChangeCollection<ResxNodeViewModel> sender, AMLib.Collections.ChangeCollectionItemEventArgs<ResxNodeViewModel> item)
		{
			item.Item.OnIsDirty += OnNodeIsDirty;
			IsDirty = true;
			RaisePropertyChanged("Nodes");
		}

		public string RessourceName
		{
			get
			{
				return GetValue(() => RessourceName);
			}
			set
			{
				SetValue(() => RessourceName, value, null, false);
			}
		}

		public bool HasBeenChanged
		{
			get
			{
				return GetValue(() => HasBeenChanged);
			}
			set
			{
				SetValue(() => HasBeenChanged, value);
			}
		}


		public bool IsLoading
		{
			get
			{
				return GetValue(() => IsLoading);
			}
			set
			{
				SetValue(() => IsLoading, value);
				CommandManager.InvalidateRequerySuggested();
			}
		}

		public bool IsSaving
		{
			get
			{
				return GetValue(() => IsSaving);
			}
			set
			{
				SetValue(() => IsSaving, value);
				CommandManager.InvalidateRequerySuggested();
			}
		}


		public ICommand AppendCreatorNodeCommand
		{
			get { return GetValue(() => AppendCreatorNodeCommand); }
			set { SetValue(() => AppendCreatorNodeCommand, value); }
		}

		public ICommand LoadCommand
		{
			get
			{
				return GetValue(() => LoadCommand);
			}
			set
			{
				SetValue(() => LoadCommand, value);
			}
		}

		public ICommand NodeDownCommand
		{
			get
			{
				return GetValue(() => NodeDownCommand);
			}
			set
			{
				SetValue(() => NodeDownCommand, value);
			}
		}

		public ICommand NodeUpCommand
		{
			get
			{
				return GetValue(() => NodeUpCommand);
			}
			set
			{
				SetValue(() => NodeUpCommand, value);
			}
		}

		public ICommand DeleteCommand
		{
			get
			{
				return GetValue(() => DeleteCommand);
			}
			set
			{
				SetValue(() => DeleteCommand, value);
			}
		}

		public ICommand CreateNodeCommand
		{
			get
			{
				return GetValue(() => CreateNodeCommand);
			}
			set
			{
				SetValue(() => CreateNodeCommand, value);
			}
		}


		public ICommand OpenSettingsCommand
		{
			get
			{
				return GetValue(() => OpenSettingsCommand);
			}
			set
			{
				SetValue(() => OpenSettingsCommand, value);
			}
		}


		public string NewNodeText
		{
			get
			{
				return GetValue(() => NewNodeText);
			}
			set
			{
				SetValue(() => NewNodeText, value);
				CommandManager.InvalidateRequerySuggested();
			}
		}

		#endregion

		#region Private Methods

		private void BuildInternal()
		{
			IsLoading = true;
			Nodes.Clear();
			Exception ex;
			Parallel.ForEach(_resource.Files, file => file.Load(out ex));

			LazyDictionary<string, ResxNodeViewModel> cache = new LazyDictionary<string, ResxNodeViewModel>();
			LazyDictionary<ResxFile, string> fileLanguages = new LazyDictionary<ResxFile, string>();
			HashSet<string> tagBuffer = new HashSet<string>();

			foreach (var file in _resource.Files)
			{
				tagBuffer.Clear();

				var language = ResxFile.GetLanguageFromPath(file.ResourceFilePath);
				fileLanguages.Set(file, language);
				foreach (var node in file.Nodes)
				{
					if(tagBuffer.Contains(node.Tag))
						continue;

					tagBuffer.Add(node.Tag);

					var collection = cache.GetOrCreate(node.Tag, () => new ResxNodeViewModel(node.Tag));
					collection.Attributes.Add(new ResxAttributeViewModel(){Language = language, Value = node.Value});
				}
			}

			foreach (var element in cache)
			{
				Nodes.Add(element.Value);
				if (element.Value.Attributes.Count < fileLanguages.Count)
				{
					var missingLanguages = fileLanguages.Values.Select(s => s).Except(element.Value.Attributes.Select(s => s.Language));
					foreach (var missingLanguage in missingLanguages)
					{
						element.Value.Attributes.Add(new ResxAttributeViewModel(){Language = missingLanguage, Value = ""});
					}
				}
			}

			var view = CollectionViewSource.GetDefaultView(this.Nodes);
			if (view != null)
			{
				view.Refresh();
				view.MoveCurrentToFirst();

				RaisePropertyChanged("Nodes");
			}

			CreatorNode = CreateNode("ResourceName");

			HasBeenChanged = false;
			IsLoading = false;
			IsDirty = false;

			CommandManager.InvalidateRequerySuggested();
		}
		
		private bool RequestReload()
		{
			// todo am aktuell führt ein build nicht zum korrekten neuladen der ansicht
			//			return MessageBox.Show("Die Dateien wurden geändert. Soll neu geladen werden?","Frage", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
			return MessageBox.Show("Die Dateien wurden geändert. Soll neu geladen werden? (BUG: Bitte Neu laden drücken.)", "Frage", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
		}

		#endregion

		#region Constructors

		public ResourceViewModel(Ressource resource) : base()
		{
			Nodes = new ResxNodeCollection();
			DeleteCommand = new RelayCommand(DeleteExecute, o => o != null);
			CreateNodeCommand = new RelayCommand(o => CreateNodeExecute(), o => CanCreateNode());
			LoadCommand = new RelayCommand(o => LoadCommandExecute(), o => !IsLoading);
			AppendCreatorNodeCommand = new RelayCommand(AppendCreatorNodeExecute, o => CreatorNode.IsDirty && !Nodes.Any(d => d.Tag.Equals(CreatorNode.Tag)));

			var firstFile = resource.Files.First();
			if (firstFile != null)
			{
				var fullPath = Path.GetDirectoryName(firstFile.ResourceFilePath);
				var projectPath = Path.GetDirectoryName(firstFile.Project.Path);
				var resPath = Ressource.GetRessourceName(firstFile.ResourceFilePath);

				var subtraction = fullPath.Replace(projectPath, "");

				// todo am
				// hier muss optimiert werden. lkönnten zwei unterschiedliche gruppen im hauptverzeichnis liegen :)

				if (subtraction.Length > 0)
				{
					if (resPath != null && resPath[1] != null)
					{
						RessourceName = string.Format("{0} - {1}/{2}", firstFile.Project.Name, subtraction.TrimStart('/', '\\'), resPath[1]);
					}
					else
					{
						RessourceName = string.Format("{0} - {1}", firstFile.Project.Name, subtraction.TrimStart('/', '\\'));
					}
				}
				else
				{
					RessourceName = string.Format("{0} - {1}", firstFile.Project.Name, resPath[1]);
				}
			}

			_resource = resource;
		}

		private void AppendCreatorNodeExecute(object o)
		{
			Nodes.Add(CreatorNode);

			var view = CollectionViewSource.GetDefaultView(Nodes);
			view.Refresh();

			Save().ContinueWith(task =>
			{
				if (OnCreatorNodeCreated != null)
				{
					ResxFile parsed = _resource.Files.FirstOrDefault(s => s.HasDesignerFile);

					if (parsed != null)
					{
						var args = new ResourceCreatedEventArgs(CreatorNode.Tag, parsed.DesignerClassName, parsed.DesignerNamespace, _resource.Project != null ? _resource.Project.Name : "");
						OnCreatorNodeCreated(this, args);
					}
				}

				CreatorNode = CreateNode("ResourceName");
			});
		}

		private void LoadCommandExecute()
		{
			var context = TaskScheduler.FromCurrentSynchronizationContext();
			BuildAsync()
				.ContinueWith(task =>
				{
					var view = CollectionViewSource.GetDefaultView(Nodes);
					view.Refresh();
					
				}, context);
		}

		#endregion

		#region Command methods

		private bool CanCreateNode()
		{
			return !string.IsNullOrEmpty(NewNodeText) && !Nodes.Any(d => d.Tag.Equals(NewNodeText));
		}

		private void DeleteExecute(object o)
		{
			var view = CollectionViewSource.GetDefaultView(Nodes);
			view.MoveCurrentToPrevious();
			if (view.IsCurrentBeforeFirst)
				view.MoveCurrentToFirst();
			Nodes.Remove(o as ResxNodeViewModel);
			view.Refresh();
		}

		private ResxNodeViewModel CreateNode(string name)
		{
			var first = Nodes.FirstOrDefault();
			var node = new ResxNodeViewModel(name);
			if (first != null)
			{
				foreach (var attribute in first.Attributes)
				{
					ResxAttributeViewModel copy = new ResxAttributeViewModel();
					copy.Comment = attribute.Comment;
					copy.Language = attribute.Language;
					copy.Value = "";

					node.Attributes.Add(copy);
				}
			}

			return node;
		}

		private void CreateNodeExecute()
		{
			if(CanCreateNode())
			{
				var newNode = CreateNode(NewNodeText);
				Nodes.Add(newNode);

				var view = CollectionViewSource.GetDefaultView(Nodes);
				view.Refresh();
				view.MoveCurrentTo(newNode);

				NewNodeText = "";
			}
		}

		#endregion

		#region Public Methods

		public void Build()
		{
			BuildInternal();
		}

		public Task BuildAsync()
		{
			return Task.Factory.StartNew(BuildInternal);
		}

		#endregion

		#region Overrides of SaveableViewModelBase

		public override Task Save()
		{
			TaskScheduler ui = TaskScheduler.FromCurrentSynchronizationContext();
			return Task.Factory.StartNew(() =>
			{
				IsSaving = true;
			}, CancellationToken.None, TaskCreationOptions.None, ui)
			.ContinueWith(task =>
			{
				Exception ex;
				foreach (var file in _resource.Files)
				{
					var converted = CreateFromNodes(Nodes.Select(n => new Tuple<string,ResxAttributeViewModel>(n.Tag, n.Attributes.FirstOrDefault(a => a.Language.Equals(file.FileLanguage)))));
					file.Nodes.Clear();
					foreach (ResxNode resxNode in converted)
					{
						file.Nodes.Add(resxNode);
					}
					if (!file.Save(out ex))
					{
						MessageBox.Show(string.Format("Fehler beim Speichern. {0}", ex.Message));
					}
				}
			})
			.ContinueWith(task =>
			{
				IsSaving = false;
			}, ui);
		}

		private IEnumerable<ResxNode> CreateFromNodes(IEnumerable<Tuple<string, ResxAttributeViewModel>> @select)
		{
			foreach (Tuple<string, ResxAttributeViewModel> tuple in select)
			{
				var result = new ResxNode();
				result.Tag = tuple.Item1;
				result.Value = tuple.Item2.Value;
				result.Comment = tuple.Item2.Comment;

				yield return result;
			}
		}

		#endregion
	}
}
