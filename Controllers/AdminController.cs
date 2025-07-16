// AdminController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaCore.Data;
using VitaCore.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VitaCore.Models.ViewModel;

namespace VitaCore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        //correct one
        //public async Task<IActionResult> Index(string filter = "doctors")
        //{
        //    ViewBag.Filter = filter;

        //    var totalDoctors = await _context.Doctors.CountAsync();
        //    var totalPatients = await _context.Patients.CountAsync();

        //    // Replace with real login tracking logic
        //    var doctorLoginsToday = 5; // Example only
        //    var patientLoginsToday = 12;

        //    var viewModel = new DashboardViewModel
        //    {
        //        TotalDoctors = totalDoctors,
        //        TotalPatients = totalPatients,
        //        DoctorLoginsToday = doctorLoginsToday,
        //        PatientLoginsToday = patientLoginsToday
        //    };

        //    if (filter == "doctors")
        //    {
        //        viewModel.Doctors = await _context.Doctors.Include(d => d.User).ToListAsync();
        //    }
        //    else
        //    {
        //        viewModel.Patients = await _context.Patients.Include(p => p.User).ToListAsync();
        //    }

        //    return View("Dashboard", viewModel);
        //}

        public async Task<IActionResult> Index(string filter = "doctors")
        {
            ViewBag.Filter = filter;

            var totalDoctors = await _context.Doctors.CountAsync();
            var totalPatients = await _context.Patients.CountAsync();

            // 🔍 Count today's logins
            var doctorLoginsToday = 0;
            var patientLoginsToday = 0;

            var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "logins.txt");
            var today = DateTime.UtcNow.ToString("yyyy-MM-dd");

            if (System.IO.File.Exists(logPath))
            {
                var lines = await System.IO.File.ReadAllLinesAsync(logPath);

                doctorLoginsToday = lines.Count(line =>
                    line.Contains("LOGIN_EVENT") &&
                    line.Contains("Doctor") &&
                    line.Contains(today));

                patientLoginsToday = lines.Count(line =>
                    line.Contains("LOGIN_EVENT") &&
                    line.Contains("Patient") &&
                    line.Contains(today));
            }

            var viewModel = new DashboardViewModel
            {
                TotalDoctors = totalDoctors,
                TotalPatients = totalPatients,
                DoctorLoginsToday = doctorLoginsToday,
                PatientLoginsToday = patientLoginsToday
            };

            if (filter == "doctors")
            {
                viewModel.Doctors = await _context.Doctors.Include(d => d.User).ToListAsync();
            }
            else
            {
                viewModel.Patients = await _context.Patients.Include(p => p.User).ToListAsync();
            }

            return View("Dashboard", viewModel);
        }

    }
}