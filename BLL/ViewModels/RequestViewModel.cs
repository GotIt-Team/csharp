using GotIt.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.BLL.ViewModels
{
    public class RequestViewModel
    {
        public int Id { get; set; }
        public DateTime SendDate { get; set; }
        public DateTime? ReplyDate { get; set; }
        public string ReplyMessage { get; set; }
        public ERequestState State { get; set; }
        public ItemViewModel Item { get; set; }
        public UserViewModel User { get; set; }
    }
}
