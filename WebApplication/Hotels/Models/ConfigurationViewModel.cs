using Hotels.Domain;

namespace Hotels.Models
{
    public class ConfigurationViewModel : BaseModel<Configuration>
    {
        public List<Configuration> Items { get; set; }
        public Dictionary<string,string> parameterItems { get; set; }
    }
}
