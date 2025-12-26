import { describe, it, expect, vi } from 'vitest'
import { render, screen } from '@testing-library/react'
import { AuthGuard } from '../AuthGuard'
import { BrowserRouter } from 'react-router-dom'

// Mock de useAuth
const mockUseAuth = vi.fn()
vi.mock('../../context/AuthContext', () => ({
  useAuth: () => mockUseAuth(),
}))

// Mock de Navigate
const mockNavigate = vi.fn()
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom')
  return {
    ...actual,
    Navigate: ({ to }: { to: string }) => {
      mockNavigate(to)
      return <div data-testid="navigate">Redirecting to {to}</div>
    },
  }
})

const TestComponent = () => <div data-testid="protected-content">Contenido Protegido</div>

describe('AuthGuard', () => {
  beforeEach(() => {
    mockUseAuth.mockClear()
    mockNavigate.mockClear()
  })

  it('debería mostrar loading cuando isLoading es true', () => {
    mockUseAuth.mockReturnValue({
      isAuthenticated: false,
      isLoading: true,
    })

    render(
      <BrowserRouter>
        <AuthGuard>
          <TestComponent />
        </AuthGuard>
      </BrowserRouter>
    )

    expect(screen.getByText('Loading...')).toBeInTheDocument()
    expect(screen.queryByTestId('protected-content')).not.toBeInTheDocument()
  })

  it('debería redirigir a /login cuando no está autenticado', () => {
    mockUseAuth.mockReturnValue({
      isAuthenticated: false,
      isLoading: false,
    })

    render(
      <BrowserRouter>
        <AuthGuard>
          <TestComponent />
        </AuthGuard>
      </BrowserRouter>
    )

    expect(mockNavigate).toHaveBeenCalledWith('/login')
    expect(screen.getByTestId('navigate')).toBeInTheDocument()
    expect(screen.queryByTestId('protected-content')).not.toBeInTheDocument()
  })

  it('debería renderizar children cuando está autenticado', () => {
    mockUseAuth.mockReturnValue({
      isAuthenticated: true,
      isLoading: false,
    })

    render(
      <BrowserRouter>
        <AuthGuard>
          <TestComponent />
        </AuthGuard>
      </BrowserRouter>
    )

    expect(screen.getByTestId('protected-content')).toBeInTheDocument()
    expect(screen.queryByText('Loading...')).not.toBeInTheDocument()
    expect(mockNavigate).not.toHaveBeenCalled()
  })

  it('debería priorizar loading sobre autenticación', () => {
    // Incluso si está autenticado, si está loading, muestra loading
    mockUseAuth.mockReturnValue({
      isAuthenticated: true,
      isLoading: true,
    })

    render(
      <BrowserRouter>
        <AuthGuard>
          <TestComponent />
        </AuthGuard>
      </BrowserRouter>
    )

    expect(screen.getByText('Loading...')).toBeInTheDocument()
    expect(screen.queryByTestId('protected-content')).not.toBeInTheDocument()
  })
})

