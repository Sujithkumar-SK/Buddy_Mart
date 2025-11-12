namespace Kanini.Ecommerce.Application.DTOs;

public class VendorDashboardDto
{
    public VendorAnalyticsDto Analytics { get; set; } = new();
    public VendorSubscriptionDto Subscription { get; set; } = new();
}

public class VendorSubscriptionDto
{
    public string PlanName { get; set; } = string.Empty;
    public int MaxProducts { get; set; }
    public int UsedProducts { get; set; }
    public int RemainingProducts { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int DaysRemaining { get; set; }
    public bool IsActive { get; set; }
}