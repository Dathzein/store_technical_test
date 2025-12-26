/**
 * Test Utilities for Frontend Testing
 * 
 * Este archivo contiene utilidades para testing de componentes React.
 * 
 * NOTA: Los errores de TypeScript aquí son normales si no has ejecutado `npm ci`.
 * Ejecuta `cd frontend && npm ci` para instalar las dependencias.
 */

import React from 'react'
import type { ReactElement } from 'react'
import { render, type RenderOptions } from '@testing-library/react'
import { BrowserRouter } from 'react-router-dom'
import { AuthProvider } from '../context/AuthContext'
import { vi } from 'vitest'
import type { UserDto, ProductListDto, CategoryDto, ProductDto } from '../types'

// Wrapper con todos los providers necesarios
interface AllProvidersProps {
  children: React.ReactNode
}

const AllProviders: React.FC<AllProvidersProps> = ({ children }) => {
  return (
    <BrowserRouter>
      <AuthProvider>{children}</AuthProvider>
    </BrowserRouter>
  )
}

// Función customizada de render
const customRender = (
  ui: ReactElement,
  options?: Omit<RenderOptions, 'wrapper'>
) => render(ui, { wrapper: AllProviders, ...options })

// Re-exportar todo de testing library
export * from '@testing-library/react'
export { customRender as render }

// Factory functions para crear datos mock
export const createMockUser = (overrides?: Partial<UserDto>): UserDto => ({
  id: 1,
  username: 'testuser',
  email: 'test@example.com',
  roleName: 'Admin',
  permissions: ['read', 'write', 'delete'],
  ...overrides,
})

export const createMockProduct = (overrides?: Partial<ProductDto>): ProductDto => ({
  id: 1,
  name: 'Test Product',
  description: 'Test Description',
  price: 99.99,
  stock: 10,
  category: {
    id: 1,
    name: 'Test Category',
    description: 'Test Category Description',
    imageUrl: 'https://example.com/image.jpg',
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  },
  createdAt: new Date().toISOString(),
  updatedAt: new Date().toISOString(),
  ...overrides,
})

export const createMockProductList = (
  overrides?: Partial<ProductListDto>
): ProductListDto => ({
  id: 1,
  name: 'Test Product',
  description: 'Test Description',
  price: 99.99,
  stock: 10,
  categoryId: 1,
  categoryName: 'Test Category',
  ...overrides,
})

export const createMockCategory = (overrides?: Partial<CategoryDto>): CategoryDto => ({
  id: 1,
  name: 'Test Category',
  description: 'Test Category Description',
  imageUrl: 'https://example.com/image.jpg',
  createdAt: new Date().toISOString(),
  updatedAt: new Date().toISOString(),
  ...overrides,
})

// Helper para esperar que termine la carga
export const waitForLoadingToFinish = () => {
  return new Promise((resolve) => setTimeout(resolve, 0))
}

// Helper para mockear localStorage
export const mockLocalStorage = () => {
  const localStorageMock = {
    getItem: vi.fn(),
    setItem: vi.fn(),
    removeItem: vi.fn(),
    clear: vi.fn(),
  }
  globalThis.localStorage = localStorageMock as any
  return localStorageMock
}

// Helper para crear un token JWT mock
export const createMockToken = (expiresInMinutes: number = 60): string => {
  const header = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }))
  const exp = Math.floor(Date.now() / 1000) + expiresInMinutes * 60
  const payload = btoa(
    JSON.stringify({
      sub: '1',
      username: 'testuser',
      exp,
    })
  )
  const signature = btoa('mock-signature')
  return `${header}.${payload}.${signature}`
}
