using GotIt.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.BLL.ViewModels
{
    public class ItemViewModel
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string Content { get; set; }
        public EItemType Type { get; set; }
        public string Image { get; set; }
        public UserViewModel User { get; set; }
    }
}
