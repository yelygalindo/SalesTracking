using SalesTracking.Application.Common.ExternalIds;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.Common.Models;
using SalesTracking.Application.UseCases.ProjectAttachments.Comands;
using SalesTracking.Application.UseCases.ProjectAttachments.Interfaces;
using SalesTracking.Application.UseCases.ProjectAttachments.Models;
using SalesTracking.Application.UseCases.ProjectAttachments.Results;

namespace SalesTracking.Application.UseCases.ProjectAttachments.Services
{
    public sealed class ProjectAttachmentService : IProjectAttachmentService
    {
        private const long MaxFileSizeBytes = 10 * 1024 * 1024;
        private const string StorageProvider = "Local";

        private static readonly HashSet<string> AllowedAttachmentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "Photo",
            "Document",
            "Quote",
            "Other"
        };

        private static readonly Dictionary<string, string[]> AllowedContentTypesByExtension = new(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = new[] { "image/jpeg" },
            [".jpeg"] = new[] { "image/jpeg" },
            [".png"] = new[] { "image/png" },
            [".webp"] = new[] { "image/webp" },
            [".gif"] = new[] { "image/gif" },
            [".pdf"] = new[] { "application/pdf" }
        };

        private readonly IProjectAttachmentRepository _projectAttachmentRepository;
        private readonly IFileStorage _fileStorage;

        public ProjectAttachmentService(
            IProjectAttachmentRepository projectAttachmentRepository,
            IFileStorage fileStorage)
        {
            _projectAttachmentRepository = projectAttachmentRepository;
            _fileStorage = fileStorage;
        }

        public async Task<GetProjectAttachmentsResult> GetAsync(GetProjectAttachmentsCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.ProjectExternalId))
            {
                return new GetProjectAttachmentsResult
                {
                    Succeeded = false,
                    Message = "El proyecto es requerido."
                };
            }

            return await _projectAttachmentRepository.GetAsync(command.ProjectExternalId.Trim());
        }

        public async Task<ProjectAttachmentContentResult> GetContentAsync(GetProjectAttachmentContentCommand command)
        {
            if (command == null ||
                string.IsNullOrWhiteSpace(command.ProjectExternalId) ||
                string.IsNullOrWhiteSpace(command.AttachmentExternalId))
            {
                return new ProjectAttachmentContentResult
                {
                    Succeeded = false,
                    Message = "El archivo es requerido."
                };
            }

            ProjectAttachmentContentResult info = await _projectAttachmentRepository.GetContentInfoAsync(
                command.ProjectExternalId.Trim(),
                command.AttachmentExternalId.Trim());

            if (!info.Succeeded)
                return info;

            StoredFile? storedFile = await _fileStorage.OpenReadAsync(info.StorageKey);
            if (storedFile == null)
            {
                return new ProjectAttachmentContentResult
                {
                    Succeeded = false,
                    NotFound = true,
                    Message = "Archivo no encontrado."
                };
            }

            info.Content = storedFile.Content;
            return info;
        }

        public async Task<UploadProjectAttachmentResult> UploadAsync(UploadProjectAttachmentCommand command)
        {
            UploadProjectAttachmentResult? validation = ValidateUpload(command);
            if (validation != null)
                return validation;

            string extension = Path.GetExtension(command.FileName).ToLowerInvariant();
            string storageKey = $"projects/{command.ProjectExternalId.Trim()}/{Guid.NewGuid():N}{extension}";
            string externalId = ExternalIdGenerator.New(ExternalIdPrefixes.ProjectAttachment);

            await _fileStorage.SaveAsync(storageKey, command.Content);

            CreateProjectAttachment attachment = new CreateProjectAttachment
            {
                ExternalId = externalId,
                ProjectExternalId = command.ProjectExternalId.Trim(),
                FileName = Path.GetFileName(command.FileName).Trim(),
                StorageProvider = StorageProvider,
                StorageKey = storageKey,
                ContentType = command.ContentType.Trim(),
                SizeBytes = command.SizeBytes,
                AttachmentType = NormalizeAttachmentType(command.AttachmentType),
                Caption = command.Caption?.Trim(),
                IsCover = command.IsCover,
                UploadedByUserExternalId = command.UploadedByUserExternalId.Trim()
            };

            UploadProjectAttachmentResult result = await _projectAttachmentRepository.CreateAsync(attachment);
            if (!result.Succeeded)
                await _fileStorage.DeleteAsync(storageKey);

            return result;
        }

        public async Task<DeleteProjectAttachmentResult> DeleteAsync(DeleteProjectAttachmentCommand command)
        {
            if (command == null ||
                string.IsNullOrWhiteSpace(command.ProjectExternalId) ||
                string.IsNullOrWhiteSpace(command.AttachmentExternalId) ||
                string.IsNullOrWhiteSpace(command.DeletedByUserExternalId))
            {
                return new DeleteProjectAttachmentResult
                {
                    Succeeded = false,
                    Message = "El archivo y el usuario son requeridos."
                };
            }

            return await _projectAttachmentRepository.DeleteAsync(new DeleteProjectAttachmentCommand(
                command.ProjectExternalId.Trim(),
                command.AttachmentExternalId.Trim(),
                command.DeletedByUserExternalId.Trim()));
        }

        public async Task<SetProjectAttachmentCoverResult> SetCoverAsync(SetProjectAttachmentCoverCommand command)
        {
            if (command == null ||
                string.IsNullOrWhiteSpace(command.ProjectExternalId) ||
                string.IsNullOrWhiteSpace(command.AttachmentExternalId) ||
                string.IsNullOrWhiteSpace(command.UpdatedByUserExternalId))
            {
                return new SetProjectAttachmentCoverResult
                {
                    Succeeded = false,
                    Message = "El archivo y el usuario son requeridos."
                };
            }

            return await _projectAttachmentRepository.SetCoverAsync(new SetProjectAttachmentCoverCommand(
                command.ProjectExternalId.Trim(),
                command.AttachmentExternalId.Trim(),
                command.UpdatedByUserExternalId.Trim()));
        }

        private static UploadProjectAttachmentResult? ValidateUpload(UploadProjectAttachmentCommand command)
        {
            if (command == null)
                return FailedUpload("La solicitud no es valida.");

            if (string.IsNullOrWhiteSpace(command.ProjectExternalId))
                return FailedUpload("El proyecto es requerido.");

            if (string.IsNullOrWhiteSpace(command.UploadedByUserExternalId))
                return FailedUpload("El usuario que sube el archivo es requerido.");

            if (command.Content == null || command.Content == Stream.Null || command.SizeBytes <= 0)
                return FailedUpload("El archivo es requerido.");

            if (command.SizeBytes > MaxFileSizeBytes)
                return FailedUpload("El archivo supera el tamano permitido.");

            if (string.IsNullOrWhiteSpace(command.FileName))
                return FailedUpload("El nombre del archivo es requerido.");

            if (string.IsNullOrWhiteSpace(command.ContentType))
                return FailedUpload("El tipo de contenido es requerido.");

            string extension = Path.GetExtension(command.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedContentTypesByExtension.ContainsKey(extension))
                return FailedUpload("La extension del archivo no es valida.");

            if (!AllowedContentTypesByExtension[extension].Contains(command.ContentType, StringComparer.OrdinalIgnoreCase))
                return FailedUpload("El tipo de contenido del archivo no es valido.");

            if (string.IsNullOrWhiteSpace(command.AttachmentType) ||
                !AllowedAttachmentTypes.Contains(command.AttachmentType))
                return FailedUpload("El tipo de archivo no es valido.");

            return null;
        }

        private static string NormalizeAttachmentType(string attachmentType)
        {
            return AllowedAttachmentTypes.First(x => x.Equals(attachmentType, StringComparison.OrdinalIgnoreCase));
        }

        private static UploadProjectAttachmentResult FailedUpload(string message)
        {
            return new UploadProjectAttachmentResult
            {
                Succeeded = false,
                Message = message
            };
        }
    }
}
