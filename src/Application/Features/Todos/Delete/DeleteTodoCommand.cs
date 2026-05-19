using Application.Common.Abstractions.Messaging;

namespace Application.Todos.Delete;

public sealed record DeleteTodoCommand(Guid TodoItemId) : ICommand;
