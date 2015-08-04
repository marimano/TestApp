using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceData.DAL.Model
{
    public interface IReferenceDataEntities : IDisposable
    {
        List<Country> Countries { get; set; }
        List<Location> Locations { get; set; }
        List<Subdivision> Subdivisions { get; set; }
        List<User> Users { get; set; }

        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        int SaveChanges();
    }
}
