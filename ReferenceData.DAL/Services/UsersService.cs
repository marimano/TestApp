using ReferenceData.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceData.DAL.Services
{
    public class UsersService
    {
        public bool AddOrUpdate(User user)
        {
            using (var connection = new ReferenceDataEntities())
            {
                var oldValue = connection.Users.FirstOrDefault(x => x.Id == user.Id);
                if (oldValue != null)
                {
                    connection.Entry(oldValue).CurrentValues.SetValues(user);
                }
                else
                {
                    connection.Users.Add(user);
                }

                var result = connection.SaveChanges();
                var expectedResult = 1;
                return result == expectedResult;
            }
        }

        public IEnumerable<User> GetItems()
        {
            List<User> users;
            using (var connection = new ReferenceDataEntities())
            {
                users = connection.Users.ToList();
            }

            return users;
        }

        public User GetItem(int id)
        {
            User user;
            using (var connection = new ReferenceDataEntities())
            {
                user = connection.Users.FirstOrDefault(x => x.Id == id);
            }

            return user;
        }
    }
}
