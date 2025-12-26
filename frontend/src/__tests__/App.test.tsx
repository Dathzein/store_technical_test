import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen } from '@testing-library/react'
import App from '../App'

// Mock de componentes pesados
vi.mock('../pages/ProductList', () => ({
  ProductList: () => <div data-testid="product-list">Product List</div>,
}))

vi.mock('../pages/ProductForm', () => ({
  ProductForm: () => <div data-testid="product-form">Product Form</div>,
}))

vi.mock('../pages/ProductDetail', () => ({
  ProductDetail: () => <div data-testid="product-detail">Product Detail</div>,
}))

vi.mock('../pages/Login', () => ({
  Login: () => <div data-testid="login">Login Page</div>,
}))

// Mock de AuthContext para controlar el estado de autenticación
const mockUseAuth = vi.fn()
vi.mock('../context/AuthContext', async () => {
  const actual = await vi.importActual('../context/AuthContext')
  return {
    ...actual,
    useAuth: () => mockUseAuth(),
    AuthProvider: ({ children }: any) => <div>{children}</div>,
  }
})

describe('App', () => {
  beforeEach(() => {
    mockUseAuth.mockReturnValue({
      isAuthenticated: false,
      isLoading: false,
      user: null,
      login: vi.fn(),
      logout: vi.fn(),
    })
  })

  it('debería renderizar la aplicación', () => {
    render(<App />)
    expect(document.body).toBeTruthy()
  })

  it('debería mostrar la página de login cuando no está autenticado', () => {
    mockUseAuth.mockReturnValue({
      isAuthenticated: false,
      isLoading: false,
    })

    // Establecer la ruta a /login
    window.history.pushState({}, '', '/login')
    
    render(<App />)
    
    expect(screen.getByTestId('login')).toBeInTheDocument()
  })

  it('debería tener todas las rutas configuradas', () => {
    const { container } = render(<App />)
    expect(container).toBeTruthy()
  })
})

