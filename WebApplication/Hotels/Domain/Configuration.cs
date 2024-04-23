using System.ComponentModel.DataAnnotations;

namespace Hotels.Domain
{
    public class Configuration
    {
        [Key]
        public int IdBD { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Module { get; set; }
        public string Type { get; set; }
    }
}
