import { describe, it, expect, beforeEach, vi } from 'vitest'
import { categoryService } from '../categoryService'
import { server } from '../../mocks/server'
import { http, HttpResponse } from 'msw'
import type { Response, CategoryDto, CreateCategoryDto, UpdateCategoryDto } from '../../types'

describe('categoryService', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('getAll', () => {
    it('debería obtener todas las categorías exitosamente', async () => {
      const result = await categoryService.getAll()

      expect(result.isSuccess).toBe(true)
      expect(result.data).toBeInstanceOf(Array)
      expect(result.data.length).toBeGreaterThan(0)
      expect(result.data[0]).toHaveProperty('id')
      expect(result.data[0]).toHaveProperty('name')
    })

    it('debería manejar errores al obtener categorías', async () => {
      server.use(
        http.get('*/Category', () => {
          return new HttpResponse(null, { status: 500 })
        })
      )

      await expect(categoryService.getAll()).rejects.toThrow()
    })
  })

  describe('getById', () => {
    it('debería obtener una categoría por ID exitosamente', async () => {
      const categoryId = 1
      const result = await categoryService.getById(categoryId)

      expect(result.isSuccess).toBe(true)
      expect(result.data).toHaveProperty('id', categoryId)
      expect(result.data).toHaveProperty('name')
      expect(result.data).toHaveProperty('description')
    })

    it('debería manejar errores al obtener categoría por ID', async () => {
      const categoryId = 999
      server.use(
        http.get('*/Category/:id', () => {
          return new HttpResponse(null, { status: 404 })
        })
      )

      await expect(categoryService.getById(categoryId)).rejects.toThrow()
    })
  })

  describe('create', () => {
    it('debería crear una categoría exitosamente', async () => {
      const newCategory: CreateCategoryDto = {
        name: 'Nueva Categoría',
        description: 'Descripción de prueba',
        imageUrl: 'https://example.com/image.jpg',
      }

      const result = await categoryService.create(newCategory)

      expect(result.isSuccess).toBe(true)
      expect(result.data).toHaveProperty('id')
      expect(result.data.name).toBe(newCategory.name)
      expect(result.data.description).toBe(newCategory.description)
    })

    it('debería manejar errores al crear categoría', async () => {
      server.use(
        http.post('*/Category', () => {
          return new HttpResponse(null, { status: 400 })
        })
      )

      const newCategory: CreateCategoryDto = {
        name: '',
        description: '',
        imageUrl: '',
      }

      await expect(categoryService.create(newCategory)).rejects.toThrow()
    })
  })

  describe('update', () => {
    it('debería actualizar una categoría exitosamente', async () => {
      const categoryId = 1
      const updateData: UpdateCategoryDto = {
        name: 'Categoría Actualizada',
        description: 'Descripción actualizada',
        imageUrl: 'https://example.com/new-image.jpg',
      }

      const result = await categoryService.update(categoryId, updateData)

      expect(result.isSuccess).toBe(true)
      expect(result.data.id).toBe(categoryId)
      expect(result.data.name).toBe(updateData.name)
      expect(result.data.description).toBe(updateData.description)
    })

    it('debería manejar errores al actualizar categoría', async () => {
      server.use(
        http.put('*/Category/:id', () => {
          return new HttpResponse(null, { status: 404 })
        })
      )

      const updateData: UpdateCategoryDto = {
        name: 'Test',
        description: 'Test',
        imageUrl: 'test.jpg',
      }

      await expect(categoryService.update(999, updateData)).rejects.toThrow()
    })
  })

  describe('delete', () => {
    it('debería eliminar una categoría exitosamente', async () => {
      const categoryId = 1
      const result = await categoryService.delete(categoryId)

      expect(result.isSuccess).toBe(true)
      expect(result.data).toBe(true)
    })

    it('debería manejar errores al eliminar categoría', async () => {
      server.use(
        http.delete('*/Category/:id', () => {
          return new HttpResponse(null, { status: 404 })
        })
      )

      await expect(categoryService.delete(999)).rejects.toThrow()
    })
  })
})

