namespace VitaCore.Models.ViewModel
{
    public class MessageViewModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime? SentAt { get; set; }
        public bool? IsRead { get; set; }

        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
    }
}
