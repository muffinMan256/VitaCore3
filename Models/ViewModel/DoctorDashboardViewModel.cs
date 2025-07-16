namespace VitaCore.Models.ViewModel
{
    public class DoctorDashboardViewModel
    {
        public string FullName { get; set; } = "";
        public List<AppointmentViewModel> TodaysAppointments { get; set; } = new();
        public List<PatientViewModel> Patients { get; set; } = new();
        public DashboardStats Stats { get; set; } = new();
    }

    public class AppointmentViewModel
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = "";
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }
    }

    public class PatientViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public DateTime? LastVisit { get; set; }
        public string Status { get; set; } = "Unknown";
    }

    public class DashboardStats
    {
        public int WeeklyAppointments { get; set; }
        public int TotalPatients { get; set; }
        public int UnreadMessages { get; set; }
    }


}
