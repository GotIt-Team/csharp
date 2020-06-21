using GotIt.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.Common.Helper
{
    public class RequestAttributes
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public EUserType Type { get; set; }
        public string AppBaseUrl { get; set; }

        public void CopyFrom(RequestAttributes requestAttributes)
        {
            Id = requestAttributes.Id;
            Name = requestAttributes.Name;
            Email = requestAttributes.Email;
            Type = requestAttributes.Type;
        }
    }
}
