using Catalog.Core.Application.Commands;
using Catalog.Core.Domain.Interfaces;
using MediatR;

namespace Catalog.Core.Application.Handlers;

public sealed class UpdateGameCommandHandler(IGameAdminRepository repository) : IRequestHandler<UpdateGameCommand, bool>
{
    public Task<bool> Handle(UpdateGameCommand request, CancellationToken cancellationToken)
        => repository.UpdateAsync(request.IdGame, request.Name, request.Price);
}
