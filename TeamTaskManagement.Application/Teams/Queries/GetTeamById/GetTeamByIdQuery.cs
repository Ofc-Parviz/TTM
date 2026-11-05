using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.Teams.Responses;

namespace TeamTaskManagement.Application.Teams.Queries.GetTeamById;

/// <summary>
/// کوئری دریافت جزئیات یک تیم
/// </summary>
public sealed record GetTeamByIdQuery(Guid TeamId) : IQuery<TeamResponse>;