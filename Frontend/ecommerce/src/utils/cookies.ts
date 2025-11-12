import Cookies from 'js-cookie';
import type { User } from '../types/auth';
export const cookieUtils = {
  setToken: (token: string) => {
    Cookies.set('accessToken', token, {
      secure: false,
      sameSite: 'strict',
      expires: 1
    });
  },
  getToken: (): string | undefined => {
    return Cookies.get('accessToken');
  },
  removeToken: () => {
    Cookies.remove('accessToken');
  },
  setRefreshToken: (token: string) => {
    Cookies.set('refreshToken', token, {
      secure: false,
      sameSite: 'strict',
      expires: 30
    });
  },
  getRefreshToken: (): string | undefined => {
    return Cookies.get('refreshToken');
  },
  removeRefreshToken: () => {
    Cookies.remove('refreshToken');
  },
  setUser: (user: User) => {
    Cookies.set('user', JSON.stringify(user), {
      secure: false,
      sameSite: 'strict',
      expires: 1
    });
  },
  getUser: (): User | null => {
    const userData = Cookies.get('user');
    if (userData) {
      try {
        return JSON.parse(userData);
      } catch (error) {
        console.error('Error parsing user data from cookie:', error);
        return null;
      }
    }
    return null;
  },
  removeUser: () => {
    Cookies.remove('user');
  }
};