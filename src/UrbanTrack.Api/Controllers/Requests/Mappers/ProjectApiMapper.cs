using SalesTracking.Application.UseCases.Projects.Comands;
using SalesTracking.Application.UseCases.Projects.Results;
using SalesTracking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrbanTrack.Api.Controllers.Requests.Products;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.Pagination;
using UrbanTrack.Api.Controllers.Responses.Projects;

namespace UrbanTrack.Api.Controllers.Requests.Mappers
{
    public static class ProjectApiMapper
    {
        public static CreateProjectCommand ToApplication(this CreateProjectRequest request)
        {
            return new CreateProjectCommand(
                request.Name,
                request.Description,
                request.CustomerExternalId,
                request.SellerExternalId,
                request.EstimatedAmount,
                request.StartDateUtc,
                request.ExpectedCloseDateUtc,
                request.ProgressPercentage,
                request.ActualCloseDateUtc,
                request.Address,
                request.Latitude,
                request.Longitude);
        }

        public static GetProjectsCommand ToApplication(this GetProjectsRequest request)
        {
            return new GetProjectsCommand(
                request.Status,
                request.CustomerExternalId,
                request.SellerExternalId,
                request.Page,
                request.PageSize);
        }

        public static UpdateProjectCommand ToApplication(
            this UpdateProjectRequest request,
            string externalId)
        {
            return new UpdateProjectCommand
            {
                ExternalId = externalId,
                Name = request.Name,
                Description = request.Description,
                CustomerExternalId = request.CustomerExternalId,
                SellerExternalId = request.SellerExternalId,
                EstimatedAmount = request.EstimatedAmount,
                StartDateUtc = request.StartDateUtc,
                ExpectedCloseDateUtc = request.ExpectedCloseDateUtc,
                ProgressPercentage = request.ProgressPercentage,
                ActualCloseDateUtc = request.ActualCloseDateUtc,
                Address = request.Address,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            };
        }

        public static ChangeProjectStatusCommand ToApplication(
            this ChangeProjectStatusRequest request,
            string externalId)
        {
            return new ChangeProjectStatusCommand
            {
                ExternalId = externalId,
                StatusId = request.StatusId
            };
        }

        public static IdMessageResponse ToResponse(this CreateProjectResult result)
        {
            return new IdMessageResponse
            {
                Id = result.Id,
                Message = result.Message
            };
        }

        public static ProjectSummaryResponse ToResponse(this ProjectSummaryResult result)
        {
            return new ProjectSummaryResponse
            {
                Id = result.Id,
                ExternalId = result.ExternalId,
                Name = result.Name,
                Description = result.Description,
                CustomerExternalId = result.CustomerId,
                CustomerName = result.CustomerName,
                SellerExternalId = result.SellerId,
                SellerName = result.SellerName,
                Status = result.Status,
                EstimatedAmount = result.EstimatedAmount,
                StartDateUtc = result.StartDateUtc,
                ExpectedCloseDateUtc = result.ExpectedCloseDateUtc,
                ProgressPercentage = result.ProgressPercentage,
                ActualCloseDateUtc = result.ActualCloseDateUtc,
                Address = result.Address,
                Latitude = result.Latitude,
                Longitude = result.Longitude,
                CreatedAtUtc = result.CreatedAtUtc
            };
        }

        public static ProjectDetailResponse ToResponse(this ProjectDetailResult result)
        {
            return new ProjectDetailResponse
            {
                Id = result.Id,
                ExternalId = result.ExternalId,
                Name = result.Name,
                Description = result.Description,
                CustomerExternalId = result.CustomerExternalId,
                CustomerName = result.CustomerName,
                SellerExternalId = result.SellerExternalId,
                SellerName = result.SellerName,
                Status = result.Status,
                EstimatedAmount = result.EstimatedAmount,
                StartDateUtc = result.StartDateUtc,
                ExpectedCloseDateUtc = result.ExpectedCloseDateUtc,
                ProgressPercentage = result.ProgressPercentage,
                ActualCloseDateUtc = result.ActualCloseDateUtc,
                Address = result.Address,
                Latitude = result.Latitude,
                Longitude = result.Longitude,
                CreatedAtUtc = result.CreatedAtUtc
            };
        }

        public static PagedResponse<ProjectSummaryResponse> ToResponse(
            this ProjectPagedList result)
        {
            return new PagedResponse<ProjectSummaryResponse>
            {
                Items = result.Items.Select(x => x.ToResponse()).ToList(),
                Pagination = new PaginationResponse
                {
                    Page = result.Page,
                    PageSize = result.PageSize,
                    TotalItems = result.TotalItems,
                    TotalPages = result.TotalPages
                }
            };
        }
    }
}
