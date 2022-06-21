using psw_ftn.Dtos;
using psw_ftn.Dtos.CheckUpDtos;
using psw_ftn.Dtos.FeedbackDtos;
using psw_ftn.Dtos.UserDtos;

namespace Psw.UnitTests
{
    public class Utils
    {
        public PatientDto CreatePatient(int UserId, string firstName, string lastName, int status, string email)
        {
            return new()
            {
                UserId= UserId,
                Role = RoleDto.Patient,
                FirstName = firstName,
                LastName = lastName,
                AccessToken = Guid.NewGuid().ToString(),
                Status = 1,
                Email = email, 
            };
        }

    public DoctorDto CreateDoctor(int UserId, string firstName, string lastName, int status, string email, DrExpertiseDto expertise)
    {
        return new()
        {
                UserId= UserId,
                Role = RoleDto.Doctor,
                FirstName = firstName,
                LastName = lastName,
                AccessToken = Guid.NewGuid().ToString(),
                Status = 1,
                Email = email,
                Expertise = expertise.ToString()
        };
    }

    public CheckUpDto CreateCheckup(int checkUpId, PatientDto patient, DoctorDto doctor) 
    { 
        return new()
        {
            CheckUpId=checkUpId,
            Patient = patient,
            Doctor = doctor,
            CancellationTime = DateTime.Now.AddDays(-2),
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(1)
        };
    }

    public GetFeedbackDto CreateGetFeedbackDto (GradeDto grade, string comment, int feedbackId, bool incognito,bool isForDisplay, PatientDto patient)
    {
        return new()
        {
            Grade = grade,
            Comment = comment,
            FeedbackId = feedbackId,
            Incognito = incognito,
            IsForDisplay = isForDisplay,
            Patient = patient
        };
    }
    
    }      

}