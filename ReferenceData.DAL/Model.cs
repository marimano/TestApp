using ReferenceData.DAL.Model;
using ReferenceData.DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceData.DAL
{
    internal class DataModel
    {
        UsersService userService = new UsersService();
        LocationsService locationService = new LocationsService();
        SubdivisionService subdivisionService = new SubdivisionService();
        CountriesService countryService = new CountriesService();

        internal IEnumerable<User> GetUsers()
        {
            return userService.GetItems();
        }

        internal IEnumerable<Location> GetLocations()
        {
            return locationService.GetItems();
        }

        internal IEnumerable<Subdivision> GetSubdivisions()
        {
            return subdivisionService.GetItems();
        }

        internal IEnumerable<Country> GetCountries()
        {
            return countryService.GetItems();
        }

        internal bool UpdateUser(User userToUpdate)
        {
            return userService.AddOrUpdate(userToUpdate);
        }
    }
}
