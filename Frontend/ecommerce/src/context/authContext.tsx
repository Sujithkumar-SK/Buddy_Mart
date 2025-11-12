import React, { createContext, useContext, useEffect, useState, type ReactNode } from "react";
import type { User } from "../types/auth";
import { cookieUtils } from "../utils/cookies";

interface AuthContextType {
  user: User | null;
  isAuthenticated: boolean;
  login:(user: User, accessToken: string, refreshToken:string)=>void;
  logout:()=>void;
  loading:boolean;
}
const AuthContext = createContext<AuthContextType | undefined>(undefined);
interface AuthProviderProps {
  children: ReactNode;
}
export const AuthProvider: React.FC<AuthProviderProps>=({children})=>{
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const isAuthenticated = !!user;
  useEffect(() => {
    // Check if user is already logged in
    const token = cookieUtils.getToken();
    const savedUser = cookieUtils.getUser();
    if (token && savedUser) {
      setUser(savedUser);
      }
      setLoading(false);
    },[]);
  const login = (userData: User, accessToken: string, refreshToken: string) => {
    setUser(userData);
    cookieUtils.setToken(accessToken);
    cookieUtils.setRefreshToken(refreshToken);
    cookieUtils.setUser(userData);
  };

  const logout = () => {
    setUser(null);
    cookieUtils.removeToken();
    cookieUtils.removeRefreshToken();
    cookieUtils.removeUser();
  };

  const value = {
    user,
    isAuthenticated,
    login,
    logout,
    loading
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}