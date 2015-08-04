using ReferenceData.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceData.DAL.Services
{
    public class CountriesService
    {
        public void AddOrUpdate(Country country)
        {
            using (var connection = new ReferenceDataEntities())
            {
                Country oldValue = connection.Countries.FirstOrDefault(x => x.Id == country.Id);
                if (oldValue != null)
                {
                    connection.Entry(oldValue).CurrentValues.SetValues(country);
                    connection.SaveChanges();
                }
                else
                {
                    connection.Countries.Add(country);
                    connection.SaveChanges();
                }
            }
        }

        public IEnumerable<Country> GetItems()
        {
            List<Country> countries;
            using (var connection = new ReferenceDataEntities())
            {
                countries = connection.Countries.ToList();
            }

            return countries;
        }

        public Country GetItem(int id)
        {
            Country country;
            using (var connection = new ReferenceDataEntities())
            {
                country = connection.Countries.FirstOrDefault(x => x.Id == id);
            }

            return country;
        }
    }
}
