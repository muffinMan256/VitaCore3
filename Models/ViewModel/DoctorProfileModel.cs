using System.ComponentModel.DataAnnotations;

namespace VitaCore.Models.ViewModel
{
    public class DoctorProfileModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime Birthday { get; set; }

        [Required]
        public string HonorificTitle { get; set; }

        [Required]
        public string Gender { get; set; }

        public string Bio { get; set; }

        [Required]
        public string AvailabilityHours { get; set; }

        [Required]
        public string ClinicAddress { get; set; }

        [Required]
        public string Specialization { get; set; }

        [Required]
        public bool IsFavorite { get; set; }
    }


}
