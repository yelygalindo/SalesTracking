using SalesTracking.Application.Common.ExternalIds;
using SalesTracking.Application.UseCases.CustomerReminders.Comands;
using SalesTracking.Application.UseCases.CustomerReminders.Interfaces;
using SalesTracking.Application.UseCases.CustomerReminders.Models;
using SalesTracking.Application.UseCases.CustomerReminders.Results;
using SalesTracking.Domain.Entities;

namespace SalesTracking.Application.UseCases.CustomerReminders.Services
{
    public class CustomerReminderService: ICustomerReminderService
    {
        private readonly ICustomerReminderRepository _repo;

        public CustomerReminderService(ICustomerReminderRepository repo)
        {
            _repo = repo;
        }

        public async Task<IReadOnlyList<CustomerReminderResult>> GetRemindersAsync(
            GetCustomerRemindersCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.CustomerExternalId))
                return new List<CustomerReminderResult>();

            IEnumerable<CustomerReminder> customerReminder = await _repo.GetRemindersAsync(
                command.CustomerExternalId.Trim());
            return customerReminder.Select(r => new CustomerReminderResult
            {
                Id = r.Id,
                ExternalId = r.ExternalId,
                Text = r.Text,
                Customer = r.Customer,
                Completed = r.Completed,
                ReminderAtUtc = r.ReminderAtUtc
            }).ToList();
        }

        public async Task<CreateCustomerReminderResult> CreateReminderAsync(
            CreateCustomerReminderCommand command)
        {
            if (command == null)
            {
                return new CreateCustomerReminderResult
                {
                    Succeeded = false,
                    Message = "La solicitud no es válida."
                };
            }

            if (string.IsNullOrWhiteSpace(command.CustomerExternalId))
            {
                return new CreateCustomerReminderResult
                {
                    Succeeded = false,
                    Message = "El cliente es requerido."
                };
            }

            if (string.IsNullOrWhiteSpace(command.Text))
            {
                return new CreateCustomerReminderResult
                {
                    Succeeded = false,
                    Message = "El texto del recordatorio es requerido."
                };
            }

            if (command.ReminderAtUtc <= DateTime.UtcNow)
            {
                return new CreateCustomerReminderResult
                {
                    Succeeded = false,
                    Message = "La fecha del recordatorio debe ser futura."
                };
            }

            if (string.IsNullOrWhiteSpace(command.AssignedToId))
            {
                return new CreateCustomerReminderResult
                {
                    Succeeded = false,
                    Message = "El usuario asignado es requerido."
                };
            }

            var reminder = new CreateCustomerReminder
            {
                ExternalId = ExternalIdGenerator.New(ExternalIdPrefixes.CustomerReminder),
                CustomerExternalId = command.CustomerExternalId,
                Text = command.Text.Trim(),
                ReminderAtUtc = command.ReminderAtUtc,
                AssignedToId = command.AssignedToId,
                CreatedByUserId = command.CreatedByUserId
            };

            CreateCustomerReminder created = await _repo.CreateReminderAsync(reminder);

            if (!created.Succeeded)
            {
                return new CreateCustomerReminderResult
                {
                    Succeeded = false,
                    NotFound = created.NotFound,
                    Message = created.Message ?? "No se pudo crear el recordatorio."
                };
            }

            return new CreateCustomerReminderResult
            {
                Succeeded = true,
                Id = created.ExternalId,
                Message = "Recordatorio creado correctamente."
            };
        }

        public async Task<CompleteCustomerReminderResult> CompleteReminderAsync(
            CompleteCustomerReminderCommand command)
        {
            if (command == null ||
                string.IsNullOrWhiteSpace(command.CustomerExternalId) ||
                string.IsNullOrWhiteSpace(command.ReminderExternalId))
            {
                return new CompleteCustomerReminderResult
                {
                    Succeeded = false,
                    Message = "La solicitud no es válida."
                };
            }

            bool completed = await _repo.CompleteReminderAsync(
                command.CustomerExternalId,
                command.ReminderExternalId,
                command.CompletedByUserId);

            if (!completed)
            {
                return new CompleteCustomerReminderResult
                {
                    Succeeded = false,
                    NotFound = true,
                    Message = "Recordatorio no encontrado."
                };
            }

            return new CompleteCustomerReminderResult
            {
                Succeeded = true,
                Message = "Recordatorio marcado como completado."
            };
        }
    }
}
