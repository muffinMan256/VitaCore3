using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VitaCore.Data;
using VitaCore.Models;

namespace VitaCore.Controllers
{
    public class MedicalHistoryController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHostEnvironment _webhost;

        public MedicalHistoryController(ApplicationDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AccountController> logger, IHttpContextAccessor httpContextAccessor, IHostEnvironment webhost)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _webhost = webhost;
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateMedicalHistory(MedicalHistoryModel model)
        {
            if (ModelState.IsValid)
            {
                var existing = _context.MedicalHistory
                    .FirstOrDefault(m => m.PatientId == model.PatientId);

                if (existing != null)
                {
                    existing.History = model.History;
                    existing.Allergies = model.Allergies;
                    existing.CardiologyConsultations = model.CardiologyConsultations;
                    _context.Update(existing);
                }
                else
                {
                    _context.Add(model);
                }

                _context.SaveChanges();
            }

      
            return RedirectToAction("Index", "Patients", new { id = model.PatientId });
        }



        [HttpGet]
        public IActionResult Edit(int patientId)
        {
            var patient = _context.Patients.FirstOrDefault(p => p.id == patientId);
            if (patient == null)
                return NotFound();

            var medicalHistory = _context.MedicalHistory.FirstOrDefault(m => m.PatientId == patientId);

            var model = medicalHistory ?? new MedicalHistoryModel
            {
                PatientId = patientId
            };

            return View("EditMedicalHistory", model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MedicalHistoryModel model)
        {
            if (ModelState.IsValid)
            {
                var existing = _context.MedicalHistory
                    .FirstOrDefault(m => m.PatientId == model.PatientId);

                if (existing != null)
                {
                    // Update existing fields
                    existing.History = model.History;
                    existing.Allergies = model.Allergies;
                    existing.CardiologyConsultations = model.CardiologyConsultations;
                    _context.Update(existing);
                }
                else
                {
                    // Add new record
                    _context.Add(model);
                }

                _context.SaveChanges();

                // Redirect back to the patient dashboard
                return RedirectToAction("Index", "Patients", new { id = model.PatientId });
            }

            // If model validation fails, redisplay the form
            return View("EditMedicalHistory", model);
        }



    }
}
