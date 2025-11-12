import axios from 'axios';
import type { ShippingDetails, UpdateTrackingDetails, UpdateShippingStatus } from '../types/shipping';

export const shippingService = {
  getShippingsByVendor: async (vendorId: number): Promise<ShippingDetails[]> => {
    const response = await axios.get(`/shipping/vendor/${vendorId}`);
    return response.data;
  },

  createShipping: async (orderId: number): Promise<ShippingDetails> => {
    const response = await axios.post(`/shipping/order/${orderId}`);
    return response.data;
  },

  updateTrackingDetails: async (data: UpdateTrackingDetails): Promise<void> => {
    await axios.put('/shipping/tracking', data);
  },

  updateShippingStatus: async (shippingId: number, data: UpdateShippingStatus): Promise<void> => {
    await axios.patch(`/shipping/${shippingId}/status`, data);
  },

  markAsShipped: async (shippingId: number): Promise<void> => {
    await axios.patch(`/shipping/${shippingId}/ship`);
  },

  markAsDelivered: async (shippingId: number): Promise<void> => {
    await axios.patch(`/shipping/${shippingId}/deliver`);
  },

  getShippingByOrderId: async (orderId: number): Promise<ShippingDetails> => {
    const response = await axios.get(`/shipping/order/${orderId}`);
    return response.data;
  },

  trackShipment: async (trackingNumber: string): Promise<ShippingDetails> => {
    const response = await axios.get(`/shipping/track/${trackingNumber}`);
    return response.data;
  }
};