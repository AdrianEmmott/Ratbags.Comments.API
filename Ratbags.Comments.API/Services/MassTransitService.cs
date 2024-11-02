using MassTransit;
using Ratbags.Comments.API.Interfaces;
using Ratbags.Core.Events.Accounts;

namespace Ratbags.Comments.API.Services
{
    public class MassTransitService : IMassTransitService
    {
        private readonly IRequestClient<UserFullNameRequest> _massTrasitClient;
        private readonly ILogger<MassTransitService> _logger;

        public MassTransitService(
            IRequestClient<UserFullNameRequest> massTrasitClient,
            ILogger<MassTransitService> logger)
        {
            _massTrasitClient = massTrasitClient;
            _logger = logger;
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
                _logger.LogError($"error getting username details via massTransit: {e.Message}");
                throw;
            }
        }
    }
}
