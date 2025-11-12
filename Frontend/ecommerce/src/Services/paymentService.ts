import axios from '../utils/api';

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