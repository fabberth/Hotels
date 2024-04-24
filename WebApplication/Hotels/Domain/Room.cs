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

        public void ValidateRoom()
        {
            this.Observation = this.Observation ?? string.Empty;
            if (string.IsNullOrEmpty(this.Name))
            {
                throw new Exception($"Nombre no valido");
            }

            if (string.IsNullOrEmpty(this.Code))
            {
                throw new Exception($"Codigo no valido");
            }

            if (string.IsNullOrEmpty(this.TypeOfRoom))
            {
                throw new Exception($"Tipo de habitacion no valido");
            }

            if (string.IsNullOrEmpty(this.Location))
            {
                throw new Exception($"Ubicacion no valido");
            }

        }
    }
}
