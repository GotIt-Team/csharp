using GotIt.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace GotIt.MSSQL.Models
{
    public class ItemEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public  bool IsLost { get; set; }
        public string Content { get; set; }
        [Required]
        public EItemType Type { get; set; }
        public DateTime? MatchDate { get; set; }
        [ForeignKey("MatchedItem")]     
        public int? MatchedItemId { get; set; }
        public virtual ItemEntity MatchedItem { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual UserEntity User { get; set; }

        public virtual PersonEntity Person { get; set; }
        public virtual ObjectEntity _Object { get; set; }
        public virtual ItemEntity InverseMatched { get; set; }

        public  virtual ICollection<ProbablyMatchEntity> InverseProbablyMatched { get; set; }
        public virtual ICollection<ProbablyMatchEntity> ProbablyMatched { get; set; }
        public virtual ICollection<RequestEntity> Requests { get; set; }
        public virtual ICollection<CommentEntity> Comments { get; set; }





    }

}
