using System.ComponentModel.DataAnnotations;

namespace Hotels.Domain
{
    public class Reserve
    {
        [Key]
        public int IdBD { get; set; }

        public string Code { get; set; }

        public Room Room { get; set; }

        public DateTime DateOfAdmission { get; set; }

        public DateTime DateOfExit { get; set; }

        public DateTime DateOfCreation { get; set; }

        public string FullName { get; set; }

        public string Birthdate { get; set; }

        public string Gender { get; set; }

        public string DocumentType { get; set; }

        public string DocumentNumber { get; set; }

        public string Email { get; set; }

        public string Telephone { get; set; }

        public string EmergencyFullName { get; set; }

        public string EmergencyTelephone { get; set; }

        public string Observation { get; set; }
    }
}
