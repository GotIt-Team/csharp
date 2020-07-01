using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.BLL.ViewModels
{
    public class KnownViewModel
    {
        public List<string> Images { get; set; }
        public List<int[]> Boxes { get; set; }
        public string Embeddings { get; set; }
    }
}
