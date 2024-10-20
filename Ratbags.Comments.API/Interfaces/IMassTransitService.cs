using Ratbags.Core.Events.Accounts;

namespace Ratbags.Comments.API.Interfaces;

public interface IMassTransitService
{
    Task<string> GetUserNameDetailsAsync(Guid id);
}
