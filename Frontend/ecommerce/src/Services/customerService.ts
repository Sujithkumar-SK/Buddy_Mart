import axios from 'axios';

export interface CustomerProfile {
  customerId: number;
  firstName: string;
  middleName?: string;
  lastName: string;
  fullName: string;
  dateOfBirth?: string;
  gender?: string;
  address?: string;
  city?: string;
  state?: string;
  pinCode?: string;
  email: string;
  phone: string;
  isActive: boolean;
}

export interface CustomerProfileUpdate {
  firstName: string;
  middleName?: string;
  lastName: string;
  dateOfBirth?: string;
  gender?: number;
  address?: string;
  city?: string;
  state?: string;
  pinCode?: string;
}

export const customerService = {
  getMyProfile: async (): Promise<CustomerProfile> => {
    const response = await axios.get('/customer/my-profile');
    return response.data;
  },

  updateMyProfile: async (data: CustomerProfileUpdate): Promise<CustomerProfile> => {
    const response = await axios.put('/customer/my-profile', data);
    return response.data;
  },

  getProfile: async (customerId: number): Promise<CustomerProfile> => {
    const response = await axios.get(`/customer/${customerId}/profile`);
    return response.data;
  },

  updateProfile: async (customerId: number, data: CustomerProfileUpdate): Promise<CustomerProfile> => {
    const response = await axios.put(`/customer/${customerId}/profile`, data);
    return response.data;
  }
};