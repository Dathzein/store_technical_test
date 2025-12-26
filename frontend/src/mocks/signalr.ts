import * as signalR from '@microsoft/signalr'
import { vi } from 'vitest'

// Mock de SignalR HubConnection
export const createMockHubConnection = () => {
  const mockConnection = {
    start: vi.fn().mockResolvedValue(undefined),
    stop: vi.fn().mockResolvedValue(undefined),
    on: vi.fn(),
    off: vi.fn(),
    invoke: vi.fn().mockResolvedValue(undefined),
    send: vi.fn().mockResolvedValue(undefined),
    state: signalR.HubConnectionState.Connected,
  } as any

  return mockConnection
}

// Mock del HubConnectionBuilder
export const mockHubConnectionBuilder = () => {
  const mockBuilder = {
    withUrl: vi.fn().mockReturnThis(),
    withAutomaticReconnect: vi.fn().mockReturnThis(),
    build: vi.fn(),
  } as any

  return mockBuilder
}

// Helper para simular eventos de SignalR
export const simulateSignalREvent = (
  connection: any,
  eventName: string,
  data: any
) => {
  const handler = connection.on.mock.calls.find(
    (call: any) => call[0] === eventName
  )?.[1]
  
  if (handler) {
    handler(data)
  }
}

