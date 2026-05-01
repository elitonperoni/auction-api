using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Users.GetById;

internal sealed class GetUserByIdQueryHandler(IApplicationDbContext context, IUserContext userContext)
    : IQueryHandler<GetUserByIdQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        if (query.UserId != userContext.UserId)
        {
            return Result.Failure<UserResponse>(UserErrors.Unauthorized());
        }

        UserResponse? user = await context.Users
            .Where(u => u.Id == query.UserId)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                UserName = u.UserName,
                CompleteName = u.CompleteName,
                Email = u.Email,
                City = u.City,
                Country = u.Country,    
                State = u.State,
                LanguageId = u.Language,
                Phone = u.Phone,
                TimeZone = u.TimeZone,
                MemberSince = u.CreatedAt,
                TelegramConfigured = u.TelegramChatId != null,
                UserNotifications = u.UserNotifications.Select(p => p.NotificationTypeId).ToList()
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFound(query.UserId));
        }

        return user;
    }
}
