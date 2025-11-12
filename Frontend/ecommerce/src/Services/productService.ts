import axios from 'axios';
import type { Product, ProductCreateRequest, Category } from '../types/product';
import { cookieUtils } from '../utils/cookies';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5108/api';

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

export const productService = {
  getProductsByVendor: async (vendorId: number): Promise<Product[]> => {
    const response = await axios.get(`/product/vendor/${vendorId}`);
    return response.data;
  },

  createProduct: async (data: ProductCreateRequest): Promise<Product> => {
    const response = await axios.post('/product', data);
    return response.data;
  },

  updateProduct: async (id: number, data: Partial<ProductCreateRequest>): Promise<Product> => {
    const response = await axios.put(`/product/${id}`, data);
    return response.data;
  },

  deleteProduct: async (id: number): Promise<void> => {
    await axios.delete(`/product/${id}`);
  },

  updateProductStatus: async (id: number, status: string): Promise<void> => {
    await axios.patch(`/product/${id}/status`, JSON.stringify(status), {
      headers: { 'Content-Type': 'application/json' }
    });
  },

  uploadProductImages: async (id: number, images: File[]): Promise<string[]> => {
    const formData = new FormData();
    images.forEach(image => formData.append('images', image));
    
    const response = await axios.post(`/product/${id}/images`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
    return response.data.imagePaths;
  },

  getAllProducts: async (): Promise<Product[]> => {
    const response = await axios.get('/product');
    return response.data;
  },

  getProductById: async (id: number): Promise<Product> => {
    const response = await axios.get(`/product/${id}`);
    return response.data;
  },

  getProductsByCategory: async (categoryId: number): Promise<Product[]> => {
    const response = await axios.get(`/product/category/${categoryId}`);
    return response.data;
  },

  getCategories: async (): Promise<Category[]> => {
    const response = await axios.get('/category');
    return response.data;
  }
};
