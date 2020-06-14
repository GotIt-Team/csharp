using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.BLL.ViewModels
{
    public class ChatViewModel
    {
        public int Id { get; set; }
        public bool IsOnline { get; set; }
        public UserViewModel User { get; set; }
        public MessageViewModel LastMessage { get; set; }
    }
}
