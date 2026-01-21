using Catalog.Core.Application.Commands;
using Catalog.Core.Domain.Interfaces;
using MediatR;

namespace Catalog.Core.Application.Handlers;

public sealed class RegisterGameCommandHandler(IGameAdminRepository repository) : IRequestHandler<RegisterGameCommand, bool>
{
    public Task<bool> Handle(RegisterGameCommand request, CancellationToken cancellationToken)
        => repository.RegisterAsync(request.Name, request.Price);
}
