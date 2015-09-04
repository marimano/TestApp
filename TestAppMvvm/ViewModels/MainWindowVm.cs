using ReferenceData.DAL;
using ReferenceData.DAL.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TestAppMvvm.BaseImplementations;
using TestAppMvvm.Properties;

namespace TestAppMvvm.ViewModels
{
    public class MainWindowVm : ViewModelBase
    {
        // for access to database
        private readonly DataModel _model;

        // all countries in the database
        private List<Country> _countries;
        // all subdivisions grouped by country id
        private Dictionary<int, List<Subdivision>> _subdivisionsByCountry;
        // all locations grouped by subdivision id
        private Dictionary<int, List<Location>> _locationsBySubdivision;

        // all displayed user view models
        private ObservableCollection<UserDetailsVm> _userVms;
        // selected user
        private UserDetailsVm _currentUserVm;
        // may be false is data are not loaded yet from database
        private bool _isEditingAllowed;

        /// <summary>
        /// Returns the collection of users to display.
        /// </summary>
        public ObservableCollection<UserDetailsVm> UserVms
        {
            get { return _userVms ?? (_userVms = new ObservableCollection<UserDetailsVm>()); }
        }

        /// <summary>
        /// Selected user
        /// </summary>
        public UserDetailsVm CurrentUserVm
        {
            get
            {
                if (IsLoading) return null;

                return _currentUserVm ?? (_currentUserVm = CreateUserDetailsVm(new UserData()));
            }
            set
            {
                if (_currentUserVm == value) return;

                if (_currentUserVm != null)
                    _currentUserVm.CancelEdit();

                _currentUserVm = value;

                if (_currentUserVm != null)
                    _currentUserVm.BeginEdit();

                OnPropertyChanged("CurrentUserVm");
            }
        }

        /// <summary>
        /// Indicates if a user is ready to edit
        /// </summary>
        public bool IsEditingAllowed
        {
            get { return _isEditingAllowed; }
            set
            {
                if (_isEditingAllowed == value) return;

                _isEditingAllowed = value;
                OnPropertyChanged("IsEditingAllowed");
                OnPropertyChanged("IsLoading");
            }
        }

        /// <summary>
        /// Indicates if data loading is in progress
        /// </summary>
        public bool IsLoading
        {
            get { return !_isEditingAllowed; }
        }

        #region Editing Commands

        Command _addCommand, _saveCommand, _cancelCommand;

        /// <summary>
        /// Command to add a user into user list and into database
        /// </summary>
        public ICommand AddNewUserCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new Command(AddNewUser, CanAddNewUser));
            }
        }

        private bool CanAddNewUser(object obj)
        {
            return CurrentUserVm != null && CurrentUserVm.IsValid;
        }

        private void AddNewUser(object obj)
        {
            var newUser = CurrentUserVm.GetUser(true);
            CurrentUserVm.CancelEdit();
            var ok = _model.UpdateUser(newUser);
            if (!ok)
            {
                throw new ApplicationException(Resources.UserNotAdded);
            }

            var newUserVm = CreateUserDetailsVm(newUser);

            UserVms.Add(newUserVm);
            CurrentUserVm = newUserVm;
        }

        /// <summary>
        /// Command to update a user in user list and in database
        /// </summary>
        public ICommand SaveUserCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new Command(SaveUser, CanSaveUser));
            }
        }

        private bool CanSaveUser(object obj)
        {
            return UserVms.Contains(CurrentUserVm) && CurrentUserVm.IsValid && CurrentUserVm.HasChanged;
        }

        private void SaveUser(object obj)
        {
            CurrentUserVm.EndEdit();

            var ok = _model.UpdateUser(CurrentUserVm.GetUser(false));
            if (!ok)
            {
                throw new ApplicationException(Resources.UserNotUpdated);
            }
        }

        /// <summary>
        /// Command to revert changes made into a user info
        /// </summary>
        public ICommand CancelChangesCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new Command(o => CurrentUserVm.CancelEdit())); }
        }
        
        #endregion

        public MainWindowVm()
        {
            _model = new DataModel();
            LoadDataAsync();
        }

        private void LoadDataAsync()
        {
            Task.Factory.StartNew(LoadData);
        }

        private void LoadData()
        {
            _countries = new List<Country>(_model.GetCountries());
            _subdivisionsByCountry = new Dictionary<int, List<Subdivision>>();

            _countries.ForEach(c => _subdivisionsByCountry.Add(c.Id, new List<Subdivision>()));
            _countries.Add(new Country { Id = Consts.NullId, Description = Resources.NotSelectedItem });

            _locationsBySubdivision = new Dictionary<int, List<Location>>();

            var subdivisions = new List<Subdivision>(_model.GetSubdivisions());
            subdivisions.ForEach(s =>
            {
                _locationsBySubdivision.Add(s.Id, new List<Location>());
                var id = s.CountryId ?? Consts.NullId;
                _subdivisionsByCountry[id].Add(s);
            });

            var locations = new List<Location>(_model.GetLocations());
            locations.ForEach(l => _locationsBySubdivision[l.SubdivisionId].Add(l));

            var users = _model.GetUsers().Select(CreateUserDetailsVm).ToList();

            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                users.ForEach(u => UserVms.Add(u));
                IsEditingAllowed = true;
                OnPropertyChanged("CurrentUserVm");
                // commands' canExecute will be re-evaluated
                CommandManager.InvalidateRequerySuggested();
            }));
        }

        private UserDetailsVm CreateUserDetailsVm(UserData userData)
        {
            return new UserDetailsVm(userData, _countries,
                _subdivisionsByCountry, _locationsBySubdivision);
        }
    }
}
