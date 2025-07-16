using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using VitaCore.Models;
using VitaCore.Data;

namespace VitaCore.Models.ViewComponents
{
    public class InboxNavViewComponent : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;

        public InboxNavViewComponent(UserManager<AppUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null) return View(0);

            var doctor = _context.Doctors.FirstOrDefault(d => d.UserId == user.Id);
            var patient = _context.Patients.FirstOrDefault(p => p.UserId == user.Id);
            int userId = doctor?.id ?? patient?.id ?? 0;

            int unreadCount = _context.Messages.Count(m => m.receiver_id == userId && m.is_read == false);
            return View(unreadCount);
        }
    }
}