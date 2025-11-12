namespace Kanini.Ecommerce.Common;

public static class MagicStrings
{
    public static class StoredProcedures
    {
        public const string GetVendorById = "sp_GetVendorById";
        public const string GetVendorByUserId = "sp_GetVendorByUserId";
        public const string GetAllVendors = "sp_GetAllVendors";
        public const string GetSubscriptionPlans = "sp_GetSubscriptionPlans";
        public const string GetVendorProfile = "sp_GetVendorProfile";
        public const string ValidateVendorRegistration = "sp_ValidateVendorRegistration";
        public const string GetProductsByVendor = "sp_GetProductsByVendor";
        public const string GetProductById = "sp_GetProductById";
        public const string GetAllCategories = "sp_GetAllCategories";
        public const string GetCategoryById = "sp_GetCategoryById";
        public const string GetCategoryHierarchy = "sp_GetCategoryHierarchy";
        public const string CheckCategoryNameExists = "sp_CheckCategoryNameExists";
        public const string GetCustomerById = "sp_GetCustomerById";
        public const string GetCustomerByUserId = "sp_GetCustomerByUserId";
        public const string GetAllCustomers = "sp_GetAllCustomers";
        public const string GetUserByEmail = "sp_GetUserByEmail";
        public const string GetUserById = "sp_GetUserById";
        public const string GetUserByRefreshToken = "sp_GetUserByRefreshToken";
        public const string ValidateUserCredentials = "sp_ValidateUserCredentials";
        public const string CheckEmailExists = "sp_CheckEmailExists";
        public const string CheckPhoneExists = "sp_CheckPhoneExists";
        public const string CheckBusinessLicenseExists = "sp_CheckBusinessLicenseExists";
        public const string GetPendingVendors = "sp_GetPendingVendors";
        public const string GetVendorForApproval = "sp_GetVendorForApproval";

        public const string GetAllProducts = "sp_GetAllProducts";
        public const string GetProductsByCategory = "sp_GetProductsByCategory";
        public const string CheckSKUExists = "sp_CheckSKUExists";
        public const string ValidateVendor = "sp_ValidateVendor";
        public const string ValidateCategory = "sp_ValidateCategory";
        public const string GetCartByCustomerId = "sp_GetCartByCustomerId";
        public const string CheckCartItemExists = "sp_CheckCartItemExists";
        public const string GetOrdersByCustomerId = "sp_GetOrdersByCustomerId";
        public const string GetOrderById = "sp_GetOrderById";
        public const string GetOrderItems = "sp_GetOrderItems";
        public const string GetPaymentsByOrderId = "sp_GetPaymentsByOrderId";
        public const string GetPaymentById = "sp_GetPaymentById";
        public const string GetPaymentsByCustomerId = "sp_GetPaymentsByCustomerId";
        public const string GetPaymentByRazorpayOrderId = "sp_GetPaymentByRazorpayOrderId";
        public const string GetLowStockProducts = "sp_GetLowStockProducts";
        public const string CheckStockAvailability = "sp_CheckStockAvailability";
        public const string GetShippingByOrderId = "sp_GetShippingByOrderId";
        public const string GetShippingsByVendor = "sp_GetShippingsByVendor";
        public const string GetShippingByTrackingNumber = "sp_GetShippingByTrackingNumber";
        
        // Analytics Stored Procedures
        public const string GetDashboardAnalytics = "sp_GetDashboardAnalytics";
        public const string GetRecentActivities = "sp_GetRecentActivities";
        public const string GetTopSellingProducts = "sp_GetTopSellingProducts";
        public const string GetSalesAnalytics = "sp_GetSalesAnalytics";
        public const string GetDailySalesChart = "sp_GetDailySalesChart";
        public const string GetCategorySales = "sp_GetCategorySales";
        public const string GetPaymentMethodSales = "sp_GetPaymentMethodSales";
        public const string GetTopVendorSales = "sp_GetTopVendorSales";
        public const string GetCustomerAnalytics = "sp_GetCustomerAnalytics";
        public const string GetCustomerRegistrationTrends = "sp_GetCustomerRegistrationTrends";
        public const string GetCustomerSegments = "sp_GetCustomerSegments";
        public const string GetGeographicDistribution = "sp_GetGeographicDistribution";
        public const string GetTopCustomers = "sp_GetTopCustomers";
    }

    public static class LogMessages
    {
        public const string ValidationFailed = "Validation failed for request";
        public const string VendorRegistrationStarted =
            "Vendor registration started for Email: {Email}";
        public const string VendorRegistrationCompleted =
            "Vendor registration completed for VendorId: {VendorId}";
        public const string VendorRegistrationFailed = "Vendor registration failed: {Error}";
        public const string VendorProfileUpdateStarted =
            "Vendor profile update started for VendorId: {VendorId}";
        public const string VendorProfileUpdateCompleted =
            "Vendor profile updated for VendorId: {VendorId}";
        public const string VendorProfileUpdateFailed = "Vendor profile update failed: {Error}";
        public const string DocumentUploadStarted =
            "Document upload started for VendorId: {VendorId}";
        public const string DocumentUploadCompleted = "Document uploaded for VendorId: {VendorId}";
        public const string DocumentUploadFailed = "Document upload failed: {Error}";
        public const string SubscriptionSelectionStarted =
            "Subscription selection started for VendorId: {VendorId}";
        public const string SubscriptionSelectionCompleted =
            "Subscription selected for VendorId: {VendorId}, PlanId: {PlanId}";
        public const string SubscriptionSelectionFailed = "Subscription selection failed: {Error}";
        public const string ProductCreationStarted =
            "Product creation started for VendorId: {VendorId}";
        public const string ProductCreationCompleted =
            "Product created with ProductId: {ProductId}";
        public const string ProductCreationFailed = "Product creation failed: {Error}";
        public const string LoginAttemptStarted = "Login attempt started for Email: {Email}";
        public const string LoginSuccessful = "Login successful for UserId: {UserId}";
        public const string LoginFailed = "Login failed: {Error}";
        public const string TokenRefreshStarted = "Token refresh started for UserId: {UserId}";
        public const string TokenRefreshSuccessful =
            "Token refresh successful for UserId: {UserId}";
        public const string TokenRefreshFailed = "Token refresh failed: {Error}";
        public const string LogoutStarted = "Logout started for UserId: {UserId}";
        public const string LogoutSuccessful = "Logout successful for UserId: {UserId}";
        public const string PasswordChangeStarted = "Password change started for UserId: {UserId}";
        public const string PasswordChangeSuccessful =
            "Password change successful for UserId: {UserId}";
        public const string PasswordChangeFailed = "Password change failed: {Error}";
        public const string OtpGenerationStarted =
            "OTP generation started for Email: {Email}, Type: {OtpType}";
        public const string OtpSentSuccessfully = "OTP sent successfully to Email: {Email}";
        public const string OtpValidationStarted = "OTP validation started for Email: {Email}";
        public const string OtpValidationSuccessful =
            "OTP validation successful for Email: {Email}";
        public const string OtpValidationFailed = "OTP validation failed for Email: {Email}";
        public const string EmailSendingStarted = "Email sending started to: {Email}";
        public const string EmailSentSuccessfully = "Email sent successfully to: {Email}";
        public const string EmailSendingFailed = "Email sending failed to: {Email} - {Error}";
        public const string VendorApprovalStarted =
            "Vendor approval started for VendorId: {VendorId}";
        public const string VendorApprovalCompleted =
            "Vendor approved successfully for VendorId: {VendorId}";
        public const string VendorApprovalFailed = "Vendor approval failed: {Error}";
        public const string VendorRejectionStarted =
            "Vendor rejection started for VendorId: {VendorId}";
        public const string VendorRejectionCompleted =
            "Vendor rejected successfully for VendorId: {VendorId}";
        public const string VendorRejectionFailed = "Vendor rejection failed: {Error}";
        public const string GetPendingVendorsStarted = "Getting pending vendors started";
        public const string GetPendingVendorsCompleted =
            "Getting pending vendors completed. Found {Count} vendors";
        public const string GetVendorForApprovalStarted =
            "Getting vendor for approval started for VendorId: {VendorId}";
        public const string GetVendorForApprovalCompleted =
            "Getting vendor for approval completed for VendorId: {VendorId}";
        public const string GetVendorForApprovalFailed =
            "Getting vendor for approval failed for VendorId: {VendorId} - {Error}";
        public const string InvalidVendorId = "Invalid vendor ID provided: {VendorId}";
        public const string GetProductStarted =
            "Getting product started for ProductId: {ProductId}";
        public const string GetProductCompleted =
            "Getting product completed for ProductId: {ProductId}";
        public const string GetProductFailed =
            "Getting product failed for ProductId: {ProductId} - {Error}";
        public const string GetProductsByVendorStarted =
            "Getting products by vendor started for VendorId: {VendorId}";
        public const string GetProductsByVendorCompleted =
            "Getting products by vendor completed for VendorId: {VendorId}. Found {Count} products";
        public const string GetProductsByVendorFailed =
            "Getting products by vendor failed for VendorId: {VendorId} - {Error}";
        public const string GetAllProductsStarted = "Getting all products started";
        public const string GetAllProductsCompleted =
            "Getting all products completed. Found {Count} products";
        public const string GetAllProductsFailed = "Getting all products failed - {Error}";
        public const string GetProductsByCategoryStarted =
            "Getting products by category started for CategoryId: {CategoryId}";
        public const string GetProductsByCategoryCompleted =
            "Getting products by category completed for CategoryId: {CategoryId}. Found {Count} products";
        public const string GetProductsByCategoryFailed =
            "Getting products by category failed for CategoryId: {CategoryId} - {Error}";
        public const string ProductUpdateStarted =
            "Product update started for ProductId: {ProductId}";
        public const string ProductUpdateCompleted =
            "Product update completed for ProductId: {ProductId}";
        public const string ProductUpdateFailed =
            "Product update failed for ProductId: {ProductId} - {Error}";
        public const string ProductDeletionStarted =
            "Product deletion started for ProductId: {ProductId}";
        public const string ProductDeletionCompleted =
            "Product deletion completed for ProductId: {ProductId}";
        public const string ProductDeletionFailed =
            "Product deletion failed for ProductId: {ProductId} - {Error}";
        public const string ProductStatusUpdateStarted =
            "Product status update started for ProductId: {ProductId} to Status: {Status}";
        public const string ProductStatusUpdateCompleted =
            "Product status update completed for ProductId: {ProductId} to Status: {Status}";
        public const string ProductStatusUpdateFailed =
            "Product status update failed for ProductId: {ProductId} to Status: {Status} - {Error}";
        public const string ProductImageUploadStarted =
            "Product image upload started for ProductId: {ProductId}. Count: {Count}";
        public const string ProductImageUploadCompleted =
            "Product image upload completed for ProductId: {ProductId}. Count: {Count}";
        public const string ProductImageUploadFailed =
            "Product image upload failed for ProductId: {ProductId} - {Error}";
        public const string InvalidProductId = "Invalid product ID provided: {ProductId}";
        public const string InvalidCategoryId = "Invalid category ID provided: {CategoryId}";
        public const string CategoryCreationStarted = "Category creation started for Name: {Name}";
        public const string CategoryCreationCompleted = "Category created with CategoryId: {CategoryId}";
        public const string CategoryCreationFailed = "Category creation failed: {Error}";
        public const string GetCategoryStarted = "Getting category started for CategoryId: {CategoryId}";
        public const string GetCategoryCompleted = "Getting category completed for CategoryId: {CategoryId}";
        public const string GetCategoryFailed = "Getting category failed for CategoryId: {CategoryId} - {Error}";
        public const string GetAllCategoriesStarted = "Getting all categories started";
        public const string GetAllCategoriesCompleted = "Getting all categories completed. Found {Count} categories";
        public const string GetAllCategoriesFailed = "Getting all categories failed - {Error}";
        public const string CategoryUpdateStarted = "Category update started for CategoryId: {CategoryId}";
        public const string CategoryUpdateCompleted = "Category update completed for CategoryId: {CategoryId}";
        public const string CategoryUpdateFailed = "Category update failed for CategoryId: {CategoryId} - {Error}";
        public const string CategoryDeletionStarted = "Category deletion started for CategoryId: {CategoryId}";
        public const string CategoryDeletionCompleted = "Category deletion completed for CategoryId: {CategoryId}";
        public const string CategoryDeletionFailed = "Category deletion failed for CategoryId: {CategoryId} - {Error}";
        public const string CartItemAddStarted =
            "Adding item to cart for CustomerId: {CustomerId}, ProductId: {ProductId}";
        public const string CartItemAddCompleted =
            "Item added to cart successfully for CustomerId: {CustomerId}";
        public const string CartItemAddFailed = "Failed to add item to cart: {Error}";
        public const string GetCartStarted = "Getting cart for CustomerId: {CustomerId}";
        public const string GetCartCompleted =
            "Cart retrieved successfully for CustomerId: {CustomerId}";
        public const string GetCartFailed =
            "Failed to get cart for CustomerId: {CustomerId} - {Error}";
        public const string CartItemUpdateStarted =
            "Updating cart item {CartId} for CustomerId: {CustomerId}";
        public const string CartItemUpdateCompleted =
            "Cart item updated successfully for CartId: {CartId}";
        public const string CartItemUpdateFailed = "Failed to update cart item: {Error}";
        public const string CartItemRemoveStarted =
            "Removing cart item {CartId} for CustomerId: {CustomerId}";
        public const string CartItemRemoveCompleted =
            "Cart item removed successfully for CartId: {CartId}";
        public const string CartItemRemoveFailed = "Failed to remove cart item: {Error}";
        public const string CartClearStarted = "Clearing cart for CustomerId: {CustomerId}";
        public const string CartClearCompleted =
            "Cart cleared successfully for CustomerId: {CustomerId}";
        public const string CartClearFailed = "Failed to clear cart: {Error}";
        public const string OrderCreationStarted =
            "Order creation started for CustomerId: {CustomerId}";
        public const string OrderCreationCompleted =
            "Order created successfully with OrderId: {OrderId}";
        public const string OrderCreationFailed = "Order creation failed: {Error}";
        public const string GetOrdersStarted = "Getting orders for CustomerId: {CustomerId}";
        public const string GetOrdersCompleted =
            "Orders retrieved successfully for CustomerId: {CustomerId}";
        public const string GetOrdersFailed = "Failed to get orders: {Error}";
        public const string GetOrderByIdStarted = "Getting order details for OrderId: {OrderId}";
        public const string GetOrderByIdCompleted =
            "Order details retrieved for OrderId: {OrderId}";
        public const string GetOrderByIdFailed = "Failed to get order details: {Error}";
        public const string PaymentInitiationStarted =
            "Payment initiation started for OrderId: {OrderId}";
        public const string PaymentInitiationCompleted =
            "Payment initiated successfully with PaymentId: {PaymentId}";
        public const string PaymentInitiationFailed = "Payment initiation failed: {Error}";
        public const string PaymentVerificationStarted =
            "Payment verification started for PaymentId: {PaymentId}";
        public const string PaymentVerificationCompleted =
            "Payment verification completed for PaymentId: {PaymentId}";
        public const string PaymentVerificationFailed = "Payment verification failed: {Error}";
        public const string GetPaymentsStarted = "Getting payments for CustomerId: {CustomerId}";
        public const string GetPaymentsCompleted =
            "Payments retrieved successfully for CustomerId: {CustomerId}";
        public const string GetPaymentsFailed = "Failed to get payments: {Error}";
        public const string InvalidPaymentId = "Invalid payment ID provided: {PaymentId}";
        public const string StockReservationStarted =
            "Stock reservation started for OrderId: {OrderId}";
        public const string StockReservationCompleted =
            "Stock reservation completed for OrderId: {OrderId}";
        public const string StockReservationFailed =
            "Stock reservation failed for OrderId: {OrderId} - {Error}";
        public const string StockReleaseStarted = "Stock release started for OrderId: {OrderId}";
        public const string StockReleaseCompleted =
            "Stock release completed for OrderId: {OrderId}";
        public const string StockReleaseFailed =
            "Stock release failed for OrderId: {OrderId} - {Error}";
        public const string LowStockAlert =
            "Low stock alert for ProductId: {ProductId}. Current: {Current}, Min: {Min}";
        public const string ShippingCreationStarted = "Shipping creation started for OrderId: {OrderId}";
        public const string ShippingCreationCompleted = "Shipping creation completed with ShippingId: {ShippingId}";
        public const string ShippingCreationFailed = "Shipping creation failed for OrderId: {OrderId} - {Error}";
        public const string ShippingUpdateStarted = "Shipping update started for ShippingId: {ShippingId}";
        public const string ShippingUpdateCompleted = "Shipping update completed for ShippingId: {ShippingId}";
        public const string ShippingUpdateFailed = "Shipping update failed for ShippingId: {ShippingId} - {Error}";
        public const string GetShippingStarted = "Getting shipping details for OrderId: {OrderId}";
        public const string GetShippingCompleted = "Shipping details retrieved for OrderId: {OrderId}";
        public const string GetShippingFailed = "Failed to get shipping details for OrderId: {OrderId} - {Error}";
        public const string GetShippingsByVendorStarted = "Getting shipments for VendorId: {VendorId}";
        public const string GetShippingsByVendorCompleted = "Retrieved {Count} shipments for VendorId: {VendorId}";
        public const string GetShippingsByVendorFailed = "Failed to get shipments for VendorId: {VendorId} - {Error}";
        public const string GetShippingByTrackingStarted = "Getting shipping by tracking number: {TrackingNumber}";
        public const string GetShippingByTrackingCompleted = "Shipping retrieved by tracking number: {TrackingNumber}";
        public const string GetShippingByTrackingFailed = "Failed to get shipping by tracking number: {TrackingNumber} - {Error}";
        
        // Analytics Log Messages
        public const string AnalyticsRetrievalStarted = "Analytics retrieval started for type: {AnalyticsType}";
        public const string AnalyticsRetrievalCompleted = "Analytics retrieval completed for type: {AnalyticsType}";
        public const string AnalyticsRetrievalFailed = "Analytics retrieval failed for type: {AnalyticsType} - {Error}";
    }

    public static class ErrorMessages
    {
        public const string EmailRequired = "Email is required";
        public const string EmailInvalid = "Invalid email format";
        public const string PasswordRequired = "Password is required";
        public const string PasswordTooShort = "Password must be at least 8 characters";
        public const string PhoneRequired = "Phone number is required";
        public const string PhoneInvalid = "Invalid phone number format";
        public const string BusinessNameRequired = "Business name is required";
        public const string OwnerNameRequired = "Owner name is required";
        public const string BusinessLicenseRequired = "Business license number is required";
        public const string BusinessAddressRequired = "Business address is required";
        public const string DocumentRequired = "Business document is required";
        public const string VendorNotFound = "Vendor not found";
        public const string CustomerNotFound = "Customer not found";
        public const string UserNotFound = "User not found";
        public const string EmailAlreadyExists = "Email already exists";
        public const string PhoneAlreadyExists = "Phone number already exists";
        public const string BusinessLicenseExists = "Business license number already exists";
        public const string SubscriptionPlanNotFound = "Subscription plan not found";
        public const string ProductNameRequired = "Product name is required";
        public const string ProductPriceRequired = "Product price is required";
        public const string ProductSKURequired = "Product SKU is required";
        public const string CategoryNotFound = "Category not found";
        public const string CategoryNameRequired = "Category name is required";
        public const string CategoryNameAlreadyExists = "Category name already exists";
        public const string CategoryHasProducts = "Cannot delete category that has products";
        public const string CategoryHasSubCategories = "Cannot delete category that has subcategories";
        public const string InvalidParentCategory = "Invalid parent category";
        public const string CircularReferenceDetected = "Circular reference detected in category hierarchy";
        public const string DatabaseError = "Database operation failed";
        public const string UnexpectedError = "An unexpected error occurred";
        public const string InvalidCredentials = "Invalid email or password";
        public const string AccountLocked =
            "Account is locked due to multiple failed login attempts";
        public const string AccountInactive = "Account is inactive";
        public const string InvalidRefreshToken = "Invalid or expired refresh token";
        public const string TokenExpired = "Access token has expired";
        public const string InvalidCurrentPassword = "Current password is incorrect";
        public const string PasswordTooWeak =
            "Password must contain at least 8 characters with uppercase, lowercase, number and special character";
        public const string OtpExpired = "OTP has expired, please request a new one";
        public const string OtpInvalid = "Invalid OTP entered";
        public const string OtpRequired = "OTP is required";
        public const string EmailSendingFailed = "Failed to send email";
        public const string EmailNotFound = "Email address not found";
        public const string RegistrationPending = "Registration is pending email verification";
        public const string VendorAlreadyApproved = "Vendor is already approved";
        public const string VendorAlreadyRejected = "Vendor is already rejected";
        public const string InvalidVendorStatus = "Invalid vendor status for this operation";
        public const string ApprovalReasonRequired = "Approval reason is required";
        public const string RejectionReasonRequired = "Rejection reason is required";
        public const string InvalidVendorId = "Invalid vendor ID provided";
        public const string ProductNotFound = "Product not found";
        public const string SKUAlreadyExists = "SKU already exists";
        public const string InvalidProductId = "Invalid product ID provided";
        public const string InvalidCategoryId = "Invalid category ID provided";
        public const string InvalidProductStatus = "Invalid product status provided";
        public const string CartItemNotFound = "Cart item not found";
        public const string ProductOutOfStock = "Product is out of stock";
        public const string InsufficientStock = "Insufficient stock available";
        public const string CartItemAlreadyExists = "Product already exists in cart";
        public const string InvalidCustomerId = "Invalid customer ID provided";
        public const string CartEmpty = "Cart is empty";
        public const string OrderNotFound = "Order not found";
        public const string InvalidOrderId = "Invalid order ID provided";
        public const string OrderAlreadyCancelled = "Order is already cancelled";
        public const string OrderCannotBeCancelled = "Order cannot be cancelled in current status";
        public const string InsufficientStockForOrder = "Insufficient stock for one or more items";
        public const string PaymentNotFound = "Payment not found";
        public const string InvalidPaymentId = "Invalid payment ID provided";
        public const string PaymentAlreadyProcessed = "Payment is already processed";
        public const string PaymentFailed = "Payment processing failed";
        public const string InvalidPaymentAmount = "Invalid payment amount";
        public const string PaymentGatewayError = "Payment gateway error occurred";
        public const string StockReservationFailed = "Failed to reserve stock for order";
        public const string StockReleaseFailed = "Failed to release stock for order";
        public const string LowStockWarning = "Product stock is below minimum level";
        public const string ShippingNotFound = "Shipping record not found";
        public const string InvalidShippingId = "Invalid shipping ID provided";
        public const string InvalidTrackingNumber = "Invalid tracking number provided";
        public const string ShippingCreationFailed = "Failed to create shipping record";
        public const string ShippingUpdateFailed = "Failed to update shipping record";
    }

    public static class SuccessMessages
    {
        public const string VendorRegisteredSuccessfully = "Vendor registered successfully";
        public const string VendorProfileUpdatedSuccessfully =
            "Vendor profile updated successfully";
        public const string DocumentUploadedSuccessfully = "Document uploaded successfully";
        public const string SubscriptionSelectedSuccessfully =
            "Subscription plan selected successfully";
        public const string ProductCreatedSuccessfully = "Product created successfully";
        public const string LoginSuccessful = "Login successful";
        public const string TokenRefreshedSuccessfully = "Token refreshed successfully";
        public const string LogoutSuccessful = "Logout successful";
        public const string PasswordChangedSuccessfully = "Password changed successfully";
        public const string OtpSentSuccessfully = "OTP sent to your email successfully";
        public const string OtpVerifiedSuccessfully = "OTP verified successfully";
        public const string RegistrationCompletedSuccessfully =
            "Registration completed successfully";
        public const string PasswordResetSuccessful = "Password reset successful";
        public const string VendorApprovedSuccessfully = "Vendor approved successfully";
        public const string VendorRejectedSuccessfully = "Vendor rejected successfully";
        public const string ProductDeletedSuccessfully = "Product deleted successfully";
        public const string ProductStatusUpdatedSuccessfully =
            "Product status updated successfully";
        public const string ProductImagesUploadedSuccessfully =
            "Product images uploaded successfully";
        public const string CartItemAddedSuccessfully = "Item added to cart successfully";
        public const string CartItemUpdatedSuccessfully = "Cart item updated successfully";
        public const string CartItemRemovedSuccessfully = "Cart item removed successfully";
        public const string CartClearedSuccessfully = "Cart cleared successfully";
        public const string OrderCreatedSuccessfully = "Order created successfully";
        public const string OrderStatusUpdatedSuccessfully = "Order status updated successfully";
        public const string OrderCancelledSuccessfully = "Order cancelled successfully";
        public const string PaymentInitiatedSuccessfully = "Payment initiated successfully";
        public const string PaymentCompletedSuccessfully = "Payment completed successfully";
        public const string PaymentVerifiedSuccessfully = "Payment verified successfully";
        public const string StockReservedSuccessfully = "Stock reserved successfully";
        public const string StockReleasedSuccessfully = "Stock released successfully";
        public const string StockUpdatedSuccessfully = "Stock updated successfully";
        public const string ShippingCreatedSuccessfully = "Shipping record created successfully";
        public const string ShippingUpdatedSuccessfully = "Shipping record updated successfully";
        public const string TrackingDetailsUpdatedSuccessfully = "Tracking details updated successfully";
        public const string OrderShippedSuccessfully = "Order shipped successfully";
        public const string OrderDeliveredSuccessfully = "Order delivered successfully";
        public const string CategoryCreatedSuccessfully = "Category created successfully";
        public const string CategoryUpdatedSuccessfully = "Category updated successfully";
        public const string CategoryDeletedSuccessfully = "Category deleted successfully";
    }

    public static class ConfigKeys
    {
        public const string DatabaseConnectionString = "ConnectionStrings:DatabaseConnectionString";
        public const string JwtSecretKey = "JwtSettings:SecretKey";
        public const string JwtIssuer = "JwtSettings:Issuer";
        public const string JwtAudience = "JwtSettings:Audience";
        public const string JwtExpiryMinutes = "JwtSettings:ExpiryMinutes";
        public const string RefreshTokenExpiryDays = "JwtSettings:RefreshTokenExpiryDays";
        public const string SmtpHost = "EmailSettings:SmtpHost";
        public const string SmtpPort = "EmailSettings:SmtpPort";
        public const string SmtpUsername = "EmailSettings:Username";
        public const string SmtpPassword = "EmailSettings:Password";
        public const string FromEmail = "EmailSettings:FromEmail";
        public const string FromName = "EmailSettings:FromName";
        public const string RazorpayKeyId = "RazorpaySettings:KeyId";
        public const string RazorpayKeySecret = "RazorpaySettings:KeySecret";
    }

    public static class ErrorCodes
    {
        public const string ValidationFailed = "VALIDATION_FAILED";
        public const string VendorNotFound = "VENDOR_NOT_FOUND";
        public const string CustomerNotFound = "CUSTOMER_NOT_FOUND";
        public const string UserNotFound = "USER_NOT_FOUND";
        public const string EmailExists = "EMAIL_EXISTS";
        public const string PhoneExists = "PHONE_EXISTS";
        public const string BusinessLicenseExists = "BUSINESS_LICENSE_EXISTS";
        public const string DatabaseError = "DATABASE_ERROR";
        public const string UnexpectedError = "UNEXPECTED_ERROR";
        public const string InvalidCredentials = "INVALID_CREDENTIALS";
        public const string AccountLocked = "ACCOUNT_LOCKED";
        public const string AccountInactive = "ACCOUNT_INACTIVE";
        public const string InvalidRefreshToken = "INVALID_REFRESH_TOKEN";
        public const string TokenExpired = "TOKEN_EXPIRED";
        public const string InvalidCurrentPassword = "INVALID_CURRENT_PASSWORD";
        public const string WeakPassword = "WEAK_PASSWORD";
        public const string OtpExpired = "OTP_EXPIRED";
        public const string OtpInvalid = "OTP_INVALID";
        public const string EmailSendingFailed = "EMAIL_SENDING_FAILED";
        public const string EmailNotFound = "EMAIL_NOT_FOUND";
        public const string RegistrationPending = "REGISTRATION_PENDING";
        public const string VendorAlreadyApproved = "VENDOR_ALREADY_APPROVED";
        public const string VendorAlreadyRejected = "VENDOR_ALREADY_REJECTED";
        public const string InvalidVendorStatus = "INVALID_VENDOR_STATUS";
        public const string InvalidVendorId = "INVALID_VENDOR_ID";
        public const string ProductNotFound = "PRODUCT_NOT_FOUND";
        public const string SKUExists = "SKU_EXISTS";
        public const string InvalidProductId = "INVALID_PRODUCT_ID";
        public const string InvalidCategoryId = "INVALID_CATEGORY_ID";
        public const string InvalidProductStatus = "INVALID_PRODUCT_STATUS";
        public const string CartItemNotFound = "CART_ITEM_NOT_FOUND";
        public const string ProductOutOfStock = "PRODUCT_OUT_OF_STOCK";
        public const string InsufficientStock = "INSUFFICIENT_STOCK";
        public const string CartItemAlreadyExists = "CART_ITEM_ALREADY_EXISTS";
        public const string InvalidCustomerId = "INVALID_CUSTOMER_ID";
        public const string CartEmpty = "CART_EMPTY";
        public const string OrderNotFound = "ORDER_NOT_FOUND";
        public const string InvalidOrderId = "INVALID_ORDER_ID";
        public const string OrderAlreadyCancelled = "ORDER_ALREADY_CANCELLED";
        public const string OrderCannotBeCancelled = "ORDER_CANNOT_BE_CANCELLED";
        public const string InsufficientStockForOrder = "INSUFFICIENT_STOCK_FOR_ORDER";
        public const string PaymentNotFound = "PAYMENT_NOT_FOUND";
        public const string InvalidPaymentId = "INVALID_PAYMENT_ID";
        public const string PaymentAlreadyProcessed = "PAYMENT_ALREADY_PROCESSED";
        public const string PaymentFailed = "PAYMENT_FAILED";
        public const string InvalidPaymentAmount = "INVALID_PAYMENT_AMOUNT";
        public const string PaymentGatewayError = "PAYMENT_GATEWAY_ERROR";
        public const string StockReservationFailed = "STOCK_RESERVATION_FAILED";
        public const string StockReleaseFailed = "STOCK_RELEASE_FAILED";
        public const string LowStock = "LOW_STOCK";
        public const string ShippingNotFound = "SHIPPING_NOT_FOUND";
        public const string InvalidShippingId = "INVALID_SHIPPING_ID";
        public const string InvalidTrackingNumber = "INVALID_TRACKING_NUMBER";
        public const string ShippingCreationFailed = "SHIPPING_CREATION_FAILED";
        public const string ShippingUpdateFailed = "SHIPPING_UPDATE_FAILED";
        public const string CategoryNameExists = "CATEGORY_NAME_EXISTS";
        public const string CategoryHasProducts = "CATEGORY_HAS_PRODUCTS";
        public const string CategoryHasSubCategories = "CATEGORY_HAS_SUBCATEGORIES";
        public const string InvalidParentCategory = "INVALID_PARENT_CATEGORY";
        public const string CircularReference = "CIRCULAR_REFERENCE";
    }
}
