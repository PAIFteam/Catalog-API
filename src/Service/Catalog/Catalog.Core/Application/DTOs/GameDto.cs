namespace Catalog.Core.Application.DTOs;

public sealed class GameDto
{
    public int IdGame { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
