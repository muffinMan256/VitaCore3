using System.Numerics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using VitaCore.Data;
using VitaCore.Models;
using VitaCore.Models.ViewModel;

namespace VitaCore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHostEnvironment _webhost;

        public AccountController(ApplicationDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AccountController> logger, IHttpContextAccessor httpContextAccessor, IHostEnvironment webhost)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _webhost = webhost;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Modelul nu este valid.");
                return View(model);
            }

            //var existingUser = await _userManager.FindByEmailAsync(model.Email);
            //if (existingUser != null)
            //{
            //    _logger.LogInformation("Utilizatorul există deja în baza de date.");
            //    return View(model);
            //}

            var defaultImagePath = "/images/default-profile.png";

            var newUser = new AppUser
            {
                Email = model.Email,
                UserName = model.UserName,
            };

            string assignedRole = "User";
            string password = model.Password;

            if (model.Email.ToLower() == "admin@admin.com")
            {
                assignedRole = "Admin";
                password = "AdminSibiu3#";
            }
            else if (model.Email.ToLower().Contains("@doctor"))
            {
                assignedRole = "Doctor";
            }
            else if (model.Email.ToLower().Contains("@patient"))
            {
                assignedRole = "Patient";
            }

            var createResult = await _userManager.CreateAsync(newUser, password);
            if (!createResult.Succeeded)
            {
                _logger.LogError("Eroare la crearea utilizatorului.");
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    _logger.LogError(error.Description);
                }
                return View(model);
            }

            var roleResult = await _userManager.AddToRoleAsync(newUser, assignedRole);
            if (!roleResult.Succeeded)
            {
                _logger.LogError($"Eroare la atribuirea rolului {assignedRole}.");
                await _userManager.DeleteAsync(newUser); 
                return View(model);
            }

            _logger.LogInformation($"Utilizatorul {newUser.UserName} a fost creat cu rolul {assignedRole}.");

            if (assignedRole == "Doctor")
            {
                var doctor = new DoctorModel
                {
                    UserId = newUser.Id,
                    Specialization = "General"
                };
                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();
            }
            else if (assignedRole == "Patient")
            {
                var patient = new PatientModel
                {
                    UserId = newUser.Id,
                    email = newUser.Email,
                    cnp = Guid.NewGuid().ToString().Substring(0, 13),
                    address_city = string.Empty,
                    address_county = string.Empty,
                    address_street = string.Empty,
                    occupation = string.Empty,
                    phone_number = string.Empty,
                    workplace = string.Empty,
                    age = 0
                };
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }


            return RedirectToAction("Login", "Account");
        }



        // LOGIN
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(bool isRegister = false)
        {
            _logger.LogInformation("Start Login");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
           

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user == null)
                {
                    _logger.LogInformation($"User {user.Email} is locked out and cannot log in.");
                    _logger.LogInformation($"User {user.UserName} is locked out and cannot log in.");
                    //notificare
                    return View(model);
                }

                if (user.LockoutEnabled == false)
                {
                    _logger.LogInformation($"User {user.Email} is locked out and cannot log in.");
                    return RedirectToAction("Login", "Account");
                }

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {


                    _logger.LogInformation($"utilizatorul cu numele {user.Email} s-a logat pe pagina");
                    var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "Unknown";
                    _logger.LogInformation("LOGIN_EVENT | {Email} | {Role} | {Time}", user.Email, role, DateTime.UtcNow);

                    user.LastLoginTime = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Patient"))
                    {
                        var patient = await _context.Patients
                            .FirstOrDefaultAsync(p => p.UserId == user.Id);

                        if (patient == null ||
                            string.IsNullOrWhiteSpace(user.FirstName) ||
                            string.IsNullOrWhiteSpace(user.LastName) ||
                            user.Birthday == null ||
                            string.IsNullOrWhiteSpace(patient.cnp) ||
                            string.IsNullOrWhiteSpace(patient.phone_number))
                        {
                            return RedirectToAction("Edit", "Patients", new { id = patient.id });
                        }
                    }

                    else if (roles.Contains("Doctor"))
                    {
                        var doctor = await _context.Doctors
                            .FirstOrDefaultAsync(d => d.UserId == user.Id);

                        if (doctor == null ||
                            string.IsNullOrWhiteSpace(user.FirstName) ||
                            string.IsNullOrWhiteSpace(user.LastName) ||
                            user.Birthday == null ||
                            string.IsNullOrWhiteSpace(doctor.Specialization) ||
                            string.IsNullOrWhiteSpace(doctor.clinic_address))
                        {
                            return RedirectToAction("Edit", "Doctors", new { id = doctor.id });
                        }
                    }


                    return RedirectToAction("Index", "Home");
                }
                
                _logger.LogInformation("Utilizatorul nu se poate loga");
                return RedirectToAction("Login", "Account");
            }
            _logger.LogInformation("Modelul nu este valid");
            return RedirectToAction("Login", "Account");
        }

        //LOGOUT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User has logged out");
            return RedirectToAction("Login", "Account");
        }



        //[HttpGet]
        public async Task<IActionResult> CompleteProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            return View(new CompleteProfileModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteProfile(CompleteProfileModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Birthday = model.Birthday;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }
}