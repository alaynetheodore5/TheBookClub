# TheBookClub

This is a social distancing book club for users to collaborate in a book club and discuss books. Feel free to suggest modifications to the club or even add new features!

## Features

<img src="https://media0.giphy.com/media/IhxxlsU4EmOSHyLUhu/giphy.gif" alt="demo gif">

Users can register or login to TheBookClub.

<img src="https://media1.giphy.com/media/Vdin358oUYbQei9yV4/giphy.gif" alt="demo login registration">

View your dashboard, add books, search or view for books and check out the discussion boards for each specific book. 

## Example Code

```
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
```

### Technologies Used

- C#/.Net
- SQL
- CSS/HTML
- Bootstrap
