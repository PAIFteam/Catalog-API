using Catalog.Core.Application.Commands;
using Catalog.Core.Domain.Interfaces;
using MediatR;

namespace Catalog.Core.Application.Handlers;

public sealed class DeleteGameCommandHandler(IGameAdminRepository repository) : IRequestHandler<DeleteGameCommand, bool>
{
    public Task<bool> Handle(DeleteGameCommand request, CancellationToken cancellationToken)
        => repository.DeleteAsync(request.IdGame);
}
