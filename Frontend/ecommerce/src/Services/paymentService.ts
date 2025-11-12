import axios from 'axios';
import { cookieUtils } from '../utils/cookies';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://65.2.30.236:5108/api';

axios.defaults.baseURL = API_BASE_URL;

axios.interceptors.request.use((config) => {
  const token = cookieUtils.getToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export interface PaymentInitiateRequest {
  orderId: number;
  paymentMethod: string;
}

export interface RazorpayOrderResponse {
  razorpayOrderId: string;
  amount: number;
  key: string;
  description: string;
  prefillName: string;
  prefillEmail: string;
  prefillContact: string;
}

export interface PaymentVerifyRequest {
  razorpayOrderId: string;
  razorpayPaymentId: string;
  razorpaySignature: string;
}

export const paymentService = {
  initiatePayment: async (data: PaymentInitiateRequest): Promise<RazorpayOrderResponse> => {
    const response = await axios.post('/payment/initiate', data);
    return response.data;
  },

  verifyPayment: async (data: PaymentVerifyRequest): Promise<any> => {
    const response = await axios.post('/payment/verify', data);
    return response.data;
  }
};