# E-commerce API Documentation

## Base URL
```
https://localhost:7000/api
```

## Authentication
Most endpoints require JWT Bearer token authentication. Include the token in the Authorization header:
```
Authorization: Bearer <your_jwt_token>
```

---

## 🔐 Authentication Endpoints

### 1. Login
**POST** `/auth/login`

Authenticate user and get access tokens.

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "userId": 1,
  "email": "user@example.com",
  "role": "Customer",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh_token_here",
  "accessTokenExpiry": "2024-01-01T12:00:00Z",
  "refreshTokenExpiry": "2024-01-31T12:00:00Z",
  "message": "Login successful"
}
```

### 2. Refresh Token
**POST** `/auth/refresh`

Get new access token using refresh token.

**Request Body:**
```json
{
  "refreshToken": "refresh_token_here"
}
```

### 3. Logout
**POST** `/auth/logout` 🔒

Revoke refresh token and logout user.

**Request Body:**
```json
{
  "refreshToken": "refresh_token_here"
}
```

### 4. Change Password
**POST** `/auth/change-password` 🔒

Change user password.

**Request Body:**
```json
{
  "currentPassword": "oldPassword123",
  "newPassword": "newPassword123"
}
```

### 5. Register with OTP
**POST** `/auth/register-with-otp`

Start registration process by sending OTP to email.

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "password123",
  "phone": "9876543210",
  "role": 1,
  "firstName": "John",
  "middleName": "M",
  "lastName": "Doe",
  "dateOfBirth": "1990-01-01",
  "gender": 1,
  "address": "123 Main St",
  "city": "Chennai",
  "state": "Tamil Nadu",
  "pinCode": "600001",
  "businessName": "Tech Store",
  "ownerName": "John Doe",
  "businessLicenseNumber": "BL123456",
  "businessAddress": "123 Business St",
  "taxRegistrationNumber": "TAX123456"
}
```

**Field Descriptions:**
- `role`: 1 = Customer, 2 = Vendor
- `gender`: 1 = Male, 2 = Female, 3 = Other
- Vendor fields (`businessName`, `ownerName`, etc.) are required only when `role = 2`

**Response:**
```json
{
  "email": "user@example.com",
  "otpToken": "jwt_token_with_registration_data",
  "expiresAt": "2024-01-01T12:05:00Z",
  "message": "OTP sent to your email successfully"
}
```

### 6. Verify Registration OTP
**POST** `/auth/verify-registration-otp`

Complete registration by verifying OTP.

**Request Body:**
```json
{
  "email": "user@example.com",
  "otp": "123456",
  "otpToken": "jwt_token_from_previous_step",
  "role": 1
}
```

**Response:**
```json
{
  "userId": 1,
  "email": "user@example.com",
  "role": "Customer",
  "customerId": 1,
  "vendorId": null,
  "message": "Registration completed successfully",
  "requiresApproval": false
}
```

### 7. Forgot Password
**POST** `/auth/forgot-password`

Send OTP for password reset.

**Request Body:**
```json
{
  "email": "user@example.com"
}
```

### 8. Verify Forgot Password OTP
**POST** `/auth/verify-forgot-password-otp`

Verify OTP for password reset.

**Request Body:**
```json
{
  "email": "user@example.com",
  "otp": "123456",
  "otpToken": "jwt_token_from_forgot_password"
}
```

### 9. Reset Password
**POST** `/auth/reset-password`

Reset password after OTP verification.

**Request Body:**
```json
{
  "email": "user@example.com",
  "newPassword": "newPassword123"
}
```

---

## 👤 Customer Endpoints

### 1. Get Customer Profile
**GET** `/customer/{customerId}/profile` 🔒

Get customer profile by ID.

**Response:**
```json
{
  "customerId": 1,
  "firstName": "John",
  "middleName": "M",
  "lastName": "Doe",
  "fullName": "John M Doe",
  "email": "john@example.com",
  "phone": "9876543210",
  "dateOfBirth": "1990-01-01",
  "gender": "Male",
  "address": "123 Main St",
  "city": "Chennai",
  "state": "Tamil Nadu",
  "pinCode": "600001",
  "isActive": true
}
```

### 2. Get All Customers
**GET** `/customer/all` 🔒

Get list of all customers.

**Response:**
```json
[
  {
    "customerId": 1,
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com",
    "phone": "9876543210",
    "isActive": true
  }
]
```

### 3. Update Customer Profile
**PUT** `/customer/{customerId}/profile` 🔒

Update customer profile information.

**Request Body:**
```json
{
  "firstName": "John",
  "middleName": "M",
  "lastName": "Doe",
  "dateOfBirth": "1990-01-01",
  "gender": 1,
  "address": "123 Updated St",
  "city": "Chennai",
  "state": "Tamil Nadu",
  "pinCode": "600001"
}
```

### 4. Delete Customer
**DELETE** `/customer/{customerId}` 🔒

Soft delete customer account.

**Response:**
```json
{
  "message": "Customer deleted successfully"
}
```

---

## 🏪 Vendor Endpoints

### 1. Get Vendor Profile
**GET** `/vendor/{vendorId}/profile`

Get vendor profile by ID.

**Response:**
```json
{
  "vendorId": 1,
  "businessName": "Tech Store",
  "ownerName": "John Doe",
  "businessLicenseNumber": "BL123456",
  "businessAddress": "123 Business St",
  "city": "Chennai",
  "state": "Tamil Nadu",
  "pinCode": "600001",
  "taxRegistrationNumber": "TAX123456",
  "documentPath": "/documents/license.pdf",
  "documentStatus": "Verified",
  "status": "Active",
  "currentPlan": "Basic",
  "isActive": true
}
```

### 2. Get Subscription Plans
**GET** `/vendor/subscription-plans`

Get available subscription plans for vendors.

**Response:**
```json
[
  {
    "planId": 1,
    "planName": "Basic",
    "description": "Basic plan with 10 products",
    "price": 999.00,
    "maxProducts": 10,
    "durationDays": 30,
    "isActive": true
  },
  {
    "planId": 2,
    "planName": "Standard",
    "description": "Standard plan with 50 products",
    "price": 2999.00,
    "maxProducts": 50,
    "durationDays": 30,
    "isActive": true
  }
]
```

### 3. Select Subscription Plan
**POST** `/vendor/{vendorId}/select-subscription/{planId}`

Select a subscription plan for vendor.

**Response:**
```json
{
  "message": "Subscription plan selected successfully"
}
```

### 4. Upload Document
**POST** `/vendor/{vendorId}/upload-document`

Upload business document for verification.

**Request:** Multipart form data with PDF file

**Response:**
```json
{
  "message": "Document uploaded successfully",
  "path": "/documents/1_20240101120000_license.pdf"
}
```

---

## 📊 Status Codes

| Code | Description |
|------|-------------|
| 200 | Success |
| 201 | Created |
| 400 | Bad Request |
| 401 | Unauthorized |
| 403 | Forbidden |
| 404 | Not Found |
| 409 | Conflict |
| 500 | Internal Server Error |

---

## 🔑 Error Response Format

```json
{
  "type": "Validation",
  "code": "VALIDATION_FAILED",
  "description": "Email is required"
}
```

---

## 📝 Notes

### User Roles
- **1**: Customer - Can browse and purchase products
- **2**: Vendor - Can sell products (requires admin approval)
- **3**: Admin - System administrator

### Vendor Approval Process
1. Vendor registers with business details
2. Account created with `IsActive = false`
3. Admin reviews and approves vendor
4. Vendor can start selling products

### OTP Expiry
- All OTPs expire in 5 minutes
- JWT tokens contain encrypted registration data

### Account Security
- Failed login attempts: Max 5 attempts
- Account lockout: 30 minutes after max attempts
- Password requirements: Minimum 8 characters

### File Upload
- Only PDF files allowed for vendor documents
- Files stored in `/wwwroot/documents/` directory
- Filename format: `{vendorId}_{timestamp}_{originalName}`

---

## 🚀 Getting Started

1. **Register as Customer:**
   ```
   POST /auth/register-with-otp (role: 1)
   POST /auth/verify-registration-otp
   ```

2. **Register as Vendor:**
   ```
   POST /auth/register-with-otp (role: 2, include business fields)
   POST /auth/verify-registration-otp
   POST /vendor/{vendorId}/upload-document
   POST /vendor/{vendorId}/select-subscription/{planId}
   ```

3. **Login:**
   ```
   POST /auth/login
   ```

4. **Access Protected Resources:**
   ```
   Include JWT token in Authorization header
   ```