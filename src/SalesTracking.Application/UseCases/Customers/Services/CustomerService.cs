using SalesTracking.Application.Common.ExternalIds;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.Customers.Comands;
using SalesTracking.Application.UseCases.Customers.Interfaces;
using SalesTracking.Application.UseCases.Customers.Mappers;
using SalesTracking.Application.UseCases.Customers.Models;
using SalesTracking.Application.UseCases.Customers.Results;
using SalesTracking.Domain.Entities;
using SalesTracking.Domain.Enums;
using System.Linq;

namespace SalesTracking.Application.UseCases.Customers.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repo;
        private readonly ICurrentUser _currentUser;

        public CustomerService(ICustomerRepository repo, ICurrentUser currentUser)
        {
            _repo = repo;
            _currentUser = currentUser;
        }

        public async Task<GetCustomersResult> GetCustomersAsync(GetCustomersCommand command)
        {
            int page = command.Page <= 0 ? 1 : command.Page;
            int pageSize = command.PageSize <= 0 ? 20 : command.PageSize;
            CustomerStatus? status = null;

            if (!string.IsNullOrWhiteSpace(command.Status))
            {
                if (!CustomerStatusParser.TryParse(command.Status, out CustomerStatus parsedStatus))
                {
                    return new GetCustomersResult
                    {
                        Items = new List<CustomerSummaryResult>(),
                        Page = command.Page,
                        PageSize = command.PageSize,
                        TotalItems = 0
                    };
                }

                status = parsedStatus;
            }

            GetCustomersFilter getCustomersFilter = new GetCustomersFilter
            {
                Status = status,
                SellerExternalId = command.SellerExternalId,
                Search = command.Search,
                Page = page,
                PageSize = pageSize
            };

            CustomerPagedList customers = await _repo.GetCustomersAsync(getCustomersFilter);

            return new GetCustomersResult
            {
                Items = customers.Items.Select(x => new CustomerSummaryResult
                {
                    Id = x.Id,
                    ExternalId = x.ExternalId,
                    Name = x.Name,
                    CompanyName = x.CompanyName,
                    Phone = x.Phone,
                    Email = x.Email,
                    Status = x.Status,
                    SellerResult = new SellerResult
                    {
                        Id = x.Seller?.Id,
                        ExternalId = x.Seller?.ExternalId,
                        Name = x.Seller?.Name
                    },
                    CreatedAtUtc = x.CreatedAtUtc
                }).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalItems = customers.TotalItems,
                TotalPages = (int)Math.Ceiling(customers.TotalItems / (double)pageSize)
            };
        }

        public async Task<CustomerDetailResult?> GetCustomerByIdAsync(GetCustomerByIdCommand command)
        {
            Customer? customer = await _repo.GetCustomerByExternalIdAsync(command.ExternalId);

            if (customer == null)
                return null;

            return new CustomerDetailResult
            {
                Id = customer.Id,
                ExternalId = customer.ExternalId,
                Name = customer.Name,
                CompanyName = customer.CompanyName,
                Phone = customer.Phone,
                Email = customer.Email,
                Status = customer.Status,
                SellerResult = new SellerResult
                {
                    Id = customer.Seller?.Id,
                    ExternalId = customer.Seller?.ExternalId,
                    Name = customer.Seller?.Name
                },
                Address = customer.Address,
                Latitude = customer.Latitude,
                Longitude = customer.Longitude,
                CreatedAtUtc = customer.CreatedAtUtc,
                Notes = customer.Notes.Select(n => new CustomerNoteResult
                {
                    Id = n.Id,
                    ExternalId = n.ExternalId,
                    Text = n.Text,
                    Author = new AuthorNoteResult(n.Author.Id, n.Author.ExternalId, n.Author.Name),
                    CreatedAtUtc = n.CreatedAtUtc
                }).ToList(),
                Reminders = customer.Reminders.Select(r =>
                    new SalesTracking.Application.UseCases.CustomerReminders.Results.CustomerReminderResult
                    {
                        Id = r.Id,
                        ExternalId = r.ExternalId,
                        Text = r.Text,
                        Customer = r.Customer,
                        Completed = r.Completed,
                        ReminderAtUtc = r.ReminderAtUtc
                }).ToList()
            };
        }

        public async Task<CreateCustomerResult> CreateCustomerAsync(CreateCustomerCommand command)
        {
            if (command == null)
            {
                return new CreateCustomerResult
                {
                    Succeeded = false,
                    Message = "La solicitud no es válida."
                };
            }

            if (string.IsNullOrWhiteSpace(command.Name))
            {
                return new CreateCustomerResult
                {
                    Succeeded = false,
                    Message = "El nombre del cliente es requerido."
                };
            }

            if (command.Latitude is < -90 or > 90)
            {
                return new CreateCustomerResult
                {
                    Succeeded = false,
                    Message = "La latitud no es válida."
                };
            }

            if (command.Longitude is < -180 or > 180)
            {
                return new CreateCustomerResult
                {
                    Succeeded = false,
                    Message = "La longitud no es válida."
                };
            }

            string externalId = ExternalIdGenerator.New(ExternalIdPrefixes.Customer);

            var customer = new CreateCustomer
            {
                ExternalId = externalId,
                Name = command.Name.Trim(),
                CompanyName = command.CompanyName?.Trim(),
                Phone = command.Phone?.Trim(),
                Email = command.Email?.Trim(),
                SellerExternalId = IsSeller
                    ? _currentUser.UserExternalId
                    : Normalize(command.SellerExternalId) ?? _currentUser.UserExternalId,
                Address = command.Address?.Trim(),
                Latitude = command.Latitude,
                Longitude = command.Longitude,
                Status = CustomerStatus.Prospect,
                CreatedByUserId = command.CreatedByUserId
            };

            CreateCustomer created = await _repo.CreateCustomerAsync(customer);

            if (!created.Succeeded)
            {
                return new CreateCustomerResult
                {
                    Succeeded = false,
                    Message = created.Message ?? "No se pudo crear el cliente."
                };
            }

            return new CreateCustomerResult
            {
                Succeeded = true,
                Id = created.ExternalId,
                Message = "Cliente creado correctamente."
            };
        }

        public Task<IReadOnlyList<CustomerStatusResult>> GetCustomerStatusesAsync()
        {
            IReadOnlyList<CustomerStatusResult> statuses = Enum
               .GetValues<CustomerStatus>()
               .Select(status => new CustomerStatusResult
               {
                   Value = status.ToValue(),
                   Label = status.ToLabel()
               })
               .ToList();

            return Task.FromResult(statuses);
        }

        public async Task<UpdateCustomerResult> UpdateCustomerAsync(UpdateCustomerCommand command)
        {
            if (command == null)
            {
                return new UpdateCustomerResult
                {
                    Succeeded = false,
                    Message = "La solicitud no es válida."
                };
            }

            if (string.IsNullOrWhiteSpace(command.ExternalId))
            {
                return new UpdateCustomerResult
                {
                    Succeeded = false,
                    Message = "El cliente es requerido."
                };
            }

            if (string.IsNullOrWhiteSpace(command.Name))
            {
                return new UpdateCustomerResult
                {
                    Succeeded = false,
                    Message = "El nombre del cliente es requerido."
                };
            }

            if (command.Latitude is < -90 or > 90)
            {
                return new UpdateCustomerResult
                {
                    Succeeded = false,
                    Message = "La latitud no es válida."
                };
            }

            if (command.Longitude is < -180 or > 180)
            {
                return new UpdateCustomerResult
                {
                    Succeeded = false,
                    Message = "La longitud no es válida."
                };
            }

            var customer = new UpdateCustomer
            {
                ExternalId = command.ExternalId.Trim(),
                Name = command.Name.Trim(),
                CompanyName = command.CompanyName?.Trim(),
                Phone = command.Phone?.Trim(),
                Email = command.Email?.Trim(),
                SellerId = IsSeller
                    ? _currentUser.UserExternalId
                    : Normalize(command.SellerExternalId),
                Address = command.Address?.Trim(),
                Latitude = command.Latitude,
                Longitude = command.Longitude,
                UpdatedByUserId = command.UpdatedByUserId
            };

            UpdateCustomer updated = await _repo.UpdateCustomerAsync(customer);

            return new UpdateCustomerResult
            {
                Succeeded = updated.Succeeded,
                NotFound = updated.NotFound,
                Message = updated.Message ?? "Cliente actualizado correctamente."
            };
        }

        public async Task<ChangeCustomerStatusResult> ChangeCustomerStatusAsync(ChangeCustomerStatusCommand command)
        {
            if (command == null)
            {
                return new ChangeCustomerStatusResult
                {
                    Succeeded = false,
                    Message = "La solicitud no es válida."
                };
            }

            if (string.IsNullOrWhiteSpace(command.ExternalId))
            {
                return new ChangeCustomerStatusResult
                {
                    Succeeded = false,
                    Message = "El cliente es requerido."
                };
            }

            if (!Enum.IsDefined(typeof(CustomerStatus), command.StatusId))
            {
                return new ChangeCustomerStatusResult
                {
                    Succeeded = false,
                    Message = "Estado de cliente inválido."
                };
            }

            CustomerStatus status = (CustomerStatus)command.StatusId;

            bool updated = await _repo.ChangeCustomerStatusAsync(
                command.ExternalId.Trim(), status, command.ChangedByUserId);

            if (!updated)
            {
                return new ChangeCustomerStatusResult
                {
                    Succeeded = false,
                    NotFound = true,
                    Message = "Cliente no encontrado."
                };
            }

            return new ChangeCustomerStatusResult
            {
                Succeeded = true,
                Message = $"Estado cambiado a '{status.ToValue()}-{status.ToLabel()}'."
            };
        }

        public async Task<DeleteCustomerResult> DeleteCustomerAsync(DeleteCustomerCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.ExternalId))
            {
                return new DeleteCustomerResult
                {
                    Succeeded = false,
                    Message = "El cliente es requerido."
                };
            }

            bool deleted = await _repo.DeleteCustomerAsync(command.ExternalId.Trim(), command.DeletedByUserId);

            if (!deleted)
            {
                return new DeleteCustomerResult
                {
                    Succeeded = false,
                    NotFound = true,
                    Message = "Cliente no encontrado."
                };
            }

            return new DeleteCustomerResult
            {
                Succeeded = true,
                Message = "Cliente eliminado correctamente."
            };
        }

        private bool IsSeller =>
            _currentUser.Roles.Contains("seller", StringComparer.OrdinalIgnoreCase);

        private static string? Normalize(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
