import apiClient from './api';
import type {
  Response,
  ProductDto,
  ProductListDto,
  CreateProductDto,
  UpdateProductDto,
  ProductQueryDto,
  PagedResultDto,
} from '../types';

export const productService = {
  getAll: async (query: ProductQueryDto): Promise<Response<PagedResultDto<ProductListDto>>> => {
    const response = await apiClient.get<Response<PagedResultDto<ProductListDto>>>('/Product', {
      params: query,
    });
    return response.data;
  },

  getById: async (id: number): Promise<Response<ProductDto>> => {
    const response = await apiClient.get<Response<ProductDto>>(`/Product/${id}`);
    return response.data;
  },

  create: async (product: CreateProductDto): Promise<Response<ProductDto>> => {
    const response = await apiClient.post<Response<ProductDto>>('/Product', product);
    return response.data;
  },

  update: async (id: number, product: UpdateProductDto): Promise<Response<ProductDto>> => {
    const response = await apiClient.put<Response<ProductDto>>(`/Product/${id}`, product);
    return response.data;
  },

  delete: async (id: number): Promise<Response<boolean>> => {
    const response = await apiClient.delete<Response<boolean>>(`/Product/${id}`);
    return response.data;
  },
};

