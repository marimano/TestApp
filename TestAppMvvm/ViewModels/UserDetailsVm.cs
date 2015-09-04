using ReferenceData.DAL;
using ReferenceData.DAL.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TestAppMvvm.BaseImplementations;
using TestAppMvvm.Properties;

namespace TestAppMvvm.ViewModels
{
    public class UserDetailsVm : ViewModelBase, IDataErrorInfo, IEditableObject
    {
        #region Private members

        private readonly UserData _userModel;
        private readonly List<Country> _countries;
        private readonly Dictionary<int, List<Subdivision>> _subdivisionsByCountry;
        private readonly Dictionary<int, List<Location>> _locationsBySubdivision;
        
        private int? _countryId;
        private int? _subdivisionId;
        private int? _locationId;
        private string _originalFirstName;
        private string _originalSecondName;

        #endregion

        #region Properties

        /// <summary>
        /// First name of a current user
        /// </summary>
        public string FirstName
        {
            get { return _userModel.FirstName; }
            set { _userModel.FirstName = value; }
        }

        /// <summary>
        /// Second name of a current user
        /// </summary>
        public string SecondName
        {
            get { return _userModel.SecondName; }
            set { _userModel.SecondName = value; }
        }

        /// <summary>
        /// Not null representation of a current user's location id
        /// </summary>
        public int LocationId
        {
            get { return _locationId ?? Consts.NullId; }
            set
            {
                var newValue = value == Consts.NullId ? null : (int?)value;
                if (_locationId == value) return;

                _locationId = newValue;
                OnPropertyChanged("LocationId");
            }
        }

        /// <summary>
        /// Name of a current user's location
        /// </summary>
        public string Location
        {
            get
            {
                return _userModel.Location == null
                    ? Resources.NotSelectedItem
                    : _userModel.Location.Description;
            }
        }

        /// <summary>
        /// Not null representation of a current user's subdivision id
        /// </summary>
        public int SubdivisionId
        {
            get { return _subdivisionId ?? Consts.NullId; }
            set
            {
                var newValue = value == Consts.NullId ? null : (int?)value;
                if (_subdivisionId == newValue) return;

                _subdivisionId = newValue;
                OnPropertyChanged("SubdivisionId");
                OnPropertyChanged("Locations");
            }
        }

        /// <summary>
        /// Name of a current user's subdivision
        /// </summary>
        public string Subdivision
        {
            get
            {
                return _userModel.Subdivision == null
                    ? Resources.NotSelectedItem
                    : _userModel.Subdivision.Description;
            }
        }

        /// <summary>
        /// Not null representation of a current user's country id
        /// </summary>
        public int CountryId
        {
            get { return _countryId ?? Consts.NullId; }
            set
            {
                var newValue = value == Consts.NullId ? null : (int?)value;
                if (_countryId == newValue) return;

                _countryId = newValue;
                OnPropertyChanged("CountryId");
                OnPropertyChanged("Subdivisions");
            }
        }

        /// <summary>
        /// Name of a current user's country
        /// </summary>
        public string Country
        {
            get
            {
                return _userModel.Country == null
                    ? Resources.NotSelectedItem
                    : _userModel.Country.Description;
            }
        }

        /// <summary>
        /// All available countries to select
        /// </summary>
        public IEnumerable<Country> Countries { get { return _countries; } }

        /// <summary>
        /// All available subdivisions of a selected country to select
        /// </summary>
        public IEnumerable<Subdivision> Subdivisions
        {
            get
            {
                return _subdivisionsByCountry.ContainsKey(CountryId)
                    ? _subdivisionsByCountry[CountryId]
                    : EmptySubdivisions;
            }
        }

        /// <summary>
        /// All available locations of a selected subdivision to select
        /// </summary>
        public IEnumerable<Location> Locations
        {
            get
            {
                return _locationsBySubdivision.ContainsKey(SubdivisionId)
                    ? _locationsBySubdivision[SubdivisionId]
                    : EmptyLocations;
            }
        }

        #endregion

        #region Validation

        /// <summary>
        /// Returns true if this object has no validation errors.
        /// </summary>
        public bool IsValid
        {
            get { return PropertiesToValidate.All(property => this[property] == null); }
        }

        /// <summary>
        /// Indicates if it has not saved changes
        /// </summary>
        public bool HasChanged
        {
            get
            {
                return _originalFirstName != _userModel.FirstName ||
                       _originalSecondName != _userModel.SecondName ||
                       _locationId != _userModel.Location.Id;
            }
        }

        public string Error { get { return null; } }

        /// <summary>
        /// Get an error message if a specified property has errors
        /// </summary>
        /// <param name="propertyName">Property to verify</param>
        /// <returns>Error message if it has errors</returns>
        public string this[string propertyName]
        {
            get
            {
                string error = null;

                switch (propertyName)
                {
                    case "FirstName":
                        if (string.IsNullOrWhiteSpace(FirstName))
                            error = Resources.IncorrectFirstName;
                        break;
                    case "SecondName":
                        if (string.IsNullOrWhiteSpace(SecondName))
                            error = Resources.IncorrectSecondName;
                        break;
                    case "CountryId":
                        if (!_countryId.HasValue)
                            error = Resources.IncorrectCountry;
                        break;
                    case "SubdivisionId":
                        if (!_subdivisionId.HasValue)
                            error = Resources.IncorrectSubdivision;
                        break;
                    case "LocationId":
                        if (!_locationId.HasValue)
                            error = Resources.IncorrectLocation;
                        break;
                }

                return error;
            }
        }

        #endregion

        public UserDetailsVm(UserData user, List<Country> countries,
            Dictionary<int, List<Subdivision>> subdivisionsByCountry,
            Dictionary<int, List<Location>> locationsBySubdivision)
        {
            _userModel = user;
            _countries = countries;
            _subdivisionsByCountry = subdivisionsByCountry;
            _locationsBySubdivision = locationsBySubdivision;

            SetLocationIds();
        }

        #region IEditableObject

        /// <summary>
        /// Keeps editable properties in cache ones
        /// </summary>
        public void BeginEdit()
        {
            _originalFirstName = _userModel.FirstName;
            _originalSecondName = _userModel.SecondName;
        }

        /// <summary>
        /// Restores saved values
        /// </summary>
        public void CancelEdit()
        {
            FirstName = _originalFirstName;
            SecondName = _originalSecondName;
            SetLocationIds();
            OnAllPropertyChanged();
        }

        /// <summary>
        /// Save current values
        /// </summary>
        public void EndEdit()
        {
            _originalFirstName = _userModel.FirstName;
            _originalSecondName = _userModel.SecondName;

            SetLocation(_userModel);

            OnAllPropertyChanged();
        } 

        #endregion

        #region Helpers

        internal UserData GetUser(bool isNew)
        {
            if (!isNew) return _userModel;

            var newUser = new UserData
            {
                FirstName = FirstName,
                SecondName = SecondName
            };

            SetLocation(newUser);
            return newUser;
        }

        private void SetLocation(UserData target)
        {
            target.Location = _locationId.HasValue
                ? Locations.FirstOrDefault(l => l.Id == _locationId.Value)
                : null;

            target.Subdivision = _subdivisionId.HasValue
                ? Subdivisions.FirstOrDefault(s => s.Id == _subdivisionId.Value)
                : null;

            target.Country = _countryId.HasValue
                ? Countries.FirstOrDefault(c => c.Id == _countryId.Value)
                : null;
        }

        private void SetLocationIds()
        {
            _locationId = _userModel.Location != null ? (int?)_userModel.Location.Id : null;

            _subdivisionId = _userModel.Subdivision != null ? (int?)_userModel.Subdivision.Id : null;

            _countryId = _userModel.Country != null ? (int?)_userModel.Country.Id : null;
        }

        static readonly string[] PropertiesToValidate = 
        { 
            "FirstName", 
            "SecondName",
            "CountryId",
            "SubdivisionId",
            "LocationId"
        };

        private static readonly List<Subdivision> EmptySubdivisions = new List<Subdivision>
        {
            new Subdivision {Id = Consts.NullId, Description = Resources.NotSelectedItem}
        };

        private static readonly List<Location> EmptyLocations = new List<Location>
        {
            new Location {Id = Consts.NullId, Description = Resources.NotSelectedItem}
        };

        #endregion
    }
}
