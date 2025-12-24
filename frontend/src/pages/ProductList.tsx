import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { productService } from '../services/productService';
import { categoryService } from '../services/categoryService';
import type { ProductListDto, CategoryDto, ProductQueryDto, PagedResultDto } from '../types';
import { BulkImportModal } from '../components/BulkImportModal';
import './ProductList.css';

export const ProductList: React.FC = () => {
  const [products, setProducts] = useState<ProductListDto[]>([]);
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [pagedResult, setPagedResult] = useState<PagedResultDto<ProductListDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>('');
  const [showBulkImportModal, setShowBulkImportModal] = useState(false);

  const [query, setQuery] = useState<ProductQueryDto>({
    pageNumber: 1,
    pageSize: 10,
    searchTerm: '',
    categoryId: undefined,
    minPrice: undefined,
    maxPrice: undefined,
    minStock: undefined,
    sortBy: 'name',
    sortOrder: 'asc',
  });

  const navigate = useNavigate();

  useEffect(() => {
    loadCategories();
  }, []);

  useEffect(() => {
    loadProducts();
  }, [query]);

  const loadCategories = async () => {
    try {
      const response = await categoryService.getAll();
      if (response.isSuccess) {
        setCategories(response.data);
      }
    } catch (err: any) {
      console.error('Error loading categories:', err);
    }
  };

  const loadProducts = async () => {
    setLoading(true);
    setError('');
    try {
      const response = await productService.getAll(query);
      if (response.isSuccess) {
        setProducts(response.data.items);
        setPagedResult(response.data);
      } else {
        setError(response.message);
      }
    } catch (err: any) {
      setError(err.message || 'Error al cargar productos');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('¿Estás seguro de eliminar este producto?')) {
      return;
    }

    try {
      const response = await productService.delete(id);
      if (response.isSuccess) {
        loadProducts();
      } else {
        alert(response.message);
      }
    } catch (err: any) {
      alert(err.message || 'Error al eliminar producto');
    }
  };

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setQuery({ ...query, searchTerm: e.target.value, pageNumber: 1 });
  };

  const handleCategoryChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const value = e.target.value;
    setQuery({ ...query, categoryId: value ? parseInt(value) : undefined, pageNumber: 1 });
  };

  const handlePageChange = (newPage: number) => {
    setQuery({ ...query, pageNumber: newPage });
  };

  const handleBulkImportComplete = () => {
    setShowBulkImportModal(false);
    loadProducts();
  };

  return (
    <div className="product-list-container">
      <div className="header">
        <h1>Gestión de Productos</h1>
        <div className="header-actions">
          <button className="btn-secondary" onClick={() => setShowBulkImportModal(true)}>
            📦 Carga Masiva
          </button>
          <button className="btn-primary" onClick={() => navigate('/products/new')}>
            ➕ Nuevo Producto
          </button>
        </div>
      </div>

      <div className="filters">
        <input
          type="text"
          placeholder="Buscar por nombre o descripción..."
          value={query.searchTerm || ''}
          onChange={handleSearchChange}
          className="search-input"
        />

        <select value={query.categoryId || ''} onChange={handleCategoryChange} className="category-filter">
          <option value="">Todas las categorías</option>
          {categories.map((cat) => (
            <option key={cat.id} value={cat.id}>
              {cat.name}
            </option>
          ))}
        </select>
      </div>

      {error && <div className="error-message">{error}</div>}

      {loading ? (
        <div className="loading">Cargando productos...</div>
      ) : (
        <>
          <div className="table-container">
            <table className="products-table">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Nombre</th>
                  <th>Descripción</th>
                  <th>Categoría</th>
                  <th>Precio</th>
                  <th>Stock</th>
                  <th>Acciones</th>
                </tr>
              </thead>
              <tbody>
                {products.length === 0 ? (
                  <tr>
                    <td colSpan={7} className="no-data">
                      No se encontraron productos
                    </td>
                  </tr>
                ) : (
                  products.map((product) => (
                    <tr key={product.id}>
                      <td>{product.id}</td>
                      <td>{product.name}</td>
                      <td className="description">{product.description}</td>
                      <td>{product.categoryName}</td>
                      <td>${product.price.toFixed(2)}</td>
                      <td>{product.stock}</td>
                      <td className="actions">
                        <button
                          className="btn-view"
                          onClick={() => navigate(`/products/${product.id}`)}
                        >
                          Ver
                        </button>
                        <button
                          className="btn-edit"
                          onClick={() => navigate(`/products/${product.id}/edit`)}
                        >
                          Editar
                        </button>
                        <button className="btn-delete" onClick={() => handleDelete(product.id)}>
                          Eliminar
                        </button>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>

          {pagedResult && pagedResult.totalPages > 1 && (
            <div className="pagination">
              <button
                onClick={() => handlePageChange(query.pageNumber! - 1)}
                disabled={query.pageNumber === 1}
                className="btn-page"
              >
                ← Anterior
              </button>

              <span className="page-info">
                Página {pagedResult.pageNumber} de {pagedResult.totalPages} (Total: {pagedResult.totalCount}{' '}
                productos)
              </span>

              <button
                onClick={() => handlePageChange(query.pageNumber! + 1)}
                disabled={query.pageNumber === pagedResult.totalPages}
                className="btn-page"
              >
                Siguiente →
              </button>
            </div>
          )}
        </>
      )}

      {showBulkImportModal && (
        <BulkImportModal onClose={() => setShowBulkImportModal(false)} onComplete={handleBulkImportComplete} />
      )}
    </div>
  );
};

