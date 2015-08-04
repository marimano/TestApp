using ReferenceData.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ReferenceData
{
    /// <summary>
    /// Contains all User's data to show in table
    /// </summary>
    public class UserData
    {
        public int Id { get; private set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }

        public int? CountryId { get; set; }
        public string Country { get; set; }

        public int? SubdivisionId { get; set; }
        public string Subdivision { get; set; }

        public int? LocationId { get; set; }
        public string Location { get; set; }

        private UserData()
        {
            Country = ReferenceDataConsts.NotSpecified;
            Subdivision = ReferenceDataConsts.NotSpecified;
            Location = ReferenceDataConsts.NotSpecified;
        }
        
        /// <summary>
        /// Set properties from a given user. Location, Subdivision 
        /// and Country need to be set additionally
        /// </summary>
        /// <param name="user">A user whose data should be set</param>
        public UserData(User user) : this()
        {
            Id = user.Id;
            FirstName = user.FirstName;
            SecondName = user.SecondName;
            LocationId = user.LocationId;
        }

        /// <summary>
        /// Set all properties from a corresponding BindingGroup. 
        /// No additional set should be done
        /// </summary>
        /// <param name="id">User id</param>
        /// <param name="details">Binding group with all corresponding 
        /// binding expressions</param>
        public UserData(int id, BindingGroup details) : this()
        {
            Id = id;
            var type = GetType();
            foreach (BindingExpression exp in details.BindingExpressions)
            {
                var propName = exp.ParentBinding.Path.Path.Split('.').Last();
                var prop = type.GetProperty(propName);
                var value = exp.Target.GetValue(exp.TargetProperty);
                if (prop == null || !prop.CanWrite) continue;

                prop.SetValue(this, value);
            }
        }

        /// <summary>
        /// Sets location name and subdivisionId if it has locationId
        /// </summary>
        /// <param name="locations">Pairs of [locationId, Location]</param>
        public void SetLocation(Dictionary<int, Location> locations)
        {
            if (!LocationId.HasValue) return;

            var location = locations[LocationId.Value];
            Location = location.Description;
            SubdivisionId = location.SubdivisionId;
        }

        /// <summary>
        /// Sets subdivision name and countryId if it has subdivisionId
        /// </summary>
        /// <param name="subs">Pairs of [subdivisionId, Subdivision]</param>
        public void SetSubdivision(Dictionary<int, Subdivision> subs)
        {
            if (!SubdivisionId.HasValue) return;

            var sub = subs[SubdivisionId.Value];
            Subdivision = sub.Description;
            CountryId = sub.CountryId;
        }

        /// <summary>
        /// Sets country name if it has countryId
        /// </summary>
        /// <param name="subs">Pairs of [countryId, Country]</param>
        public void SetCountry(Dictionary<int, Country> countries)
        {
            if (!CountryId.HasValue) return;

            var country = countries[CountryId.Value];
            Country = country.Description;
        }

        /// <summary>
        /// Allows casting from UserData to User object
        /// </summary>
        /// <param name="ud">UserData to cast</param>
        /// <returns>Result User</returns>
        public static explicit operator User(UserData ud)
        {
            return new User
            {
                Id = ud.Id,
                FirstName = ud.FirstName,
                SecondName = ud.SecondName,
                LocationId = ud.LocationId
            };
        }
    }

    /// <summary>
    /// Allows to create an item with default Id and Description
    /// </summary>
    /// <typeparam name="T">Type of item to create</typeparam>
    public static class EmptyItem<T> where T : class, new()
    {
        public static T Create()
        {
            var item = new T();
            SetProperty(item, "Id", ReferenceDataConsts.NullId);
            SetProperty(item, "Description", ReferenceDataConsts.NotSpecified);
            return item;
        }

        private static void SetProperty(T item, string propName, object propValue)
        {
            var type = typeof(T);
            var prop = type.GetProperty(propName);
            if (prop != null && prop.CanWrite && prop.PropertyType == propValue.GetType())
            {
                prop.SetValue(item, propValue);
                return;
            }

            throw new ApplicationException("Cannot set " + propName + " to " + typeof(T).FullName);
        }
    }

    public static class ReferenceDataConsts
    {
        public const string NotSpecified = "[not specified]";
        public const int NullId = -1;
        public const string DefaultFName = "Default first name";
        public const string DefaultSName = "Default second name";
    }
}
