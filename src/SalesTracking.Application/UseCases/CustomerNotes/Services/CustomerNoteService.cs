using SalesTracking.Application.Common.ExternalIds;
using SalesTracking.Application.UseCases.CustomerNotes.Comands;
using SalesTracking.Application.UseCases.CustomerNotes.Interfaces;
using SalesTracking.Application.UseCases.CustomerNotes.Models;
using SalesTracking.Application.UseCases.CustomerNotes.Results;
using SalesTracking.Domain.Entities;

namespace SalesTracking.Application.UseCases.CustomerNotes.Services
{
    public class CustomerNoteService : ICustomerNoteService
    {
        private readonly ICustomerNoteRepository _repo;

        public CustomerNoteService(ICustomerNoteRepository repo)
        {
            _repo = repo;
        }

        public async Task<AddCustomerNoteResult> AddNoteAsync(AddCustomerNoteCommand command)
        {
            if (command == null)
            {
                return new AddCustomerNoteResult
                {
                    Succeeded = false,
                    Message = "La solicitud no es válida."
                };
            }

            if (string.IsNullOrWhiteSpace(command.CustomerExternalId))
            {
                return new AddCustomerNoteResult
                {
                    Succeeded = false,
                    Message = "El cliente es requerido."
                };
            }

            if (string.IsNullOrWhiteSpace(command.Text))
            {
                return new AddCustomerNoteResult
                {
                    Succeeded = false,
                    Message = "La nota es requerida."
                };
            }

            if (command.AuthorUserId <= 0)
            {
                return new AddCustomerNoteResult
                {
                    Succeeded = false,
                    Message = "El autor es requerido."
                };
            }

            CreateCustomerNote note = new CreateCustomerNote
            {
                ExternalId = ExternalIdGenerator.New(ExternalIdPrefixes.CustomerNote),
                CustomerExternalId = command.CustomerExternalId,
                Text = command.Text.Trim(),
                AuthorUserId = command.AuthorUserId
            };

            ResponseCreateCustomerNote created = await _repo.AddNoteAsync(note);

            if (!created.Succeeded)
            {
                return new AddCustomerNoteResult
                {
                    Succeeded = false,
                    NotFound = created.NotFound,
                    Message = created.Message ?? "No se pudo agregar la nota."
                };
            }

            return new AddCustomerNoteResult
            {
                Succeeded = true,
                Id = created.CreateCustomerNote.ExternalId,
                Message = "Nota agregada correctamente."
            };
        }        

        public async Task<IReadOnlyList<CustomerNote>> GetNotesAsync(GetCustomerNotesCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.CustomerExternalId))
                return new List<CustomerNote>();

            return await _repo.GetNotesAsync(command.CustomerExternalId);
        }
    }
}
