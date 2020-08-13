using System.ComponentModel.DataAnnotations;

namespace TheBookClub.Models
{
    public class BookClub
    {
        [Key]
        public int BookClubId { get; set; }

        public int UserId  {get; set;}
        public int BookId { get; set; }

        public User ClubGoer { get; set;}
        public Book ThisBook { get; set; }

    }
}