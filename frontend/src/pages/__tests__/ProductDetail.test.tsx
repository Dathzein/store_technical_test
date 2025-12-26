import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import { ProductDetail } from '../ProductDetail'
import { BrowserRouter } from 'react-router-dom'

// Mock de useNavigate y useParams
const mockNavigate = vi.fn()
const mockParams = { id: '1' }

vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom')
  return {
    ...actual,
    useNavigate: () => mockNavigate,
    useParams: () => mockParams,
  }
})

describe('ProductDetail', () => {
  beforeEach(() => {
    mockNavigate.mockClear()
  })

  it('debería renderizar el componente en estado de carga', () => {
    render(
      <BrowserRouter>
        <ProductDetail />
      </BrowserRouter>
    )
    
    expect(screen.getByText('Cargando producto...')).toBeInTheDocument()
  })

  it('debería cargar el producto exitosamente', async () => {
    render(
      <BrowserRouter>
        <ProductDetail />
      </BrowserRouter>
    )

    await waitFor(() => {
      expect(screen.queryByText('Cargando producto...')).not.toBeInTheDocument()
    })
  })

  it('debería tener el componente renderizado', () => {
    const { container } = render(
      <BrowserRouter>
        <ProductDetail />
      </BrowserRouter>
    )

    expect(container).toBeTruthy()
  })
})

