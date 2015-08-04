using ReferenceData.DAL.Model;
using ReferenceData.DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceData
{
    public interface IView
    {
        event Func<IEnumerable<User>> LoadUserList;
        event Func<IEnumerable<Location>> LoadLocationList;
        event Func<IEnumerable<Subdivision>> LoadSubdivisionList;
        event Func<IEnumerable<Country>> LoadCountryList;

        event Func<User, bool> ChangeUser;
        event Func<User, bool> AddUser;
    }
}
