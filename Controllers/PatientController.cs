using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaCore.Data;
using VitaCore.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using VitaCore.Models.ViewModel;

namespace VitaCore.Controllers
{
    public class PatientsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(ApplicationDbContext context, UserManager<AppUser> userManager, ILogger<PatientsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        //public async Task<IActionResult> Index()
        //{
        //    // 1. Get current logged-in user
        //    var userId = _userManager.GetUserId(User);

        //    // 2. Load AppUser with related patient, medical history, activities, and location
        //    var appUser = await _context.Users
        //        .Include(u => u.Patient)
        //        .ThenInclude(p => p.MedicalHistories)
        //        .Include(u => u.Patient)
        //        .ThenInclude(p => p.PhysicalActivities)
        //        .Include(u => u.Patient)
        //        .ThenInclude(p => p.LocationMaps)
        //        .FirstOrDefaultAsync(u => u.Id == userId);

        //    if (appUser?.Patient == null)
        //    {
        //        return NotFound("Patient data not found.");
        //    }

        //    var patient = appUser.Patient;

        //    // 3. Load all doctors (you can filter by patient-doctor relation later)
        //    var doctors = await _context.Doctors
        //        .Include(d => d.User)
        //        .ToListAsync();

        //    // 4. Load recommendations for this patient
        //    var recommendations = await _context.Recommendation
        //        .Where(r => r.PatientId == patient.id)
        //        .ToListAsync();

        //    // 5. Get the most recent location
        //    var latestLocation = patient.LocationMaps?
        //        .OrderByDescending(l => l.RecordedAt)
        //        .FirstOrDefault();

        //    // 6. Prepare the ViewModel
        //    var viewModel = new PatientDashboardViewModel
        //    {
        //        AppUser = appUser,
        //        Doctors = doctors,
        //        Recommendations = recommendations,
        //        MedicalHistory = patient.MedicalHistories?.FirstOrDefault(),
        //        PhysicalActivities = patient.PhysicalActivities?.ToList() ?? new List<PhysicalActivityModel>(),
        //        Latitude = latestLocation != null ? (double?)latestLocation.Latitude : null,
        //        Longitude = latestLocation != null ? (double?)latestLocation.Longitude : null
        //    };

        //    return View(viewModel);
        //}

        public async Task<IActionResult> Index(string specialization = null, int pageNumber = 1, int pageSize = 6)
        {
            var userId = _userManager.GetUserId(User);

            var appUser = await _context.Users
                .Include(u => u.Patient)
                .ThenInclude(p => p.MedicalHistories)
                .Include(u => u.Patient)
                .ThenInclude(p => p.PhysicalActivities)
                .Include(u => u.Patient)
                .ThenInclude(p => p.LocationMaps)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (appUser?.Patient == null)
            {
                return NotFound("Patient data not found.");
            }

            var patient = appUser.Patient;

            // Filter and paginate doctors
            var doctorQuery = _context.Doctors
                .Include(d => d.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(specialization))
            {
                doctorQuery = doctorQuery.Where(d =>
                    d.Specialization != null &&
                    d.Specialization.ToLower().Contains(specialization.ToLower()));
            }

            var totalDoctors = await doctorQuery.CountAsync();

            var doctors = await doctorQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var recommendations = await _context.Recommendation
                .Where(r => r.PatientId == patient.id)
                .ToListAsync();

            var latestLocation = patient.LocationMaps?
                .OrderByDescending(l => l.RecordedAt)
                .FirstOrDefault();

            var viewModel = new PatientDashboardViewModel
            {
                AppUser = appUser,
                Doctors = doctors,
                Recommendations = recommendations,
                MedicalHistory = patient.MedicalHistories?.FirstOrDefault(),
                PhysicalActivities = patient.PhysicalActivities?.ToList() ?? new List<PhysicalActivityModel>(),
                Pagination = new PaginationViewModel
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = totalDoctors
                },
                SpecializationFilter = specialization,
            };

            return View(viewModel);
        }



        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientModel model)
        {
            ModelState.Remove("User");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            model.UserId = userId;

            await _context.Patients.AddAsync(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound();

            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PatientModel model)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
                var patient = await _context.Patients.FindAsync(model.id);
                if (patient == null)
                    return NotFound();

                patient.UserId = userid;
                patient.age = model.age;
                patient.cnp = model.cnp;
                patient.address_street = model.address_street;
                patient.address_city = model.address_city;
                patient.address_county = model.address_county;
                patient.phone_number = model.phone_number;
                patient.email = model.email;
                patient.occupation = model.occupation;
                patient.workplace = model.workplace;

                _context.Patients.Update(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction("CompleteProfile", "Account");
                
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound();

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .Include(p => p.MedicalHistories)
                .Include(p => p.Recommendations)
                .Include(p => p.Alarms)
                .Include(p => p.PhysicalActivities)
                .Include(p => p.EcgSignals)
                .Include(p => p.SensorDatas)
                .Include(p => p.LocationMaps)
                .FirstOrDefaultAsync(p => p.id == id);

            if (patient == null)
                return NotFound();

            return View(patient);
        }

    }
}
