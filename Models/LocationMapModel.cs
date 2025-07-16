using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VitaCore.Models
{
    [Table("LocationMap")]
    public class LocationMapModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("patient_id")]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public PatientModel Patient { get; set; }

        [Column("latitude", TypeName = "decimal(9,6)")]
        public decimal? Latitude { get; set; }

        [Column("longitude", TypeName = "decimal(9,6)")]
        public decimal? Longitude { get; set; }

        [Column("recorded_at")]
        public DateTime? RecordedAt { get; set; }
    }
}