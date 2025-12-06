using Jewelry_Model.Entity;

namespace Jewelry_Model.Payload.Response.Product;

public class CreateProductResponse
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }
    public List<Entity.ProductSize> ProductSizes { get; set; } = new List<Entity.ProductSize>();
}