using ReferenceData.DAL.Model;
using System.Collections.Generic;
using System.Linq;

namespace ReferenceData.DAL.Services
{
    public class UsersService
    {
        public bool AddOrUpdate(UserData userData)
        {
            using (var connection = new ReferenceDataEntities())
            {
                var user = connection.Users.FirstOrDefault(x => x.Id == userData.Id);
                var isAdd = false;
                if (user == null)
                {
                    isAdd = true;
                    user = new User();
                }

                SetValues(user, userData);
                if (isAdd)
                {
                    connection.Users.Add(user);
                }

                var result = connection.SaveChanges();
                userData.Id = user.Id;
                var expectedResult = 1;
                return result == expectedResult;
            }
        }

        private static void SetValues(User to, UserData from)
        {
            to.FirstName = from.FirstName;
            to.SecondName = from.SecondName;
            to.LocationId = from.Location != null ? (int?)from.Location.Id : null;
        }

        public IEnumerable<UserData> GetDataItems()
        {
            using (var connection = new ReferenceDataEntities())
            {
                var query = from u in connection.Users
                    let l = u.Location
                    let s = l != null ? l.Subdivision : null
                    let c = s != null ? s.Country : null
                    select new UserData
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        SecondName = u.SecondName,
                        Country = c,
                        Subdivision = s,
                        Location = l
                    };

                return query.ToList();
            }
        }
    }
}
