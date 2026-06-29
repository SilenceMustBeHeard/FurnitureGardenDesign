namespace FurnitureGardenDesign.Api.Common;

public class ReviewResponse
{
    public string Username { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}