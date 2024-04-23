using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Hotels.Domain
{
    public class AppUser
    {
        [Key]
        public int IdBD { get; set; }
        public string Id { get; set; }
        public string Names { get; set; }
        public string? LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public void ValidateUser()
        {
            this.Names = this.Names ?? "";
            this.LastName = this.LastName ?? "";
            this.Id = this.Id ?? "";
            this.UserName = this.UserName ?? "";
            this.Email = this.Email ?? "";

            string patronNombres = "^([a-zA-Z]+)(\\s?[a-zA-Z]+)+[a-zA-Z]$";
            if (!Regex.IsMatch(this.Names, patronNombres))
            {
                throw new Exception($"Los nombres no son validos");
            }

            /*if (!Regex.IsMatch(this.LastName, patronNombres))
            {
                throw new Exception($"Los apellidos no son validos");
            }*/

            if (!Regex.IsMatch(this.Id, "^[0-9]+$"))
            {
                throw new Exception($"Identificacion no valida");
            }

            if (!Regex.IsMatch(this.UserName, "^([a-zA-Z])+\\.([a-zA-Z])+$"))
            {
                throw new Exception($"Username no valido");
            }

            if (!Regex.IsMatch(this.Email, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$"))
            {
                throw new Exception($"Correo no valido");
            }
        }
    }
}
