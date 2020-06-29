using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.BLL.ViewModels
{
    public class MatchRequestViewModel
    {
        public KnownViewModel Known { get; set; }
        public List<CandidateViewModel> Candidates { get; set; }
    }
}
