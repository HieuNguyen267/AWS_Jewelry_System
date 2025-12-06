namespace Jewelry_Model.Payload.Response.Size;

public class CreateSizeResponse
{
    public Guid Id { get; set; }
    public string? Label { get; set; }
    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }
}