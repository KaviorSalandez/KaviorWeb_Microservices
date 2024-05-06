using Kavior.Services.RewardAPI.Message;

namespace Kavior.Services.RewardAPI.Service
{
    public interface IRewardService
    {
        Task UpdateRewards(RewardsMessage rewardsMessage);

    }
}
