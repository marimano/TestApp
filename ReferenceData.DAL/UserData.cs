using ReferenceData.DAL.Model;

namespace ReferenceData.DAL
{
    public class UserData
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public Location Location { get; set; }
        public Subdivision Subdivision { get; set; }
        public Country Country { get; set; }
    }
}
