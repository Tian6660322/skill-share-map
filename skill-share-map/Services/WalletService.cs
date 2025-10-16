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
    /// Process 10% deposit payment for a task
    /// Deducts from creator's wallet and credits helper as upfront deposit
    /// </summary>
    public async Task<bool> ProcessDepositAsync(int helperId, int taskId, decimal amount)
    {
        var task = await _context.SkillTasks.FindAsync(taskId);
        if (task == null)
            return false;

        // Ensure the helper requesting the deposit matches the assigned helper
        if (task.AssignedToId != helperId)
            return false;

        // Get helper's wallet (person accepting the task)
        var helperWallet = await GetOrCreateWalletAsync(helperId);
        if (helperWallet == null)
            return false;

        // Get creator's wallet (person who posted the task)
        var creatorWallet = await GetOrCreateWalletAsync(task.CreatorId);
        if (creatorWallet == null)
            return false;

        // Check if creator has enough balance
        if (creatorWallet.Balance < amount)
            return false;

        // Deduct deposit from creator's wallet
        creatorWallet.Balance -= amount;
        creatorWallet.LastUpdated = DateTime.UtcNow;

        // Add deposit to helper's wallet
        helperWallet.Balance += amount;
        helperWallet.LastUpdated = DateTime.UtcNow;

        // Create transaction record for creator (deduction)
        var creatorTransaction = new WalletTransaction
        {
            WalletId = creatorWallet.Id,
            Type = TransactionType.TaskDeposit,
            Amount = -amount,
            Description = $"10% deposit paid for task #{taskId}",
            TaskId = taskId,
            BalanceAfter = creatorWallet.Balance
        };

        // Create transaction record for helper (addition)
        var helperTransaction = new WalletTransaction
        {
            WalletId = helperWallet.Id,
            Type = TransactionType.TaskDeposit,
            Amount = amount,
            Description = $"10% deposit received for task #{taskId}",
            TaskId = taskId,
            BalanceAfter = helperWallet.Balance
        };

        _context.WalletTransactions.Add(helperTransaction);
        _context.WalletTransactions.Add(creatorTransaction);

        // Update task
        task.DepositAmount = amount;
        task.IsDepositPaid = true;
        task.Status = SkillTaskStatus.Assigned; // Update status to Assigned after deposit is paid

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Process final payment when task is completed
    /// Creator pays remaining balance (Budget - Deposit) to Helper
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

        // Get creator's wallet
        var creatorWallet = await GetOrCreateWalletAsync(task.CreatorId);
        if (creatorWallet == null)
            return false;

        // Calculate payment amount (use negotiated price or original budget)
        var totalAmount = task.NegotiatedPrice ?? task.Budget;
        var depositAmount = task.DepositAmount ?? 0;

        // Remaining amount that creator needs to pay (Budget - Deposit already paid upfront)
        var remainingPayment = totalAmount - depositAmount;
        if (remainingPayment < 0)
            remainingPayment = 0;

        // Check if creator has enough balance
        if (creatorWallet.Balance < remainingPayment)
            return false;

        // Deduct remaining payment from creator's wallet
        creatorWallet.Balance -= remainingPayment;
        creatorWallet.LastUpdated = DateTime.UtcNow;

        // Add remaining amount to helper's wallet
        helperWallet.Balance += remainingPayment;
        helperWallet.LastUpdated = DateTime.UtcNow;

        // Create transaction record for creator (deduction)
        var creatorTransaction = new WalletTransaction
        {
            WalletId = creatorWallet.Id,
            Type = TransactionType.TaskPayment,
            Amount = -remainingPayment,
            Description = $"Final payment for task #{taskId} (${remainingPayment:F2} remaining of ${totalAmount:F2} total, ${depositAmount:F2} deposit paid upfront)",
            TaskId = taskId,
            BalanceAfter = creatorWallet.Balance
        };

        // Create transaction record for helper (addition)
        var helperTransaction = new WalletTransaction
        {
            WalletId = helperWallet.Id,
            Type = TransactionType.TaskPayment,
            Amount = remainingPayment,
            Description = $"Remaining payment for completing task #{taskId}",
            TaskId = taskId,
            BalanceAfter = helperWallet.Balance
        };

        _context.WalletTransactions.Add(creatorTransaction);
        _context.WalletTransactions.Add(helperTransaction);
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

        // Get helper's wallet
        if (task.AssignedToId == null)
            return false;

        var helperWallet = await GetOrCreateWalletAsync(task.AssignedToId.Value);
        if (helperWallet == null)
            return false;

        var depositAmount = task.DepositAmount.Value;

        // Refund deposit: deduct from helper, credit creator
        helperWallet.Balance -= depositAmount;
        helperWallet.LastUpdated = DateTime.UtcNow;

        creatorWallet.Balance += depositAmount;
        creatorWallet.LastUpdated = DateTime.UtcNow;

        // Create transaction records
        var helperTransaction = new WalletTransaction
        {
            WalletId = helperWallet.Id,
            Type = TransactionType.TaskRefund,
            Amount = -depositAmount,
            Description = $"Deposit returned for cancelled task #{taskId}",
            TaskId = taskId,
            BalanceAfter = helperWallet.Balance
        };

        var creatorTransaction = new WalletTransaction
        {
            WalletId = creatorWallet.Id,
            Type = TransactionType.TaskRefund,
            Amount = depositAmount,
            Description = $"Deposit refunded for cancelled task #{taskId}",
            TaskId = taskId,
            BalanceAfter = creatorWallet.Balance
        };

        _context.WalletTransactions.Add(helperTransaction);
        _context.WalletTransactions.Add(creatorTransaction);
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
