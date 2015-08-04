using ReferenceData.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReferenceData
{
    public partial class MainWindow : IView
    {
        public event Func<IEnumerable<User>> LoadUserList;
        public event Func<IEnumerable<Location>> LoadLocationList;
        public event Func<IEnumerable<Subdivision>> LoadSubdivisionList;
        public event Func<IEnumerable<Country>> LoadCountryList;

        private void OnLoadCountries()
        {
            if (LoadCountryList == null) return;

            var result = LoadCountryList();
            foreach (var c in result)
            {
                var id = c.Id;
                subdivisionsByCountry.Add(id, new List<Subdivision>());
            }

            countries.AddRange(result);
        }

        private void OnLoadSubdivisions()
        {
            if (LoadSubdivisionList == null) return;

            subdivisions = new List<Subdivision>(LoadSubdivisionList());
            subdivisions.ForEach(s =>
            {
                locationsBySubdivision.Add(s.Id, new List<Location>());
                var id = s.CountryId.HasValue ? s.CountryId.Value : ReferenceDataConsts.NullId;
                subdivisionsByCountry[id].Add(s);
            });
        }

        private void OnLoadLocations()
        {
            if (LoadLocationList == null) return;

            locations = new List<Location>(LoadLocationList());
            locations.ForEach(l => locationsBySubdivision[l.SubdivisionId].Add(l));
        }

        private void OnLoadUserList()
        {
            if (LoadUserList == null) return;

            var result = LoadUserList();
            users = new List<UserData>();
            var locationDict = locations.ToDictionary(l => l.Id);
            var subDict = subdivisions.ToDictionary(s => s.Id);
            var countryDict = countries.ToDictionary(c => c.Id);

            foreach (var user in result)
            {
                var userData = new UserData(user);
                userData.SetLocation(locationDict);
                userData.SetSubdivision(subDict);
                userData.SetCountry(countryDict);
                users.Add(userData);
            }
        }

        public event Func<User, bool> ChangeUser;

        private void OnChangeUser(User user)
        {
            if (ChangeUser == null) return;

            var ok = ChangeUser(user);
            if (!ok)
            {
                throw new ApplicationException("User has not been updated");
            }
        }

        public event Func<User, bool> AddUser;

        private void OnAddUser(User user)
        {
            if (AddUser == null) return;

            var ok = AddUser(user);
            if (!ok)
            {
                throw new ApplicationException("User has not been added");
            }
        }
    }
}
