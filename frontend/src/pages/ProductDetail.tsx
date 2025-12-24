import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { productService } from '../services/productService';
import type { ProductDto } from '../types';
import './ProductDetail.css';

export const ProductDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [product, setProduct] = useState<ProductDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>('');

  useEffect(() => {
    if (id) {
      loadProduct(parseInt(id));
    }
  }, [id]);

  const loadProduct = async (productId: number) => {
    setLoading(true);
    setError('');
    try {
      const response = await productService.getById(productId);
      if (response.isSuccess) {
        setProduct(response.data);
      } else {
        setError(response.message);
      }
    } catch (err: any) {
      setError(err.message || 'Error al cargar producto');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (!window.confirm('¿Estás seguro de eliminar este producto?')) {
      return;
    }

    if (!id) return;

    try {
      const response = await productService.delete(parseInt(id));
      if (response.isSuccess) {
        navigate('/products');
      } else {
        alert(response.message);
      }
    } catch (err: any) {
      alert(err.message || 'Error al eliminar producto');
    }
  };

  if (loading) {
    return <div className="loading">Cargando producto...</div>;
  }

  if (error || !product) {
    return (
      <div className="product-detail-container">
        <div className="error-message">{error || 'Producto no encontrado'}</div>
        <button className="btn-secondary" onClick={() => navigate('/products')}>
          Volver al listado
        </button>
      </div>
    );
  }

  return (
    <div className="product-detail-container">
      <div className="detail-card">
        <div className="detail-header">
          <h1>{product.name}</h1>
          <div className="detail-actions">
            <button className="btn-edit" onClick={() => navigate(`/products/${product.id}/edit`)}>
              ✏️ Editar
            </button>
            <button className="btn-delete" onClick={handleDelete}>
              🗑️ Eliminar
            </button>
          </div>
        </div>

        <div className="detail-content">
          <div className="detail-section">
            <h2>Información General</h2>
            <div className="detail-grid">
              <div className="detail-item">
                <label>ID:</label>
                <span>{product.id}</span>
              </div>
              <div className="detail-item">
                <label>Nombre:</label>
                <span>{product.name}</span>
              </div>
              <div className="detail-item">
                <label>Precio:</label>
                <span className="price">${product.price.toFixed(2)}</span>
              </div>
              <div className="detail-item">
                <label>Stock:</label>
                <span className={product.stock > 0 ? 'stock-available' : 'stock-unavailable'}>
                  {product.stock} unidades
                </span>
              </div>
            </div>
          </div>

          <div className="detail-section">
            <h2>Descripción</h2>
            <p className="description">{product.description}</p>
          </div>

          <div className="detail-section">
            <h2>Categoría</h2>
            <div className="category-info">
              {product.category.imageUrl && (
                <img src={product.category.imageUrl} alt={product.category.name} className="category-image" />
              )}
              <div>
                <h3>{product.category.name}</h3>
                <p>{product.category.description}</p>
              </div>
            </div>
          </div>

          <div className="detail-section">
            <h2>Fechas</h2>
            <div className="detail-grid">
              <div className="detail-item">
                <label>Creado:</label>
                <span>{new Date(product.createdAt).toLocaleString('es-ES')}</span>
              </div>
              <div className="detail-item">
                <label>Actualizado:</label>
                <span>{new Date(product.updatedAt).toLocaleString('es-ES')}</span>
              </div>
            </div>
          </div>
        </div>

        <div className="detail-footer">
          <button className="btn-secondary" onClick={() => navigate('/products')}>
            ← Volver al listado
          </button>
        </div>
      </div>
    </div>
  );
};

