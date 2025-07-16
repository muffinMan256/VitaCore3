using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VitaCore.Models
{
    [Table("SensorData")]
    public class SensorDataModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("patient_id")]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public PatientModel Patient { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Column("sensor_type")]
        public SensorType SensorType { get; set; }

        [Column("value")]
        public decimal Value { get; set; }
    }
}