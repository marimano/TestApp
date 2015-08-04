using ReferenceData.DAL.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceData
{
    public class ViewModel
    {
        public ObservableCollection<UserData> Users { get; private set; }
        public ViewModel()
        {

        }
    }
}
