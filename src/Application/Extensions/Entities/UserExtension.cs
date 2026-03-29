using Application.Users.Update;
using Domain.Entities;

namespace Application.Extensions.Entities;

public static class UserExtension
{
    public static void UpdateData(this User user, UpdateUserCommand command)
    {
        user.CompleteName = command.Name;
        user.UserName = command.UserName;
        user.Email = command.Email;
        user.Phone = command.Phone;
        user.City = command.City;
        user.Country = command.Country;
        user.State = command.State;
        user.Language = command.Language;
        user.TimeZone = command.TimeZone;
        user.LastUpdateDate = DateTime.UtcNow;
    }
}
