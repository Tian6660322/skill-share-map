using Microsoft.EntityFrameworkCore;
using SkillShareMap.Data;
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

public class WalletService : IWalletService
{
    private readonly ApplicationDbContext _context;

    public WalletService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get wallet for user, or create one if it doesn't exist
    /// </summary>
    public async Task<Wallet?> GetOrCreateWalletAsync(int userId)
    {
        var wallet = await _context.Wallets
            .Include(w => w.Transactions)
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null)
        {
            wallet = new Wallet
            {
                UserId = userId,
                Balance = 1000 // Start with $1000 for demo
            };
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
        }

        return wallet;
    }

    /// <summary>
    /// Get current balance for user
    /// </summary>
    public async Task<decimal> GetBalanceAsync(int userId)
    {
        var wallet = await GetOrCreateWalletAsync(userId);
        return wallet?.Balance ?? 0;
    }

    /// <summary>
    /// Process 10% deposit payment for a task (simulated)
    /// </summary>
    public async Task<bool> ProcessDepositAsync(int userId, int taskId, decimal amount)
    {
        var wallet = await GetOrCreateWalletAsync(userId);
        if (wallet == null)
            return false;

        var task = await _context.SkillTasks.FindAsync(taskId);
        if (task == null)
            return false;

        // Check if user has enough balance
        if (wallet.Balance < amount)
            return false;

        // Deduct deposit from wallet
        wallet.Balance -= amount;
        wallet.LastUpdated = DateTime.UtcNow;

        // Create transaction record
        var transaction = new WalletTransaction
        {
            WalletId = wallet.Id,
            Type = TransactionType.TaskDeposit,
            Amount = -amount,
            Description = $"10% deposit for task #{taskId}",
            TaskId = taskId,
            BalanceAfter = wallet.Balance
        };

        _context.WalletTransactions.Add(transaction);

        // Update task
        task.DepositAmount = amount;
        task.IsDepositPaid = true;
        task.Status = SkillTaskStatus.Assigned;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Process full payment when task is completed
    /// </summary>
    public async Task<bool> ProcessPaymentAsync(int taskId)
    {
        var task = await _context.SkillTasks
            .Include(t => t.Creator)
            .Include(t => t.AssignedTo)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null || task.AssignedToId == null)
            return false;

        // Get helper's wallet
        var helperWallet = await GetOrCreateWalletAsync(task.AssignedToId.Value);
        if (helperWallet == null)
            return false;

        // Calculate payment amount (use negotiated price or original budget)
        var paymentAmount = task.NegotiatedPrice ?? task.Budget;

        // Add payment to helper's wallet
        helperWallet.Balance += paymentAmount;
        helperWallet.LastUpdated = DateTime.UtcNow;

        // Create transaction record
        var transaction = new WalletTransaction
        {
            WalletId = helperWallet.Id,
            Type = TransactionType.TaskPayment,
            Amount = paymentAmount,
            Description = $"Payment for completing task #{taskId}",
            TaskId = taskId,
            BalanceAfter = helperWallet.Balance
        };

        _context.WalletTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Process refund if task is cancelled
    /// </summary>
    public async Task<bool> ProcessRefundAsync(int taskId)
    {
        var task = await _context.SkillTasks
            .Include(t => t.Creator)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null || !task.IsDepositPaid || task.DepositAmount == null)
            return false;

        // Get creator's wallet
        var creatorWallet = await GetOrCreateWalletAsync(task.CreatorId);
        if (creatorWallet == null)
            return false;

        // Refund deposit
        creatorWallet.Balance += task.DepositAmount.Value;
        creatorWallet.LastUpdated = DateTime.UtcNow;

        // Create transaction record
        var transaction = new WalletTransaction
        {
            WalletId = creatorWallet.Id,
            Type = TransactionType.TaskRefund,
            Amount = task.DepositAmount.Value,
            Description = $"Refund for cancelled task #{taskId}",
            TaskId = taskId,
            BalanceAfter = creatorWallet.Balance
        };

        _context.WalletTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Get wallet by user ID
    /// </summary>
    public async Task<Wallet?> GetWalletByUserIdAsync(int userId)
    {
        return await GetOrCreateWalletAsync(userId);
    }

    /// <summary>
    /// Get transaction history for a user
    /// </summary>
    public async Task<List<WalletTransaction>> GetTransactionHistoryAsync(int userId)
    {
        var wallet = await GetOrCreateWalletAsync(userId);
        if (wallet == null)
            return new List<WalletTransaction>();

        return await _context.WalletTransactions
            .Where(t => t.WalletId == wallet.Id)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}
