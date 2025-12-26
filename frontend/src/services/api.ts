import axios, { type AxiosInstance, type AxiosError } from 'axios';
import type { Response } from '../types';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api';

// Create axios instance
const apiClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor to add JWT token
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor to handle errors globally
apiClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError<Response<any>>) => {
    // Extraer mensaje de error del formato Response<T> del backend
    const errorMessage = error.response?.data?.message || 'Error en la comunicación con el servidor';
    const errorCode = error.response?.data?.code || error.response?.status || 500;
    
    if (errorCode === 401) {
      const currentPath = window.location.pathname;
      
      // Solo redirigir si no estamos en la página de login y hay un token inválido
      if (currentPath !== '/login' && localStorage.getItem('token')) {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        window.location.href = '/login';
      }
    }
    
    // Crear un error mejorado con el mensaje del backend
    const enhancedError = new Error(errorMessage);
    (enhancedError as any).code = errorCode;
    (enhancedError as any).response = error.response;
    
    return Promise.reject(enhancedError);
  }
);

export default apiClient;

