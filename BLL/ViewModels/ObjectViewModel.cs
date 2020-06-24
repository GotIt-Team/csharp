using GotIt.Common.Enums;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GotIt.BLL.ViewModels
{
    public class ObjectViewModel
    {
        public int Id { get; set; }

        [JsonPropertyName("objectClass")]
        public string Class { get; set; }
        public string Colors { get; set; }
        public Dictionary<EObjectAttribute, string> Attributes { get; set; }
    }
}