import axios from '../utils/api';
import type { CartSummary, CartItem, AddToCartRequest, UpdateCartItemRequest } from '../types/cart';

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