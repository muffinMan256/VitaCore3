using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VitaCore.Models
{
    [Table("Recommendation")]
    public class RecommendationModel
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
        [Column("recommendation_type")]
        public RecommendationType RecommendationType { get; set; }

        [Column("daily_duration")]
        public int? DailyDuration { get; set; }

        [Column("additional_instructions")]
        public string? AdditionalInstructions { get; set; }
    }
}