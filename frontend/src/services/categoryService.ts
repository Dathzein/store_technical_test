import apiClient from './api';
import type { Response, CategoryDto, CreateCategoryDto, UpdateCategoryDto } from '../types';

export const categoryService = {
  getAll: async (): Promise<Response<CategoryDto[]>> => {
    const response = await apiClient.get<Response<CategoryDto[]>>('/Category');
    return response.data;
  },

  getById: async (id: number): Promise<Response<CategoryDto>> => {
    const response = await apiClient.get<Response<CategoryDto>>(`/Category/${id}`);
    return response.data;
  },

  create: async (category: CreateCategoryDto): Promise<Response<CategoryDto>> => {
    const response = await apiClient.post<Response<CategoryDto>>('/Category', category);
    return response.data;
  },

  update: async (id: number, category: UpdateCategoryDto): Promise<Response<CategoryDto>> => {
    const response = await apiClient.put<Response<CategoryDto>>(`/Category/${id}`, category);
    return response.data;
  },

  delete: async (id: number): Promise<Response<boolean>> => {
    const response = await apiClient.delete<Response<boolean>>(`/Category/${id}`);
    return response.data;
  },
};

