using GotIt.Common.Enums;
using System;

namespace GotIt.BLL.ViewModels
{
    public class MessageViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public EContentType Type { get; set; }
        public DateTime Time { get; set; }
        public int SenderId { get; set; }
        public UserViewModel Sender { get; set; }
    }
}