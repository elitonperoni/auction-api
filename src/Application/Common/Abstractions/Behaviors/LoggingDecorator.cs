using Application.Common.Abstractions.Messaging;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel;

namespace Application.Common.Abstractions.Behaviors;

internal static partial class LoggingDecorator
{
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ILogger<CommandHandler<TCommand, TResponse>> logger)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;

            LogProcessing(logger, "command", commandName);

            Result<TResponse> result = await innerHandler.Handle(command, cancellationToken);

            LogCompletion(logger, "command", commandName, result);

            return result;
        }
    }

    internal sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,
        ILogger<CommandBaseHandler<TCommand>> logger)
        : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;

            LogProcessing(logger, "command", commandName);

            Result result = await innerHandler.Handle(command, cancellationToken);

            LogCompletion(logger, "command", commandName, result);

            return result;
        }
    }

    internal sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler,
        ILogger<QueryHandler<TQuery, TResponse>> logger)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            string queryName = typeof(TQuery).Name;

            LogProcessing(logger, "query", queryName);

            Result<TResponse> result = await innerHandler.Handle(query, cancellationToken);

            LogCompletion(logger, "query", queryName, result);

            return result;
        }
    }

    private static void LogCompletion(ILogger logger, string kind, string name, Result result)
    {
        if (result.IsSuccess)
        {
            LogCompleted(logger, kind, name);
            return;
        }

        using (LogContext.PushProperty("Error", result.Error, true))
        {
            LogCompletedWithError(logger, kind, name);
        }
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Processing {Kind} {Name}")]
    private static partial void LogProcessing(ILogger logger, string kind, string name);

    [LoggerMessage(Level = LogLevel.Information, Message = "Completed {Kind} {Name}")]
    private static partial void LogCompleted(ILogger logger, string kind, string name);

    [LoggerMessage(Level = LogLevel.Error, Message = "Completed {Kind} {Name} with error")]
    private static partial void LogCompletedWithError(ILogger logger, string kind, string name);
}
