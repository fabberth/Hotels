using Hotels.Domain;

namespace Hotels.Models
{
    public class RoomViewModel : BaseModel<Room>
    {
        public Hotel Hotel { get; set; }
    }
}
