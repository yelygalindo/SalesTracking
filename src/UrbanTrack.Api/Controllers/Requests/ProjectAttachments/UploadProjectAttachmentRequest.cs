using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace UrbanTrack.Api.Controllers.Requests.ProjectAttachments
{
    public sealed class UploadProjectAttachmentRequest
    {
        [Required(ErrorMessage = "El archivo es requerido.")]
        public IFormFile? File { get; set; }

        [Required(ErrorMessage = "El tipo de archivo es requerido.")]
        public string? AttachmentType { get; set; }

        public string? Caption { get; set; }

        [Required(ErrorMessage = "Debe indicar si el archivo será la portada.")]
        public bool? IsCover { get; set; }
    }
}
