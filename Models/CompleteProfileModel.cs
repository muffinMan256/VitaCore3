using System.ComponentModel.DataAnnotations;

namespace VitaCore.Models
{
    public class CompleteProfileModel
    {
        //[Required(ErrorMessage = "First name is required.")]
        //[StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        //[RegularExpression(@"^[A-Za-z\s\-']+$", ErrorMessage = "First name contains invalid characters.")]
        public string FirstName { get; set; }

        //[Required(ErrorMessage = "Last name is required.")]
        //[StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters.")]
        //[RegularExpression(@"^[A-Za-z\s\-']+$", ErrorMessage = "Last name contains invalid characters.")]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "Birthday is required.")]
        //[DataType(DataType.Date)]
        //[Display(Name = "Date of Birth")]
        //[CustomValidation(typeof(CompleteProfileModel), nameof(ValidateBirthday))]


        [Required]
        [DataType(DataType.Date)]
        [Display(Name = @"Data Primire", Prompt = @"Data Primire")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyy}", ApplyFormatInEditMode = true)]
        public DateTime Birthday { get; set; }

        public static ValidationResult? ValidateBirthday(DateTime birthday, ValidationContext context)
        {
            if (birthday > DateTime.Today)
                return new ValidationResult("Birthday cannot be in the future.");
            if (birthday < DateTime.Today.AddYears(-120))
                return new ValidationResult("Please enter a valid age.");
            return ValidationResult.Success;
        }
    }
}