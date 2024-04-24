using System.ComponentModel.DataAnnotations;

namespace Hotels.Domain
{
    public class Reserve
    {
        [Key]
        public int IdReserve { get; set; }

        public string Code { get; set; }

        public int RoomIdBD { get; set; }

        public Room Room { get; set; }

        public DateTime DateOfAdmission { get; set; }

        public DateTime DateOfExit { get; set; }

        public DateTime DateOfCreation { get; set; }

        public string FullName { get; set; }

        public DateTime Birthdate { get; set; }

        public string Gender { get; set; }

        public string DocumentType { get; set; }

        public string DocumentNumber { get; set; }

        public string Email { get; set; }

        public string Telephone { get; set; }

        public string? EmergencyFullName { get; set; }

        public string? EmergencyTelephone { get; set; }

        public string? Observation { get; set; }

        public void ValidateReserve()
        {
            this.Observation = this.Observation ?? string.Empty;
            this.Telephone = this.Telephone ?? string.Empty;
            this.Code = this.Code ?? string.Empty;

            if(this.RoomIdBD <= 0)
            {
                throw new Exception($"Habitacion no valida");
            }

            if (string.IsNullOrEmpty(this.FullName))
            {
                throw new Exception($"Nombre no valido");
            }

            if (string.IsNullOrEmpty(this.Gender))
            {
                throw new Exception($"Genero no valido, valores aceptados: M,F,masculino,femenino u otro");
            }
            else
            {
                var gende = this.Gender.ToLower();
                if (gende != "m" && gende != "f" && gende != "masculino" && gende != "femenino" && gende != "otro")
                {
                    throw new Exception($"Genero no valido, valores aceptados: M,F,masculino,femenino u otro");
                }
            }

            if (string.IsNullOrEmpty(this.DocumentType))
            {
                throw new Exception($"Tipo Documento no valido");
            }

            if (string.IsNullOrEmpty(this.DocumentNumber))
            {
                throw new Exception($"Numero Documento no valido");
            }

            if (string.IsNullOrEmpty(this.Email))
            {
                throw new Exception($"Correo no valido");
            }

        }
    }
}
