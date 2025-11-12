# E-commerce API Endpoints Test Guide

## Base URL
```
https://localhost:7000/api
```

## Authentication
Most endpoints require JWT Bearer token in the Authorization header:
```
Authorization: Bearer <your_jwt_token>
```

---

## 🔐 Authentication Endpoints

### 1. User Login
**POST** `/auth/login`

**Request Body:**
```json
{
  "email": "customer1@gmail.com",
  "password": "Password123"
}
```

**Response (200):**
```json
{
  "userId": 2,
  "email": "customer1@gmail.com",
  "role": "Customer",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "abc123def456...",
  "accessTokenExpiry": "2024-01-01T12:30:00Z",
  "refreshTokenExpiry": "2024-01-31T12:00:00Z",
  "message": "Login successful"
}
```

---

### 2. Refresh Token
**POST** `/auth/refresh`

**Request Body:**
```json
{
  "refreshToken": "abc123def456..."
}
```

**Response (200):**
```json
{
  "userId": 2,
  "email": "customer1@gmail.com",
  "role": "Customer",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "new_refresh_token...",
  "accessTokenExpiry": "2024-01-01T12:30:00Z",
  "refreshTokenExpiry": "2024-01-31T12:00:00Z",
  "message": "Token refreshed successfully"
}
```

---

### 3. User Logout
**POST** `/auth/logout`
**Authorization:** Bearer Token Required

**Request Body:**
```json
{
  "refreshToken": "abc123def456..."
}
```

**Response (200):**
```json
{
  "message": "Logout successful"
}
```

---

### 4. Change Password
**POST** `/auth/change-password`
**Authorization:** Bearer Token Required

**Request Body:**
```json
{
  "currentPassword": "OldPassword123",
  "newPassword": "NewPassword123"
}
```

**Response (200):**
```json
{
  "message": "Password changed successfully"
}
```

---

### 5. Register with OTP (Customer)
**POST** `/auth/register-with-otp`

**Request Body:**
```json
{
  "email": "newcustomer@gmail.com",
  "password": "Password123",
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
  "pinCode": "600001"
}
```

**Response (200):**
```json
{
  "email": "newcustomer@gmail.com",
  "otpToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-01-01T12:05:00Z",
  "message": "OTP sent to your email successfully"
}
```

---

### 6. Register with OTP (Vendor)
**POST** `/auth/register-with-otp`

**Note:** Business details are not required during registration. Create vendor profile separately after registration.

**Request Body:**
```json
{
  "email": "newvendor@gmail.com",
  "password": "Password123",
  "phone": "9876543211",
  "role": 2,
  "firstName": "Jane",
  "lastName": "Smith"
}
```

**Response (200):**
```json
{
  "email": "newvendor@gmail.com",
  "otpToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-01-01T12:05:00Z",
  "message": "OTP sent to your email successfully"
}
```

---

### 7. Verify Registration OTP (Customer)
**POST** `/auth/verify-registration-otp`

**Request Body:**
```json
{
  "email": "newcustomer@gmail.com",
  "otp": "123456",
  "otpToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "role": 1
}
```

**Response (200):**
```json
{
  "userId": 3,
  "email": "newcustomer@gmail.com",
  "role": "Customer",
  "customerId": 2,
  "vendorId": null,
  "message": "Registration completed successfully",
  "requiresApproval": false
}
```

---

### 8. Verify Registration OTP (Vendor)
**POST** `/auth/verify-registration-otp`

**Request Body:**
```json
{
  "email": "newvendor@gmail.com",
  "otp": "123456",
  "otpToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "role": 2
}
```

**Response (200):**
```json
{
  "userId": 4,
  "email": "newvendor@gmail.com",
  "role": "Vendor",
  "customerId": null,
  "vendorId": null,
  "message": "Vendor user account created. Please complete your vendor profile to start selling.",
  "requiresApproval": false
}
```

---

### 9. Forgot Password
**POST** `/auth/forgot-password`

**Request Body:**
```json
{
  "email": "customer1@gmail.com"
}
```

**Response (200):**
```json
{
  "email": "customer1@gmail.com",
  "otpToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-01-01T12:05:00Z",
  "message": "OTP sent to your email successfully"
}
```

---

### 10. Verify Forgot Password OTP
**POST** `/auth/verify-forgot-password-otp`

**Request Body:**
```json
{
  "email": "customer1@gmail.com",
  "otp": "123456",
  "otpToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Response (200):**
```json
{
  "message": "OTP verified successfully"
}
```

---

### 11. Reset Password
**POST** `/auth/reset-password`

**Request Body:**
```json
{
  "email": "customer1@gmail.com",
  "newPassword": "NewPassword123"
}
```

**Response (200):**
```json
{
  "message": "Password reset successful"
}
```

---

## 👤 Customer Endpoints

### 12. Get Customer Profile
**GET** `/customer/{customerId}/profile`
**Authorization:** Bearer Token Required

**Example:** `/customer/1/profile`

**Response (200):**
```json
{
  "customerId": 1,
  "firstName": "Jane",
  "middleName": null,
  "lastName": "Smith",
  "fullName": "Jane Smith",
  "dateOfBirth": "1990-05-15",
  "gender": "Female",
  "address": "456 Oak St, Chennai",
  "city": "Chennai",
  "state": "Tamil Nadu",
  "pinCode": "600002",
  "email": "customer1@gmail.com",
  "phone": "9876543211",
  "isActive": true
}
```

---

### 13. Get All Customers
**GET** `/customer/all`
**Authorization:** Bearer Token Required

**Response (200):**
```json
[
  {
    "customerId": 1,
    "firstName": "Jane",
    "lastName": "Smith",
    "fullName": "Jane Smith",
    "email": "customer1@gmail.com",
    "phone": "9876543211",
    "city": "Chennai",
    "isActive": true
  }
]
```

---

### 14. Update Customer Profile
**PUT** `/customer/{customerId}/profile`
**Authorization:** Bearer Token Required

**Example:** `/customer/1/profile`

**Request Body:**
```json
{
  "firstName": "Jane",
  "middleName": "M",
  "lastName": "Smith",
  "dateOfBirth": "1990-05-15",
  "gender": 2,
  "address": "456 Updated Oak St, Chennai",
  "city": "Chennai",
  "state": "Tamil Nadu",
  "pinCode": "600002"
}
```

**Response (200):**
```json
{
  "customerId": 1,
  "firstName": "Jane",
  "middleName": "M",
  "lastName": "Smith",
  "fullName": "Jane M Smith",
  "dateOfBirth": "1990-05-15",
  "gender": "Female",
  "address": "456 Updated Oak St, Chennai",
  "city": "Chennai",
  "state": "Tamil Nadu",
  "pinCode": "600002",
  "isActive": true
}
```

---

### 15. Delete Customer
**DELETE** `/customer/{customerId}`
**Authorization:** Bearer Token Required

**Example:** `/customer/1`

**Response (200):**
```json
{
  "message": "Customer deleted successfully"
}
```

---

## 🏪 Vendor Endpoints

### 16. Get Vendor Profile
**GET** `/vendor/{vendorId}/profile`

**Example:** `/vendor/1/profile`

**Response (200):**
```json
{
  "vendorId": 1,
  "businessName": "Tech Store",
  "ownerName": "John Doe",
  "businessLicenseNumber": "BL123456",
  "businessAddress": "123 Main St, Chennai",
  "city": "Chennai",
  "state": "Tamil Nadu",
  "pinCode": "600001",
  "taxRegistrationNumber": null,
  "documentPath": "/docs/license.pdf",
  "documentStatus": "Verified",
  "currentPlan": "Basic",
  "status": "Active",
  "isActive": true
}
```

---

### 17. Get Subscription Plans
**GET** `/vendor/subscription-plans`

**Response (200):**
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

---

### 18. Select Subscription Plan
**POST** `/vendor/{vendorId}/select-subscription/{planId}`

**Example:** `/vendor/1/select-subscription/2`

**Response (200):**
```json
{
  "message": "Subscription plan selected successfully"
}
```

---

### 19. Create Vendor Profile
**POST** `/vendor/create-profile`
**Authorization:** Bearer Token Required (Vendor Role)

**Note:** Get available subscription plans from `/vendor/subscription-plans` endpoint first.

**Request Body:**
```json
{
  "businessName": "Tech Solutions Pvt Ltd",
  "ownerName": "Jane Smith",
  "businessLicenseNumber": "BL789012",
  "businessAddress": "456 Business Park, Chennai",
  "city": "Chennai",
  "state": "Tamil Nadu",
  "pinCode": "600002",
  "taxRegistrationNumber": "GST123456789",
  "subscriptionPlanId": 1
}
```

**Response (200):**
```json
{
  "message": "Vendor profile created successfully. Awaiting admin approval.",
  "vendorProfile": {
    "vendorId": 2,
    "businessName": "Tech Solutions Pvt Ltd",
    "ownerName": "Jane Smith",
    "businessLicenseNumber": "BL789012",
    "status": "PendingApproval",
    "documentStatus": "Pending",
    "isActive": false
  }
}
```

---

### 20. Upload Document
**POST** `/vendor/{vendorId}/upload-document`
**Content-Type:** multipart/form-data

**Example:** `/vendor/1/upload-document`

**Form Data:**
- `document`: [PDF file]

**Response (200):**
```json
{
  "message": "Document uploaded successfully",
  "path": "/documents/1_20241105123456_business_license.pdf"
}
```

---

## 🧪 Test Scenarios

### Authentication Flow Test
1. **Register Customer with OTP**
   - POST `/auth/register-with-otp` with role: 1
   - Check email for OTP
   - POST `/auth/verify-registration-otp` with received OTP

2. **Register Vendor with OTP**
   - POST `/auth/register-with-otp` with role: 2 and business details
   - Check email for OTP
   - POST `/auth/verify-registration-otp` with received OTP

3. **Login Flow**
   - POST `/auth/login` with registered credentials
   - Use received accessToken for authenticated requests
   - POST `/auth/refresh` when token expires
   - POST `/auth/logout` to end session

4. **Password Management**
   - POST `/auth/forgot-password` with email
   - POST `/auth/verify-forgot-password-otp` with OTP
   - POST `/auth/reset-password` with new password
   - POST `/auth/change-password` when logged in

---

## 🛍️ Product Management Endpoints

### 21. Create Product
**POST** `/product`
**Authorization:** Bearer Token Required (Vendor Role)

**Request Body:**
```json
{
  "name": "iPhone 15 Pro Max",
  "description": "Latest flagship smartphone with titanium design",
  "sku": "APPLE-IPH15PM-256GB",
  "price": 134900.00,
  "discountPrice": 129900.00,
  "stockQuantity": 25,
  "minStockLevel": 5,
  "brand": "Apple",
  "weight": "221g",
  "dimensions": "159.9 x 76.7 x 8.25 mm",
  "vendorId": 1,
  "categoryId": 2
}
```

**Response (201):**
```json
{
  "productId": 1,
  "name": "iPhone 15 Pro Max",
  "description": "Latest flagship smartphone with titanium design",
  "sku": "APPLE-IPH15PM-256GB",
  "price": 134900.00,
  "discountPrice": 129900.00,
  "stockQuantity": 25,
  "minStockLevel": 5,
  "brand": "Apple",
  "weight": "221g",
  "dimensions": "159.9 x 76.7 x 8.25 mm",
  "status": "Draft",
  "isActive": true,
  "vendorId": 1,
  "vendorName": "Tech Store",
  "categoryId": 2,
  "categoryName": "Smartphones",
  "images": []
}
```

---

### 22. Upload Product Images
**POST** `/product/{productId}/images`
**Authorization:** Bearer Token Required (Vendor Role)
**Content-Type:** multipart/form-data

**Example:** `/product/1/images`

**Form Data:**
- `images`: [Multiple image files - JPG, JPEG, PNG, GIF]

**Response (200):**
```json
{
  "message": "Product images uploaded successfully",
  "imagePaths": [
    "/images/1/20241106123045_abc123_image1.jpg",
    "/images/1/20241106123046_def456_image2.jpg"
  ]
}
```

---

### 23. Get Product by ID
**GET** `/product/{id}`

**Example:** `/product/1`

**Response (200):**
```json
{
  "productId": 1,
  "name": "iPhone 15 Pro Max",
  "description": "Latest flagship smartphone with titanium design",
  "sku": "APPLE-IPH15PM-256GB",
  "price": 134900.00,
  "discountPrice": 129900.00,
  "stockQuantity": 25,
  "brand": "Apple",
  "status": "Active",
  "vendorId": 1,
  "vendorName": "Tech Store",
  "categoryId": 2,
  "categoryName": "Smartphones",
  "images": [
    "/images/1/20241106123045_abc123_image1.jpg",
    "/images/1/20241106123046_def456_image2.jpg"
  ]
}
```

---

### 24. Get All Products
**GET** `/product`

**Response (200):**
```json
[
  {
    "productId": 1,
    "name": "iPhone 15 Pro Max",
    "sku": "APPLE-IPH15PM-256GB",
    "price": 134900.00,
    "discountPrice": 129900.00,
    "stockQuantity": 25,
    "brand": "Apple",
    "status": "Active",
    "vendorName": "Tech Store",
    "categoryName": "Smartphones",
    "primaryImagePath": "/images/1/20241106123045_abc123_image1.jpg"
  }
]
```

---

### 25. Get Products by Vendor
**GET** `/product/vendor/{vendorId}`
**Authorization:** Bearer Token Required (Vendor/Admin Role)

**Example:** `/product/vendor/1`

**Response (200):**
```json
[
  {
    "productId": 1,
    "name": "iPhone 15 Pro Max",
    "sku": "APPLE-IPH15PM-256GB",
    "price": 134900.00,
    "stockQuantity": 25,
    "status": "Active",
    "categoryName": "Smartphones",
    "primaryImagePath": "/images/1/20241106123045_abc123_image1.jpg"
  }
]
```

---

### 26. Get Products by Category
**GET** `/product/category/{categoryId}`

**Example:** `/product/category/2`

**Response (200):**
```json
[
  {
    "productId": 1,
    "name": "iPhone 15 Pro Max",
    "sku": "APPLE-IPH15PM-256GB",
    "price": 134900.00,
    "stockQuantity": 25,
    "vendorName": "Tech Store",
    "primaryImagePath": "/images/1/20241106123045_abc123_image1.jpg"
  }
]
```

---

### 27. Update Product
**PUT** `/product/{id}`
**Authorization:** Bearer Token Required (Vendor Role)

**Example:** `/product/1`

**Request Body:**
```json
{
  "name": "iPhone 15 Pro Max - Updated",
  "description": "Updated description with new features",
  "price": 139900.00,
  "discountPrice": 134900.00,
  "stockQuantity": 30,
  "minStockLevel": 10,
  "brand": "Apple",
  "weight": "221g",
  "dimensions": "159.9 x 76.7 x 8.25 mm",
  "categoryId": 2
}
```

**Response (200):**
```json
{
  "productId": 1,
  "name": "iPhone 15 Pro Max - Updated",
  "description": "Updated description with new features",
  "price": 139900.00,
  "discountPrice": 134900.00,
  "stockQuantity": 30,
  "status": "Active",
  "vendorName": "Tech Store",
  "categoryName": "Smartphones"
}
```

---

### 28. Update Product Status
**PATCH** `/product/{id}/status`
**Authorization:** Bearer Token Required (Vendor/Admin Role)

**Example:** `/product/1/status`

**Request Body:**
```json
"Active"
```

**Valid Status Values:**
- `"Draft"`
- `"Active"`
- `"Inactive"`
- `"OutOfStock"`

**Response (200):**
```json
{
  "message": "Product status updated successfully"
}
```

---

### 29. Delete Product
**DELETE** `/product/{id}`
**Authorization:** Bearer Token Required (Vendor/Admin Role)

**Example:** `/product/1`

**Response (200):**
```json
{
  "message": "Product deleted successfully"
}
```

---

## 👑 Admin Endpoints

### 30. Get Pending Vendors
**GET** `/admin/vendors/pending`
**Authorization:** Bearer Token Required (Admin Role)

**Response (200):**
```json
[
  {
    "vendorId": 2,
    "businessName": "New Tech Solutions",
    "ownerName": "Jane Smith",
    "businessLicenseNumber": "BL789012",
    "email": "newvendor@gmail.com",
    "phone": "9876543211",
    "documentStatus": "Pending",
    "status": "PendingApproval",
    "createdOn": "2024-01-01T10:00:00Z"
  }
]
```

---

### 31. Get Vendor for Approval
**GET** `/admin/vendors/{vendorId}/approval`
**Authorization:** Bearer Token Required (Admin Role)

**Example:** `/admin/vendors/2/approval`

**Response (200):**
```json
{
  "vendorId": 2,
  "businessName": "New Tech Solutions",
  "ownerName": "Jane Smith",
  "businessLicenseNumber": "BL789012",
  "businessAddress": "456 Business Park, Chennai",
  "city": "Chennai",
  "state": "Tamil Nadu",
  "pinCode": "600002",
  "taxRegistrationNumber": "GST123456789",
  "documentPath": "/documents/2_20241105123456_business_license.pdf",
  "documentStatus": "Pending",
  "status": "PendingApproval",
  "email": "newvendor@gmail.com",
  "phone": "9876543211",
  "createdOn": "2024-01-01T10:00:00Z"
}
```

---

### 32. Approve Vendor
**POST** `/admin/vendors/approve`
**Authorization:** Bearer Token Required (Admin Role)

**Request Body:**
```json
{
  "vendorId": 2,
  "approvalReason": "All documents verified successfully. Business license is valid."
}
```

**Response (200):**
```json
{
  "message": "Vendor approved successfully"
}
```

---

### 33. Reject Vendor
**POST** `/admin/vendors/reject`
**Authorization:** Bearer Token Required (Admin Role)

**Request Body:**
```json
{
  "vendorId": 2,
  "rejectionReason": "Invalid business license number. Please resubmit with correct documentation."
}
```

**Response (200):**
```json
{
  "message": "Vendor rejected successfully"
}
```

---

## 🧪 Complete Test Scenarios

### Product Management Flow Test
1. **Create Product**
   - Login as Vendor
   - POST `/product` with product details
   - Note the returned productId

2. **Upload Product Images**
   - POST `/product/{productId}/images` with image files
   - Verify images are stored in `/images/{productId}/` folder

3. **Verify Product Creation**
   - GET `/product/{productId}` to see product with images
   - GET `/product` to see product in list
   - GET `/product/vendor/{vendorId}` to see in vendor's products

4. **Update Product**
   - PUT `/product/{productId}` with updated details
   - PATCH `/product/{productId}/status` to change status

5. **Clean Up**
   - DELETE `/product/{productId}` to remove test product

### Admin Workflow Test
1. **Vendor Registration**
   - Register new vendor user with POST `/auth/register-with-otp` (role: 2)
   - Verify OTP with POST `/auth/verify-registration-otp`
   - Vendor logs in and creates profile with POST `/vendor/create-profile`

2. **Admin Approval**
   - Login as Admin
   - GET `/admin/vendors/pending` to see pending vendors
   - GET `/admin/vendors/{vendorId}/approval` for details
   - POST `/admin/vendors/approve` or `/admin/vendors/reject`

3. **Vendor Activation**
   - Approved vendor can now create products
   - POST `/vendor/{vendorId}/select-subscription/{planId}`

### Error Testing Scenarios
1. **Authentication Errors**
   - Invalid credentials → 400 Bad Request
   - Expired token → 401 Unauthorized
   - Wrong role access → 403 Forbidden

2. **Validation Errors**
   - Duplicate SKU → 400 Bad Request
   - Invalid file format → 400 Bad Request
   - Missing required fields → 400 Bad Request

3. **Business Logic Errors**
   - Non-existent product → 404 Not Found
   - Inactive vendor creating product → 400 Bad Request
   - File size exceeding limit → 400 Bad Request

---

## 📝 Testing Checklist

### ✅ Authentication
- [ ] User registration (Customer & Vendor)
- [ ] OTP verification
- [ ] Login/Logout
- [ ] Token refresh
- [ ] Password reset
- [ ] Change password

### ✅ Product Management
- [ ] Create product
- [ ] Upload product images
- [ ] Get product by ID
- [ ] Get all products
- [ ] Get products by vendor
- [ ] Get products by category
- [ ] Update product
- [ ] Update product status
- [ ] Delete product

### ✅ Admin Functions
- [ ] Get pending vendors
- [ ] Get vendor approval details
- [ ] Approve vendor
- [ ] Reject vendor

### ✅ Vendor Functions
- [ ] Get vendor profile
- [ ] Get subscription plans
- [ ] Select subscription plan
- [ ] Upload documents

### ✅ Customer Functions
- [ ] Get customer profile
- [ ] Update customer profile
- [ ] Get all customers
- [ ] Delete customer

### ✅ File Operations
- [ ] Document upload (PDF)
- [ ] Image upload (JPG, PNG, GIF)
- [ ] File size validation
- [ ] File type validation
- [ ] Folder structure verification

---

## 🚨 Common Issues & Solutions

### Issue: 401 Unauthorized
**Solution:** Ensure Bearer token is included in Authorization header

### Issue: 403 Forbidden
**Solution:** Check if user has correct role for the endpoint

### Issue: 400 Bad Request - SKU exists
**Solution:** Use unique SKU for each product

### Issue: File upload fails
**Solution:** Check file size (<5MB) and format (JPG, PNG, GIF for images, PDF for documents)

### Issue: Product images not showing
**Solution:** Verify images are uploaded to correct folder `/images/{productId}/`

---

## 📊 Expected File Structure

```
wwwroot/
├── documents/
│   ├── 1_20241105123456_business_license.pdf
│   └── 2_20241105123457_tax_registration.pdf
└── images/
    ├── 1/                    # Product ID 1
    │   ├── 20241106123045_abc123_image1.jpg
    │   └── 20241106123046_def456_image2.jpg
    └── 2/                    # Product ID 2
        └── 20241106123047_ghi789_image1.jpg
``` returned access token for authenticated requests

4. **Token Refresh**
   - POST `/auth/refresh` with refresh token
   - Use new access token

5. **Password Reset Flow**
   - POST `/auth/forgot-password`
   - POST `/auth/verify-forgot-password-otp` with OTP
   - POST `/auth/reset-password` with new password

### Customer Management Test
1. **Profile Operations**
   - GET `/customer/{id}/profile`
   - PUT `/customer/{id}/profile` with updates
   - GET `/customer/all` to list all customers

### Vendor Management Test
1. **Vendor Registration Flow**
   - POST `/auth/register-with-otp` with role: 2 (no business fields)
   - POST `/auth/verify-registration-otp` to complete user registration
   - POST `/auth/login` to get vendor token
   - POST `/vendor/create-profile` to create vendor business profile
   - POST `/vendor/{id}/upload-document` with PDF file
   - Admin approves vendor via `/admin/vendors/approve`

2. **Vendor Operations**
   - GET `/vendor/{id}/profile`
   - GET `/vendor/subscription-plans`
   - POST `/vendor/{id}/select-subscription/{planId}`

---

## 🚨 Error Responses

### Common Error Formats

**400 Bad Request:**
```json
{
  "type": "VALIDATION_FAILED",
  "code": "VALIDATION_FAILED",
  "description": "Role must be either 1 (Customer) or 2 (Vendor)"
}
```

**401 Unauthorized:**
```json
{
  "type": "INVALID_CREDENTIALS",
  "code": "INVALID_CREDENTIALS", 
  "description": "Invalid email or password"
}
```

**404 Not Found:**
```json
{
  "type": "USER_NOT_FOUND",
  "code": "USER_NOT_FOUND",
  "description": "User not found"
}
```

**409 Conflict:**
```json
{
  "type": "EMAIL_EXISTS",
  "code": "EMAIL_EXISTS",
  "description": "Email already exists"
}
```

**500 Internal Server Error:**
```json
{
  "error": "An unexpected error occurred"
}
```

---

## 📝 Notes

1. **Role Values:**
   - 1 = Customer
   - 2 = Vendor

2. **Gender Values:**
   - 1 = Male
   - 2 = Female
   - 3 = Other

3. **Document Status:**
   - Pending = 1
   - Verified = 2
   - Rejected = 3

4. **Vendor Status:**
   - PendingApproval = 0
   - Active = 1
   - Inactive = 2
   - Suspended = 3
   - Rejected = 4

5. **Authentication:**
   - Access tokens expire in 30 minutes
   - Refresh tokens expire in 30 days
   - OTP tokens expire in 5 minutes

6. **File Upload:**
   - Only PDF files are allowed for vendor documents
   - Maximum file size limits apply (check server configuration)

---

## 🔧 Testing Tools

### Swagger UI
Access the interactive API documentation at:
```
https://localhost:7000/swagger
```

---

## 🚀 Quick Swagger Testing Guide

### Step 1: Start the API
```bash
cd Backend/Kanini.Ecommerce.Api
dotnet run
```

### Step 2: Open Swagger
Navigate to: `https://localhost:7xxx/swagger` (check console for exact port)

### Step 3: Authentication Setup
1. **Login First:**
   - Expand `POST /api/auth/login`
   - Click "Try it out"
   - Use test credentials:
     ```json
     {
       "email": "admin@ecommerce.com",
       "password": "hashedpassword123"
     }
     ```
   - Copy the `accessToken` from response

2. **Set Authorization:**
   - Click the **"Authorize"** button (🔒 icon) at top
   - Enter: `Bearer YOUR_ACCESS_TOKEN`
   - Click "Authorize"

### Step 4: Test Endpoints

#### **Product Management Testing:**
1. **Create Product** (Vendor role required):
   ```json
   {
     "name": "Test iPhone",
     "sku": "TEST-IPH-001",
     "price": 99999.00,
     "stockQuantity": 10,
     "vendorId": 1,
     "categoryId": 2
   }
   ```

2. **Upload Images**:
   - Use the returned `productId` from step 1
   - Select image files (JPG, PNG, GIF)
   - Images will be stored in `/images/{productId}/`

3. **Get Product**:
   - Use `GET /api/product/{id}` with the productId
   - Verify images are included in response

#### **Admin Testing:**
1. **Get Pending Vendors**:
   - `GET /api/admin/vendors/pending`
   - Requires Admin role

2. **Approve/Reject Vendor**:
   - Use vendorId from pending list
   - Provide approval/rejection reason

#### **File Upload Testing:**
1. **Document Upload**:
   - `POST /api/vendor/{id}/upload-document`
   - Select PDF file only
   - Max file size limits apply

2. **Image Upload**:
   - `POST /api/product/{id}/images`
   - Select multiple image files
   - Check file size (<5MB each)

### Step 5: Common Test Data

#### **Test Users:**
```json
// Admin
{"email": "admin@ecommerce.com", "password": "hashedpassword123"}

// Customer  
{"email": "customer1@gmail.com", "password": "hashedpassword123"}

// Vendor (after creating profile)
{"email": "vendor1@gmail.com", "password": "hashedpassword123"}
```

#### **Test Vendor Profile:**
```json
{
  "businessName": "Test Electronics Store",
  "ownerName": "John Vendor",
  "businessLicenseNumber": "BL123TEST",
  "businessAddress": "123 Business Street, Chennai",
  "city": "Chennai",
  "state": "Tamil Nadu",
  "pinCode": "600001",
  "taxRegistrationNumber": "GST123456789"
}
```

#### **Test Product:**
```json
{
  "name": "Samsung Galaxy S24",
  "description": "Latest Android flagship",
  "sku": "SAM-GAL-S24-256",
  "price": 79999.00,
  "discountPrice": 74999.00,
  "stockQuantity": 15,
  "minStockLevel": 3,
  "brand": "Samsung",
  "weight": "196g",
  "dimensions": "147 x 70.6 x 7.6 mm",
  "vendorId": 1,
  "categoryId": 2
}
```

### Step 6: Verification

#### **Check File Structure:**
After uploads, verify files exist:
```
wwwroot/
├── documents/
│   └── 1_20241106_document.pdf
└── images/
    └── 1/
        ├── 20241106_image1.jpg
        └── 20241106_image2.jpg
```

#### **Check Database:**
- Products table has new entries
- ProductImages table has image paths
- Vendors table shows correct status

---

### Postman Collection
Import the endpoints into Postman for automated testing.

### cURL Examples

**Login:**
```bash
curl -X POST "https://localhost:7000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@ecommerce.com","password":"hashedpassword123"}'
```

**Create Product:**
```bash
curl -X POST "https://localhost:7000/api/product" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"name":"Test Product","sku":"TEST-001","price":999.00,"stockQuantity":10,"vendorId":1,"categoryId":2}'
```