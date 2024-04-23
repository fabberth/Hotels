using System.ComponentModel.DataAnnotations;

namespace Hotels.Domain
{
    public class Room
    {
        [Key]
        public int IdBD { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int IdHotel { get; set; }
        public Hotel Hotel { get; set; }

        public int NumberOfPeople { get; set; }

        public int BaseCost { get; set; }

        public int ImposedCost { get; set; }

        public string TypeOfRoom { get; set; }

        public string Location { get; set; }

        public string Observation { get; set; }

        public bool IsEnabled { get; set; }
    }
}
