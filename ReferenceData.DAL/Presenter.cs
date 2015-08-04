using ReferenceData.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ReferenceData.DAL
{
    public sealed class Presenter
    {
        private readonly IView view;
        private readonly DataModel model;

        public Presenter(IView view)
        {
            model = new DataModel();
            this.view = view;

            view.AddUser += view_AddUser;
            view.ChangeUser += view_ChangeUser;

            view.LoadCountryList +=view_LoadCountryList;
            view.LoadLocationList +=view_LoadLocationList;
            view.LoadSubdivisionList +=view_LoadSubdivisionList;
            view.LoadUserList += view_LoadUserList;
        }

        bool view_AddUser(User userToAdd)
        {
            return model.UpdateUser(userToAdd);
        }

        bool view_ChangeUser(User userToUpdate)
        {
            return model.UpdateUser(userToUpdate);
        }

        IEnumerable<User> view_LoadUserList()
        {
            return model.GetUsers();
        }

        IEnumerable<Subdivision> view_LoadSubdivisionList()
        {
            return model.GetSubdivisions();
        }

        IEnumerable<Location> view_LoadLocationList()
        {
            return model.GetLocations();
        }

        IEnumerable<Country> view_LoadCountryList()
        {
            return model.GetCountries();
        }
    }
}
