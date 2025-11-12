namespace Kanini.Ecommerce.Application.DTOs;

public class SubscriptionPlanDto
{
    public int PlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int MaxProducts { get; set; }
    public int DurationDays { get; set; }
}
