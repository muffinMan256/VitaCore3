using System.ComponentModel.DataAnnotations;

namespace VitaCore.Models
{
    public class UserFavoritesModel 
    {
        [Key]
        public int id { get; set; }

        [Required]
        public int user_id { get; set; }

        [Required]
        public int doctor_id { get; set; }
    }
}
