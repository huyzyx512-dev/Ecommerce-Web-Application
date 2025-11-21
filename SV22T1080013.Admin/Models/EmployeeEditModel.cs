using SV22T1080013.DomainModels;

namespace SV22T1080013.Admin.Models
{
    public class EmployeeEditModel : Employee
    {
        public IFormFile? UploadPhoto { get; set; }
    }
}
