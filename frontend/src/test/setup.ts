import '@testing-library/jest-dom'
import { cleanup } from '@testing-library/react'
import { afterEach, beforeAll, afterAll, vi } from 'vitest'
import { server } from '../mocks/server'

// Iniciar MSW server
beforeAll(() => server.listen({ onUnhandledRequest: 'warn' }))

// Cleanup después de cada test
afterEach(async () => {
  cleanup()
  localStorage.clear()
  sessionStorage.clear()
  vi.clearAllMocks()
  server.resetHandlers()
  
  // Esperar a que se resuelvan las promesas pendientes
  await new Promise(resolve => setTimeout(resolve, 0))
})

// Cerrar MSW server
afterAll(() => server.close())

// Mock de window.matchMedia
Object.defineProperty(window, 'matchMedia', {
  writable: true,
  value: vi.fn().mockImplementation(query => ({
    matches: false,
    media: query,
    onchange: null,
    addListener: vi.fn(),
    removeListener: vi.fn(),
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    dispatchEvent: vi.fn(),
  })),
})

// Mock de IntersectionObserver
global.IntersectionObserver = class IntersectionObserver {
  constructor() {}
  disconnect() {}
  observe() {}
  takeRecords() {
    return []
  }
  unobserve() {}
} as any

// Mock de window.alert
global.alert = vi.fn()

// Mock de window.confirm
global.confirm = vi.fn(() => true)

// Mock de console.error para tests más limpios (opcional)
const originalError = console.error
const setupConsoleError = () => {
  console.error = (...args: any[]) => {
    if (
      typeof args[0] === 'string' &&
      (args[0].includes('Warning: ReactDOM.render') ||
       args[0].includes('Not implemented: HTMLFormElement.prototype.submit'))
    ) {
      return
    }
    originalError.call(console, ...args)
  }
}

setupConsoleError()

