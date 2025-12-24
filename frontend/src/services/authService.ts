import apiClient from './api';
import type { Response, LoginRequest, LoginResponse } from '../types';

export const authService = {
  login: async (credentials: LoginRequest): Promise<Response<LoginResponse>> => {
    const response = await apiClient.post<Response<LoginResponse>>('/Auth/login', credentials);
    return response.data;
  },

  logout: (): void => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  },

  getToken: (): string | null => {
    return localStorage.getItem('token');
  },

  isAuthenticated: (): boolean => {
    const token = localStorage.getItem('token');
    if (!token) return false;

    // Check if token is expired (simple check)
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const exp = payload.exp * 1000; // Convert to milliseconds
      return Date.now() < exp;
    } catch {
      return false;
    }
  },

  getUser: () => {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  },

  saveUser: (user: any) => {
    localStorage.setItem('user', JSON.stringify(user));
  },

  saveToken: (token: string) => {
    localStorage.setItem('token', token);
  },
};

