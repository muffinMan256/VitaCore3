using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VitaCore.Models
{
    public class ChartDataModel
    {
        [Key]
        public int id { get; set; }

        [Required]
        public int patient_id { get; set; }

        [StringLength(50)]
        public string? char_type { get; set; }

        [StringLength(225)]
        public string? data_label { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? value { get; set; }

        public DateTime? recorded_at { get; set; }
    }
}
