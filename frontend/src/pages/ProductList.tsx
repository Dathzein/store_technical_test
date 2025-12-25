import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { productService } from '../services/productService';
import { categoryService } from '../services/categoryService';
import type { ProductListDto, CategoryDto, ProductQueryDto, PagedResultDto } from '../types';
import { BulkImportModal } from '../components/BulkImportModal';

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
    <div className="p-8 max-w-7xl mx-auto">
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-gray-900 text-3xl font-bold m-0">Gestión de Productos</h1>
        <div className="flex gap-4">
          <button 
            className="bg-gray-600 text-white px-4 py-2 border-none rounded-md text-sm font-semibold cursor-pointer transition-colors hover:bg-gray-700"
            onClick={() => setShowBulkImportModal(true)}
          >
            📦 Carga Masiva
          </button>
          <button 
            className="bg-gradient-to-r from-[#667eea] to-[#764ba2] text-white px-4 py-2 border-none rounded-md text-sm font-semibold cursor-pointer transition-all hover:-translate-y-0.5 hover:shadow-lg"
            onClick={() => navigate('/products/new')}
          >
            ➕ Nuevo Producto
          </button>
        </div>
      </div>

      <div className="flex gap-4 mb-6">
        <input
          type="text"
          placeholder="Buscar por nombre o descripción..."
          value={query.searchTerm || ''}
          onChange={handleSearchChange}
          className="flex-1 px-3 py-3 border border-gray-300 rounded-md text-base"
        />

        <select 
          value={query.categoryId || ''} 
          onChange={handleCategoryChange}
          className="px-3 py-3 border border-gray-300 rounded-md text-base min-w-[200px]"
        >
          <option value="">Todas las categorías</option>
          {categories.map((cat) => (
            <option key={cat.id} value={cat.id}>
              {cat.name}
            </option>
          ))}
        </select>
      </div>

      {error && (
        <div className="bg-red-50 text-red-700 px-4 py-3 rounded-md mb-4 border border-red-200">
          {error}
        </div>
      )}

      {loading ? (
        <div className="text-center py-12 text-[#667eea] text-xl">
          Cargando productos...
        </div>
      ) : (
        <>
          <div className="overflow-x-auto bg-white rounded-xl shadow-md">
            <table className="w-full border-collapse">
              <thead className="bg-gradient-to-r from-[#667eea] to-[#764ba2] text-white">
                <tr>
                  <th className="px-4 py-4 text-left">ID</th>
                  <th className="px-4 py-4 text-left">Nombre</th>
                  <th className="px-4 py-4 text-left">Descripción</th>
                  <th className="px-4 py-4 text-left">Categoría</th>
                  <th className="px-4 py-4 text-left">Precio</th>
                  <th className="px-4 py-4 text-left">Stock</th>
                  <th className="px-4 py-4 text-left">Acciones</th>
                </tr>
              </thead>
              <tbody>
                {products.length === 0 ? (
                  <tr>
                    <td colSpan={7} className="text-center py-8 text-gray-400">
                      No se encontraron productos
                    </td>
                  </tr>
                ) : (
                  products.map((product) => (
                    <tr key={product.id} className="border-b border-gray-100 transition-colors hover:bg-gray-50">
                      <td className="px-4 py-4">{product.id}</td>
                      <td className="px-4 py-4">{product.name}</td>
                      <td className="px-4 py-4 max-w-xs whitespace-nowrap overflow-hidden text-ellipsis">
                        {product.description}
                      </td>
                      <td className="px-4 py-4">{product.categoryName}</td>
                      <td className="px-4 py-4">${product.price.toFixed(2)}</td>
                      <td className="px-4 py-4">{product.stock}</td>
                      <td className="px-4 py-4">
                        <div className="flex gap-2">
                          <button
                            className="bg-cyan-500 text-white px-4 py-2 border-none rounded-md text-sm font-semibold cursor-pointer transition-colors hover:bg-cyan-600"
                            onClick={() => navigate(`/products/${product.id}`)}
                          >
                            Ver
                          </button>
                          <button
                            className="bg-yellow-400 text-gray-900 px-4 py-2 border-none rounded-md text-sm font-semibold cursor-pointer transition-colors hover:bg-yellow-500"
                            onClick={() => navigate(`/products/${product.id}/edit`)}
                          >
                            Editar
                          </button>
                          <button 
                            className="bg-red-500 text-white px-4 py-2 border-none rounded-md text-sm font-semibold cursor-pointer transition-colors hover:bg-red-600"
                            onClick={() => handleDelete(product.id)}
                          >
                            Eliminar
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>

          {pagedResult && pagedResult.totalPages > 1 && (
            <div className="flex justify-between items-center mt-6 p-4 bg-white rounded-xl shadow-md">
              <button
                onClick={() => handlePageChange(query.pageNumber! - 1)}
                disabled={query.pageNumber === 1}
                className="bg-[#667eea] text-white px-4 py-2 border-none rounded-md text-sm font-semibold cursor-pointer transition-colors hover:bg-[#5568d3] disabled:bg-gray-300 disabled:cursor-not-allowed"
              >
                ← Anterior
              </button>

              <span className="text-gray-600 font-medium">
                Página {pagedResult.pageNumber} de {pagedResult.totalPages} (Total: {pagedResult.totalCount}{' '}
                productos)
              </span>

              <button
                onClick={() => handlePageChange(query.pageNumber! + 1)}
                disabled={query.pageNumber === pagedResult.totalPages}
                className="bg-[#667eea] text-white px-4 py-2 border-none rounded-md text-sm font-semibold cursor-pointer transition-colors hover:bg-[#5568d3] disabled:bg-gray-300 disabled:cursor-not-allowed"
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
