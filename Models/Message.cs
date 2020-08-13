using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheBookClub.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        [Required(ErrorMessage="Message is required")]
        [MinLength(2)]
        public string Content { get; set; }

        public int UserId { get; set; }

        public User Creator { get; set; }

        public int BookId { get; set; }

        public Book BookMessage { get; set; }
        
        public List<Comment> Comments { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}