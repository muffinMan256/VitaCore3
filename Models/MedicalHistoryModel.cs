using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VitaCore.Models
{
    [Table("MedicalHistory")]
    public class MedicalHistoryModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("patient_id")]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public PatientModel Patient { get; set; }

        [Column("history")]
        public string? History { get; set; }

        [Column("allergies")]
        public string? Allergies { get; set; }

        [Column("cardiology_consultations")]
        public string? CardiologyConsultations { get; set; }
    }
}