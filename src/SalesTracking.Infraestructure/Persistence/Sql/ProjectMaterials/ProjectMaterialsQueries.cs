namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectMaterials;

internal static class ProjectMaterialsQueries
{
    public const string ProjectExists = @"
SELECT COUNT(1)
FROM Projects
WHERE ExternalId = @ProjectExternalId
  AND CompanyId = @CompanyId
  AND IsDeleted = 0;";

    public const string GetSummary = @"
SELECT
    product.ExternalId AS ProductExternalId,
    product.Name AS ProductName,
    unit.Name AS Unit,
    SUM(item.Quantity) AS CommittedQuantity,
    SUM(item.DeliveredQuantity) AS DeliveredQuantity,
    CASE
        WHEN SUM(item.Quantity) > SUM(item.DeliveredQuantity)
            THEN SUM(item.Quantity) - SUM(item.DeliveredQuantity)
        ELSE 0
    END AS PendingQuantity
FROM Deliveries delivery
INNER JOIN Projects project ON project.Id = delivery.ProjectId
INNER JOIN DeliveryItems item ON item.DeliveryId = delivery.Id AND item.IsDeleted = 0
INNER JOIN Products product ON product.Id = item.ProductId
INNER JOIN Units unit ON unit.Id = item.UnitId
WHERE project.ExternalId = @ProjectExternalId
  AND project.CompanyId = @CompanyId
  AND delivery.CompanyId = @CompanyId
  AND item.CompanyId = @CompanyId
  AND product.CompanyId = @CompanyId
  AND unit.CompanyId = @CompanyId
  AND delivery.IsDeleted = 0
GROUP BY product.ExternalId, product.Name, unit.Id, unit.Name
ORDER BY product.Name, unit.Name;";
}
