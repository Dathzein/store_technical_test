import { setupServer } from 'msw/node'
import { handlers } from './handlers'

// Configurar el servidor MSW
// NOTA: El lifecycle (beforeAll, afterEach, afterAll) se maneja en setup.ts
export const server = setupServer(...handlers)
