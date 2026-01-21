using Catalog.Core.Application.DTOs;
using Catalog.Core.Application.Queries;
using Catalog.Core.Domain.Interfaces;
using MediatR;

namespace Catalog.Core.Application.Handlers;

public sealed class GetGameByUIdQueryHandler(IGameAdminRepository repository) : IRequestHandler<GetGameByUIdQuery, GameDto?>
{
    public Task<GameDto?> Handle(GetGameByUIdQuery request, CancellationToken cancellationToken)
        => repository.GetByIdAsync(request.IdGame);
}
