using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Hotels.Domain
{
    public class Hotel
    {
        public int IdHotel { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public List<Room> Rooms { get; set; }

        public bool IsEnabled { get; set; }

        public void ValidateHotel()
        {
            if(string.IsNullOrEmpty(this.Name) )
            {
                throw new Exception($"Nombre no valido");
            }

            if (string.IsNullOrEmpty(this.Code))
            {
                throw new Exception($"Codigo no valido");
            }

            if (int.TryParse(this.Code, out int num))
            {
                throw new Exception($"Codigo no puede ser solo numeros");
            }

        }

    }
}
