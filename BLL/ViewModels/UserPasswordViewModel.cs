using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.BLL.ViewModels
{
    public class UserPasswordViewModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string RepeatedNewPassword { get; set; }
    }
}
