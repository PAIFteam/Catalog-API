using Catalog.Core.Application.DTOs;
using MediatR;

namespace Catalog.Core.Application.Queries;

public sealed record GetGameByUIdQuery(int IdGame) : IRequest<GameDto?>;
