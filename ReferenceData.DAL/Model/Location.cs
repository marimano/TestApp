//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ReferenceData.DAL.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Location
    {
        public Location()
        {
            this.Users = new HashSet<User>();
        }
    
        public int Id { get; set; }
        public string Description { get; set; }
        public int SubdivisionId { get; set; }
    
        public virtual Subdivision Subdivision { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
