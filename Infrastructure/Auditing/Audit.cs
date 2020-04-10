using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exelor.Infrastructure.Auditing
{
    public class Audit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; }  
        public string UserName { get; set; }  
        public string Table { get; set; }
        public string KeyValues { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
    }
}