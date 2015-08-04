using ReferenceData.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceData.DAL.Services
{
    public class LocationsService
    {
        public void AddOrUpdate(Location location)
        {
            using (var connection = new ReferenceDataEntities())
            {
                Location oldValue = connection.Locations.FirstOrDefault(x => x.Id == location.Id);
                if (oldValue != null)
                {
                    connection.Entry(oldValue).CurrentValues.SetValues(location);
                    connection.SaveChanges();
                }
                else
                {
                    connection.Locations.Add(location);
                    connection.SaveChanges();
                }
            }
        }

        public IEnumerable<Location> GetItems()
        {
            List<Location> locations;
            using (var connection = new ReferenceDataEntities())
            {
                locations = connection.Locations.ToList();
            }

            return locations;
        }

        public Location GetItem(int id)
        {
            Location location;
            using (var connection = new ReferenceDataEntities())
            {
                location = connection.Locations.FirstOrDefault(x => x.Id == id);
            }

            return location;
        }

        public IEnumerable<Location> GetLocationsBySubdivisionId(int subdivisionId)
        {
            List<Location> locations;
            using (var connection = new ReferenceDataEntities())
            {
                locations = connection.Locations.Where(x => x.SubdivisionId == subdivisionId).ToList();
            }

            return locations;
        }
    }
}
