using GotIt.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.MSSQL.Models
{
    public class NotificationEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public string Link { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public bool IsSeen { get; set; }
        [Required]
        public ENotificationType Type { get; set; }

        [ForeignKey("Sender")]
        public int SenderId { get; set; }
        public virtual UserEntity Sender { get; set; }

        [ForeignKey("Receiver")]
        public int ReceiverId { get; set; }
        public virtual UserEntity Receiver { get; set; }
    }
}
