using GotIt.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.MSSQL.Models
{
    public class UserEntity
    {
        public UserEntity()
        {
            Items = new HashSet<ItemEntity>();
            Requests = new HashSet<RequestEntity>();
            Comments = new HashSet<CommentEntity>();
            Feedbacks = new HashSet<FeedbackEntity>();
            Messages = new HashSet<MessageEntity>();
            SentNotifications = new HashSet<NotificationEntity>();
            ReceivedNotifications = new HashSet<NotificationEntity>();
            Chats = new HashSet<UserChatEntity>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(32)]
        public string Name { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string HashPassword { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Picture { get; set; }
        [Required]
        public EGender Gender { get; set; }
        public bool IsConfirmed { get; set; }
        [Required]
        public EUserType Type { get; set; }

        public virtual ICollection<ItemEntity> Items { get; set; }
        public virtual ICollection<RequestEntity> Requests { get; set; }
        public virtual ICollection<CommentEntity> Comments { get; set; }
        public virtual ICollection<FeedbackEntity> Feedbacks { get; set; }
        public virtual ICollection<MessageEntity> Messages { get; set; }
        public virtual ICollection<NotificationEntity> SentNotifications { get; set; }
        public virtual ICollection<NotificationEntity> ReceivedNotifications { get; set; }
        public virtual ICollection<UserChatEntity> Chats { get; set; }
    }
}
