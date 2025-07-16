using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using VitaCore.Data;
using Xunit;

namespace VitaCore.Models
{
    public class PatientModel
    {
        public int id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Range(0, 120, ErrorMessage = "Age must be between 0 and 120.")]
        public int age { get; set; }

        [StringLength(13, MinimumLength = 13, ErrorMessage = "CNP must be exactly 13 characters.")]
        [RegularExpression("^[0-9]{13}$", ErrorMessage = "CNP must be numeric and 13 digits.")]
        public string? cnp { get; set; }

        [Required(ErrorMessage = "Street address is mandatory!")]
        [StringLength(100)]
        public string address_street { get; set; }

        [Required(ErrorMessage = "City is mandatory!")]
        [StringLength(50)]
        public string address_city { get; set; }

        [Required(ErrorMessage = "County is mandatory!")]
        [StringLength(50)]
        public string address_county { get; set; }

        [Required(ErrorMessage = "Phone number is mandatory!")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Phone number must contain digits only.")]
        public string phone_number { get; set; }

        [Required(ErrorMessage = "Email is mandatory!")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string email { get; set; }

        [Required(ErrorMessage = "Occupation is mandatory!")]
        [StringLength(100)]
        public string occupation { get; set; }

        [Required(ErrorMessage = "Workplace is mandatory!")]
        [StringLength(100)]
        public string workplace { get; set; }

        // Navigation properties
        [ValidateNever]
        public AppUser User { get; set; }

        [ValidateNever]
        public ICollection<MedicalHistoryModel> MedicalHistories { get; set; }

        [ValidateNever]
        public ICollection<RecommendationModel> Recommendations { get; set; }

        [ValidateNever]
        public ICollection<AlarmModel> Alarms { get; set; }

        [ValidateNever]
        public ICollection<PhysicalActivityModel> PhysicalActivities { get; set; }

        [ValidateNever]
        public ICollection<EcgSignalModel> EcgSignals { get; set; }

        [ValidateNever]
        public ICollection<SensorDataModel> SensorDatas { get; set; }

        [ValidateNever]
        public ICollection<LocationMapModel> LocationMaps { get; set; }
    }
}
