import apiClient from './api';
import type { Response, BulkImportJobDto } from '../types';

export const bulkImportService = {
  startImport: async (file?: File, generateCount?: number): Promise<Response<BulkImportJobDto>> => {
    const formData = new FormData();
    
    if (file) {
      formData.append('CsvFile', file);
    }
    
    if (generateCount) {
      formData.append('GenerateCount', generateCount.toString());
    }

    const response = await apiClient.post<Response<BulkImportJobDto>>('/BulkImport', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  },

  getJobStatus: async (jobId: string): Promise<Response<BulkImportJobDto>> => {
    const response = await apiClient.get<Response<BulkImportJobDto>>(`/BulkImport/${jobId}`);
    return response.data;
  },

  getAllJobs: async (): Promise<Response<BulkImportJobDto[]>> => {
    const response = await apiClient.get<Response<BulkImportJobDto[]>>('/BulkImport');
    return response.data;
  },
};

