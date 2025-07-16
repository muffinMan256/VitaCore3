using System.ComponentModel.DataAnnotations;

namespace VitaCore.Models.ViewModel
{
    public class PatientProfileModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime Birthday { get; set; }

        [Required]
        [Range(0, 130)]
        public int Age { get; set; }

        [Required]
        public string Cnp { get; set; }

        public string AddressStreet { get; set; }
        public string AddressCity { get; set; }
        public string AddressCounty { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public string Email { get; set; }
        public string Occupation { get; set; }
        public string Workplace { get; set; }
    }

}
