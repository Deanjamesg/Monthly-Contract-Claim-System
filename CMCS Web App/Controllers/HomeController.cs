using CMCS_Web_App.Data;
using CMCS_Web_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CMCS_Web_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
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
            return View();
        }

        //-----------------------------------------------------------------------------------

        public IActionResult CreateClaim()
        {
            return View();
        }

        //-----------------------------------------------------------------------------------

        // Actions for the ReviewClaim View

        public IActionResult ReviewClaim()
        {
            // https://learn.microsoft.com/en-us/ef/core/querying/

            var claims = _context.Claim.Include(c => c.User).ToList();

            return View(claims);
        }

        public IActionResult DownloadFile(int Id)
        {
            // https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.firstordefault?view=net-8.0

            var claim = _context.Claim.FirstOrDefault(c => c.UserId == Id);

            if (claim == null || claim.FileData == null)
            {
                return NotFound();
            }

            //https://learn.microsoft.com/en-us/dotnet/api/system.web.mvc.filecontentresult?view=aspnet-mvc-5.2

            return File(claim.FileData, "application/octet-stream", claim.FileName);
        }

        public IActionResult ApproveClaim(int Id)
        {
            var claim = _context.Claim.FirstOrDefault(c => c.ClaimId == Id);

            claim.ClaimStatus = "Approved";

            _context.SaveChanges();

            return RedirectToAction("ReviewClaim");
        }

        public IActionResult RejectClaim(int Id)
        {
            var claim = _context.Claim.FirstOrDefault(c => c.ClaimId == Id);

            claim.ClaimStatus = "Rejected";

            _context.SaveChanges();

            return RedirectToAction("ReviewClaim");
        }

        //-----------------------------------------------------------------------------------

        public IActionResult TrackAllClaims()
        {
            var claims = _context.Claim.Include(c => c.User).ToList();

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
        public async Task<IActionResult> SubmitNewClaim(Claim claim, IFormFile file)
        {
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
