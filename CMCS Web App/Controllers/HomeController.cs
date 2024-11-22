using CMCS_Web_App.Data;
using CMCS_Web_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace CMCS_Web_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        //-----------------------------------------------------------------------------------

        public HomeController(ILogger<HomeController> logger, AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        //-----------------------------------------------------------------------------------

        private User GetUserInSession()
        {
            int? userID = _httpContextAccessor.HttpContext.Session.GetInt32("UserID");

            if (userID.HasValue)
            {
                return _context.User.Find(userID.Value);
            }

            return null;
        }

        //-----------------------------------------------------------------------------------

        private bool IsUserLoggedIn()
        {
            int? userID = _httpContextAccessor.HttpContext.Session.GetInt32("UserID");

            return userID.HasValue;
        }

        //-----------------------------------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                _httpContextAccessor.HttpContext.Session.SetInt32("UserID", user.UserId);

                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Invalid login attempt.";
            return View();
        }

        //-----------------------------------------------------------------------------------

        public IActionResult Logout()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login");
            }

            _httpContextAccessor.HttpContext.Session.Remove("UserID");

            return RedirectToAction("Login", "Home");
        }

        //-----------------------------------------------------------------------------------

        public IActionResult Index()
        {
            return View();
        }

        //-----------------------------------------------------------------------------------

        public IActionResult Login()
        {
            return View();
        }

        //-----------------------------------------------------------------------------------

        public IActionResult Register()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        //-----------------------------------------------------------------------------------

        public IActionResult CreateClaim()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        //-----------------------------------------------------------------------------------

        public IActionResult Profile()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login");
            }

            return View(GetUserInSession());
        }

        //-----------------------------------------------------------------------------------

        // Actions for the ReviewClaim View

        public IActionResult ReviewClaim()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login");
            }

            // Null-coalescing Operator
            var claims = _context.UserClaim.Include(c => c.User).ToList() ?? new List<UserClaim>();

            return View(claims);
        }

        //-----------------------------------------------------------------------------------

        public IActionResult DownloadFile(int Id)
        {
            var claim = _context.UserClaim.FirstOrDefault(c => c.UserId == Id);

            if (claim == null || claim.FileData == null)
            {
                return NotFound();
            }

            //https://learn.microsoft.com/en-us/dotnet/api/system.web.mvc.filecontentresult?view=aspnet-mvc-5.2

            return File(claim.FileData, "application/octet-stream", claim.FileName);
        }

        //-----------------------------------------------------------------------------------

        public IActionResult ApproveClaim(int Id)
        {
            var claim = _context.UserClaim.FirstOrDefault(c => c.UserClaimId == Id);

            claim.ClaimStatus = "Approved";

            _context.SaveChanges();

            return RedirectToAction("ReviewClaim");
        }

        //-----------------------------------------------------------------------------------

        public IActionResult RejectClaim(int Id)
        {
            var claim = _context.UserClaim.FirstOrDefault(c => c.UserClaimId == Id);

            claim.ClaimStatus = "Rejected";

            _context.SaveChanges();

            return RedirectToAction("ReviewClaim");
        }

        //-----------------------------------------------------------------------------------

        public IActionResult TrackAllClaims()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login");
            }

            var claims = _context.UserClaim.Include(c => c.User).ToList();

            return View(claims);
        }

        //-----------------------------------------------------------------------------------

        public IActionResult RegisterUser(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);

                _context.SaveChanges();
            }
            else
            {
                TempData["RegisterFailed"] = "Incorrect details have been entered. Please ensure, all details have been entered correctly and you have filled in all the fields.";
                return View("Register");
            }


            return RedirectToAction("Login", "Home");
        }

        //-----------------------------------------------------------------------------------

        //https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-8.0

        [HttpPost]
        public async Task<IActionResult> SubmitNewClaim(UserClaim claim, IFormFile file)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login");
            }

            if (file != null && file.Length > 0)
            {
                var fileName = Path.GetFileName(file.FileName);

                // Check if the file type is valid.
                string[] fileTypes = { ".pdf", ".docx", ".png", ".jpg" };

                if (!fileTypes.Any(type => fileName.EndsWith(type, StringComparison.OrdinalIgnoreCase)))
                {
                    TempData["SubmitClaimFailed"] = "Please submit a file type of one of the following: .pdf / .docx / .png / .jpg";

                    return View("CreateClaim");
                }

                claim.FileName = fileName;

                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);

                    claim.FileData = memoryStream.ToArray();
                }
            }

            int? userID = _httpContextAccessor.HttpContext.Session.GetInt32("UserID");

            claim.UserId = userID.Value;

            claim.FlaggedClaim = false;

            claim.ClaimStatus = "Pending";

            try
            {
                _context.Add(claim);

                _context.SaveChanges();

                return RedirectToAction("ReviewClaim");
            }
            catch (Exception ex)
            {
                TempData["SubmitClaimFailed"] = "Incorrect details have been entered. Please ensure, all details have been entered correctly and you have filled in all the fields, and have attached the supporting documents.";
                return View("CreateClaim");
            }

        }

        //-----------------------------------------------------------------------------------

        // Button on Index view to create samples of Users, making it easier for testing the application. ONLY PRESS ONCE.
        public IActionResult CreateUserSamples()
        {
            // Creating a list of User objects, all administrators.

            // UNCOMMENT THIS FOR TESTING PURPOSES

            //var users = new List<User>
            //{
            //    new User
            //    {
            //        FirstName = "John",
            //        Surname = "Doe",
            //        Email = "programme@example.com",
            //        ContactNumber = "0792317568",
            //        Faculty = "Science",
            //        Password = "Admin",
            //        Role = "ProgrammeCoordinator"
            //    },
            //    new User
            //    {
            //        FirstName = "Dean",
            //        Surname = "James",
            //        Email = "academic@example.com",
            //        ContactNumber = "0823418964",
            //        Password = "Admin",
            //        Role = "AcademicManager"
            //    },
            //    new User
            //    {
            //        FirstName = "Liam",
            //        Surname = "Knipe",
            //        Email = "hr@example.com",
            //        ContactNumber = "0725143329",
            //        Password = "Admin",
            //        Role = "HR"
            //    }
            //};

            //// Inserting a collection of entities into the database in a single call.
            //_context.User.AddRange(users);

            //_context.SaveChanges();

            return RedirectToAction("ReviewClaim", "Home");
        }

        //-----------------------------------------------------------------------------------

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
