using GotIt.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.MSSQL.Models
{
    public class ObjectAttributeEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public EObjectAttribute Key { get; set; }
        [Required]
        public string Value { get; set; }

        [ForeignKey("Object")]
        public int ObjectId { get; set; }
        public virtual ObjectEntity Object { get; set; }
    }
}
