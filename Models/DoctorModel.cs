using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using VitaCore.Data;

namespace VitaCore.Models
{
    public class DoctorModel
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string UserId { get; set; }

        [StringLength(20)]
        public string? honorific_title { get; set; }

        [StringLength(10)]
        public string? gender { get; set; }

        public bool? is_favorite { get; set; }

        public string? bio { get; set; }

        [StringLength(255)]
        public string? availability_hours { get; set; }

        [StringLength(255)]
        public string? clinic_address { get; set; }

        public string? profile_picture_base64 { get; set; }

        [Required]
        [StringLength(255)]
        public string Specialization { get; set; }

        public AppUser User { get; set; }


        [NotMapped]
        public string FullName => $"{User?.FirstName} {User?.LastName}".Trim();

        [NotMapped]
        public string LastName => User?.LastName ?? string.Empty;

    }
}