namespace Jewelry_Model.Payload.Request.ProductSize;

public class UpdateProductSizeRequest
{
    public double? Price { get; set; }
    
    public int? Quantity { get; set; }
    public bool? IsActive { get; set; }
}