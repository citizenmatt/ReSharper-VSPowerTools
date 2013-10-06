using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using AMLib.Utility;
using AMLib.VisualStudio;
using AMLib.Wpf.Common;
using VSPowerTools.ToolWindows.LanguageMassEditor.BusinessObjects;
using VSPowerTools.ToolWindows.LanguageMassEditor.Windows;

namespace VSPowerTools.ToolWindows.LanguageMassEditor.ViewModels
{

	public class LanguageMassEditorViewModel : DependencyObject, INotifyPropertyChanged
	{
		public static readonly DependencyProperty IsInDesignerModeProperty =
			DependencyProperty.Register("IsInDesignerMode", typeof (bool), typeof (LanguageMassEditorViewModel), new PropertyMetadata(default(bool)));

		public bool IsInDesignerMode
		{
			get { return (bool) GetValue(IsInDesignerModeProperty); }
			set { SetValue(IsInDesignerModeProperty, value); }
		}

		#region DependencyProperty "SolutionPath"

		public static readonly DependencyProperty SolutionPathProperty =
			DependencyProperty.Register("SolutionPath", typeof(string), typeof(LanguageMassEditorViewModel), new FrameworkPropertyMetadata(default(string), SolutionChanged));

		private static void SolutionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
		{
			var vm = dependencyObject as LanguageMassEditorViewModel;
			string solutionPath = args.NewValue as string;
			if (vm == null || args.Property != SolutionPathProperty || solutionPath == null) return;

			var solution = Solution.FromPath(solutionPath);

			vm.IsApplicationBusy = true;
			vm.Ressources.Clear();

			foreach (Project project in solution.Projects)
			{
				foreach (ResxFile resxFile in project.ResxFiles)
				{
					if (!ResxFile.GetLanguageFromPath(resxFile.ResourceFilePath).Equals(ResxFile.InvalidLanguage))
					{
						vm.Ressources.AddFile(resxFile);
						resxFile.OnHasChanged += delegate(ResxFile sender, ResxFileChangedEventArgs eventArgs) { ResourceFileChangedPhysically(sender, vm); };
					}
				}
			}

			vm.Ressources
				.BuildAsync()
				.ContinueWith(task =>
				{
					dependencyObject.Dispatcher.BeginInvoke(new Action(() =>
					{
						vm.RessourcesView.Refresh();
						vm.RessourcesView.MoveCurrentToFirst();
						vm.IsApplicationBusy = false;
						foreach (var ressource in vm.Ressources)
						{
							if (vm.OnCreatorNodeCreated != null)
							{
								ressource.OnCreatorNodeCreated += vm.OnCreatorNodeCreated;
							}
						}
					}));
				});
		}

		private static void ResourceFileChangedPhysically(ResxFile sender, LanguageMassEditorViewModel vm)
		{
			vm.Dispatcher.BeginInvoke(new Action(() =>
			{
				var allFiles = vm.Ressources.Select(s => new { Resource = s, Files = s.Files });

				foreach (var resourceFilePack in allFiles)
				{
					if (resourceFilePack.Files.Any(d => d.Equals(sender)))
					{
						var closure = resourceFilePack.Resource;
						DelayedExecution.Trigger(resourceFilePack.Resource.RessourceDisplayName, delegate { RequestUserReloadConfirmation(closure); });
					}
				}
			}), DispatcherPriority.ApplicationIdle);
		}

		private static void RequestUserReloadConfirmation(ResourceViewModel resourceViewModel)
		{
			TaskScheduler context = TaskScheduler.FromCurrentSynchronizationContext();
			if (MessageBox.Show("The resource files have been changed externally. Do you want to reload?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				resourceViewModel.BuildAsync().ContinueWith(task =>
				{
					var view = CollectionViewSource.GetDefaultView(resourceViewModel.Nodes);
					if (view != null)
					{
						view.Refresh();
					}
				}, context);
			}
		}

		private static readonly MultiPushbackTimer DelayedExecution = new MultiPushbackTimer(100, 1); 

		public event ResourceCreatedEventHandler OnCreatorNodeCreated;

		public string SolutionPath
		{
			get { return (string)GetValue(SolutionPathProperty); }
			set { SetValue(SolutionPathProperty, value); }
		}

		#endregion DependencyProperty "SolutionPath"

		public static readonly DependencyProperty RessourcesProperty =
			DependencyProperty.Register("Ressources", typeof(RessourceCollection), typeof(LanguageMassEditorViewModel), new PropertyMetadata(default(RessourceCollection), RessourcesChanged));

		private static void RessourcesChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			LanguageMassEditorViewModel vm = dependencyObject as LanguageMassEditorViewModel;
			if (vm != null)
			{
				vm._ressourcesView = CollectionViewSource.GetDefaultView(dependencyPropertyChangedEventArgs.NewValue);
				vm.RessourcesView.Refresh();
			}
		}

		public RessourceCollection Ressources
		{
			get
			{
				return (RessourceCollection)GetValue(RessourcesProperty);
			}
			set
			{
				SetValue(RessourcesProperty, value);
			}
		}

		private ICollectionView _ressourcesView;
		public ICollectionView RessourcesView
		{
			get
			{
				if (_ressourcesView == null)
					_ressourcesView = new CollectionViewSource() {Source = Ressources}.View;
				
				ApplyRessourceFilter(_ressourcesView);

				return _ressourcesView;
			}
			private set
			{
				if (_ressourcesView != null)
				{
					_ressourcesView.CurrentChanged -= new EventHandler(view_CurrentChanged);
				}

				_ressourcesView = value;

				ApplyRessourceFilter(_ressourcesView);

			}
		}

		public string AssemblyShortNameFilter
		{
			get { return _assemblyShortNameFilter; }
			set
			{
				_assemblyShortNameFilter = value;
				if (RessourcesAssemblyFilterView != null)
				{
					RessourcesAssemblyFilterView.Refresh();
				}
			}
		}

		private ICollectionView _ressourcesAssemblyFilterView;
		public ICollectionView RessourcesAssemblyFilterView
		{
			get
			{
				if (_ressourcesAssemblyFilterView == null)
					_ressourcesAssemblyFilterView = new CollectionViewSource() { Source = Ressources }.View;

				ApplyRessourceAssemblyFilter(_ressourcesAssemblyFilterView);

				return _ressourcesAssemblyFilterView;
			}
			private set
			{
				_ressourcesAssemblyFilterView = value;

				ApplyRessourceAssemblyFilter(_ressourcesAssemblyFilterView);

			}
		}

		private void ApplyRessourceAssemblyFilter(ICollectionView ressourcesAssemblyFilterView)
		{
			if (ressourcesAssemblyFilterView != null && ressourcesAssemblyFilterView.Filter == null)
			{
				ressourcesAssemblyFilterView.Filter = AssemblyResourceFilter;
			}
		}

		private bool AssemblyResourceFilter(object obj)
		{
			var vm = obj as ResourceViewModel;
			if (vm == null)
				return false;

			if (vm.Nodes.Count == 0)
				return false;

			if (vm.ProjectNamespace.ToUpper().Equals(AssemblyShortNameFilter.ToUpper()))
				return true;

			var target = Ressources.FirstOrDefault(d => d.ProjectNamespace.ToUpper().Equals(AssemblyShortNameFilter.ToUpper()));
			if (target != null)
			{
				if (target.ReferencesNamespaces.Select(s => s.ToUpper()).Any(s => vm.ProjectNamespace.ToUpper().Equals(s)))
					return true;

				return false;
			}

			return true;
			
		}

		private void ApplyRessourceFilter(ICollectionView ressourcesView)
		{
			if (ressourcesView != null && ressourcesView.Filter == null)
			{
				Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action<ICollectionView>(view =>
				{
					view.Filter = FilterResourceViewModels;
					view.CurrentChanged += view_CurrentChanged;
					
				}), ressourcesView);
			}
		}

		private bool FilterResourceViewModels(object o)
		{
			var vm = o as ResourceViewModel;
			if (vm == null)
				return false;

			if (vm.Nodes.Count == 0)
				return false;

			return true;
		}

		public static readonly DependencyProperty FilterStringProperty =
			DependencyProperty.Register("FilterString", typeof(string), typeof(LanguageMassEditorViewModel), new PropertyMetadata(default(string), FilterStringChanged));

		private static void FilterStringChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			LanguageMassEditorViewModel casted = dependencyObject as LanguageMassEditorViewModel;
			casted.RefreshCurrentRessourceFilter();
		}

		public string FilterString
		{
			get
			{
				return (string)GetValue(FilterStringProperty);
			}
			set
			{
				SetValue(FilterStringProperty, value);
			}
		}

		public static readonly DependencyProperty HasInvalidSearchInputProperty =
			DependencyProperty.Register("HasInvalidSearchInput", typeof(bool), typeof(LanguageMassEditorViewModel), new PropertyMetadata(default(bool)));

		public bool HasInvalidSearchInput
		{
			get
			{
				return (bool)GetValue(HasInvalidSearchInputProperty);
			}
			set
			{
				SetValue(HasInvalidSearchInputProperty, value);
			}
		}

		public static readonly DependencyProperty OpenSettingsCommandProperty =
			DependencyProperty.Register("OpenSettingsCommand", typeof(ICommand), typeof(LanguageMassEditorViewModel), new PropertyMetadata(default(ICommand)));

		public ICommand OpenSettingsCommand
		{
			get
			{
				return (ICommand)GetValue(OpenSettingsCommandProperty);
			}
			set
			{
				SetValue(OpenSettingsCommandProperty, value);
			}
		}

		public static readonly DependencyProperty SettingsProperty =
			DependencyProperty.Register("Settings", typeof(Settings), typeof(LanguageMassEditorViewModel), new PropertyMetadata(default(Settings)));

		public Settings Settings
		{
			get
			{
				return (Settings)GetValue(SettingsProperty);
			}
			set
			{
				SetValue(SettingsProperty, value);
			}
		}

		public static readonly DependencyProperty IsApplicationBusyProperty =
			DependencyProperty.Register("IsApplicationBusy", typeof(bool), typeof(LanguageMassEditorViewModel), new PropertyMetadata(default(bool)));

		public bool IsApplicationBusy
		{
			get
			{
				return (bool)GetValue(IsApplicationBusyProperty);
			}
			set
			{
				SetValue(IsApplicationBusyProperty, value);
			}
		}

		public LanguageMassEditorViewModel()
		{
			Ressources = new RessourceCollection();
			Settings = new Settings();
			Settings.PropertyChanged += new PropertyChangedEventHandler(Settings_PropertyChanged);
			OpenSettingsCommand = new RelayCommand(_ => OpenSettingsExecute(), _ => true);

#if (DEBUG)
 
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				IsInDesignerMode = true;

				var path = @"D:\Dateien\Software\Eigene Programme\C#\ResourceFileSolution\ResourceFileSolution.sln";
				if (File.Exists(path))
				{
					SolutionPath = path;
				}
			} 
#endif

		}

		void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			RefreshCurrentRessourceFilter();
		}

		void RefreshCurrentRessourceFilter()
		{
			ResourceViewModel vm;
			if (RessourcesView.CurrentItem != null && (vm = RessourcesView.CurrentItem as ResourceViewModel) != null)
			{
				var view = vm.Nodes.GetDefaultView();
				SetFilter(view);
			}
		}

		void view_CurrentChanged(object sender, EventArgs e)
		{
			RefreshCurrentRessourceFilter();
		}

		private LMESettings _openSettings;
		private string _assemblyShortNameFilter;

		void OpenSettingsExecute()
		{
			if (_openSettings != null)
			{
				if (!_openSettings.IsVisible)
				{
					_openSettings.Close();
					_openSettings = null;
				}
			}
			if (_openSettings == null)
			{
				_openSettings = new LMESettings();
				_openSettings.SetBinding(LMESettings.SettingsProperty, new Binding() { Source = Settings });
				_openSettings.Show();
			}
		}

		#region Implementation of INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		private void SetFilter(ICollectionView view)
		{
			if (view != null)
			{
				if (view.Filter != null)
					view.Filter -= Filter;

				view.Filter += Filter;
			}
		}

		private bool Filter(object o)
		{
			ResxNodeViewModel collection = o as ResxNodeViewModel;

			var filterString = FilterString ?? "";
			HasInvalidSearchInput = false;

			if (Settings.IsRegexSearch)
			{
				try
				{
					RegexOptions regexOptions = RegexOptions.Multiline;
					if (!Settings.IsSearchCaseSensitive)
						regexOptions ^= RegexOptions.IgnoreCase;

					if (Settings.IsNonEmpty)
					{
						if (filterString.Length > 0)
						{
							var hasEmpty = collection.Attributes.Any(node => string.IsNullOrEmpty(node.Value));
							var hasSimilarNode = collection.Attributes.Any(node => Regex.IsMatch(node.Value, filterString, regexOptions));
							var hasSimilarTagName = Regex.IsMatch(collection.Tag, filterString, regexOptions);

							if (hasEmpty && (hasSimilarNode || hasSimilarTagName))
								return true;
							return false;
						}
						else
						{
							return collection.Attributes.Any(node => string.IsNullOrEmpty(node.Value));
						}
					}
					else
					{
						return Regex.IsMatch(collection.Tag, filterString, regexOptions)
							   || collection.Attributes.Any(node => Regex.IsMatch(node.Value, filterString, regexOptions));
					}
				}
				catch (Exception exception)
				{
					HasInvalidSearchInput = true;
					return true;
				}
			}
			else
			{
				if (Settings.IsNonEmpty)
				{
					if (filterString.Length > 0)
					{
						bool hasEmpty;
						bool hasSimilarNode;
						bool hasSimilarTagName;

						if (Settings.IsFullTextSearch)
						{
							if (Settings.IsSearchCaseSensitive)
							{
								hasEmpty = collection.Attributes.Any(node => string.IsNullOrEmpty(node.Value));
								hasSimilarNode = collection.Attributes.Any(node => node.Value.Contains(filterString));
								hasSimilarTagName = collection.Tag.Contains(filterString);
							}
							else
							{
								hasEmpty = collection.Attributes.Any(node => string.IsNullOrEmpty(node.Value));
								hasSimilarNode = collection.Attributes.Any(node => node.Value.Contains(filterString, StringComparison.OrdinalIgnoreCase));
								hasSimilarTagName = collection.Tag.Contains(filterString, StringComparison.OrdinalIgnoreCase);
							}
						}
						else
						{
							hasEmpty = collection.Attributes.Any(node => string.IsNullOrEmpty(node.Value));
							hasSimilarNode = collection.Attributes.Any(node => node.Value.StartsWith(filterString, !Settings.IsSearchCaseSensitive, CultureInfo.InvariantCulture));
							hasSimilarTagName = collection.Tag.StartsWith(filterString, !Settings.IsSearchCaseSensitive, CultureInfo.InvariantCulture);
						}

						if (hasEmpty && (hasSimilarNode || hasSimilarTagName))
							return true;
						return false;
					}
					else
					{
						return collection.Attributes.Any(node => string.IsNullOrEmpty(node.Value));
					}
				}
				else
				{
					if (Settings.IsFullTextSearch)
					{
						if (Settings.IsSearchCaseSensitive)
						{
							return collection.Tag.Contains(filterString)
								   || collection.Attributes.Any(node => node.Value != null && node.Value.Contains(filterString));
						}
						else
						{
							return collection.Tag.Contains(filterString, StringComparison.OrdinalIgnoreCase)
								   || collection.Attributes.Any(node => node.Value != null && node.Value.Contains(filterString, StringComparison.OrdinalIgnoreCase));
						}
					}
					else
					{
						return collection.Tag.StartsWith(filterString, !Settings.IsSearchCaseSensitive, CultureInfo.InvariantCulture)
							   || collection.Attributes.Any(node => node.Value != null && node.Value.StartsWith(filterString, !Settings.IsSearchCaseSensitive, CultureInfo.InvariantCulture));
					}
				}
			}
		}
	}
}
