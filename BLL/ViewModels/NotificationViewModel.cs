using GotIt.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GotIt.BLL.ViewModels
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public string Link { get; set; }
        public string Content { get; set; }
        public bool IsSeen { get; set; }
        public ENotificationType type { get; set; }
        public DateTime? Date { get; set; }
        public UserViewModel Sender { get; set; }
    }
}
