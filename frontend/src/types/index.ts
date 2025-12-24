// Types for API responses
export interface Response<T> {
  data: T;
  code: number;
  message: string;
  isSuccess: boolean;
}

// Auth types
export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
  user: UserDto;
}

export interface UserDto {
  id: number;
  username: string;
  email: string;
  roleName: string;
  permissions: string[];
}

export interface RoleDto {
  id: number;
  name: string;
  permissions: string[];
}

// Product types
export interface ProductDto {
  id: number;
  name: string;
  description: string;
  price: number;
  stock: number;
  category: CategoryDto;
  createdAt: string;
  updatedAt: string;
}

export interface ProductListDto {
  id: number;
  name: string;
  description: string;
  price: number;
  stock: number;
  categoryId: number;
  categoryName: string;
}

export interface CreateProductDto {
  name: string;
  description: string;
  price: number;
  stock: number;
  categoryId: number;
}

export interface UpdateProductDto {
  name: string;
  description: string;
  price: number;
  stock: number;
  categoryId: number;
}

export interface ProductQueryDto {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  categoryId?: number;
  minPrice?: number;
  maxPrice?: number;
  minStock?: number;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}

export interface PagedResultDto<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

// Category types
export interface CategoryDto {
  id: number;
  name: string;
  description: string;
  imageUrl: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateCategoryDto {
  name: string;
  description: string;
  imageUrl: string;
}

export interface UpdateCategoryDto {
  name: string;
  description: string;
  imageUrl: string;
}

// Bulk Import types
export interface BulkImportJobDto {
  id: string;
  status: 'Pending' | 'Processing' | 'Completed' | 'Failed';
  totalRecords: number;
  processedRecords: number;
  startedAt?: string;
  completedAt?: string;
  errorMessage?: string;
  createdAt: string;
}

export interface BulkImportStatusDto {
  jobId: string;
  status: string;
  processedRecords: number;
  totalRecords: number;
  percentage: number;
  message: string;
}

