using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheBookClub.Models
{
    public class Book
    {
        [Key]
        public int BookId { get;set; }


        [Required(ErrorMessage="Book Title is required")]
        [MinLength(2,ErrorMessage="Book Title must be at least 2 characters")]
        public string Title { get;set; }

        [Required(ErrorMessage="Author is required")]
        [MinLength(2,ErrorMessage="Author must be at least 2 characters")]
        public string Author { get;set; }

        [Required(ErrorMessage="Description is required")]
        [MinLength(10,ErrorMessage="Description must be at least 10 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage="Image Url is required")]
        public string ImgUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // This is the foreign key
        public int UserId { get; set; }

        // An activity can have only one user that adds it.
        public User Adder { get; set; }

        // many to many
        public List<BookClub> Members { get; set; }

        public List<Message> BookConvo { get; set; }

    }
}