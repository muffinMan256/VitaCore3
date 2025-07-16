using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VitaCore.Data;
using VitaCore.Models;
using VitaCore.Models.ViewModel;

namespace VitaCore.Controllers
{
    [Authorize]
    [Route("[controller]")]

    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AppointmentsController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("CreateAjax")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAjax([FromBody] AppointmentDto data)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (patient == null) return BadRequest("Patient profile not found.");

            if (!DateTime.TryParse($"{data.Date} {data.Time}", out DateTime startTime))
                return BadRequest("Invalid date or time format.");

            var appointment = new AppointmentModel
            {
                DoctorId = data.DoctorId,
                PatientId = patient.id,
                StartTime = startTime,
                EndTime = null,
                Notes = data.Notes
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        public class AppointmentDto
        {
            public int DoctorId { get; set; }
            public string Date { get; set; } = string.Empty;
            public string Time { get; set; } = string.Empty;
            public string? Notes { get; set; }
        }

        [HttpGet("GetAppointments")]
        public async Task<IActionResult> GetAppointments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                .Where(a => a.Patient.UserId == user.Id)
                .Select(a => new
                {
                    id = a.Id,
                    title = "Appointment with Dr. " + a.Doctor.User.LastName,
                    start = a.StartTime,
                    allDay = false
                })
                .ToListAsync();

            return Json(appointments);
        }

        [HttpPost("UpdateTime")]
        public async Task<IActionResult> UpdateTime([FromBody] AppointmentTimeUpdateModel model)
        {
            var appointment = await _context.Appointments.FindAsync(model.Id);
            if (appointment == null) return NotFound();

            appointment.StartTime = model.NewStartTime;
            await _context.SaveChangesAsync();

            return Ok();
        }

        public class AppointmentTimeUpdateModel
        {
            public int Id { get; set; }
            public DateTime NewStartTime { get; set; }
        }

        [HttpGet("PatientDashboard")]
        public async Task<IActionResult> PatientDashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
            {
                return NotFound();
            }

            var model = new PatientDashboardViewModel
            {
                
                Appointments = await _context.Appointments
                    .Where(a => a.PatientId == patient.id)
                    .Include(a => a.Doctor)
                        .ThenInclude(d => d.User)
                    .OrderBy(a => a.StartTime)
                    .ToListAsync()
                
            };

            return RedirectToAction("Index", "Patients");

        }

        [HttpGet]
        [Route("Details/{id}")]
        [Authorize(Roles = "Admin,Patient,Doctor")]
        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            var viewModel = new AppointmentDetailsViewModel
            {
                Id = appointment.Id,
                DoctorName = $"Dr. {appointment.Doctor.User.FirstName} {appointment.Doctor.User.LastName}",
                PatientName = $"{appointment.Patient.User.FirstName} {appointment.Patient.User.LastName}",
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Notes = appointment.Notes
            };

            return View();
        }


       
    }
}
