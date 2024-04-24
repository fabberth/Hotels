using Hotels.Domain;

namespace Hotels.Models
{
    public class ReserveViewModel : BaseModel<Reserve>
    {
        public string DateOfAdmission { get; set; } = "";
        public string DateOfExit { get; set; } = "";
        public int NumberOfPeople { get; set; }
        public string Location { get; set; } = "";
        public Room Room { get; set; }
        public IList<Room> Rooms { get; set; }
    }
}
