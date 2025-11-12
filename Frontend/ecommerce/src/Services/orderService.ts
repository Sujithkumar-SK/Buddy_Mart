import axios from 'axios';
import type { CheckoutSummary, CreateOrderRequest, OrderDetails, OrderSummary, OrderTracking } from '../types/order';
import { cookieUtils } from '../utils/cookies';

const API_BASE_URL = 'http://localhost:5108/api';

// Use global axios instance with interceptors
axios.defaults.baseURL = API_BASE_URL;

// Add request interceptor to global axios
axios.interceptors.request.use((config) => {
  const token = cookieUtils.getToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const orderService = {
  getCheckoutSummary: async (): Promise<CheckoutSummary> => {
    const response = await axios.get('/order/checkout-summary');
    return response.data;
  },

  createOrder: async (data: CreateOrderRequest): Promise<OrderDetails> => {
    const response = await axios.post('/order', data);
    return response.data;
  },

  getOrders: async (): Promise<OrderSummary[]> => {
    const response = await axios.get('/order');
    return response.data;
  },

  getOrderById: async (id: number): Promise<OrderDetails> => {
    const response = await axios.get(`/order/${id}`);
    return response.data;
  },

  getOrderTracking: async (id: number): Promise<OrderTracking> => {
    const response = await axios.get(`/order/${id}/tracking`);
    return response.data;
  },

  downloadInvoice: async (id: number): Promise<void> => {
    const response = await axios.get(`/order/${id}/invoice`, {
      responseType: 'blob'
    });
    
    const url = window.URL.createObjectURL(new Blob([response.data], { type: 'application/pdf' }));
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', `Invoice_Order_${id}.pdf`);
    document.body.appendChild(link);
    link.click();
    link.remove();
    window.URL.revokeObjectURL(url);
  }
};