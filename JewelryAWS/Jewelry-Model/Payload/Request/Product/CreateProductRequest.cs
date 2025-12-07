using System.Collections;
using Jewelry_Model.Payload.Request.ProductSize;
using Jewelry_Model.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jewelry_Model.Payload.Request.Product;

public class CreateProductRequest
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public IFormFile Image { get; set; }
}