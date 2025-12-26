import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { productService } from '../services/productService';
import type { ProductDto } from '../types';

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
    return (
      <div className="text-center py-12 text-[#667eea] text-xl">
        Cargando producto...
      </div>
    );
  }

  if (error || !product) {
    return (
      <div className="p-8 max-w-5xl mx-auto">
        <div className="bg-red-50 text-red-700 px-4 py-3 rounded-md mb-4 border border-red-200">
          {error || 'Producto no encontrado'}
        </div>
        <button 
          className="bg-gray-600 text-white px-6 py-3 border-none rounded-md text-base font-semibold cursor-pointer transition-colors hover:bg-gray-700"
          onClick={() => navigate('/products')}
        >
          Volver al listado
        </button>
      </div>
    );
  }

  return (
    <div className="p-8 max-w-5xl mx-auto">
      <div className="bg-white rounded-xl shadow-md overflow-hidden">
        <div className="flex justify-between items-center px-8 py-6 bg-gradient-to-r from-[#667eea] to-[#764ba2] text-white">
          <h1 className="text-3xl font-bold m-0">{product.name}</h1>
          <div className="flex gap-4">
            <button 
              className="bg-yellow-400 text-gray-900 px-6 py-3 border-none rounded-md text-base font-semibold cursor-pointer transition-all hover:bg-yellow-500 hover:-translate-y-0.5"
              onClick={() => navigate(`/products/${product.id}/edit`)}
            >
              ✏️ Editar
            </button>
            <button 
              className="bg-red-500 text-white px-6 py-3 border-none rounded-md text-base font-semibold cursor-pointer transition-all hover:bg-red-600 hover:-translate-y-0.5"
              onClick={handleDelete}
            >
              🗑️ Eliminar
            </button>
          </div>
        </div>

        <div className="p-8">
          <div className="mb-8">
            <h2 className="text-[#667eea] mb-4 text-2xl border-b-2 border-[#667eea] pb-2">
              Información General
            </h2>
            <div className="grid grid-cols-[repeat(auto-fit,minmax(200px,1fr))] gap-6">
              <div className="flex flex-col gap-1">
                <label className="font-semibold text-gray-700 text-sm">ID:</label>
                <span className="text-gray-900 text-lg">{product.id}</span>
              </div>
              <div className="flex flex-col gap-1">
                <label className="font-semibold text-gray-700 text-sm">Nombre:</label>
                <span className="text-gray-900 text-lg">{product.name}</span>
              </div>
              <div className="flex flex-col gap-1">
                <label className="font-semibold text-gray-700 text-sm">Precio:</label>
                <span className="text-green-600 font-bold text-2xl">
                  ${product.price.toFixed(2)}
                </span>
              </div>
              <div className="flex flex-col gap-1">
                <label className="font-semibold text-gray-700 text-sm">Stock:</label>
                <span className={`text-lg font-semibold ${
                  product.stock > 0 ? 'text-green-600' : 'text-red-500'
                }`}>
                  {product.stock} unidades
                </span>
              </div>
            </div>
          </div>

          <div className="mb-8">
            <h2 className="text-[#667eea] mb-4 text-2xl border-b-2 border-[#667eea] pb-2">
              Descripción
            </h2>
            <p className="text-gray-600 leading-relaxed text-lg">
              {product.description}
            </p>
          </div>

          <div className="mb-8">
            <h2 className="text-[#667eea] mb-4 text-2xl border-b-2 border-[#667eea] pb-2">
              Categoría
            </h2>
            {product.category ? (
              <div className="flex gap-6 items-start p-4 bg-gray-50 rounded-lg">
                {product.category.imageUrl && (
                  <img 
                    src={product.category.imageUrl} 
                    alt={product.category.name}
                    className="w-36 h-36 object-cover rounded-lg shadow-md"
                  />
                )}
                <div>
                  <h3 className="text-gray-900 text-xl font-bold my-0 mb-2">
                    {product.category.name}
                  </h3>
                  <p className="text-gray-600 m-0">
                    {product.category.description}
                  </p>
                </div>
              </div>
            ) : (
              <div className="p-4 bg-gray-50 rounded-lg text-gray-500">
                No hay información de categoría disponible
              </div>
            )}
          </div>

          <div className="mb-8">
            <h2 className="text-[#667eea] mb-4 text-2xl border-b-2 border-[#667eea] pb-2">
              Fechas
            </h2>
            <div className="grid grid-cols-[repeat(auto-fit,minmax(200px,1fr))] gap-6">
              <div className="flex flex-col gap-1">
                <label className="font-semibold text-gray-700 text-sm">Creado:</label>
                <span className="text-gray-900 text-lg">
                  {new Date(product.createdAt).toLocaleString('es-ES')}
                </span>
              </div>
              <div className="flex flex-col gap-1">
                <label className="font-semibold text-gray-700 text-sm">Actualizado:</label>
                <span className="text-gray-900 text-lg">
                  {new Date(product.updatedAt).toLocaleString('es-ES')}
                </span>
              </div>
            </div>
          </div>
        </div>

        <div className="px-8 py-6 bg-gray-50 border-t border-gray-200">
          <button 
            className="bg-gray-600 text-white px-6 py-3 border-none rounded-md text-base font-semibold cursor-pointer transition-colors hover:bg-gray-700"
            onClick={() => navigate('/products')}
          >
            ← Volver al listado
          </button>
        </div>
      </div>
    </div>
  );
};
