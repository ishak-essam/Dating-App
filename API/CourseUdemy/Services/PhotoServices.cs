using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CourseUdemy.Helpers;
using CourseUdemy.Interfaces;
using Microsoft.Extensions.Options;

namespace CourseUdemy.Services
{
    public class PhotoServices : IPhotoServices
    {
        public readonly Cloudinary _Cloud;
        public PhotoServices(IOptions<CloudinarySettings> options)
        {
            var acc=new Account(options.Value.CloudName,options.Value.ApiKey,options.Value.ApiSecret);
            _Cloud = new Cloudinary(acc);
        }


        public async Task<ImageUploadResult> AddPhotoAsync ( IFormFile formFile )
        {
            var uploadResulat=new ImageUploadResult();
            if ( formFile.Length > 0 ) {
                using var stream=formFile.OpenReadStream();
                var uploadParam= new ImageUploadParams
                {

                    File=new FileDescription(formFile.FileName,stream),
                    Transformation=new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    Folder="da-net7"
                };
                uploadResulat = await _Cloud.UploadAsync (uploadParam);
            }
            return uploadResulat;
        }

        public async Task<DeletionResult> DeletePhotoAsync ( string publicid )
        {
            var DeleteParams= new DeletionParams(publicid);
            return await _Cloud.DestroyAsync (DeleteParams);
        }
    }
}
