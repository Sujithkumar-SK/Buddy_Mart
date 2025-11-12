import axios from 'axios';
import { authService } from '../Services/authService';
import { cookieUtils } from './cookies';

// Response interceptor to handle token refresh
axios.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const refreshToken = cookieUtils.getRefreshToken();
        
        if (refreshToken) {
          const response = await authService.refreshToken(refreshToken);
          
          // Update cookies with new tokens
          cookieUtils.setToken(response.accessToken);
          cookieUtils.setRefreshToken(response.refreshToken);
          
          // Retry original request with new token
          originalRequest.headers.Authorization = `Bearer ${response.accessToken}`;
          return axios(originalRequest);
        }
      } catch (refreshError) {
        // Refresh failed, redirect to login
        cookieUtils.removeToken();
        cookieUtils.removeRefreshToken();
        cookieUtils.removeUser();
        window.location.href = '/login';
      }
    }

    return Promise.reject(error);
  }
);
