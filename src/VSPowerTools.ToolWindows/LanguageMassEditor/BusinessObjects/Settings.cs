namespace VSPowerTools.ToolWindows.LanguageMassEditor.BusinessObjects
{
    public enum CreateBehaviour
    {
        SelectedCreateNode,
        FocusDefaultTranslation
    }

    public class Settings : InterfaceObject
    {
        private bool _isNonEmpty;
        public bool IsNonEmpty
        {
            get
            {
                return _isNonEmpty;
            }
            set
            {
                _isNonEmpty = value;
                RaisePropertyChanged("IsNonEmpty");
            }
        }

        private bool _isSearchCaseSensitive;
        public bool IsSearchCaseSensitive
        {
            get
            {
                return _isSearchCaseSensitive;
            }
            set
            {
                _isSearchCaseSensitive = value;
                RaisePropertyChanged("IsSearchCaseSensitive");
            }
        }

        private bool _isRegexSearch;
        public bool IsRegexSearch
        {
            get
            {
                return _isRegexSearch;
            }
            set
            {
                _isRegexSearch = value;
                RaisePropertyChanged("IsRegexSearch");
            }
        }

        private bool _isFullTextSearch;
        public bool IsFullTextSearch
        {
            get
            {
                return _isFullTextSearch;
            }
            set
            {
                _isFullTextSearch = value;
                RaisePropertyChanged("IsFullTextSearch");
            }
        }

        public Settings()
        {
            IsNonEmpty = false;
            IsSearchCaseSensitive = false;
            IsRegexSearch = false;
            IsFullTextSearch = true;
        }
    }
}
