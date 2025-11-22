using SV22T1080013.DomainModels;

namespace SV22T1080013.Admin.Models
{
    public class ProductPhotoEditModel : ProductPhoto
    {
        public IFormFile? UpLoadPhoto { get; set; }
    }
}
