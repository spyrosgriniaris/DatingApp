using System;

namespace DatingApp.API.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public User Sender { get; set; }
        public int RecipientId { get; set; }
        public User Recipient { get; set; }
        public string Content { get; set; }
        public bool isRead { get; set; }
        public DateTime? DateRead { get; set; } // to thelw null an to message einai unread
        public DateTime MessageSent { get; set; }
        public bool SenderDeleted { get; set; } // an enas apo tous 2 svisei to mnm, o allos tha mporei akoma na to dei
        public bool RecipientDeleted { get; set; }
    }
}