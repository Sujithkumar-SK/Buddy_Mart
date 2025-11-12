import axios from 'axios';
import { cookieUtils } from './cookies';

// Centralized API configuration
export const API_BASE_URL = 'http://65.2.30.236:5108/api';

// Configure axios defaults once
axios.defaults.baseURL = API_BASE_URL;

// Add request interceptor
axios.interceptors.request.use((config) => {
  const token = cookieUtils.getToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default axios;