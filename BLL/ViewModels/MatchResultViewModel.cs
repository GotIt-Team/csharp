using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.BLL.ViewModels
{
    public class MatchResultViewModel
    {
        public List<ScoreViewModel> Score { get; set; }
        public string Embeddings { get; set; }
    }

    public class ScoreViewModel
    {
        public int ItemId { get; set; }
        public double Score { get; set; }
    }
}
