// DoctorsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaCore.Data;
using VitaCore.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using AutoMapper;
using System.Numerics;
using Microsoft.AspNetCore.Authorization;
using VitaCore.Models.ViewModel;

namespace VitaCore.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;
        public DoctorsController(ApplicationDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AccountController> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor == null) return NotFound("Doctor profile not found.");

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var todaysAppointments = await _context.Appointments
                .Where(a => a.DoctorId == doctor.id && a.StartTime >= today && a.StartTime < tomorrow)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .OrderBy(a => a.StartTime)
                .Select(a => new AppointmentViewModel
                {
                    Id = a.Id,
                    PatientName = a.Patient.User.FirstName + " " + a.Patient.User.LastName,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime ?? a.StartTime.AddMinutes(30), // Fallback if EndTime is null
                    Notes = a.Notes
                })
                .ToListAsync();

            var patientList = await _context.Appointments
                .Where(a => a.DoctorId == doctor.id)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .GroupBy(a => a.PatientId)
                .Select(g => new PatientViewModel
                {
                    Id = g.Key,
                    FullName = g.First().Patient.User.FirstName + " " + g.First().Patient.User.LastName,
                    LastVisit = g.Max(a => a.StartTime),
                    Status = "Active" // You can customize this based on logic
                })
                .ToListAsync();

            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);

            var weeklyAppointments = await _context.Appointments
                .Where(a => a.DoctorId == doctor.id && a.StartTime >= startOfWeek && a.StartTime < endOfWeek)
                .CountAsync();

            var model = new DoctorDashboardViewModel
            {
                FullName = "Dr. " + doctor.User.FirstName + " " + doctor.User.LastName,
                TodaysAppointments = todaysAppointments,
                Patients = patientList,
                Stats = new DashboardStats
                {
                    WeeklyAppointments = weeklyAppointments,
                    TotalPatients = patientList.Count,
                    UnreadMessages = 0 // Add logic if you implement messaging
                }
            };

            return View(model);
        }

        // Create - GET
        public IActionResult Create()
        {
            return View();
        }
        // Create - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorModel model)
        {
            ModelState.Remove("User");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); // The user is not logged in
            }
            var newDoc = new DoctorModel()
                {
                    UserId = userId, // Set from the logged-in user, not from the model
                    honorific_title = model.honorific_title,
                    gender = model.gender,
                    bio = model.bio,
                    availability_hours = model.availability_hours,
                    clinic_address = model.clinic_address,
                    Specialization = model.Specialization,
                    is_favorite = model.is_favorite
                };

                await _context.Doctors.AddAsync(newDoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
            return View(model);
        }


        // EDIT - GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor); 
        }


        //Edit - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DoctorModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                
                    var doctor = await _context.Doctors.FindAsync(model.id);
                    if (doctor == null)
                    {
                        return NotFound();
                    }

                    doctor.UserId = userId;
                    doctor.honorific_title = model.honorific_title;
                    doctor.gender = model.gender;
                    doctor.bio = model.bio;
                    doctor.availability_hours = model.availability_hours;
                    doctor.clinic_address = model.clinic_address;
                    doctor.Specialization = model.Specialization;
                    doctor.is_favorite = model.is_favorite;

                    _context.Doctors.Update(doctor);
                    await _context.SaveChangesAsync();
                return RedirectToAction("CompleteProfile", "Account");


                return View(model);

            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError($"{ex.InnerException}");
                return RedirectToAction("Index");
            }
        }


        //DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var doctors = await _context.Doctors.FindAsync(id);

                if (doctors == null)
                {
                    return NotFound();
                }

                _logger.LogInformation($"Categoria cu id-ul {id} a fost stearsa");
               
                _context.Doctors.Remove(doctors);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                _logger.LogError($"Error deleting category with ID {id}");
                throw;
            }
        }

    }
}