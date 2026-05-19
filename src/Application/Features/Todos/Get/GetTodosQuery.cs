using Application.Common.Abstractions.Messaging;

namespace Application.Todos.Get;

public sealed record GetTodosQuery(Guid UserId) : IQuery<List<TodoResponse>>;
