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
        public ItemEntity()
        {
            ProbablyMatched = new HashSet<ProbablyMatchEntity>();
            InverseProbablyMatched = new HashSet<ProbablyMatchEntity>();
            Requests = new HashSet<RequestEntity>();
            Comments = new HashSet<CommentEntity>();
        }

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
        public string Embeddings { get; set; }

        [ForeignKey("MatchedItem")]     
        public int? MatchedItemId { get; set; }
        public virtual ItemEntity MatchedItem { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual UserEntity User { get; set; }

        public virtual ItemEntity InverseMatched { get; set; }

        public virtual ICollection<ItemImageEntity> Images { get; set; }
        public virtual ICollection<ItemAttributeEntity> Attributes { get; set; }
        public virtual ICollection<ProbablyMatchEntity> ProbablyMatched { get; set; }
        public virtual ICollection<ProbablyMatchEntity> InverseProbablyMatched { get; set; }
        public virtual ICollection<RequestEntity> Requests { get; set; }
        public virtual ICollection<CommentEntity> Comments { get; set; }
    }
}
