using GotIt.Common.Enums;
using System.Collections.Generic;

namespace GotIt.BLL.ViewModels
{
    public class ObjectViewModel
    {
        public int Id { get; set; }
        public string Class { get; set; }
        public string Colors { get; set; }
        public Dictionary<EObjectAttribute, string> Attributes { get; set; }
    }
}