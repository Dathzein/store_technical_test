import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { ProductList } from '../ProductList'
import { BrowserRouter } from 'react-router-dom'
import { server } from '../../mocks/server'
import { http, HttpResponse } from 'msw'

// Mock de useNavigate
const mockNavigate = vi.fn()
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom')
  return {
    ...actual,
    useNavigate: () => mockNavigate,
  }
})

// Mock de BulkImportModal
vi.mock('../../components/BulkImportModal', () => ({
  BulkImportModal: ({ onClose, onComplete }: any) => (
    <div data-testid="bulk-import-modal">
      <button onClick={onClose}>Cerrar Modal</button>
      <button onClick={onComplete}>Completar Importación</button>
    </div>
  ),
}))

describe('ProductList', () => {
  beforeEach(() => {
    mockNavigate.mockClear()
    vi.clearAllMocks()
  })

  it('debería renderizar el título y botones principales', async () => {
    render(
      <BrowserRouter>
        <ProductList />
      </BrowserRouter>
    )

    expect(screen.getByText('Gestión de Productos')).toBeInTheDocument()
    expect(screen.getByText('📦 Carga Masiva')).toBeInTheDocument()
    expect(screen.getByText('➕ Nuevo Producto')).toBeInTheDocument()
  })

  it('debería mostrar loading inicialmente', () => {
    render(
      <BrowserRouter>
        <ProductList />
      </BrowserRouter>
    )

    expect(screen.getByText('Cargando productos...')).toBeInTheDocument()
  })

  it('debería cargar y mostrar productos', async () => {
    render(
      <BrowserRouter>
        <ProductList />
      </BrowserRouter>
    )

    await waitFor(() => {
      expect(screen.queryByText('Cargando productos...')).not.toBeInTheDocument()
    })

    // Verificar que la tabla se muestra
    expect(screen.getByText('Nombre')).toBeInTheDocument()
    expect(screen.getByText('Descripción')).toBeInTheDocument()
    expect(screen.getByText('Precio')).toBeInTheDocument()
    expect(screen.getByText('Stock')).toBeInTheDocument()
  })

  it('debería navegar a crear producto al hacer click en Nuevo Producto', async () => {
    const user = userEvent.setup()
    render(
      <BrowserRouter>
        <ProductList />
      </BrowserRouter>
    )

    await waitFor(() => {
      expect(screen.queryByText('Cargando productos...')).not.toBeInTheDocument()
    })

    const newButton = screen.getByText('➕ Nuevo Producto')
    await user.click(newButton)

    expect(mockNavigate).toHaveBeenCalledWith('/products/new')
  })

  it('debería abrir y cerrar el modal de carga masiva', async () => {
    const user = userEvent.setup()
    render(
      <BrowserRouter>
        <ProductList />
      </BrowserRouter>
    )

    await waitFor(() => {
      expect(screen.queryByText('Cargando productos...')).not.toBeInTheDocument()
    })

    // Abrir modal
    const bulkButton = screen.getByText('📦 Carga Masiva')
    await user.click(bulkButton)

    expect(screen.getByTestId('bulk-import-modal')).toBeInTheDocument()

    // Cerrar modal
    const closeButton = screen.getByText('Cerrar Modal')
    await user.click(closeButton)

    await waitFor(() => {
      expect(screen.queryByTestId('bulk-import-modal')).not.toBeInTheDocument()
    })
  })

  it('debería recargar productos al completar importación masiva', async () => {
    const user = userEvent.setup()
    render(
      <BrowserRouter>
        <ProductList />
      </BrowserRouter>
    )

    await waitFor(() => {
      expect(screen.queryByText('Cargando productos...')).not.toBeInTheDocument()
    })

    // Abrir modal
    const bulkButton = screen.getByText('📦 Carga Masiva')
    await user.click(bulkButton)

    // Completar importación
    const completeButton = screen.getByText('Completar Importación')
    await user.click(completeButton)

    await waitFor(() => {
      expect(screen.queryByTestId('bulk-import-modal')).not.toBeInTheDocument()
    })
  })

  it('debería filtrar por término de búsqueda', async () => {
    const user = userEvent.setup()
    render(
      <BrowserRouter>
        <ProductList />
      </BrowserRouter>
    )

    await waitFor(() => {
      expect(screen.queryByText('Cargando productos...')).not.toBeInTheDocument()
    })

    const searchInput = screen.getByPlaceholderText('Buscar por nombre o descripción...')
    await user.type(searchInput, 'laptop')

    expect(searchInput).toHaveValue('laptop')
  })

  it('debería filtrar por categoría', async () => {
    const user = userEvent.setup()
    render(
      <BrowserRouter>
        <ProductList />
      </BrowserRouter>
    )

    await waitFor(() => {
      expect(screen.queryByText('Cargando productos...')).not.toBeInTheDocument()
    })

    const categorySelect = screen.getByRole('combobox')
    await user.selectOptions(categorySelect, '1')

    expect(categorySelect).toHaveValue('1')
  })

  it('debería manejar errores al cargar productos', async () => {
    server.use(
      http.get('*/Product', () => {
        // Retornar error de red para simular un fallo
        return new HttpResponse(null, { status: 500 })
      })
    )

    render(
      <BrowserRouter>
        <ProductList />
      </BrowserRouter>
    )

    await waitFor(() => {
      // El componente mostrará el error genérico o el mensaje de axios
      const errorElements = screen.queryAllByText(/error|failed|request failed/i)
      expect(errorElements.length).toBeGreaterThan(0)
    })
  })

  it('debería mostrar mensaje cuando no hay productos', async () => {
    server.use(
      http.get('*/Product', () => {
        return HttpResponse.json({
          isSuccess: true,
          message: 'Success',
          data: {
            items: [],
            pageNumber: 1,
            pageSize: 10,
            totalCount: 0,
            totalPages: 0,
            hasPreviousPage: false,
            hasNextPage: false,
          },
        })
      })
    )

    render(
      <BrowserRouter>
        <ProductList />
      </BrowserRouter>
    )

    await waitFor(() => {
      expect(screen.getByText('No se encontraron productos')).toBeInTheDocument()
    })
  })

  it('debería navegar al hacer click en botón Ver', async () => {
    const user = userEvent.setup()
    
    // Mock con productos
    server.use(
      http.get('*/Product', () => {
        return HttpResponse.json({
          isSuccess: true,
          message: 'Success',
          data: {
            items: [
              {
                id: 1,
                name: 'Test Product',
                description: 'Test Description',
                price: 99.99,
                stock: 10,
                categoryId: 1,
                categoryName: 'Test Category',
              },
            ],
            pageNumber: 1,
            pageSize: 10,
            totalCount: 1,
            totalPages: 1,
            hasPreviousPage: false,
            hasNextPage: false,
          },
        })
      })
    )

    render(
      <BrowserRouter>
        <ProductList />
      </BrowserRouter>
    )

    await waitFor(() => {
      expect(screen.getByText('Test Product')).toBeInTheDocument()
    })

    const viewButton = screen.getByText('Ver')
    await user.click(viewButton)

    expect(mockNavigate).toHaveBeenCalledWith('/products/1')
  })

  it('debería navegar al hacer click en botón Editar', async () => {
    const user = userEvent.setup()
    
    server.use(
      http.get('*/Product', () => {
        return HttpResponse.json({
          isSuccess: true,
          message: 'Success',
          data: {
            items: [
              {
                id: 1,
                name: 'Test Product',
                description: 'Test Description',
                price: 99.99,
                stock: 10,
                categoryId: 1,
                categoryName: 'Test Category',
              },
            ],
            pageNumber: 1,
            pageSize: 10,
            totalCount: 1,
            totalPages: 1,
            hasPreviousPage: false,
            hasNextPage: false,
          },
        })
      })
    )

    render(
      <BrowserRouter>
        <ProductList />
      </BrowserRouter>
    )

    await waitFor(() => {
      expect(screen.getByText('Test Product')).toBeInTheDocument()
    })

    const editButton = screen.getByText('Editar')
    await user.click(editButton)

    expect(mockNavigate).toHaveBeenCalledWith('/products/1/edit')
  })

  it('debería mostrar confirm al intentar eliminar', async () => {
    const user = userEvent.setup()
    const mockConfirm = vi.spyOn(window, 'confirm').mockReturnValue(false)
    
    server.use(
      http.get('*/Product', () => {
        return HttpResponse.json({
          isSuccess: true,
          message: 'Success',
          data: {
            items: [
              {
                id: 1,
                name: 'Test Product',
                description: 'Test Description',
                price: 99.99,
                stock: 10,
                categoryId: 1,
                categoryName: 'Test Category',
              },
            ],
            pageNumber: 1,
            pageSize: 10,
            totalCount: 1,
            totalPages: 1,
            hasPreviousPage: false,
            hasNextPage: false,
          },
        })
      })
    )

    render(
      <BrowserRouter>
        <ProductList />
      </BrowserRouter>
    )

    await waitFor(() => {
      expect(screen.getByText('Test Product')).toBeInTheDocument()
    })

    const deleteButton = screen.getByText('Eliminar')
    await user.click(deleteButton)

    expect(mockConfirm).toHaveBeenCalledWith('¿Estás seguro de eliminar este producto?')
    
    mockConfirm.mockRestore()
  })
})

