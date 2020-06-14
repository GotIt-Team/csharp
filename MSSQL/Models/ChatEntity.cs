using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.MSSQL.Models
{
    public class ChatEntity
    {
        public ChatEntity()
        {
            Messages = new HashSet<MessageEntity>();
            Users = new HashSet<UserChatEntity>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public virtual ICollection<MessageEntity> Messages { get; set; }
        public virtual ICollection<UserChatEntity> Users { get; set; }
    }
}
