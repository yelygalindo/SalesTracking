using SalesTracking.Application.UseCases.ProjectVisits.Comands;
using SalesTracking.Application.UseCases.ProjectVisits.Results;
using UrbanTrack.Api.Controllers.Requests.ProjectVisits;
using UrbanTrack.Api.Controllers.Responses.ProjectVisits;

namespace UrbanTrack.Api.Controllers.Requests.Mappers;

public static class ProjectVisitApiMapper
{
    public static CreateProjectVisitCommand ToApplication(
        this CreateProjectVisitRequest request,
        string projectExternalId,
        int sellerUserId) => new(
            projectExternalId,
            request.VisitedAtUtc,
            request.Latitude,
            request.Longitude,
            request.Notes,
            request.Result,
            sellerUserId);

    public static GetProjectVisitsCommand ToApplication(
        this GetProjectVisitsRequest request,
        string projectExternalId) => new(
            projectExternalId,
            request.SellerExternalId,
            request.From,
            request.To);

    public static ProjectVisitResponse ToResponse(this ProjectVisitResult result) => new()
    {
        ExternalId = result.ExternalId,
        VisitedAtUtc = result.VisitedAtUtc,
        Latitude = result.Latitude,
        Longitude = result.Longitude,
        Notes = result.Notes,
        Result = result.Result,
        SellerExternalId = result.SellerExternalId,
        SellerName = result.SellerName
    };
}
