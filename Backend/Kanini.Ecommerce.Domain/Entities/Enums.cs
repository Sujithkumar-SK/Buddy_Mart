namespace Kanini.Ecommerce.Domain.Enums;

public enum UserRole
{
    Customer = 1,
    Vendor = 2,
    Admin = 3,
}

public enum Gender
{
    Male = 1,
    Female = 2,
    Other = 3,
}

public enum VendorStatus
{
    PendingApproval = 0,
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    Rejected = 4,
}

public enum DocumentStatus
{
    Pending = 1,
    Verified = 2,
    Rejected = 3,
}

public enum DocumentType
{
    BusinessLicense = 1,
    TaxRegistration = 2,
    BankDetails = 3,
    IdentityProof = 4,
    AddressProof = 5,
}

public enum SubscriptionPlan
{
    Basic = 1,
    Standard = 2,
    Premium = 3,
}

public enum ProductStatus
{
    Draft = 0,
    Active = 1,
    Inactive = 2,
    OutOfStock = 3,
}

public enum OrderStatus
{
    Pending = 1,
    Confirmed = 2,
    Processing = 3,
    Shipped = 4,
    Delivered = 5,
    Cancelled = 6,
    Returned = 7,
}

public enum PaymentStatus
{
    Pending = 1,
    Success = 2,
    Failed = 3,
    Refunded = 4,
}

public enum PaymentMethod
{
    Mock = 1,
    UPI = 2,
    Card = 3,
    NetBanking = 4,
    Wallet = 5,
    Razorpay = 6,
}

public enum ShippingStatus
{
    Pending = 1,
    Shipped = 2,
    InTransit = 3,
    Delivered = 4,
    Returned = 5,
}
