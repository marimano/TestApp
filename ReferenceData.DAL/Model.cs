using ReferenceData.DAL.Model;
using ReferenceData.DAL.Services;
using System.Collections.Generic;

namespace ReferenceData.DAL
{
    public class DataModel
    {
        readonly UsersService _userService = new UsersService();
        readonly LocationsService _locationService = new LocationsService();
        readonly SubdivisionService _subdivisionService = new SubdivisionService();
        readonly CountriesService _countryService = new CountriesService();

        public IEnumerable<UserData> GetUsers()
        {
            return _userService.GetDataItems();
        }

        public IEnumerable<Location> GetLocations()
        {
            return _locationService.GetItems();
        }

        public IEnumerable<Subdivision> GetSubdivisions()
        {
            return _subdivisionService.GetItems();
        }

        public IEnumerable<Country> GetCountries()
        {
            return _countryService.GetItems();
        }

        public bool UpdateUser(UserData userToUpdate)
        {
            return _userService.AddOrUpdate(userToUpdate);
        }
    }
}
