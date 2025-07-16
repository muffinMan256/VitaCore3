using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VitaCore.Models
{
    public class MessageModel
    {
        [Key]
        public int id { get; set; }

        [Required]
        public int sender_id { get; set; }

        [Required]
        public int receiver_id { get; set; }

        [Required]
        public string message { get; set; }

        public DateTime? sent_at { get; set; }

        public bool? is_read { get; set; }
    }
}
