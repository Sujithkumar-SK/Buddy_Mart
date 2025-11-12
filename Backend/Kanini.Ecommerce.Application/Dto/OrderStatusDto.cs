using System.ComponentModel.DataAnnotations;

namespace Kanini.Ecommerce.Application.DTOs;

public class OrderStatusUpdateDto
{
    [Required]
    [Range(1, 7, ErrorMessage = "Invalid order status")]
    public int Status { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
}

public class OrderStatusHistoryDto
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = null!;
    public string CurrentStatus { get; set; } = null!;
    public DateTime StatusUpdatedOn { get; set; }
    public string UpdatedBy { get; set; } = null!;
    public List<OrderStatusTimelineDto> StatusHistory { get; set; } = new();
}

public class OrderStatusTimelineDto
{
    public string Status { get; set; } = null!;
    public DateTime UpdatedOn { get; set; }
    public string UpdatedBy { get; set; } = null!;
    public string? Notes { get; set; }
}