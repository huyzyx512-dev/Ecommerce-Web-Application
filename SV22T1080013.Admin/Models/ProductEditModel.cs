using SV22T1080013.DomainModels;

namespace SV22T1080013.Admin.Models
{
    public class ProductEditModel : Product
    {
        public IFormFile? UploadPhoto { get; set; }
    }
}
