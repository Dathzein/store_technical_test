import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { Layout } from '../Layout'
import { BrowserRouter } from 'react-router-dom'

// Mock de useAuth
const mockLogout = vi.fn()
const mockUseAuth = vi.fn()
vi.mock('../../context/AuthContext', () => ({
  useAuth: () => mockUseAuth(),
}))

// Mock de useNavigate
const mockNavigate = vi.fn()
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom')
  return {
    ...actual,
    useNavigate: () => mockNavigate,
    Outlet: () => <div data-testid="outlet">Page Content</div>,
  }
})

describe('Layout', () => {
  beforeEach(() => {
    mockUseAuth.mockReturnValue({
      user: {
        id: 1,
        username: 'testuser',
        email: 'test@example.com',
        roleName: 'Admin',
        permissions: ['read', 'write'],
      },
      logout: mockLogout,
    })
    mockLogout.mockClear()
    mockNavigate.mockClear()
  })

  it('debería renderizar el layout correctamente', () => {
    render(
      <BrowserRouter>
        <Layout />
      </BrowserRouter>
    )

    expect(screen.getByText('ServerCloudStore')).toBeInTheDocument()
    expect(screen.getByText(/testuser/)).toBeInTheDocument()
    expect(screen.getByText(/Admin/)).toBeInTheDocument()
    expect(screen.getByText('📦 Productos')).toBeInTheDocument()
    expect(screen.getByText('Cerrar Sesión')).toBeInTheDocument()
    expect(screen.getByTestId('outlet')).toBeInTheDocument()
  })

  it('debería renderizar el footer correctamente', () => {
    render(
      <BrowserRouter>
        <Layout />
      </BrowserRouter>
    )

    expect(screen.getByText(/2024 ServerCloudStore/)).toBeInTheDocument()
    expect(screen.getByText(/Gestión de Productos/)).toBeInTheDocument()
  })

  it('debería navegar a /products al hacer click en el título', async () => {
    const user = userEvent.setup()
    render(
      <BrowserRouter>
        <Layout />
      </BrowserRouter>
    )

    const title = screen.getByText('ServerCloudStore')
    await user.click(title)

    expect(mockNavigate).toHaveBeenCalledWith('/products')
  })

  it('debería navegar a /products al hacer click en botón Productos', async () => {
    const user = userEvent.setup()
    render(
      <BrowserRouter>
        <Layout />
      </BrowserRouter>
    )

    const productButton = screen.getByText('📦 Productos')
    await user.click(productButton)

    expect(mockNavigate).toHaveBeenCalledWith('/products')
  })

  it('debería llamar logout y navegar al hacer click en Cerrar Sesión', async () => {
    const user = userEvent.setup()
    render(
      <BrowserRouter>
        <Layout />
      </BrowserRouter>
    )

    const logoutButton = screen.getByText('Cerrar Sesión')
    await user.click(logoutButton)

    expect(mockLogout).toHaveBeenCalledTimes(1)
    expect(mockNavigate).toHaveBeenCalledWith('/login')
  })

  it('debería mostrar la información del usuario correctamente', () => {
    render(
      <BrowserRouter>
        <Layout />
      </BrowserRouter>
    )

    const userInfo = screen.getByText(/👤 testuser \(Admin\)/)
    expect(userInfo).toBeInTheDocument()
  })

  it('debería renderizar el Outlet para el contenido de las páginas', () => {
    render(
      <BrowserRouter>
        <Layout />
      </BrowserRouter>
    )

    expect(screen.getByTestId('outlet')).toBeInTheDocument()
    expect(screen.getByText('Page Content')).toBeInTheDocument()
  })
})

