using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.MSSQL.Models
{
    public class ObjectEntity
    {
        public ObjectEntity()
        {
            Attributes = new HashSet<ObjectAttributeEntity>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public string Class { get; set; }
        [Required]
        public string Colors { get; set; }
        [Required]
        public string Image { get; set; }

        [ForeignKey("Item")]
        public int ItemId { get; set; }
        public virtual ItemEntity Item { get; set; }

        public virtual ICollection<ObjectAttributeEntity> Attributes { get; set; }
    }
}
