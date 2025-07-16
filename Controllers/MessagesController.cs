using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using VitaCore.Data;
using VitaCore.Models;
using Microsoft.EntityFrameworkCore;
using VitaCore.Models.ViewModel;

namespace VitaCore.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public MessagesController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Send message POST
        [HttpPost]
        public async Task<IActionResult> SendMessage(MessageModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var senderDoctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);
            var senderPatient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id);

            int senderId = senderDoctor?.id ?? senderPatient?.id ?? 0;
            if (senderId == 0) return BadRequest("Sender not found");

            model.sender_id = senderId;
            model.sent_at = DateTime.UtcNow;
            model.is_read = false;

            _context.Messages.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Inbox");
        }

        // List of conversations
        public async Task<IActionResult> Inbox()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var doctor = await _context.Doctors.Include(d => d.User).FirstOrDefaultAsync(d => d.UserId == user.Id);
            var patient = await _context.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.UserId == user.Id);

            int currentUserId = doctor?.id ?? patient?.id ?? 0;
            ViewBag.CurrentUserId = currentUserId;

            var messages = await _context.Messages
                .Where(m => m.sender_id == currentUserId || m.receiver_id == currentUserId)
                .OrderByDescending(m => m.sent_at)
                .ToListAsync();

            // Get all involved user IDs
            var userIds = messages.Select(m => m.sender_id)
                                  .Concat(messages.Select(m => m.receiver_id))
                                  .Distinct()
                                  .ToList();

            // Fetch all doctors and patients for those IDs
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Where(d => userIds.Contains(d.id))
                .ToListAsync();

            var patients = await _context.Patients
                .Include(p => p.User)
                .Where(p => userIds.Contains(p.id))
                .ToListAsync();

            // Helper to get user name
            string GetUserName(int id)
            {
                var doc = doctors.FirstOrDefault(d => d.id == id);
                if (doc != null) return $"{doc.User.FirstName} {doc.User.LastName}";

                var pat = patients.FirstOrDefault(p => p.id == id);
                if (pat != null) return $"{pat.User.FirstName} {pat.User.LastName}";

                return "Unknown";
            }

            // Group messages to show latest per conversation
            var grouped = messages
                .GroupBy(m => m.sender_id == currentUserId ? m.receiver_id : m.sender_id)
                .Select(g => g.First())
                .ToList();

            var messageViewModels = grouped.Select(m => new MessageViewModel
            {
                Id = m.id,
                Message = m.message,
                SentAt = m.sent_at,
                IsRead = m.is_read,
                SenderId = m.sender_id,
                ReceiverId = m.receiver_id,
                SenderName = GetUserName(m.sender_id),
                ReceiverName = GetUserName(m.receiver_id)
            }).ToList();

            return View(messageViewModels);
        }

        // Conversation thread with a specific user
        //public async Task<IActionResult> Conversation(int withUserId)
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null) return Unauthorized();

        //    var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);
        //    var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id);

        //    int currentUserId = doctor?.id ?? patient?.id ?? 0;

        //    var messages = await _context.Messages
        //        .Where(m => (m.sender_id == currentUserId && m.receiver_id == withUserId)
        //                 || (m.sender_id == withUserId && m.receiver_id == currentUserId))
        //        .OrderBy(m => m.sent_at)
        //        .ToListAsync();

        //    // Mark unread messages as read
        //    var unread = messages.Where(m => m.receiver_id == currentUserId && m.is_read == false);
        //    foreach (var msg in unread)
        //    {
        //        msg.is_read = true;
        //    }
        //    await _context.SaveChangesAsync();

        //    ViewBag.CurrentUserId = currentUserId;
        //    ViewBag.WithUserId = withUserId;
        //    return View(messages);
        //}
        public async Task<IActionResult> Conversation(int withUserId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id);

            int currentUserId = doctor?.id ?? patient?.id ?? 0;

            var messages = await _context.Messages
                .Where(m => (m.sender_id == currentUserId && m.receiver_id == withUserId)
                            || (m.sender_id == withUserId && m.receiver_id == currentUserId))
                .OrderBy(m => m.sent_at)
                .ToListAsync();

            // Mark unread messages as read
            var unread = messages.Where(m => m.receiver_id == currentUserId && m.is_read == false);
            foreach (var msg in unread)
            {
                msg.is_read = true;
            }
            await _context.SaveChangesAsync();

            // 🟢 Get the name of the other user
            string withUserName = await _context.Doctors
                .Include(d => d.User)
                .Where(d => d.id == withUserId)
                .Select(d => d.User.FirstName + " " + d.User.LastName)
                .FirstOrDefaultAsync();

            if (withUserName == null)
            {
                withUserName = await _context.Patients
                    .Include(p => p.User)
                    .Where(p => p.id == withUserId)
                    .Select(p => p.User.FirstName + " " + p.User.LastName)
                    .FirstOrDefaultAsync();
            }

            // 🟢 Pass the name to the view
            ViewBag.CurrentUserId = currentUserId;
            ViewBag.WithUserId = withUserId;
            ViewBag.WithUserName = withUserName ?? "Unknown";

            return View(messages);
        }

    }
}
