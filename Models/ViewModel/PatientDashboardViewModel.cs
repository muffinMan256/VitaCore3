using System.ComponentModel.DataAnnotations.Schema;
using VitaCore.Data;

namespace VitaCore.Models.ViewModel
{
    public class PatientDashboardViewModel
    {


        public List<AppointmentModel> Appointments { get; set; }
        public PatientModel Patients { get; set; }
        public List<DoctorModel> Doctors { get; set; }
        public List<RecommendationModel> Recommendations { get; set; }
        public MedicalHistoryModel? MedicalHistory { get; set; }
        public List<PhysicalActivityModel> PhysicalActivities { get; set; }
        public PaginationViewModel Pagination { get; set; }
        public string SpecializationFilter { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public AppUser AppUser { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return $"{AppUser?.FirstName} {AppUser?.LastName}".Trim();
            }
        }

        [NotMapped]
        public List<string> DoctorFullNames
        {
            get
            {
                return Doctors?
                    .Select(d => d.FullName)
                    .ToList() ?? new List<string>();
            }
        }
    }
}
