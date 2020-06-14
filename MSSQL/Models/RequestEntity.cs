using GotIt.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.MSSQL.Models
{
    public class RequestEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime SendDate { get; set; }
        public DateTime? ReplyDate { get; set; }
        [Required]
        public ERequestState State { get; set; }
        public string ReplyMessage { get; set; }

        [ForeignKey("Item")]
        public int ItemId { get; set; }
        public virtual ItemEntity Item { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual UserEntity User { get; set; }
    }
}
