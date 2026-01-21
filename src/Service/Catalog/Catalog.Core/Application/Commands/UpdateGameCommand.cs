using MediatR;

namespace Catalog.Core.Application.Commands;

public sealed class UpdateGameCommand : IRequest<bool>
{
    public int IdGame { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
