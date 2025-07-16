using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VitaCore.Models
{
   
        public class AppointmentModel
        {
            [Key]
            public int Id { get; set; }

            [Required]
            public int PatientId { get; set; }

            [Required]
            public int DoctorId { get; set; }

            [Required]
            public DateTime StartTime { get; set; }

            public DateTime? EndTime { get; set; }

            public string? Notes { get; set; }

            // Navigation properties
            [ValidateNever]
            [ForeignKey("PatientId")]
            public PatientModel Patient { get; set; }

            [ValidateNever]
            [ForeignKey("DoctorId")]
            public DoctorModel Doctor { get; set; }
           
    }
}

