using MassTransit;
using Ratbags.Comments.API.Interfaces;
using Ratbags.Core.Events.Accounts;

namespace Ratbags.Comments.API.Services
{
    public class MassTransitService : IMassTransitService
    {
        private readonly IRequestClient<UserFullNameRequest> _massTrasitClient;

        public MassTransitService(
            IRequestClient<UserFullNameRequest> massTrasitClient)
        {
            _massTrasitClient = massTrasitClient;
        }

        public async Task<string> GetUserNameDetailsAsync(Guid id)
        {
            try
            {
                var response = await _massTrasitClient
                                .GetResponse<UserFullNameResponse>
                                (new UserFullNameRequest
                                {
                                    UserId = id
                                });

                return response.Message.FullName;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
