namespace SkillShareMap.Models;

// User's wallet for managing virtual currency
public class Wallet
{
    public int Id { get; set; }

    // User reference
    public int UserId { get; set; }
    public User? User { get; set; }

    // Current balance (simulated)
    public decimal Balance { get; set; } = 1000; // Start with $1000 for demo

    // Transaction history
    public List<WalletTransaction> Transactions { get; set; } = new();

    // Timestamp
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

// Wallet transaction record
public class WalletTransaction
{
    public int Id { get; set; }

    // Wallet reference
    public int WalletId { get; set; }
    public Wallet? Wallet { get; set; }

    // Transaction type
    public TransactionType Type { get; set; }

    // Amount (positive for credit, negative for debit)
    public decimal Amount { get; set; }

    // Description
    public string Description { get; set; } = string.Empty;

    // Related task (if applicable)
    public int? TaskId { get; set; }
    public SkillTask? Task { get; set; }

    // Balance after transaction
    public decimal BalanceAfter { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum TransactionType
{
    Deposit,           // Money added to wallet
    Withdrawal,        // Money withdrawn from wallet
    TaskDeposit,       // 10% deposit for task (escrow)
    TaskPayment,       // Payment for completed task
    TaskRefund,        // Refund for cancelled task
    EscrowHold,        // Money held in escrow
    EscrowRelease      // Money released from escrow
}
