using GotIt.Common.Enums;

namespace GotIt.BLL.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Token { get; set; }
        public EUserType Type { get; set; }
    }
}