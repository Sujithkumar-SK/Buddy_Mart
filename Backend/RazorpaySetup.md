# Razorpay Integration Setup

## 1. Install Razorpay NuGet Package

Add to your API project:
```bash
dotnet add package Razorpay
```

## 2. Get Razorpay Test Credentials

1. Sign up at https://razorpay.com/
2. Go to Dashboard → Settings → API Keys
3. Generate Test Keys (FREE)
4. Copy Key ID and Key Secret

## 3. Update Configuration

Replace in `appsettings.json`:
```json
"RazorpaySettings": {
  "KeyId": "rzp_test_your_actual_key_id",
  "KeySecret": "your_actual_key_secret"
}
```

## 4. Register Services

Add to `ApplicationServiceRegistration.cs`:
```csharp
services.AddScoped<IPaymentService, PaymentService>();
services.AddScoped<IPaymentRepository, PaymentRepository>();
```

## 5. Test Payment Flow

### Initiate Payment:
```
POST /api/payment/initiate
{
  "orderId": 1,
  "paymentMethod": "Card",
  "notes": "Test payment"
}
```

### Verify Payment:
```
POST /api/payment/verify
{
  "razorpayPaymentId": "pay_xxxxx",
  "razorpayOrderId": "order_xxxxx", 
  "razorpaySignature": "signature_xxxxx"
}
```

## 6. Test Cards (Razorpay Test Mode)

- **Success**: 4111 1111 1111 1111
- **Failure**: 4000 0000 0000 0002
- **CVV**: Any 3 digits
- **Expiry**: Any future date

## 7. Frontend Integration

Use Razorpay Checkout.js with the response from `/api/payment/initiate`

**All transactions in test mode are FREE!**