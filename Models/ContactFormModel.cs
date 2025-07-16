using System.ComponentModel.DataAnnotations;

namespace VitaCore.Models
{
    public class ContactFormModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 10)]
        public string Message { get; set; }

        [Display(Name = "Urgency Level")]
        public string UrgencyLevel { get; set; } = "Routine Inquiry";
    }
}
