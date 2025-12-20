public class UserCertificate_futureAdminVerify
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FileUrl { get; set; }
    public string FileType { get; set; } // "StudentId", "DriverLicense", etc.
    public VerificationStatus Status { get; set; } // Pending, Approved, Rejected
    public DateTime UploadedAt { get; set; }
}

public enum VerificationStatus { Pending, Approved, Rejected }