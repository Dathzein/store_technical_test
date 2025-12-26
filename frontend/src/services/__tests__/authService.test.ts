import { describe, it, expect, beforeEach } from 'vitest'
import { authService } from '../authService'

describe('authService', () => {
  beforeEach(() => {
    localStorage.clear()
  })

  describe('login', () => {
    it('debería hacer login exitoso y retornar datos del usuario', async () => {
      const credentials = { username: 'testuser', password: 'password' }
      const response = await authService.login(credentials)

      expect(response.isSuccess).toBe(true)
      expect(response.data.user.username).toBe('testuser')
      expect(response.data.token).toBeDefined()
    })

    it('debería fallar con credenciales inválidas', async () => {
      const credentials = { username: 'wronguser', password: 'wrongpass' }
      
      try {
        await authService.login(credentials)
        expect.fail('Debería haber lanzado un error')
      } catch (error: any) {
        expect(error).toBeDefined()
        expect(error.response?.status).toBe(401)
      }
    })
  })

  describe('logout', () => {
    it('debería limpiar token y usuario del localStorage', () => {
      localStorage.setItem('token', 'mock-token')
      localStorage.setItem('user', JSON.stringify({ id: 1, username: 'test' }))

      authService.logout()

      expect(localStorage.getItem('token')).toBeNull()
      expect(localStorage.getItem('user')).toBeNull()
    })
  })

  describe('getToken', () => {
    it('debería retornar el token del localStorage', () => {
      const mockToken = 'mock-token-123'
      localStorage.setItem('token', mockToken)

      const token = authService.getToken()

      expect(token).toBe(mockToken)
    })

    it('debería retornar null si no hay token', () => {
      const token = authService.getToken()
      expect(token).toBeNull()
    })
  })

  describe('isAuthenticated', () => {
    it('debería retornar true con token válido', () => {
      const futureTime = Math.floor(Date.now() / 1000) + 3600
      const payload = btoa(JSON.stringify({ exp: futureTime }))
      const token = `header.${payload}.signature`
      localStorage.setItem('token', token)

      const isAuth = authService.isAuthenticated()

      expect(isAuth).toBe(true)
    })

    it('debería retornar false con token expirado', () => {
      const pastTime = Math.floor(Date.now() / 1000) - 3600
      const payload = btoa(JSON.stringify({ exp: pastTime }))
      const token = `header.${payload}.signature`
      localStorage.setItem('token', token)

      const isAuth = authService.isAuthenticated()

      expect(isAuth).toBe(false)
    })

    it('debería retornar false sin token', () => {
      const isAuth = authService.isAuthenticated()
      expect(isAuth).toBe(false)
    })

    it('debería retornar false con token malformado', () => {
      localStorage.setItem('token', 'invalid-token')

      const isAuth = authService.isAuthenticated()

      expect(isAuth).toBe(false)
    })
  })

  describe('getUser', () => {
    it('debería retornar el usuario parseado del localStorage', () => {
      const mockUser = { id: 1, username: 'testuser', email: 'test@example.com' }
      localStorage.setItem('user', JSON.stringify(mockUser))

      const user = authService.getUser()

      expect(user).toEqual(mockUser)
    })

    it('debería retornar null si no hay usuario', () => {
      const user = authService.getUser()
      expect(user).toBeNull()
    })
  })

  describe('saveUser', () => {
    it('debería guardar el usuario en localStorage', () => {
      const mockUser = { id: 1, username: 'testuser' }

      authService.saveUser(mockUser)

      const savedUser = localStorage.getItem('user')
      expect(savedUser).toBe(JSON.stringify(mockUser))
    })
  })

  describe('saveToken', () => {
    it('debería guardar el token en localStorage', () => {
      const mockToken = 'mock-token-123'

      authService.saveToken(mockToken)

      expect(localStorage.getItem('token')).toBe(mockToken)
    })
  })
})

