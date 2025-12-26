import React, { useState, useEffect, useRef, useCallback } from 'react';
import * as signalR from '@microsoft/signalr';
import { bulkImportService } from '../services/bulkImportService';
import type { BulkImportStatusDto } from '../types';

interface BulkImportModalProps {
  onClose: () => void;
  onComplete: () => void;
}

export const BulkImportModal: React.FC<BulkImportModalProps> = ({ onClose, onComplete }) => {
  const [file, setFile] = useState<File | null>(null);
  const [generateCount, setGenerateCount] = useState<number>(100000);
  const [importType, setImportType] = useState<'generate' | 'csv'>('generate');
  const [isImporting, setIsImporting] = useState(false);
  const [progress, setProgress] = useState<BulkImportStatusDto | null>(null);
  const [error, setError] = useState<string>('');

  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const currentJobIdRef = useRef<string | null>(null);

  const setupSignalR = useCallback(async () => {
    const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';
    const token = localStorage.getItem('token');

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${API_BASE_URL}/hubs/import`, {
        accessTokenFactory: () => token || '',
      })
      .withAutomaticReconnect()
      .build();

    connection.on('ImportProgress', (status: BulkImportStatusDto) => {
      console.log('Import progress:', status);
      setProgress(status);

      if (status.status === 'Completed' || status.status === 'Failed') {
        setIsImporting(false);
        if (status.status === 'Completed') {
          setTimeout(() => {
            onComplete();
          }, 2000);
        }
      }
    });

    try {
      await connection.start();
      console.log('SignalR connected');
      connectionRef.current = connection;
    } catch (err) {
      console.error('Error connecting to SignalR:', err);
    }
  }, [onComplete]);

  useEffect(() => {
    setupSignalR();

    return () => {
      if (connectionRef.current) {
        connectionRef.current.stop();
      }
    };
  }, [setupSignalR]);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      setFile(e.target.files[0]);
    }
  };

  const handleStartImport = async () => {
    setError('');
    setProgress(null);
    setIsImporting(true);

    try {
      const response =
        importType === 'generate'
          ? await bulkImportService.startImport(undefined, generateCount)
          : await bulkImportService.startImport(file || undefined, undefined);

      if (response.isSuccess) {
        const jobId = response.data.id;
        currentJobIdRef.current = jobId;

        // Join SignalR group for this job
        if (connectionRef.current) {
          await connectionRef.current.invoke('JoinJobGroup', jobId);
        }

        setProgress({
          jobId: jobId,
          status: 'Processing',
          processedRecords: 0,
          totalRecords: importType === 'generate' ? generateCount : 0,
          percentage: 0,
          message: 'Iniciando importación...',
        });
      } else {
        setError(response.message);
        setIsImporting(false);
      }
    } catch (err: any) {
      setError(err.message || 'Error al iniciar importación');
      setIsImporting(false);
    }
  };

  const getProgressPercentage = () => {
    if (!progress) return 0;
    return progress.percentage || 0;
  };

  const getStatusColor = (status?: string) => {
    switch (status?.toLowerCase()) {
      case 'pending':
        return 'bg-yellow-400 text-gray-900';
      case 'processing':
        return 'bg-cyan-500 text-white';
      case 'completed':
        return 'bg-green-500 text-white';
      case 'failed':
        return 'bg-red-500 text-white';
      default:
        return 'bg-gray-500 text-white';
    }
  };

  return (
    <div 
      className="fixed inset-0 bg-black/50 flex justify-center items-center z-[1000]"
      onClick={onClose}
    >
      <div 
        className="bg-white rounded-xl w-[90%] max-w-2xl max-h-[90vh] overflow-y-auto shadow-2xl"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="flex justify-between items-center px-6 py-4 border-b border-gray-200 bg-gradient-to-r from-[#667eea] to-[#764ba2] text-white rounded-t-xl">
          <h2 className="text-2xl font-bold m-0">Carga Masiva de Productos</h2>
          <button 
            className="bg-transparent border-none text-3xl text-white cursor-pointer leading-none p-0 w-8 h-8 flex items-center justify-center rounded-full transition-colors hover:bg-white/20"
            onClick={onClose}
          >
            ×
          </button>
        </div>

        <div className="p-8">
          {!isImporting ? (
            <>
              <div className="flex gap-4 mb-8">
                <label className={`flex-1 p-4 border-2 rounded-lg cursor-pointer transition-all flex items-center gap-2 ${
                  importType === 'generate' ? 'border-[#667eea] bg-blue-50' : 'border-gray-300 hover:border-[#667eea] hover:bg-gray-50'
                }`}>
                  <input
                    type="radio"
                    name="importType"
                    value="generate"
                    checked={importType === 'generate'}
                    onChange={() => setImportType('generate')}
                    className="m-0"
                  />
                  <span>Generar productos aleatorios</span>
                </label>

                <label className={`flex-1 p-4 border-2 rounded-lg cursor-pointer transition-all flex items-center gap-2 ${
                  importType === 'csv' ? 'border-[#667eea] bg-blue-50' : 'border-gray-300 hover:border-[#667eea] hover:bg-gray-50'
                }`}>
                  <input
                    type="radio"
                    name="importType"
                    value="csv"
                    checked={importType === 'csv'}
                    onChange={() => setImportType('csv')}
                    className="m-0"
                  />
                  <span>Subir archivo CSV</span>
                </label>
              </div>

              {importType === 'generate' ? (
                <div className="mb-6">
                  <label htmlFor="generateCount" className="block mb-2 text-gray-700 font-medium">
                    Cantidad de productos a generar:
                  </label>
                  <input
                    type="number"
                    id="generateCount"
                    value={generateCount}
                    onChange={(e) => setGenerateCount(parseInt(e.target.value) || 0)}
                    min="1"
                    max="1000000"
                    className="w-full px-3 py-2 border border-gray-300 rounded-md text-base"
                  />
                </div>
              ) : (
                <div className="mb-6">
                  <label htmlFor="csvFile" className="block mb-2 text-gray-700 font-medium">
                    Archivo CSV:
                  </label>
                  <input 
                    type="file" 
                    id="csvFile" 
                    accept=".csv" 
                    onChange={handleFileChange}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md text-base"
                  />
                  {file && <p className="mt-2 text-gray-600 text-sm">Archivo seleccionado: {file.name}</p>}
                </div>
              )}

              {error && (
                <div className="bg-red-50 text-red-700 px-4 py-3 rounded-md mb-4 border border-red-200">
                  {error}
                </div>
              )}

              <button
                className="w-full px-6 py-3 border-none rounded-md text-base font-semibold cursor-pointer transition-all bg-gradient-to-r from-[#667eea] to-[#764ba2] text-white hover:-translate-y-0.5 hover:shadow-lg disabled:opacity-60 disabled:cursor-not-allowed"
                onClick={handleStartImport}
                disabled={importType === 'csv' && !file}
              >
                Iniciar Importación
              </button>
            </>
          ) : (
            <div className="text-center">
              <div className="mb-6">
                <h3 className="text-gray-900 mb-2">{progress?.message || 'Procesando...'}</h3>
                <p className="text-gray-600 mb-6">
                  {progress?.processedRecords || 0} de {progress?.totalRecords || 0} productos procesados
                </p>
              </div>

              <div className="w-full h-10 bg-gray-200 rounded-full overflow-hidden mb-4 relative">
                <div 
                  className="h-full bg-gradient-to-r from-[#667eea] to-[#764ba2] transition-all duration-300 flex items-center justify-center relative"
                  style={{ width: `${getProgressPercentage()}%` }}
                >
                  <span className="text-white font-semibold text-base absolute left-1/2 -translate-x-1/2" style={{ left: '50%', transform: 'translateX(-50%)' }}>
                    {getProgressPercentage().toFixed(1)}%
                  </span>
                </div>
              </div>

              <div className="mt-4">
                <span className={`inline-block px-4 py-2 rounded-full font-semibold text-sm ${getStatusColor(progress?.status)}`}>
                  {progress?.status || 'Processing'}
                </span>
              </div>

              {progress?.status === 'Completed' && (
                <div className="mt-4 p-4 bg-green-100 text-green-800 border border-green-300 rounded-md font-medium">
                  ✅ Importación completada exitosamente!
                </div>
              )}

              {progress?.status === 'Failed' && (
                <div className="mt-4 p-4 bg-red-50 text-red-700 border border-red-200 rounded-md">
                  ❌ Error en la importación: {progress.message}
                </div>
              )}
            </div>
          )}
        </div>

        <div className="px-6 py-4 border-t border-gray-200 bg-gray-50 flex justify-end">
          <button 
            className="px-6 py-3 bg-gray-600 text-white border-none rounded-md text-base font-semibold cursor-pointer transition-colors hover:bg-gray-700 disabled:opacity-60 disabled:cursor-not-allowed"
            onClick={onClose} 
            disabled={isImporting && progress?.status === 'Processing'}
          >
            {isImporting && progress?.status === 'Processing' ? 'Importando...' : 'Cerrar'}
          </button>
        </div>
      </div>
    </div>
  );
};
