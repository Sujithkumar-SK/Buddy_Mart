import axios from 'axios';
import type { CartSummary, CartItem, AddToCartRequest, UpdateCartItemRequest } from '../types/cart';
import { cookieUtils } from '../utils/cookies';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://65.2.30.236:5108/api';

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

export const cartService = {
  getCart: async (): Promise<CartSummary> => {
    const response = await axios.get('/cart');
    return response.data;
  },

  addToCart: async (data: AddToCartRequest): Promise<CartItem> => {
    const response = await axios.post('/cart/add', data);
    return response.data.cartItem;
  },

  updateCartItem: async (cartId: number, data: UpdateCartItemRequest): Promise<CartItem> => {
    const response = await axios.put(`/cart/${cartId}`, data);
    return response.data.cartItem;
  },

  removeCartItem: async (cartId: number): Promise<void> => {
    await axios.delete(`/cart/${cartId}`);
  },

  clearCart: async (): Promise<void> => {
    await axios.delete('/cart/clear');
  }
};