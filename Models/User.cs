using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheBookClub.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage="Name is required")]
        [MinLength(2)]
        public string Name { get; set; }

        [Required(ErrorMessage="Email is required")]
        [EmailAddress(ErrorMessage="Please enter a valid Email address")]
        public string Email { get; set; }

        [SecurePassword]
        [Required(ErrorMessage="Password is required")]
        [MinLength(8, ErrorMessage="Password must be 8 characters or longer")]
        [DataType("Password")]
        public string Password { get; set; }

        [Required(ErrorMessage="Confirming a Password is required")]
        [Compare("Password")]
        [DataType("Password")]
        [NotMapped]
        public string Confirm { get; set; }

        public bool UserAdmin {get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public List<Book> MyBooks { get; set; }
        public List<BookClub> MyClubs { get; set; }
    }
}