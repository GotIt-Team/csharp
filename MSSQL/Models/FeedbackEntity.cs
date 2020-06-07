using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.MSSQL.Models
{
    public class FeedbackEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public int Rate { get; set; }
        public string Opinion { get; set; }
        [ForeignKey("User")]
        public int? UserId { get; set; }
        public virtual UserEntity User { get; set; }
    }
}
