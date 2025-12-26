import { describe, it, expect, beforeEach, vi } from 'vitest'
import { bulkImportService } from '../bulkImportService'
import { server } from '../../mocks/server'
import { http, HttpResponse } from 'msw'
import type { Response, BulkImportJobDto } from '../../types'

describe('bulkImportService', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('startImport', () => {
    it('debería iniciar importación con archivo CSV', async () => {
      const file = new File(['test,data'], 'test.csv', { type: 'text/csv' })
      
      const result = await bulkImportService.startImport(file)

      expect(result.isSuccess).toBe(true)
      expect(result.data).toHaveProperty('id')
      expect(result.data).toHaveProperty('status')
      expect(result.data.status).toBe('Pending')
    })

    it('debería iniciar importación con generateCount', async () => {
      const result = await bulkImportService.startImport(undefined, 100)

      expect(result.isSuccess).toBe(true)
      expect(result.data).toHaveProperty('id')
      expect(result.data.totalRecords).toBe(100)
    })

    it('debería iniciar importación con archivo y generateCount', async () => {
      const file = new File(['test,data'], 'test.csv', { type: 'text/csv' })
      
      const result = await bulkImportService.startImport(file, 50)

      expect(result.isSuccess).toBe(true)
      expect(result.data).toHaveProperty('id')
    })

    it('debería manejar errores al iniciar importación', async () => {
      server.use(
        http.post('*/BulkImport', () => {
          return new HttpResponse(null, { status: 400 })
        })
      )

      const file = new File(['invalid'], 'test.csv', { type: 'text/csv' })
      await expect(bulkImportService.startImport(file)).rejects.toThrow()
    })
  })

  describe('getJobStatus', () => {
    it('debería obtener el estado de un trabajo exitosamente', async () => {
      const jobId = 'test-job-id'
      
      server.use(
        http.get(`*/BulkImport/${jobId}`, () => {
          const response: Response<any> = {
            data: {
              id: jobId,
              status: 'Completed',
              totalRecords: 100,
              processedRecords: 100,
              createdAt: new Date().toISOString(),
            },
            code: 200,
            message: 'Success',
            isSuccess: true,
          }
          return HttpResponse.json(response)
        })
      )

      const result = await bulkImportService.getJobStatus(jobId)

      expect(result.isSuccess).toBe(true)
      expect(result.data).toHaveProperty('id')
    })

    it('debería manejar errores al obtener estado', async () => {
      server.use(
        http.get('*/BulkImport/*', () => {
          return new HttpResponse(null, { status: 404 })
        })
      )

      await expect(bulkImportService.getJobStatus('invalid-id')).rejects.toThrow()
    })
  })

  describe('getAllJobs', () => {
    it('debería obtener todos los trabajos exitosamente', async () => {
      server.use(
        http.get('*/BulkImport', () => {
          const response: Response<BulkImportJobDto[]> = {
            data: [
              {
                id: 'job-1',
                status: 'Completed',
                totalRecords: 100,
                processedRecords: 100,
                createdAt: new Date().toISOString(),
              },
              {
                id: 'job-2',
                status: 'Pending',
                totalRecords: 50,
                processedRecords: 0,
                createdAt: new Date().toISOString(),
              },
            ],
            code: 200,
            message: 'Success',
            isSuccess: true,
          }
          return HttpResponse.json(response)
        })
      )

      const result = await bulkImportService.getAllJobs()

      expect(result.isSuccess).toBe(true)
      expect(result.data).toBeInstanceOf(Array)
      expect(result.data.length).toBe(2)
    })

    it('debería manejar errores al obtener todos los trabajos', async () => {
      server.use(
        http.get('*/BulkImport', () => {
          return new HttpResponse(null, { status: 500 })
        })
      )

      await expect(bulkImportService.getAllJobs()).rejects.toThrow()
    })
  })
})

