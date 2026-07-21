using SalesTracking.Application.UseCases.ProjectMaterials.Results;
using UrbanTrack.Api.Controllers.Responses.ProjectMaterials;

namespace UrbanTrack.Api.Controllers.Responses.Mappers;

public static class ProjectMaterialsResponseMapper
{
    public static ProjectMaterialsSummaryResponse ToResponse(this GetProjectMaterialsSummaryResult result) => new()
    {
        Items = result.Items.Select(item => new ProjectMaterialSummaryItemResponse
        {
            ProductExternalId = item.ProductExternalId,
            ProductName = item.ProductName,
            Unit = item.Unit,
            CommittedQuantity = item.CommittedQuantity,
            DeliveredQuantity = item.DeliveredQuantity,
            PendingQuantity = item.PendingQuantity
        }).ToList()
    };
}
