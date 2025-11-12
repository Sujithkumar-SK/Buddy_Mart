import axios from 'axios';
import type { VendorProfile, VendorProfileUpdate, VendorDashboard, VendorAnalytics } from '../types/vendor';
import type { VendorOrder } from '../types/shipping';

export const vendorService = {
  getMyProfile: async (): Promise<VendorProfile> => {
    const response = await axios.get('/vendor/my-profile');
    return response.data;
  },

  updateMyProfile: async (data: VendorProfileUpdate): Promise<VendorProfile> => {
    const response = await axios.put('/vendor/my-profile', data);
    return response.data;
  },

  uploadDocument: async (vendorId: number, file: File): Promise<{ message: string; path: string }> => {
    const formData = new FormData();
    formData.append('document', file);
    const response = await axios.post(`/vendor/${vendorId}/upload-document`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
    return response.data;
  },

  getVendorOrders: async (vendorId: number): Promise<VendorOrder[]> => {
    const response = await axios.get(`/vendor/${vendorId}/orders`);
    return response.data;
  },

  acceptOrder: async (orderId: number): Promise<void> => {
    await axios.patch(`/order/${orderId}/accept`);
  },

  shipOrder: async (orderId: number): Promise<void> => {
    await axios.patch(`/order/${orderId}/ship`);
  },

  updateOrderStatus: async (orderId: number, status: number): Promise<void> => {
    await axios.patch(`/order/${orderId}/status`, status);
  },

  getVendorAnalytics: async (vendorId: number): Promise<VendorAnalytics> => {
    const response = await axios.get(`/vendor/${vendorId}/analytics`);
    return response.data;
  },

  getVendorDashboard: async (vendorId: number): Promise<VendorDashboard> => {
    const response = await axios.get(`/vendor/${vendorId}/dashboard`);
    return response.data;
  },

  getMyDashboard: async (): Promise<VendorDashboard> => {
    const response = await axios.get('/vendor/my-dashboard');
    return response.data;
  }
};