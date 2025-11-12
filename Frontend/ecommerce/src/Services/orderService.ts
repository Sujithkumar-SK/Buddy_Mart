import axios from '../utils/api';
import type { CheckoutSummary, CreateOrderRequest, OrderDetails, OrderSummary, OrderTracking } from '../types/order';

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