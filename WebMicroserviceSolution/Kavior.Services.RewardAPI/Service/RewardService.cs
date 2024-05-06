using Kavior.Services.RewardAPI.Data;
using Kavior.Services.RewardAPI.Message;
using Kavior.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Kavior.Services.RewardAPI.Service
{
    public class RewardService : IRewardService
    {
        private DbContextOptions<AppDbContext> _dbOptions;
        public RewardService(DbContextOptions<AppDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        public async Task UpdateRewards(RewardsMessage rewardsMessage)
        {
            try
            {
                Rewards reward= new()
                {
                    UserId = rewardsMessage.UserId,
                    RewardsDate= DateTime.Now,
                    RewardsActivity = rewardsMessage.RewardsActivity   ,
                    OrderId = rewardsMessage.OrderId

                };
                await using var _db = new AppDbContext(_dbOptions);
                await _db.Rewards.AddAsync(reward);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
