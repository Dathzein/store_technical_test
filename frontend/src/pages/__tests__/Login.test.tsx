import { describe, it, expect, vi, beforeEach } from 'vitest'
import { screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { Login } from '../Login'
import { render } from '../../test/utils'

// Mock de useNavigate
const mockNavigate = vi.fn()
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom')
  return {
    ...actual,
    useNavigate: () => mockNavigate,
  }
})

describe('Login', () => {
  beforeEach(() => {
    mockNavigate.mockClear()
    localStorage.clear()
  })

  it('debería renderizar el formulario de login', () => {
    render(<Login />)

    expect(screen.getByText('ServerCloudStore')).toBeInTheDocument()
    expect(screen.getByRole('heading', { name: 'Iniciar Sesión' })).toBeInTheDocument()
    expect(screen.getByLabelText('Usuario')).toBeInTheDocument()
    expect(screen.getByLabelText('Contraseña')).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /iniciar sesión/i })).toBeInTheDocument()
  })

  it('debería actualizar los campos al escribir', async () => {
    const user = userEvent.setup()
    render(<Login />)

    const usernameInput = screen.getByLabelText('Usuario')
    const passwordInput = screen.getByLabelText('Contraseña')

    await user.type(usernameInput, 'testuser')
    await user.type(passwordInput, 'password123')

    expect(usernameInput).toHaveValue('testuser')
    expect(passwordInput).toHaveValue('password123')
  })

  it('debería mostrar errores de validación cuando los campos están vacíos', async () => {
    const user = userEvent.setup()
    render(<Login />)

    const submitButton = screen.getByRole('button', { name: /iniciar sesión/i })
    
    // Intentar enviar formulario vacío
    await user.click(submitButton)

    await waitFor(() => {
      expect(screen.getByText('El usuario es requerido')).toBeInTheDocument()
      expect(screen.getByText('La contraseña es requerida')).toBeInTheDocument()
    })
  })

  it('debería mostrar error de validación cuando el usuario es muy corto', async () => {
    const user = userEvent.setup()
    render(<Login />)

    const usernameInput = screen.getByLabelText('Usuario')
    
    await user.type(usernameInput, 'ab')
    await user.tab() // Trigger onBlur

    await waitFor(() => {
      expect(screen.getByText('El usuario debe tener al menos 3 caracteres')).toBeInTheDocument()
    })
  })

  it('debería mostrar error de validación cuando la contraseña es muy corta', async () => {
    const user = userEvent.setup()
    render(<Login />)

    const passwordInput = screen.getByLabelText('Contraseña')
    
    await user.type(passwordInput, '12345')
    await user.tab() // Trigger onBlur

    await waitFor(() => {
      expect(screen.getByText('La contraseña debe tener al menos 6 caracteres')).toBeInTheDocument()
    })
  })

  it('debería navegar a /products después de login exitoso', async () => {
    const user = userEvent.setup()
    render(<Login />)

    const usernameInput = screen.getByLabelText('Usuario')
    const passwordInput = screen.getByLabelText('Contraseña')
    const submitButton = screen.getByRole('button', { name: /iniciar sesión/i })

    await user.type(usernameInput, 'testuser')
    await user.type(passwordInput, 'password')
    await user.click(submitButton)

    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith('/products')
    })
  })

  it('debería mostrar error del API cuando las credenciales son incorrectas', async () => {
    const user = userEvent.setup()
    render(<Login />)

    const usernameInput = screen.getByLabelText('Usuario')
    const passwordInput = screen.getByLabelText('Contraseña')
    const submitButton = screen.getByRole('button', { name: /iniciar sesión/i })

    await user.type(usernameInput, 'wronguser')
    await user.type(passwordInput, 'wrongpass')
    await user.click(submitButton)

    await waitFor(() => {
      // El mensaje exacto del API es "Credenciales inválidas"
      expect(screen.getByText(/credenciales inválidas|invalid credentials|request failed/i)).toBeInTheDocument()
    })
  })

  it('debería tener el botón habilitado cuando los campos son válidos', async () => {
    const user = userEvent.setup()
    render(<Login />)

    const usernameInput = screen.getByLabelText('Usuario')
    const passwordInput = screen.getByLabelText('Contraseña')
    const submitButton = screen.getByRole('button', { name: /iniciar sesión/i })

    await user.type(usernameInput, 'testuser')
    await user.type(passwordInput, 'password')
    
    expect(submitButton).not.toBeDisabled()
  })

  it('debería mostrar información de usuario de prueba', () => {
    render(<Login />)

    expect(screen.getByText(/usuario de prueba/i)).toBeInTheDocument()
    expect(screen.getByText('Usuario: admin')).toBeInTheDocument()
    expect(screen.getByText('Contraseña: admin123')).toBeInTheDocument()
  })

  it('debería aplicar estilos de error a los campos con validación fallida', async () => {
    const user = userEvent.setup()
    render(<Login />)

    const usernameInput = screen.getByLabelText('Usuario')
    const submitButton = screen.getByRole('button', { name: /iniciar sesión/i })

    // Intentar enviar con campo vacío
    await user.click(submitButton)

    await waitFor(() => {
      // El campo debería tener la clase de error (borde rojo)
      expect(usernameInput).toHaveClass('border-red-500')
    })
  })
})

