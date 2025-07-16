using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VitaCore.Models
{
    [Table("PhysicalActivities")]
    public class PhysicalActivityModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("patient_id")]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public PatientModel Patient { get; set; }

        [Required]
        [Column("activity_type")]
        public ActivityType ActivityType { get; set; }

        [Column("start_time")]
        public DateTime? StartTime { get; set; }

        [Column("end_time")]
        public DateTime? EndTime { get; set; }

        [Column("duration")]
        public int? Duration { get; set; }
    }
}