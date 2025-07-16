namespace VitaCore.Models.ViewModel
{
    public class DashboardViewModel
    {
        public IEnumerable<DoctorModel> Doctors { get; set; }
        public IEnumerable<PatientModel> Patients { get; set; }

        // Analytics
        public int TotalDoctors { get; set; }
        public int TotalPatients { get; set; }
        public int DoctorLoginsToday { get; set; }
        public int PatientLoginsToday { get; set; }
    }


}
