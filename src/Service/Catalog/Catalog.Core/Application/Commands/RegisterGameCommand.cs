using MediatR;

namespace Catalog.Core.Application.Commands;

public sealed class RegisterGameCommand : IRequest<bool>
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
