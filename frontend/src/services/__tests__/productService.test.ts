import { describe, it, expect } from 'vitest'
import { productService } from '../productService'
import type { CreateProductDto, UpdateProductDto } from '../../types'

describe('productService', () => {
  describe('getAll', () => {
    it('debería obtener lista paginada de productos', async () => {
      const query = {
        pageNumber: 1,
        pageSize: 10,
      }

      const response = await productService.getAll(query)

      expect(response.isSuccess).toBe(true)
      expect(response.data.items).toBeDefined()
      expect(Array.isArray(response.data.items)).toBe(true)
      expect(response.data.items.length).toBeGreaterThan(0)
      expect(response.data.pageNumber).toBe(1)
      expect(response.data.pageSize).toBe(10)
      expect(response.data.totalCount).toBeGreaterThan(0)
    })

    it('debería filtrar productos por término de búsqueda', async () => {
      const query = {
        pageNumber: 1,
        pageSize: 10,
        searchTerm: 'Dell',
      }

      const response = await productService.getAll(query)

      expect(response.isSuccess).toBe(true)
      expect(response.data.items.length).toBeGreaterThan(0)
      expect(response.data.items[0].name).toContain('Dell')
    })

    it('debería retornar lista vacía cuando no hay coincidencias', async () => {
      const query = {
        pageNumber: 1,
        pageSize: 10,
        searchTerm: 'ProductoQueNoExiste',
      }

      const response = await productService.getAll(query)

      expect(response.isSuccess).toBe(true)
      expect(response.data.items.length).toBe(0)
    })
  })

  describe('getById', () => {
    it('debería obtener un producto por ID', async () => {
      const response = await productService.getById(1)

      expect(response.isSuccess).toBe(true)
      expect(response.data).toBeDefined()
      expect(response.data.id).toBe(1)
      expect(response.data.name).toBeDefined()
      expect(response.data.category).toBeDefined()
    })

    it('debería retornar error cuando el producto no existe', async () => {
      try {
        await productService.getById(9999)
        expect.fail('Debería haber lanzado un error')
      } catch (error: any) {
        expect(error).toBeDefined()
        expect(error.response?.status).toBe(404)
      }
    })
  })

  describe('create', () => {
    it('debería crear un nuevo producto', async () => {
      const newProduct: CreateProductDto = {
        name: 'Nuevo Servidor',
        description: 'Servidor de prueba',
        price: 1999.99,
        stock: 5,
        categoryId: 1,
      }

      const response = await productService.create(newProduct)

      expect(response.isSuccess).toBe(true)
      expect(response.data).toBeDefined()
      expect(response.data.name).toBe(newProduct.name)
      expect(response.data.price).toBe(newProduct.price)
      expect(response.data.id).toBeDefined()
      expect(response.code).toBe(201)
    })
  })

  describe('update', () => {
    it('debería actualizar un producto existente', async () => {
      const updateData: UpdateProductDto = {
        name: 'Producto Actualizado',
        description: 'Descripción actualizada',
        price: 2999.99,
        stock: 20,
        categoryId: 1,
      }

      const response = await productService.update(1, updateData)

      expect(response.isSuccess).toBe(true)
      expect(response.data).toBeDefined()
      expect(response.data.name).toBe(updateData.name)
      expect(response.data.price).toBe(updateData.price)
      expect(response.code).toBe(200)
    })
  })

  describe('delete', () => {
    it('debería eliminar un producto', async () => {
      const response = await productService.delete(1)

      expect(response.isSuccess).toBe(true)
      expect(response.code).toBe(200)
      expect(response.message).toContain('deleted')
    })
  })
})

