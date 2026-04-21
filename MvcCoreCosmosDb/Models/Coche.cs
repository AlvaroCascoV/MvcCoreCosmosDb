using Newtonsoft.Json;

namespace MvcCoreCosmosDb.Models
{
    public class Coche
    {
        //hay que indicar explicitamente que el id es el id
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Imagen { get; set; }
        public Motor Motor { get; set; }
    }
}
