using Microsoft.AspNetCore.Http;

namespace UrbanTrack.Api.Controllers.Requests.ProjectAttachments
{
    public sealed class UploadProjectAttachmentRequest
    {
        public IFormFile? File { get; set; }
        public string AttachmentType { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public bool IsCover { get; set; }
        public string UploadedByUserExternalId { get; set; } = string.Empty;
    }
}
