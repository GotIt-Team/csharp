using GotIt.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.MSSQL.Models
{
    public class PersonEntity
    {
        public PersonEntity()
        {
            Images = new HashSet<PersonImageEntity>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(32)]
        public string Name{ get; set; }
        [Required]
        public EAgeStage AgeStage { get; set; }
        [Required]
        public EGender Gender { get; set; }
        [Required]
        public string Embeddings { get; set; }

        [ForeignKey("Item")]
        public int ItemId { get; set; }
        public virtual ItemEntity Item { get; set; }

        public virtual ICollection<PersonImageEntity> Images { get; set; }
    }
}
