import axios from 'axios';
import type { AuthResponse, LoginRequest, OtpVerificationResponse, RegisterRequest } from '../types/auth';
import { cookieUtils } from '../utils/cookies';

// Use global axios instance (already configured in productService)
export const authService = {
  login: async (data: LoginRequest): Promise<AuthResponse> =>{
    const response = await axios.post('/auth/login',data);
    return response.data;
  },
  register: async (Data: RegisterRequest)=>{
    const response = await axios.post('/auth/register-with-otp', Data);
    return response.data;
  },
  verifyOtp: async (email: string, otp:string, otpToken: string, role:number): Promise<OtpVerificationResponse>=>{
    const response = await axios.post('/auth/verify-registration-otp',{email,otp,otpToken,role});
    return response.data;
  },
  resendOtp: async (email: string)=>{
    const response = await axios.post('/auth/resend-registration-otp', {email});
    return response.data;
  },
  forgotPassword: async (email: string) => {
    const response = await axios.post('/auth/forgot-password', { email });
    return response.data;
  },
  verifyForgotPasswordOtp: async (email: string, otp: string, otpToken: string) => {
    const response = await axios.post('/auth/verify-forgot-password-otp', {
      email,
      otp,
      otpToken
    });
    return response.data;
  },
  resetPassword: async (email: string, newPassword: string,confirmPassword: string) => {
    const response = await axios.post('/auth/reset-password', {
      email,
      newPassword,
      confirmPassword
    });
    return response.data;
  },
  refreshToken: async (refreshToken: string)=>{
    const response = await axios.post('/auth/refresh',{refreshToken});
    return response.data;
  }
};