import React, { useState, useEffect, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import { bulkImportService } from '../services/bulkImportService';
import type { BulkImportStatusDto } from '../types';
import './BulkImportModal.css';

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

  useEffect(() => {
    setupSignalR();

    return () => {
      if (connectionRef.current) {
        connectionRef.current.stop();
      }
    };
  }, []);

  const setupSignalR = async () => {
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
  };

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

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>Carga Masiva de Productos</h2>
          <button className="btn-close" onClick={onClose}>
            ×
          </button>
        </div>

        <div className="modal-body">
          {!isImporting ? (
            <>
              <div className="import-type-selector">
                <label className={importType === 'generate' ? 'active' : ''}>
                  <input
                    type="radio"
                    name="importType"
                    value="generate"
                    checked={importType === 'generate'}
                    onChange={() => setImportType('generate')}
                  />
                  <span>Generar productos aleatorios</span>
                </label>

                <label className={importType === 'csv' ? 'active' : ''}>
                  <input
                    type="radio"
                    name="importType"
                    value="csv"
                    checked={importType === 'csv'}
                    onChange={() => setImportType('csv')}
                  />
                  <span>Subir archivo CSV</span>
                </label>
              </div>

              {importType === 'generate' ? (
                <div className="form-group">
                  <label htmlFor="generateCount">Cantidad de productos a generar:</label>
                  <input
                    type="number"
                    id="generateCount"
                    value={generateCount}
                    onChange={(e) => setGenerateCount(parseInt(e.target.value) || 0)}
                    min="1"
                    max="1000000"
                  />
                </div>
              ) : (
                <div className="form-group">
                  <label htmlFor="csvFile">Archivo CSV:</label>
                  <input type="file" id="csvFile" accept=".csv" onChange={handleFileChange} />
                  {file && <p className="file-name">Archivo seleccionado: {file.name}</p>}
                </div>
              )}

              {error && <div className="error-message">{error}</div>}

              <button
                className="btn-primary btn-full"
                onClick={handleStartImport}
                disabled={importType === 'csv' && !file}
              >
                Iniciar Importación
              </button>
            </>
          ) : (
            <div className="progress-container">
              <div className="progress-info">
                <h3>{progress?.message || 'Procesando...'}</h3>
                <p>
                  {progress?.processedRecords || 0} de {progress?.totalRecords || 0} productos procesados
                </p>
              </div>

              <div className="progress-bar-container">
                <div className="progress-bar" style={{ width: `${getProgressPercentage()}%` }}>
                  <span className="progress-text">{getProgressPercentage().toFixed(1)}%</span>
                </div>
              </div>

              <div className="progress-status">
                <span className={`status-badge status-${progress?.status.toLowerCase()}`}>
                  {progress?.status || 'Processing'}
                </span>
              </div>

              {progress?.status === 'Completed' && (
                <div className="success-message">
                  ✅ Importación completada exitosamente!
                </div>
              )}

              {progress?.status === 'Failed' && (
                <div className="error-message">
                  ❌ Error en la importación: {progress.message}
                </div>
              )}
            </div>
          )}
        </div>

        <div className="modal-footer">
          <button className="btn-secondary" onClick={onClose} disabled={isImporting && progress?.status === 'Processing'}>
            {isImporting && progress?.status === 'Processing' ? 'Importando...' : 'Cerrar'}
          </button>
        </div>
      </div>
    </div>
  );
};

