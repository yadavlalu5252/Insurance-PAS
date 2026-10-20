
namespace PolicyIssuance.Domain.Enums
{
    public enum PolicyStatus
    {
        Draft = 1,
        PendingPayment = 2,
        Active = 3,
        Suspended = 4,
        Cancelled = 5,
        Expired = 6,
        Closed = 7
    }
}
