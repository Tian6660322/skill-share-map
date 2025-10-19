using SkillShareMap.Models;

namespace SkillShareMap.Services;

public interface IWalletService
{
    Task<Wallet?> GetOrCreateWalletAsync(int userId);
    Task<Wallet?> GetWalletByUserIdAsync(int userId);
    Task<decimal> GetBalanceAsync(int userId);
    Task<bool> ProcessDepositAsync(int userId, int taskId, decimal amount);
    Task<bool> ProcessPaymentAsync(int taskId);
    Task<bool> ProcessRefundAsync(int taskId);
    Task<List<WalletTransaction>> GetTransactionHistoryAsync(int userId);
}
