import { http, HttpResponse } from 'msw'
import type {
  Response,
  LoginResponse,
  ProductDto,
  ProductListDto,
  CategoryDto,
  PagedResultDto,
  BulkImportJobDto,
  BulkImportStatusDto,
} from '../types'

const API_URL = 'http://localhost:5000/api'

// Mock data
const mockUser = {
  id: 1,
  username: 'testuser',
  email: 'test@example.com',
  roleName: 'Admin',
  permissions: ['read', 'write', 'delete'],
}

const mockCategories: CategoryDto[] = [
  {
    id: 1,
    name: 'Servidores',
    description: 'Servidores de alto rendimiento',
    imageUrl: 'https://example.com/servers.jpg',
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  },
  {
    id: 2,
    name: 'Cloud Storage',
    description: 'Soluciones de almacenamiento en la nube',
    imageUrl: 'https://example.com/cloud.jpg',
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  },
]

const mockProducts: ProductListDto[] = [
  {
    id: 1,
    name: 'Dell PowerEdge R740',
    description: 'Servidor rack de 2U',
    price: 4999.99,
    stock: 15,
    categoryId: 1,
    categoryName: 'Servidores',
  },
  {
    id: 2,
    name: 'HP ProLiant DL380',
    description: 'Servidor empresarial',
    price: 5499.99,
    stock: 10,
    categoryId: 1,
    categoryName: 'Servidores',
  },
]

export const handlers = [
  // Auth endpoints
  http.post(`${API_URL}/Auth/login`, async ({ request }) => {
    const body = (await request.json()) as { username: string; password: string }

    if (body.username === 'testuser' && body.password === 'password') {
      const token = btoa(
        JSON.stringify({
          sub: '1',
          username: 'testuser',
          exp: Math.floor(Date.now() / 1000) + 3600,
        })
      )

      const response: Response<LoginResponse> = {
        data: {
          token: `header.${token}.signature`,
          expiresAt: new Date(Date.now() + 3600000).toISOString(),
          user: mockUser,
        },
        code: 200,
        message: 'Login successful',
        isSuccess: true,
      }

      return HttpResponse.json(response)
    }

    return HttpResponse.json(
      {
        data: null,
        code: 401,
        message: 'Invalid credentials',
        isSuccess: false,
      },
      { status: 401 }
    )
  }),

  // Product endpoints
  http.get(`${API_URL}/Product`, ({ request }) => {
    const url = new URL(request.url)
    const pageNumber = parseInt(url.searchParams.get('pageNumber') || '1')
    const pageSize = parseInt(url.searchParams.get('pageSize') || '10')
    const searchTerm = url.searchParams.get('searchTerm') || ''

    let filteredProducts = [...mockProducts]

    if (searchTerm) {
      filteredProducts = filteredProducts.filter(
        (p) =>
          p.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
          p.description.toLowerCase().includes(searchTerm.toLowerCase())
      )
    }

    const totalCount = filteredProducts.length
    const totalPages = Math.ceil(totalCount / pageSize)
    const startIndex = (pageNumber - 1) * pageSize
    const endIndex = startIndex + pageSize
    const items = filteredProducts.slice(startIndex, endIndex)

    const response: Response<PagedResultDto<ProductListDto>> = {
      data: {
        items,
        totalCount,
        pageNumber,
        pageSize,
        totalPages,
      },
      code: 200,
      message: 'Success',
      isSuccess: true,
    }

    return HttpResponse.json(response)
  }),

  http.get(`${API_URL}/Product/:id`, ({ params }) => {
    const _id = parseInt(params.id as string)
    const product = mockProducts.find((p) => p.id === _id)

    if (product) {
      const detailedProduct: ProductDto = {
        ...product,
        category: mockCategories.find((c) => c.id === product.categoryId)!,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      }

      const response: Response<ProductDto> = {
        data: detailedProduct,
        code: 200,
        message: 'Success',
        isSuccess: true,
      }

      return HttpResponse.json(response)
    }

    return HttpResponse.json(
      {
        data: null,
        code: 404,
        message: 'Product not found',
        isSuccess: false,
      },
      { status: 404 }
    )
  }),

  http.post(`${API_URL}/Product`, async ({ request }) => {
    const body = (await request.json()) as any

    const newProduct: ProductDto = {
      id: mockProducts.length + 1,
      ...body,
      category: mockCategories.find((c) => c.id === body.categoryId)!,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    }

    const response: Response<ProductDto> = {
      data: newProduct,
      code: 201,
      message: 'Product created successfully',
      isSuccess: true,
    }

    return HttpResponse.json(response)
  }),

  http.put(`${API_URL}/Product/:id`, async ({ params, request }) => {
    const _id = parseInt(params.id as string)
    const body = (await request.json()) as any

    const updatedProduct: ProductDto = {
      _id,
      ...body,
      category: mockCategories.find((c) => c.id === body.categoryId)!,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    }

    const response: Response<ProductDto> = {
      data: updatedProduct,
      code: 200,
      message: 'Product updated successfully',
      isSuccess: true,
    }

    return HttpResponse.json(response)
  }),

  http.delete(`${API_URL}/Product/:id`, () => {
    const response: Response<void> = {
      data: undefined as any,
      code: 200,
      message: 'Product deleted successfully',
      isSuccess: true,
    }

    return HttpResponse.json(response)
  }),

  // Category endpoints
  http.get(`${API_URL}/Category`, () => {
    const response: Response<CategoryDto[]> = {
      data: mockCategories,
      code: 200,
      message: 'Success',
      isSuccess: true,
    }

    return HttpResponse.json(response)
  }),

  http.get(`${API_URL}/Category/:id`, ({ params }) => {
    const _id = parseInt(params.id as string)
    const category = mockCategories.find((c) => c.id === _id)

    if (category) {
      const response: Response<CategoryDto> = {
        data: category,
        code: 200,
        message: 'Success',
        isSuccess: true,
      }

      return HttpResponse.json(response)
    }

    return HttpResponse.json(
      {
        data: null,
        code: 404,
        message: 'Category not found',
        isSuccess: false,
      },
      { status: 404 }
    )
  }),

  http.post(`${API_URL}/Category`, async ({ request }) => {
    const body = (await request.json()) as any

    const newCategory: CategoryDto = {
      id: mockCategories.length + 1,
      ...body,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    }

    const response: Response<CategoryDto> = {
      data: newCategory,
      code: 201,
      message: 'Category created successfully',
      isSuccess: true,
    }

    return HttpResponse.json(response)
  }),

  http.put(`${API_URL}/Category/:id`, async ({ params, request }) => {
    const id = parseInt(params.id as string)
    const body = (await request.json()) as any

    const updatedCategory: CategoryDto = {
      id,
      ...body,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    }

    const response: Response<CategoryDto> = {
      data: updatedCategory,
      code: 200,
      message: 'Category updated successfully',
      isSuccess: true,
    }

    return HttpResponse.json(response)
  }),

  http.delete(`${API_URL}/Category/:id`, () => {
    const response: Response<boolean> = {
      data: true,
      code: 200,
      message: 'Category deleted successfully',
      isSuccess: true,
    }

    return HttpResponse.json(response)
  }),

  // Bulk Import endpoints
  http.post(`${API_URL}/BulkImport`, async () => {
    const jobId = crypto.randomUUID()

    const response: Response<BulkImportJobDto> = {
      data: {
        id: jobId,
        status: 'Pending',
        totalRecords: 100,
        processedRecords: 0,
        createdAt: new Date().toISOString(),
      },
      code: 200,
      message: 'Import job started',
      isSuccess: true,
    }

    return HttpResponse.json(response)
  }),

  http.get(`${API_URL}/BulkImport/:id/status`, ({ params }) => {
    const jobId = params.id as string

    const response: Response<BulkImportStatusDto> = {
      data: {
        jobId,
        status: 'Completed',
        processedRecords: 100,
        totalRecords: 100,
        percentage: 100,
        message: 'Import completed successfully',
      },
      code: 200,
      message: 'Success',
      isSuccess: true,
    }

    return HttpResponse.json(response)
  }),
]

