using System;

namespace GotIt.BLL.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }
        public UserViewModel User { get; set; }
    }
}