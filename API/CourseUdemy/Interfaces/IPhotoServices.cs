using CloudinaryDotNet.Actions;

namespace CourseUdemy.Interfaces
{
    public interface IPhotoServices
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile formFile);
        Task<DeletionResult> DeletePhotoAsync(string publicid);
    }
}
