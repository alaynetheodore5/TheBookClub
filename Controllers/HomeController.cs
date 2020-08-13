using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheBookClub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace TheBookClub.Controllers
{
    public class HomeController : Controller
    {

        private MyContext _context { get; set; }
        private PasswordHasher<User> regHasher = new PasswordHasher<User>();
        private PasswordHasher<LoginUser> logHasher = new PasswordHasher<LoginUser>();

        public  User GetUser()
        {
            return _context.Users.FirstOrDefault( u =>  u.UserId == HttpContext.Session.GetInt32("userId"));
        }

        public HomeController(MyContext context)
        {
            _context = context; 
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("loginindex")]
        public IActionResult LoginIndex()
        {
            return View("_Login");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser lu)
        {
            if(ModelState.IsValid)
            {
                User userInDB = _context.Users.FirstOrDefault(u => u.Email == lu.LoginEmail);
                if(userInDB == null)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Email or Password");
                    return View("_Login");
                }
                var result = logHasher.VerifyHashedPassword(lu, userInDB.Password, lu.LoginPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("LoginPassword", "Invalid Email or Password");
                    return View("_Login");
                }
                HttpContext.Session.SetInt32("userId", userInDB.UserId);
                return Redirect("/home");
            }
            return View("_Login");
        }

        [HttpGet("registerindex")]
        public IActionResult RegisterIndex()
        {
            return View("_Register");
        }

        [HttpPost("register")]
        public IActionResult Register(User u)
        {
            if(ModelState.IsValid)
            {
                if(_context.Users.FirstOrDefault(usr => usr.Email == u.Email) !=null)
                {
                    ModelState.AddModelError("Email", "Email is already in use, try logging in!");
                    return View("_Register");
                }
                string hash = regHasher.HashPassword(u, u.Password);
                u.Password = hash;
                _context.Users.Add(u);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("userId", u.UserId);
                return Redirect("/home");
            }
            return View("_Register");
        }

        [HttpGet("home")]
        public IActionResult Home()
        {
            User current = GetUser();
            if (current == null)
            {
                return Redirect ("/");
            }
            ViewBag.User = current;
            List<Book> Trending = _context.Books
                                            .Include(b => b.Adder)
                                            .Include(b => b.Members)
                                            .ThenInclude(wp => wp.ClubGoer)
                                            .OrderBy( b => b.CreatedAt )
                                            .Take(2)
                                            .ToList();
            ViewBag.MyFaves = _context.Users
                                .Include(u =>u.MyClubs )
                                .ThenInclude( bc => bc.ThisBook)
                                .FirstOrDefault(u => u.UserId == current.UserId)
                                .MyClubs.Select( bc => bc.ThisBook)
                                .ToList();
            return View("Home", Trending);
        }

        [HttpPost("search")]
        public IActionResult Search(string q)
        {
            User current = GetUser();
            if (current == null)
            {
                return Redirect ("/");
            }
            ViewBag.User = current;
            List<Book> SearchResults = _context.Books
                                    .Include(b => b.Adder)
                                    .Include(b => b.Members)
                                    .ThenInclude(wp => wp.ClubGoer)
                                    .Where(
                                        b => b.Title.Contains(q) || 
                                        b.Author.Contains(q)
                                    )
                                    .ToList();
            ViewBag.MyFaves = _context.Users
                                .Include(u =>u.MyClubs )
                                .ThenInclude( bc => bc.ThisBook)
                                .FirstOrDefault(u => u.UserId == current.UserId)
                                .MyClubs.Select( bc => bc.ThisBook);
            return View("Home", SearchResults);
        }

        [HttpGet("adminhome")]
        public IActionResult AdminHome()
        {
            User current = GetUser();
            if (current == null)
            {
                return Redirect ("/");
            }
            ViewBag.User = current;
            List<Book> AllBooks = _context.Books
                                            .Include(b => b.Adder)
                                            .Include(b => b.Members)
                                            .ThenInclude(wp => wp.ClubGoer)
                                            .OrderBy( b => b.Title )
                                            .ToList();            
            return View("AdminHome", AllBooks);
        }

        [HttpGet("book/{bookId}/{status}")]
        public IActionResult ToggleParty(int bookId, string status)
        {
            User current = GetUser();
            if (current == null)
            {
                return Redirect ("/");
            }
            if(status == "add")
            {
                BookClub newClub = new BookClub();
                newClub.UserId = current.UserId;
                newClub.BookId = bookId;
                _context.BookClubs.Add(newClub);
            }
            else if(status == "remove")
            {
                BookClub backout = _context.BookClubs.FirstOrDefault( w => w.UserId == current.UserId && w.BookId == bookId );
                _context.BookClubs.Remove(backout);
            }
            _context.SaveChanges();
            return RedirectToAction("Home");
        }

        [HttpGet("book/{bookId}")]
        public IActionResult ShowBook(int bookId)
        {
            User current = GetUser();
            if (current == null)
            {
                return Redirect ("/");
            }
            ViewBag.User = current;
            Book thisbook = _context.Books
                                    .Include( b => b.Members )
                                    .ThenInclude( w => w.ClubGoer )
                                    .Include( b => b.Adder )
                                    .FirstOrDefault( b => b.BookId == bookId );
            ViewBag.Books = thisbook;
            List<Message> Messages = _context.Messages
                                    .Include(m => m.Creator)
                                    .Include(m => m.Comments)
                                    .ThenInclude(c => c.Writer)
                                    .Where(m => m.BookId == bookId)
                                    .OrderByDescending(m => m.CreatedAt)
                                    .ToList();
            ViewBag.Messages = Messages;
            List<Comment> Comments = _context.Comments
                                    .Include(m => m.Maker)
                                    .OrderBy(m => m.CreatedAt)
                                    .ToList();
            ViewBag.Comments = Comments;
            return View("ShowBook");
        }


        [HttpPost("book/{bookId}/message")]
        public IActionResult Message(Message newMessage, int bookId)
        {
            User current = GetUser();
            if (current == null)
            {
                return Redirect ("/");
            }
            ViewBag.User = current;
            Book thisbook = _context.Books
                                    .Include( b => b.Members )
                                    .ThenInclude( w => w.ClubGoer )
                                    .Include( b => b.Adder )
                                    .FirstOrDefault( b => b.BookId == bookId );
            ViewBag.Books = thisbook;
            newMessage.UserId = current.UserId;
            newMessage.BookId = bookId;
            _context.Messages.Add(newMessage);
            _context.SaveChanges();
            List<Message> Messages = _context.Messages
                            .Include(m => m.Creator)
                            .Include(m => m.Comments)
                            .ThenInclude(c => c.Writer)
                            .OrderBy(m => m.CreatedAt)
                            .ToList();
            ViewBag.Messages = Messages;
            return Redirect($"/book/{bookId}");
        }

        [HttpGet("book/{bookId}/message/{messageId}/delete")]
        public IActionResult DeleteMessage(int messageId, int bookId)
        {
            User current = GetUser();
            if (current == null)
            {
                return Redirect ("/");
            }
            ViewBag.User = current;
            Book thisbook = _context.Books
                                    .Include( b => b.Members )
                                    .ThenInclude( w => w.ClubGoer )
                                    .Include( b => b.Adder )
                                    .FirstOrDefault( b => b.BookId == bookId );
            ViewBag.Books = thisbook;
            Message todelete = _context.Messages
                            .FirstOrDefault(m => m.MessageId == messageId);
            _context.Messages.Remove(todelete);
            _context.SaveChanges();
            return Redirect($"/book/{bookId}");
        }

        [HttpPost("book/{bookId}/{messageId}/comment")]
        public IActionResult Comment(Comment newComment, int commentId, int messageId, int bookId)
        {
            User current = GetUser();
            if (current == null)
            {
                return Redirect ("/");
            }
            ViewBag.User = current;            
            newComment.UserId = current.UserId;
            _context.Comments.Add(newComment);
            _context.SaveChanges();
            List<Comment> Comments = _context.Comments
                                    .Include(m => m.Maker)
                                    .OrderBy(m => m.CreatedAt)
                                    .ToList();
            ViewBag.Comments = Comments;
            Book thisbook = _context.Books
                                    .Include( b => b.Members )
                                    .ThenInclude( w => w.ClubGoer )
                                    .Include( b => b.Adder )
                                    .FirstOrDefault( b => b.BookId == bookId );
            ViewBag.Books = thisbook;
            return Redirect($"/book/{bookId}");
        }

        [HttpGet("book/{bookId}/comment/{commentId}/delete")]
        public IActionResult DeleteComment(int commentId, int bookId)
        {
                        User current = GetUser();
            if (current == null)
            {
                return Redirect ("/");
            }
            ViewBag.User = current;  
            Book thisbook = _context.Books
                                    .Include( b => b.Members )
                                    .ThenInclude( w => w.ClubGoer )
                                    .Include( b => b.Adder )
                                    .FirstOrDefault( b => b.BookId == bookId );
            ViewBag.Books = thisbook;
            Comment todelete = _context.Comments
                                .FirstOrDefault(m => m.CommentId == commentId);
            _context.Comments.Remove(todelete);
            _context.SaveChanges();
            return Redirect($"/book/{bookId}");
        }

        [HttpGet("books/all")]
        public IActionResult ShowAll(string q)
        {
            User current = GetUser();
            if (current == null)
            {
                return Redirect ("/");
            }
            ViewBag.User = current;
            List<Book> AllBooks = new List<Book>();
            if (q != null)
            {
            AllBooks = _context.Books
                            .Include(b => b.Adder)
                            .Include(b => b.Members)
                            .ThenInclude(wp => wp.ClubGoer)
                            .Where(
                                    b => b.Title.Contains(q) || 
                                    b.Author.Contains(q)
                            )
                            .OrderBy( b => b.Title )
                            .ToList();
            }
            else
            {
            AllBooks = _context.Books
                            .Include(b => b.Adder)
                            .Include(b => b.Members)
                            .ThenInclude(wp => wp.ClubGoer)
                            .OrderBy( b => b.Title )
                            .ToList();
            }
            return View(AllBooks);
        }

        [HttpGet("newbook")]
        public IActionResult NewBook()
        {
            User current = GetUser();
            if (current == null)
            {
                return Redirect ("/");
            }
            return View("NewBook");
        }

        [HttpPost("newbook/add")]
        public IActionResult AddBook(Book newBook)
        {
            User current = GetUser();
            if (current == null)
            {
                return Redirect ("/");
            }
            if(ModelState.IsValid)
            {
                newBook.UserId = current.UserId;
                _context.Books.Add(newBook);
                _context.SaveChanges();
                return RedirectToAction("Home");
            }
            return View("NewBook");
        }

        [HttpGet("book/{bookId}/delete")]
        public IActionResult DeleteBook(int bookId)
        {
            User current = GetUser();
            if (current == null)
            {
                return Redirect ("/");
            }
            Book remove = _context.Books.FirstOrDefault( b => b.BookId == bookId );
            _context.Books.Remove(remove);
            _context.SaveChanges();
            return RedirectToAction("AdminHome");
        }



        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }
    }
}
