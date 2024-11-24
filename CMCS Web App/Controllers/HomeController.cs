using CMCS_Web_App.Data;
using CMCS_Web_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout;
using System.IO;

namespace CMCS_Web_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////

        public HomeController(ILogger<HomeController> logger, AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        /////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////

        #region VIEWS

        // Home Page
        public IActionResult Index()
        {
            return View();
        }

        //-----------------------------------------------------------------------------------

        // Create Claim Page
        public IActionResult CreateClaim()
        {
            if (!IsUserLoggedIn())
            {
                TempData["Error"] = "You must be logged in to submit a claim.";
                return RedirectToAction("Login");
            }

            TempData["AccessLevel"] = CheckAccessLevel();

            TempData["UserId"] = _httpContextAccessor.HttpContext.Session.GetInt32("UserID");

            return View();
        }

        //-----------------------------------------------------------------------------------

        // Review Claim Page
        public IActionResult ReviewClaim()
        {
            if (!IsUserLoggedIn())
            {
                TempData["Error"] = "You must be logged in to review a claim.";
                return RedirectToAction("Login");
            }

            var flaggedClaims = new List<UserClaim>();

            int _accessLvl = CheckAccessLevel();

            // Check to see if the user logged in is an administrator before populating the list of all flagged claims.
            if (_accessLvl == 2)
            {
                flaggedClaims = _context.UserClaim.Include(c => c.User).Where(c => c.FlaggedClaim == true && c.ClaimStatus == "Pending").ToList();
            }

            TempData["AccessLevel"] = CheckAccessLevel();

            return View(flaggedClaims);
        }

        //-----------------------------------------------------------------------------------

        // Process Payment for Approved Claims Page
        public IActionResult SummarizeClaim()
        {
            if (!IsUserLoggedIn())
            {
                TempData["Error"] = "You must be logged in to summarize a claim.";
                return RedirectToAction("Login");
            }

            var approvedClaims = new List<UserClaim>();

            int _accessLvl = CheckAccessLevel();

            // Check to see if the user logged in is an HR before populating the list of all flagged claims.
            if (_accessLvl == 3)
            {
                approvedClaims = _context.UserClaim.Include(c => c.User).Where(c => c.FlaggedClaim == false && c.ClaimStatus == "Approved").ToList();
            }

            TempData["AccessLevel"] = CheckAccessLevel();

            return View(approvedClaims);
        }

        //-----------------------------------------------------------------------------------

        // Track  Claims Page
        public IActionResult TrackAllClaims()
        {
            if (!IsUserLoggedIn())
            {
                TempData["Error"] = "You must be logged in to track a claim.";
                return RedirectToAction("Login");
            }

            var claims = new List<UserClaim>();

            int _accessLvl = CheckAccessLevel();

            // Check to see if the user logged in is an administrator before populating the list of all claims.
            if (_accessLvl > 1)
            {
                claims = _context.UserClaim.Include(c => c.User).ToList();
            }

            TempData["AccessLevel"] = _accessLvl;

            return View(claims);
        }

        public IActionResult ViewUserClaim()
        {
            if (!IsUserLoggedIn())
            {
                TempData["Error"] = "You must be logged in to view your claims.";
                return RedirectToAction("Login");
            }

            var userClaims = new List<UserClaim>();

            int _accessLvl = CheckAccessLevel();

            // Check to see if the user logged in is a lecturer before populating the list of all their claims.
            if (_accessLvl == 1)
            {
                var userId = GetUserInSession().UserId;

                userClaims = userClaims = _context.UserClaim.Include(c => c.User).Where(c => c.UserId == userId).ToList();
            }

            TempData["AccessLevel"] = _accessLvl;

            return View(userClaims);

        }

        //-----------------------------------------------------------------------------------

        // Login Page
        public IActionResult Login()
        {
            if (!IsUserLoggedIn())
            {
                return View();
            }
            return RedirectToAction("Account");
        }

        //-----------------------------------------------------------------------------------

        // Register Page
        public IActionResult Register()
        {
            if (!IsUserLoggedIn())
            {
                TempData["Error"] = "You must be logged in to register a new lecturer.";
                return RedirectToAction("Login");
            }

            TempData["AccessLevel"] = CheckAccessLevel();

            return View();
        }

        //-----------------------------------------------------------------------------------

        // Account Page
        public IActionResult Account()
        {
            if (!IsUserLoggedIn())
            {
                TempData["Error"] = "You must be logged in to view your account details.";
                return RedirectToAction("Login");
            }

            return View(GetUserInSession());
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////

        #region USER SESSION | ACCESS MANAGEMENT | LOGIN | LOGOUT 

        // Gets the User object from the database that is currently in session.
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

        // Checks the access level of the user in session and returns an integer value.
        private int CheckAccessLevel()
        {
            int accessLevel = 0;

            switch (GetUserInSession().Role)
            {
                case "Lecturer":
                    accessLevel = 1;
                    break;
                case "ProgrammeCoordinator":
                    accessLevel = 2;
                    break;
                case "AcademicManager":
                    accessLevel = 2;
                    break;
                case "HR":
                    accessLevel = 3;
                    break;
            }
            return accessLevel;
        }

        //-----------------------------------------------------------------------------------

        // Checks if the user is logged in by checking if the session contains a UserID and returns a boolean value.
        private bool IsUserLoggedIn()
        {
            int? userID = _httpContextAccessor.HttpContext.Session.GetInt32("UserID");

            return userID.HasValue;
        }

        //-----------------------------------------------------------------------------------

        // Checks if the details entered in the login form are correct and creates a session for the user.
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                _httpContextAccessor.HttpContext.Session.SetInt32("UserID", user.UserId);

                return RedirectToAction("Account");
            }

            TempData["LoginFailed"] = "These details do not exist or have been entered incorrectly. Please try again.";

            return View();
        }

        //-----------------------------------------------------------------------------------

        // Checks if the user is logged in and logs them out by removing the UserID from the session.
        public IActionResult Logout()
        {
            if (!IsUserLoggedIn())
            {
                TempData["Error"] = "You are not logged in.";
                return RedirectToAction("Login");
            }

            _httpContextAccessor.HttpContext.Session.Remove("UserID");

            TempData["Logout"] = "Successfully logged out!";

            return RedirectToAction("Login");
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////

        #region ACTIONS

        // Update User's Password from Account View
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string password)
        {
            var userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserID");

            var user = await _context.User.FindAsync(userId);

            user.Password = password;

            _context.Update(user);

            await _context.SaveChangesAsync();

            TempData["PasswordChangeSuccess"] = "Successfully changed your password!";

            return RedirectToAction("Account");
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////

        #region CLAIM RELATED ACTIONS

        // Download File Action
        public IActionResult DownloadFile(int Id)
        {
            var claim = _context.UserClaim.FirstOrDefault(c => c.UserId == Id);

            if (claim == null || claim.FileData == null)
            {
                return NotFound();
            }

            return File(claim.FileData, "application/octet-stream", claim.FileName);
        }

        //-----------------------------------------------------------------------------------

        // Approve Claim Action
        public IActionResult ApproveClaim(int Id)
        {
            var claim = _context.UserClaim.FirstOrDefault(c => c.UserClaimId == Id);

            claim.ClaimStatus = "Approved";

            claim.FlaggedClaim = false;

            _context.SaveChanges();

            return RedirectToAction("ReviewClaim");
        }

        //-----------------------------------------------------------------------------------

        // Reject Claim Action
        public IActionResult RejectClaim(int Id)
        {
            var claim = _context.UserClaim.FirstOrDefault(c => c.UserClaimId == Id);

            claim.ClaimStatus = "Rejected";

            claim.FlaggedClaim = false;

            _context.SaveChanges();

            return RedirectToAction("ReviewClaim");
        }

        //-----------------------------------------------------------------------------------

        // Submit New Claim Action
        [HttpPost]
        public async Task<IActionResult> SubmitNewClaim(UserClaim claim, IFormFile file, string ClaimAmount)
        {
            // CALL REMOVE CLAIM CURRENCY METHOD
            claim.ClaimAmount = RemoveClaimCurrency(ClaimAmount);

            // CALL VALIDATE CLAIM INPUT FIELDS METHOD
            if (!ValidateClaimInput(claim))
            {
                return RedirectToAction("CreateClaim");
            }

            if (file != null && file.Length > 0)
            {
                var fileName = Path.GetFileName(file.FileName);

                // Check if the file type is valid.
                string[] fileTypes = { ".pdf", ".docx", ".png", ".jpg" };

                if (!fileTypes.Any(type => fileName.EndsWith(type, StringComparison.OrdinalIgnoreCase)))
                {
                    TempData["SubmitClaimFailed"] = "Please submit a file type of one of the following: .pdf / .docx / .png / .jpg";

                    return RedirectToAction("CreateClaim");
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


            // CALL VALIDATE CLAIM DETAILS METHOD
            claim = ValidateClaimDetails(claim);

            try
            {
                _context.Add(claim);

                _context.SaveChanges();

                TempData["SubmitClaimSuccess"] = "Successfully submitted your claim!";

                return RedirectToAction("CreateClaim");
            }

            catch (Exception ex)
            {
                TempData["SubmitClaimFailed"] = "Incorrect details have been entered. Please ensure, all details have been entered correctly and you have filled in all the fields, and have attached the supporting documents.";

                return RedirectToAction("CreateClaim");
            }

        }

        //-----------------------------------------------------------------------------------

        // Validate Claim Input Details
        private bool ValidateClaimInput(UserClaim claim)
        {
            if (claim.HourlyRate <= 0)
            {
                TempData["SubmitClaimFailed"] = "Please enter a valid hourly rate.";
                return false;
            }

            if (claim.HoursWorked <= 0)
            {
                TempData["SubmitClaimFailed"] = "Please enter a valid number of hours worked.";
                return false;
            }

            if (claim.ClaimAmount <= 0)
            {
                TempData["SubmitClaimFailed"] = "Please enter a valid claim amount.";
                return false;
            }

            return true;
        }

        //-----------------------------------------------------------------------------------

        // Validate Claim for Approval
        private UserClaim ValidateClaimDetails(UserClaim claim)
        {
            // Condition 1: HourlyRate is greater than 500
            if (claim.HourlyRate > 500)
            {
                claim.FlaggedClaim = true;
                return claim;
            }

            // Condition 2: HoursWorked is greater than 80
            if (claim.HoursWorked > 80)
            {
                claim.FlaggedClaim = true;
                return claim;
            }

            // Condition 3: ClaimAmount is greater than 15000
            if (claim.ClaimAmount > 15000)
            {
                claim.FlaggedClaim = true;
                return claim;
            }

            // Condition 4: ClaimAmount does not match the calculated amount.
            if (claim.ClaimAmount != (claim.HourlyRate * claim.HoursWorked))
            {
                claim.FlaggedClaim = true;
                return claim;
            }

            // If none of the conditions are met, the claim is valid.
            claim.ClaimStatus = "Approved";

            return claim;
        }

        //-----------------------------------------------------------------------------------

        // Removes the 'R' from the ClaimAmount string and parses it to a double and returns it.
        private double RemoveClaimCurrency(string ClaimAmount)
        {
            return Convert.ToDouble(ClaimAmount.Replace("R", ""));
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////

        #region HR ACTIONS

        // Register New User Action (Access Level: HR = 3)
        [HttpPost]
        public IActionResult RegisterUser(User user)
        {
            user.Role = "Lecturer";

            if (ModelState.IsValid)
            {
                _context.Add(user);

                _context.SaveChanges();
            }
            else
            {
                TempData["RegisterFailed"] = "Incorrect details have been entered. Please ensure, all details have been entered correctly and you have filled in all the fields.";

                return RedirectToAction("Register");
            }

            TempData["RegisterSuccess"] = "Successfully registered a new lecturer!";

            return RedirectToAction("Register");
        }

        //-----------------------------------------------------------------------------------

        [HttpPost]
        public IActionResult MakePDF(int id)
        {
            var claim = _context.UserClaim.FirstOrDefault(c => c.UserClaimId == id);

            var pdfData = GeneratePdfInvoice(claim);

            claim.PdfFileName = $"Invoice_{claim.UserClaimId}.pdf";

            claim.PdfFileData = pdfData;

            _context.Update(claim);

            _context.SaveChanges();

            TempData["PdfGenerated"] = "PDF Invoice has been generated and saved successfully.";

            return RedirectToAction("SummarizeClaim");
        }

        //-----------------------------------------------------------------------------------

        // Method to Generate PDF Invoice
        private byte[] GeneratePdfInvoice(UserClaim claim)
        {
            using (var memoryStream = new MemoryStream())
            {
                // Initialize PDF writer and document
                var writer = new PdfWriter(memoryStream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Add content to the PDF
                document.Add(new Paragraph("Claim Invoice"));
                document.Add(new Paragraph($"User ID: {claim.UserId}"));
                document.Add(new Paragraph($"Hours Worked: {claim.HoursWorked}"));
                document.Add(new Paragraph($"Hourly Rate: {claim.HourlyRate}"));
                document.Add(new Paragraph($"Claim Amount: {claim.ClaimAmount}"));
                document.Add(new Paragraph($"Claim Status: {claim.ClaimStatus}"));

                // Close the document
                document.Close();

                // Return the PDF as a byte array
                return memoryStream.ToArray();
            }
        }

        //-----------------------------------------------------------------------------------

        // Method to Download PDF Invoice
        public IActionResult DownloadPdf(int id)
        {
            var claim = _context.UserClaim.FirstOrDefault(c => c.UserClaimId == id);

            if (claim == null || claim.PdfFileData == null)
            {
                return NotFound();
            }

            return File(claim.PdfFileData, "application/pdf", claim.PdfFileName);
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////

        #region CREATE TEST DATA

        // Button on Index View to create samples of Users, making it easier for testing the application.
        // TESTING PURPOSES ONLY.
        public IActionResult CreateUserSamples()
        {
            // Creating a list of User objects, 1 user per role.

            var user = new User
            {
                FirstName = "Nicholas",
                Surname = "Cage",
                Email = "lecturer@example.com",
                ContactNumber = "0792317568",
                Faculty = "Engineering",
                Password = "Lecturer",
                Role = "Lecturer"
            };

            var users = new List<User>
            {
                new User
                {
                    FirstName = "John",
                    Surname = "Doe",
                    Email = "programme@example.com",
                    ContactNumber = "0792317568",
                    Faculty = "Science",
                    Password = "Admin",
                    Role = "ProgrammeCoordinator"
                },
                new User
                {
                    FirstName = "Dean",
                    Surname = "James",
                    Email = "academic@example.com",
                    ContactNumber = "0823418964",
                    Password = "Admin",
                    Role = "AcademicManager"
                },
                new User
                {
                    FirstName = "Liam",
                    Surname = "Knipe",
                    Email = "hr@example.com",
                    ContactNumber = "0725143329",
                    Password = "Admin",
                    Role = "HR"
                },
                new User
                {
                    FirstName = "Nicholas",
                    Surname = "Cage",
                    Email = "lecturer@example.com",
                    ContactNumber = "0792317568",
                    Faculty = "Engineering",
                    Password = "Lecturer",
                    Role = "Lecturer"
                }
            };

            // Check to see if all test users already exist in database.
            bool allUsersExist = users.All(user => _context.User.Any(u => u.Email == user.Email));

            if (allUsersExist)
            {
                // If all test users exist, redirect to the Home Page, and don't insert data.
                TempData["UsersExist"] = "This test data has already been created.";

                return RedirectToAction("Index");
            }

            // Inserting a collection of entities into the database in a single call.
            _context.User.AddRange(users);

            _context.SaveChanges();

            TempData["UsersCreated"] = "Successfully created test data.";

            return RedirectToAction("Index");
        }

        //-----------------------------------------------------------------------------------

        // Buttons on Login Page to Login into the various roles, using the generated test samples.

        public async Task<IActionResult> LoginProgrammeCoordinator()
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == "programme@example.com" && u.Password == "Admin");

            if (user != null)
            {
                _httpContextAccessor.HttpContext.Session.SetInt32("UserID", user.UserId);

                return RedirectToAction("Account");
            }

            TempData["Error"] = "To use these buttons, please ensure you have created the test data using the 'Create Test Data' button on the Home Page.";

            return View("Login");
        }

        //-----------------------------------------------------------------------------------

        public async Task<IActionResult> LoginAcademicManager()
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == "academic@example.com" && u.Password == "Admin");

            if (user != null)
            {
                _httpContextAccessor.HttpContext.Session.SetInt32("UserID", user.UserId);

                return RedirectToAction("Account");
            }

            TempData["Error"] = "To use these buttons, please ensure you have created the test data using the 'Create Test Data' button on the Home Page.";

            return View("Login");
        }

        //-----------------------------------------------------------------------------------

        public async Task<IActionResult> LoginHR()
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == "hr@example.com" && u.Password == "Admin");

            if (user != null)
            {
                _httpContextAccessor.HttpContext.Session.SetInt32("UserID", user.UserId);

                return RedirectToAction("Account");
            }

            TempData["Error"] = "To use these buttons, please ensure you have created the test data using the 'Create Test Data' button on the Home Page.";

            return View("Login");
        }

        //-----------------------------------------------------------------------------------

        public async Task<IActionResult> LoginLecturer()
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == "lecturer@example.com" && u.Password == "Lecturer");

            if (user != null)
            {
                _httpContextAccessor.HttpContext.Session.SetInt32("UserID", user.UserId);

                return RedirectToAction("Account");
            }

            TempData["Error"] = "To use these buttons, please ensure you have created the test data using the 'Create Test Data' button on the Home Page.";

            return View("Login");
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
