export interface User {
  userId: number;
  email: string;
  role: string;
  vendorId?: number;
  customerId?: number;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  phone: string;
  role: number;
  firstName: string;
  middleName?: string;
  lastName: string;
}

export interface AuthResponse {
  userId: number;
  email: string;
  role: string;
  accessToken: string;
  refreshToken: string;
}

export interface OtpVerificationResponse {
  userId: number;
  email: string;
  role: string;
  customerId?: number;
  vendorId?: number;
  message: string;
  requiresApproval?: boolean;
  RequiresApproval?: boolean;
  requiresVendorProfile?: boolean;
  RequiresVendorProfile?: boolean;
}
