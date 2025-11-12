using AutoMapper;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Application.Services.Email;
using Kanini.Ecommerce.Application.Services.Products;
using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Data.Repositories.Carts;
using Kanini.Ecommerce.Data.Repositories.Customer;
using Kanini.Ecommerce.Data.Repositories.Orders;
using Kanini.Ecommerce.Data.Repositories.Products;
using Kanini.Ecommerce.Domain.Entities;
using Kanini.Ecommerce.Domain.Enums;
using Microsoft.Extensions.Logging;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Kanini.Ecommerce.Application.Services.Orders;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IShippingRepository _shippingRepository;
    private readonly ICartRepository _cartRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IEmailService _emailService;
    private readonly IInventoryService _inventoryService;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        IShippingRepository shippingRepository,
        ICartRepository cartRepository,
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        IEmailService emailService,
        IInventoryService inventoryService,
        IMapper mapper,
        ILogger<OrderService> logger
    )
    {
        _orderRepository = orderRepository;
        _shippingRepository = shippingRepository;
        _cartRepository = cartRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _emailService = emailService;
        _inventoryService = inventoryService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CheckoutSummaryDto>> GetCheckoutSummaryAsync(int customerId)
    {
        try
        {
            var cartResult = await _cartRepository.GetCartByCustomerIdAsync(customerId);
            if (cartResult.IsFailure)
                return Result.Failure<CheckoutSummaryDto>(cartResult.Error);

            var cartItems = _mapper.Map<List<CartItemDto>>(cartResult.Value);
            // Note: In the current data structure, 'Price' is the selling price and 'DiscountPrice' is the original price
            var subTotal = cartItems.Sum(x => x.DiscountPrice.HasValue ? x.DiscountPrice.Value * x.Quantity : x.Price * x.Quantity);
            var totalDiscount = cartItems.Sum(x => x.DiscountPrice.HasValue ? (x.DiscountPrice.Value - x.Price) * x.Quantity : 0);

            return new CheckoutSummaryDto
            {
                CustomerId = customerId,
                Items = cartItems,
                TotalItems = cartItems.Sum(x => x.Quantity),
                SubTotal = subTotal,
                TotalDiscount = totalDiscount,
                GrandTotal = subTotal - totalDiscount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get checkout summary: {Error}", ex.Message);
            return Result.Failure<CheckoutSummaryDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<OrderDetailsResponseDto>> CreateOrderWithPaymentAsync(
        int customerId,
        CreateOrderRequestDto request,
        string createdBy
    )
    {
        using var transaction = await _orderRepository.BeginTransactionAsync();
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.OrderCreationStarted, customerId);

            // Get cart items
            var cartResult = await _cartRepository.GetCartByCustomerIdAsync(customerId);
            if (cartResult.IsFailure)
            {
                return Result.Failure<OrderDetailsResponseDto>(cartResult.Error);
            }

            var cartItems = cartResult.Value;
            if (!cartItems.Any())
            {
                return Result.Failure<OrderDetailsResponseDto>(
                    Error.Validation(
                        MagicStrings.ErrorCodes.CartEmpty,
                        MagicStrings.ErrorMessages.CartEmpty
                    )
                );
            }

            // Validate stock availability before creating order
            foreach (var cartItem in cartItems)
            {
                var stockResult = await _inventoryService.CheckStockAvailabilityAsync(
                    cartItem.ProductId, 
                    cartItem.Quantity
                );
                if (stockResult.IsFailure || !stockResult.Value)
                {
                    return Result.Failure<OrderDetailsResponseDto>(
                        Error.Validation(
                            "INSUFFICIENT_STOCK",
                            $"Insufficient stock for product {cartItem.Product.Name}"
                        )
                    );
                }
            }

            // Calculate amounts - DiscountPrice is original price, Price is selling price
            decimal subTotal = cartItems.Sum(item => 
                (item.Product.DiscountPrice ?? item.Product.Price) * item.Quantity);
            decimal discountAmount = cartItems.Sum(item => 
                item.Product.DiscountPrice.HasValue ? 
                (item.Product.DiscountPrice.Value - item.Product.Price) * item.Quantity : 0
            );
            decimal totalAmount = subTotal - discountAmount;

            // Get the first vendor from cart items
            var firstVendorId = cartItems.First().Product.VendorId;
            _logger.LogInformation("Using VendorId: {VendorId} for order creation", firstVendorId);
            
            if (firstVendorId <= 0)
            {
                _logger.LogError("Invalid VendorId {VendorId} from cart items. Please update sp_GetCartByCustomerId stored procedure to include VendorId.", firstVendorId);
                return Result.Failure<OrderDetailsResponseDto>(
                    Error.Validation(
                        "INVALID_VENDOR",
                        "Invalid vendor information. Please contact support."
                    )
                );
            }

            // Create order with PENDING status (not confirmed until payment)
            var order = new Order
            {
                CustomerId = customerId,
                VendorId = firstVendorId,
                TotalAmount = totalAmount,
                DiscountAmount = discountAmount,
                ShippingAmount = 0, // Free shipping
                TaxAmount = 0, // No tax for now
                ShippingAddress = request.ShippingAddress,
                ShippingCity = request.City,
                ShippingState = request.State,
                ShippingPinCode = request.PinCode,
                ShippingPhone = request.Phone,
                Notes = request.OrderNotes,
                Status = OrderStatus.Pending, // Keep as pending until payment confirmation
                CreatedBy = createdBy,
                CreatedOn = DateTime.UtcNow,
                TenantId = "system",
            };

            var orderResult = await _orderRepository.CreateOrderAsync(order);
            if (orderResult.IsFailure)
            {
                return Result.Failure<OrderDetailsResponseDto>(orderResult.Error);
            }

            // Create order items with proper pricing
            var orderItems = cartItems
                .Select(cartItem => new OrderItem
                {
                    OrderId = orderResult.Value.OrderId,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product.DiscountPrice ?? cartItem.Product.Price, // Original price
                    TotalPrice = cartItem.Product.Price * cartItem.Quantity, // Discounted price * quantity
                    CreatedBy = createdBy,
                    CreatedOn = DateTime.UtcNow,
                    TenantId = "system",
                })
                .ToList();

            var orderItemsResult = await _orderRepository.CreateOrderItemsAsync(orderItems);
            if (orderItemsResult.IsFailure)
            {
                return Result.Failure<OrderDetailsResponseDto>(orderItemsResult.Error);
            }

            // Commit transaction for order creation
            await transaction.CommitAsync();

            // Get complete order details
            var orderDetailsResult = await GetOrderByIdAsync(orderResult.Value.OrderId);
            if (orderDetailsResult.IsFailure)
            {
                return Result.Failure<OrderDetailsResponseDto>(orderDetailsResult.Error);
            }

            _logger.LogInformation(
                "Order created with Pending status. OrderId: {OrderId}. Payment required for confirmation.",
                orderResult.Value.OrderId
            );
            return orderDetailsResult.Value;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, MagicStrings.LogMessages.OrderCreationFailed, ex.Message);
            return Result.Failure<OrderDetailsResponseDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result> ConfirmOrderAfterPaymentAsync(int orderId, string updatedBy)
    {
        using var transaction = await _orderRepository.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("Confirming order after successful payment. OrderId: {OrderId}", orderId);

            // Get order details
            var orderResult = await _orderRepository.GetOrderByIdAsync(orderId);
            if (orderResult.IsFailure)
            {
                return Result.Failure(orderResult.Error);
            }

            var order = orderResult.Value;
            if (order.Status != OrderStatus.Pending)
            {
                return Result.Failure(
                    Error.Validation(
                        "ORDER_NOT_PENDING",
                        "Order is not in pending status"
                    )
                );
            }

            // Get order items to reduce inventory
            var orderItemsResult = await _orderRepository.GetOrderItemsAsync(orderId);
            if (orderItemsResult.IsFailure)
            {
                return Result.Failure(orderItemsResult.Error);
            }

            // Reduce inventory for each product
            foreach (var orderItem in orderItemsResult.Value)
            {
                var reduceResult = await _inventoryService.ReduceStockAsync(
                    orderItem.ProductId,
                    orderItem.Quantity,
                    updatedBy
                );
                if (reduceResult.IsFailure)
                {
                    _logger.LogError(
                        "Failed to reduce stock for ProductId: {ProductId}, Quantity: {Quantity}",
                        orderItem.ProductId,
                        orderItem.Quantity
                    );
                    return Result.Failure(reduceResult.Error);
                }
            }

            // Update order status to confirmed
            var updateStatusResult = await _orderRepository.UpdateOrderStatusAsync(
                orderId,
                (int)OrderStatus.Confirmed,
                updatedBy
            );
            if (updateStatusResult.IsFailure)
            {
                return Result.Failure(updateStatusResult.Error);
            }

            // Clear customer's cart
            await _cartRepository.ClearCartAsync(order.CustomerId);

            // Commit all changes
            await transaction.CommitAsync();

            // Send order confirmation email
            await SendOrderConfirmationEmailAsync(order.CustomerId, order);

            _logger.LogInformation(
                "Order confirmed successfully after payment. OrderId: {OrderId}",
                orderId
            );
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to confirm order after payment. OrderId: {OrderId}", orderId);
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<List<OrderResponseDto>>> GetOrdersByCustomerIdAsync(int customerId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetOrdersStarted, customerId);

            var result = await _orderRepository.GetOrdersByCustomerIdAsync(customerId);
            if (result.IsFailure)
            {
                return Result.Failure<List<OrderResponseDto>>(result.Error);
            }

            var orderDtos = _mapper.Map<List<OrderResponseDto>>(result.Value);

            // Populate additional fields that require separate queries
            foreach (var orderDto in orderDtos)
            {
                // Get customer name
                var customerResult = await _customerRepository.GetCustomerByIdAsync(orderDto.CustomerId);
                if (customerResult.IsSuccess)
                {
                    orderDto.CustomerName = $"{customerResult.Value.FirstName} {customerResult.Value.LastName}";
                }

                // Get item count
                var itemsResult = await _orderRepository.GetOrderItemsAsync(orderDto.OrderId);
                if (itemsResult.IsSuccess)
                {
                    orderDto.ItemCount = itemsResult.Value.Count;
                }

                // Set payment method (simplified for now)
                orderDto.PaymentMethod = "Razorpay";
            }

            _logger.LogInformation(MagicStrings.LogMessages.GetOrdersCompleted, customerId);
            return orderDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetOrdersFailed, ex.Message);
            return Result.Failure<List<OrderResponseDto>>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<OrderDetailsResponseDto>> GetOrderByIdAsync(int orderId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetOrderByIdStarted, orderId);

            var orderResult = await _orderRepository.GetOrderByIdAsync(orderId);
            if (orderResult.IsFailure)
            {
                return Result.Failure<OrderDetailsResponseDto>(orderResult.Error);
            }

            var orderItemsResult = await _orderRepository.GetOrderItemsAsync(orderId);
            if (orderItemsResult.IsFailure)
            {
                return Result.Failure<OrderDetailsResponseDto>(orderItemsResult.Error);
            }

            var orderDto = _mapper.Map<OrderDetailsResponseDto>(orderResult.Value);
            orderDto.Items = _mapper.Map<List<OrderItemResponseDto>>(orderItemsResult.Value);
            orderDto.ItemCount = orderItemsResult.Value.Count;

            // Populate customer details
            var customerResult = await _customerRepository.GetCustomerByIdAsync(orderResult.Value.CustomerId);
            if (customerResult.IsSuccess)
            {
                var customer = customerResult.Value;
                orderDto.CustomerName = $"{customer.FirstName} {customer.LastName}";
                orderDto.CustomerPhone = customer.User?.Phone ?? "";
                orderDto.CustomerEmail = customer.User?.Email ?? "";
            }
            
            // Populate shipping details if available
            var shippingResult = await _shippingRepository.GetShippingByOrderIdAsync(orderId);
            if (shippingResult.IsSuccess)
            {
                orderDto.Shipping = _mapper.Map<ShippingResponseDto>(shippingResult.Value);
            }

            // Populate product details for each order item
            foreach (var item in orderDto.Items)
            {
                var productResult = await _productRepository.GetProductByIdAsync(item.ProductId);
                if (productResult.IsSuccess)
                {
                    var product = productResult.Value;
                    item.ProductName = product.Name;
                    item.ProductSKU = product.SKU;
                    item.ProductImage = product.Images?.FirstOrDefault()?.ImagePath;
                    
                    // Get vendor name from product
                    if (product.Vendor != null)
                    {
                        item.VendorName = product.Vendor.BusinessName ?? product.Vendor.OwnerName;
                    }
                }
            }

            // Set payment method
            orderDto.PaymentMethod = "Razorpay";

            _logger.LogInformation(MagicStrings.LogMessages.GetOrderByIdCompleted, orderId);
            return orderDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetOrderByIdFailed, ex.Message);
            return Result.Failure<OrderDetailsResponseDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result> UpdateOrderStatusAsync(int orderId, int status, string updatedBy)
    {
        try
        {
            if (!Enum.IsDefined(typeof(OrderStatus), status))
            {
                return Result.Failure(
                    Error.Validation(
                        MagicStrings.ErrorCodes.ValidationFailed,
                        "Invalid order status"
                    )
                );
            }

            var result = await _orderRepository.UpdateOrderStatusAsync(orderId, status, updatedBy);
            
            // Handle status-specific actions
            if (result.IsSuccess)
            {
                await HandleOrderStatusChangeAsync(orderId, (OrderStatus)status, updatedBy);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update order status: {Error}", ex.Message);
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    private async Task HandleOrderStatusChangeAsync(int orderId, OrderStatus status, string updatedBy)
    {
        try
        {
            switch (status)
            {
                case OrderStatus.Cancelled:
                    await _inventoryService.ReleaseStockAsync(orderId, updatedBy);
                    await SendOrderCancellationEmailAsync(orderId, "Order cancelled by request");
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle order status change for OrderId: {OrderId}", orderId);
        }
    }

    private async Task SendOrderConfirmationEmailAsync(int customerId, Order order)
    {
        try
        {
            var customerResult = await _customerRepository.GetCustomerByIdAsync(customerId);
            if (customerResult.IsSuccess && customerResult.Value.User != null)
            {
                var customer = customerResult.Value;
                await _emailService.SendOrderConfirmationEmailAsync(
                    customer.User.Email,
                    $"{customer.FirstName} {customer.LastName}",
                    order.OrderNumber,
                    order.TotalAmount,
                    order.CreatedOn
                );
            }
            else
            {
                _logger.LogWarning("Customer or User data not found for CustomerId: {CustomerId}", customerId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send order confirmation email for OrderId: {OrderId}", order.OrderId);
        }
    }

    private async Task SendOrderCancellationEmailAsync(int orderId, string reason)
    {
        try
        {
            var orderResult = await _orderRepository.GetOrderByIdAsync(orderId);
            if (orderResult.IsSuccess)
            {
                var order = orderResult.Value;
                var customerResult = await _customerRepository.GetCustomerByIdAsync(order.CustomerId);
                if (customerResult.IsSuccess)
                {
                    var customer = customerResult.Value;
                    await _emailService.SendOrderCancelledEmailAsync(
                        customer.User.Email,
                        $"{customer.FirstName} {customer.LastName}",
                        order.OrderNumber,
                        reason
                    );
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send order cancellation email for OrderId: {OrderId}", orderId);
        }
    }

    public async Task<Result<object>> GetOrderTrackingAsync(int orderId)
    {
        try
        {
            // Get order details directly from repository instead of using GetOrderByIdAsync
            var orderResult = await _orderRepository.GetOrderByIdAsync(orderId);
            if (orderResult.IsFailure)
            {
                return Result.Failure<object>(orderResult.Error);
            }

            var order = orderResult.Value;
            
            // Get shipping information directly
            var shippingResult = await _shippingRepository.GetShippingByOrderIdAsync(orderId);
            Shipping? shipping = shippingResult.IsSuccess ? shippingResult.Value : null;
            
            // Log shipping status for debugging
            if (shipping != null)
            {
                _logger.LogInformation("Order {OrderId} has shipping with status: {ShippingStatus} (enum value: {EnumValue})", 
                    orderId, shipping.Status, (int)shipping.Status);
            }
            else
            {
                _logger.LogInformation("Order {OrderId} has no shipping record", orderId);
            }
            
            var trackingInfo = new
            {
                OrderId = order.OrderId,
                Status = order.Status.ToString(),
                OrderDate = order.CreatedOn,
                TrackingSteps = GetTrackingSteps(order.Status.ToString(), shipping)
            };

            return Result.Success<object>(trackingInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get order tracking: {Error}", ex.Message);
            return Result.Failure<object>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<byte[]>> GenerateInvoicePdfAsync(int orderId)
    {
        try
        {
            var orderResult = await GetOrderByIdAsync(orderId);
            if (orderResult.IsFailure)
            {
                return Result.Failure<byte[]>(orderResult.Error);
            }

            var order = orderResult.Value;
            var pdfBytes = GenerateInvoicePdf(order);
            
            return Result.Success(pdfBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate invoice PDF: {Error}", ex.Message);
            return Result.Failure<byte[]>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    private List<object> GetTrackingSteps(string orderStatus, Shipping? shipping)
    {
        var steps = new List<object>();
        
        // Step 1: Order Placed - Always completed when order exists
        steps.Add(new { 
            Step = "Order Placed", 
            Status = "Completed", 
            Date = (DateTime?)DateTime.Now.AddDays(-3) // Use actual order creation date in real implementation
        });
        
        // Step 2: Order Confirmed
        var (confirmedStatus, confirmedDate) = GetConfirmedStatusAndDate(orderStatus);
        steps.Add(new { 
            Step = "Order Confirmed", 
            Status = confirmedStatus, 
            Date = confirmedDate 
        });
        
        // Step 3: Processing
        var (processingStatus, processingDate) = GetProcessingStatusAndDate(orderStatus, shipping);
        steps.Add(new { 
            Step = "Processing", 
            Status = processingStatus, 
            Date = processingDate 
        });
        
        // Step 4: Shipped
        var (shippedStatus, shippedDate) = GetShippedStatusAndDate(orderStatus, shipping);
        steps.Add(new { 
            Step = "Shipped", 
            Status = shippedStatus, 
            Date = shippedDate 
        });
        
        // Step 5: Delivered
        var (deliveredStatus, deliveredDate) = GetDeliveredStatusAndDate(orderStatus, shipping);
        steps.Add(new { 
            Step = "Delivered", 
            Status = deliveredStatus, 
            Date = deliveredDate 
        });
        
        return steps;
    }
    
    private (string status, DateTime? date) GetConfirmedStatusAndDate(string orderStatus)
    {
        if (orderStatus == "Pending") 
            return ("Pending", null);
        
        // Order is confirmed if it's any status beyond Pending
        return ("Completed", DateTime.Now.AddDays(-2)); // Use actual confirmation date in real implementation
    }
    
    private (string status, DateTime? date) GetProcessingStatusAndDate(string orderStatus, Shipping? shipping)
    {
        _logger.LogInformation("GetProcessingStatusAndDate - OrderStatus: {OrderStatus}, ShippingStatus: {ShippingStatus}", 
            orderStatus, shipping?.Status.ToString() ?? "null");
            
        // If order is still pending, processing is pending
        if (orderStatus == "Pending") 
            return ("Pending", null);
        
        // If shipping exists and is shipped/delivered, processing is completed
        if (shipping != null && (shipping.Status == ShippingStatus.Shipped || shipping.Status == ShippingStatus.Delivered))
        {
            _logger.LogInformation("Processing completed because shipping status is {Status}", shipping.Status);
            return ("Completed", shipping.CreatedOn.AddHours(1)); // Processing completed before shipping
        }
        
        // If order status is shipped/delivered but no shipping record, assume processing completed
        if (orderStatus == "Shipped" || orderStatus == "Delivered") 
            return ("Completed", DateTime.Now.AddDays(-1));
        
        // If order is confirmed or processing, show as in progress
        if (orderStatus == "Confirmed" || orderStatus == "Processing") 
            return ("In Progress", null);
        
        return ("Pending", null);
    }
    
    private (string status, DateTime? date) GetShippedStatusAndDate(string orderStatus, Shipping? shipping)
    {
        _logger.LogInformation("GetShippedStatusAndDate - OrderStatus: {OrderStatus}, ShippingStatus: {ShippingStatus}", 
            orderStatus, shipping?.Status.ToString() ?? "null");
            
        // If shipping record exists, use its status and dates
        if (shipping != null)
        {
            if (shipping.Status == ShippingStatus.Shipped || shipping.Status == ShippingStatus.Delivered)
            {
                _logger.LogInformation("Shipped completed because shipping status is {Status}", shipping.Status);
                return ("Completed", shipping.ShippedDate);
            }
            if (shipping.Status == ShippingStatus.Pending)
                return ("Pending", null);
        }
        
        // Fallback to order status if no shipping record
        if (orderStatus == "Shipped" || orderStatus == "Delivered") 
            return ("Completed", DateTime.Now);
        if (orderStatus == "Processing") 
            return ("In Progress", null);
        
        return ("Pending", null);
    }
    
    private (string status, DateTime? date) GetDeliveredStatusAndDate(string orderStatus, Shipping? shipping)
    {
        // If shipping record shows delivered, use actual delivery date
        if (shipping != null && shipping.Status == ShippingStatus.Delivered)
            return ("Completed", shipping.ActualDeliveryDate);
        
        // If order status is delivered but no shipping record
        if (orderStatus == "Delivered") 
            return ("Completed", DateTime.Now);
        
        // If shipped but not delivered yet, show as in progress
        if (shipping != null && shipping.Status == ShippingStatus.Shipped)
            return ("In Progress", null);
        
        return ("Pending", null);
    }

    private byte[] GenerateInvoicePdf(OrderDetailsResponseDto order)
    {
        using var memoryStream = new MemoryStream();
        var document = new Document(PageSize.A4, 40, 40, 40, 40);
        var writer = PdfWriter.GetInstance(document, memoryStream);
        
        document.Open();
        
        // Colors
        var primaryColor = new BaseColor(102, 126, 234);
        var successColor = new BaseColor(40, 167, 69);
        var mutedColor = new BaseColor(108, 117, 125);
        
        // Fonts
        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 24, primaryColor);
        var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.Black);
        var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.Black);
        var smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 8, mutedColor);
        
        // Header
        var headerTable = new PdfPTable(1) { WidthPercentage = 100 };
        var headerCell = new PdfPCell(new Phrase("INVOICE", titleFont))
        {
            HorizontalAlignment = Element.ALIGN_CENTER,
            BackgroundColor = new BaseColor(248, 249, 250),
            Border = Rectangle.NO_BORDER,
            Padding = 20
        };
        headerTable.AddCell(headerCell);
        
        var orderCell = new PdfPCell(new Phrase($"Order #{order.OrderId}", headerFont))
        {
            HorizontalAlignment = Element.ALIGN_CENTER,
            BackgroundColor = new BaseColor(248, 249, 250),
            Border = Rectangle.NO_BORDER,
            PaddingBottom = 20
        };
        headerTable.AddCell(orderCell);
        document.Add(headerTable);
        
        document.Add(new Paragraph(" "));
        
        // Customer & Order Info
        var infoTable = new PdfPTable(3) { WidthPercentage = 100 };
        infoTable.SetWidths(new float[] { 1, 1, 1 });
        
        // Bill To
        var billToCell = new PdfPCell()
        {
            Border = Rectangle.NO_BORDER,
            PaddingRight = 10
        };
        billToCell.AddElement(new Paragraph("Bill To", headerFont));
        billToCell.AddElement(new Paragraph(order.CustomerName ?? "", normalFont));
        billToCell.AddElement(new Paragraph(order.CustomerEmail ?? "", smallFont));
        billToCell.AddElement(new Paragraph(order.CustomerPhone ?? "", smallFont));
        infoTable.AddCell(billToCell);
        
        // Order Details
        var orderDetailsCell = new PdfPCell()
        {
            Border = Rectangle.NO_BORDER,
            PaddingRight = 10
        };
        orderDetailsCell.AddElement(new Paragraph("Order Details", headerFont));
        orderDetailsCell.AddElement(new Paragraph($"Date: {order.OrderDate:dd/MM/yyyy}", normalFont));
        orderDetailsCell.AddElement(new Paragraph($"Status: {order.Status}", normalFont));
        orderDetailsCell.AddElement(new Paragraph($"Payment: {order.PaymentMethod}", normalFont));
        infoTable.AddCell(orderDetailsCell);
        
        // Shipping Address
        var shippingCell = new PdfPCell()
        {
            Border = Rectangle.NO_BORDER
        };
        shippingCell.AddElement(new Paragraph("Shipping Address", headerFont));
        shippingCell.AddElement(new Paragraph(order.ShippingAddress ?? "", normalFont));
        infoTable.AddCell(shippingCell);
        
        document.Add(infoTable);
        document.Add(new Paragraph(" "));
        
        // Items Table
        var itemsTable = new PdfPTable(6) { WidthPercentage = 100 };
        itemsTable.SetWidths(new float[] { 3, 2, 2, 1, 2, 2 });
        
        // Table Headers
        var headers = new[] { "Product", "SKU", "Vendor", "Qty", "Original Price", "Amount Paid" };
        foreach (var header in headers)
        {
            var headerCell2 = new PdfPCell(new Phrase(header, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.White)))
            {
                BackgroundColor = primaryColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 8
            };
            itemsTable.AddCell(headerCell2);
        }
        
        // Table Rows - calculate actual amounts paid considering order-level discount
        var subtotal = order.Items.Sum(i => i.UnitPrice * i.Quantity);
        var orderDiscount = subtotal - order.TotalAmount;
        var discountRatio = subtotal > 0 ? order.TotalAmount / subtotal : 1;
        
        foreach (var item in order.Items)
        {
            itemsTable.AddCell(new PdfPCell(new Phrase(item.ProductName ?? "", normalFont)) { Padding = 8 });
            itemsTable.AddCell(new PdfPCell(new Phrase(item.ProductSKU ?? "", normalFont)) { Padding = 8 });
            itemsTable.AddCell(new PdfPCell(new Phrase(item.VendorName ?? "", normalFont)) { Padding = 8 });
            itemsTable.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString(), normalFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 8 });
            
            // Original Price cell
            itemsTable.AddCell(new PdfPCell(new Phrase($"₹{item.UnitPrice:F2}", normalFont)) { HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 8 });
            
            // Amount Paid cell - calculate proportional amount paid
            var itemOriginalTotal = item.UnitPrice * item.Quantity;
            var actualAmountPaid = itemOriginalTotal * discountRatio;
            itemsTable.AddCell(new PdfPCell(new Phrase($"₹{actualAmountPaid:F2}", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, successColor))) { HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 8 });
        }
        
        document.Add(itemsTable);
        document.Add(new Paragraph(" "));
        
        // Summary
        var summaryTable = new PdfPTable(2) { WidthPercentage = 50, HorizontalAlignment = Element.ALIGN_RIGHT };
        summaryTable.SetWidths(new float[] { 1, 1 });
        
        summaryTable.AddCell(new PdfPCell(new Phrase("Subtotal:", normalFont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 5 });
        summaryTable.AddCell(new PdfPCell(new Phrase($"₹{subtotal:F2}", normalFont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 5 });
        
        var discount = subtotal - order.TotalAmount;
        if (discount > 0)
        {
            summaryTable.AddCell(new PdfPCell(new Phrase("Discount:", normalFont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 5 });
            summaryTable.AddCell(new PdfPCell(new Phrase($"-₹{discount:F2}", FontFactory.GetFont(FontFactory.HELVETICA, 10, successColor))) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 5 });
        }
        
        summaryTable.AddCell(new PdfPCell(new Phrase("Total Amount Paid:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, primaryColor))) { Border = Rectangle.TOP_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 10 });
        summaryTable.AddCell(new PdfPCell(new Phrase($"₹{order.TotalAmount:F2}", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, primaryColor))) { Border = Rectangle.TOP_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 10 });
        
        document.Add(summaryTable);
        
        document.Close();
        return memoryStream.ToArray();
    }
}
