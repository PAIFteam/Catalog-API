using MediatR;

namespace Catalog.Core.Application.Commands;

public sealed record DeleteGameCommand(int IdGame) : IRequest<bool>;
