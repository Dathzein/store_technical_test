import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { productService } from '../services/productService';
import { categoryService } from '../services/categoryService';
import type { CreateProductDto, UpdateProductDto, CategoryDto, ProductDto } from '../types';

export const ProductForm: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const isEditMode = !!id;
  const navigate = useNavigate();

  const [formData, setFormData] = useState<CreateProductDto | UpdateProductDto>({
    name: '',
    description: '',
    price: 0,
    stock: 0,
    categoryId: 0,
  });

  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string>('');
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    loadCategories();
    if (isEditMode && id) {
      loadProduct(parseInt(id));
    }
  }, [id, isEditMode]);

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

  const loadProduct = async (productId: number) => {
    setLoading(true);
    try {
      const response = await productService.getById(productId);
      if (response.isSuccess) {
        const product: ProductDto = response.data;
        setFormData({
          name: product.name,
          description: product.description,
          price: product.price,
          stock: product.stock,
          categoryId: product.category?.id || 0,
        });
      } else {
        setError(response.message);
      }
    } catch (err: any) {
      setError(err.message || 'Error al cargar producto');
    } finally {
      setLoading(false);
    }
  };

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.name || formData.name.trim().length < 3) {
      newErrors.name = 'El nombre debe tener al menos 3 caracteres';
    }

    if (!formData.description || formData.description.trim().length < 3) {
      newErrors.description = 'La descripción debe tener al menos 3 caracteres';
    }

    if (formData.price <= 0) {
      newErrors.price = 'El precio debe ser mayor a 0';
    }

    if (formData.stock < 0) {
      newErrors.stock = 'El stock no puede ser negativo';
    }

    if (!formData.categoryId || formData.categoryId === 0) {
      newErrors.categoryId = 'Debe seleccionar una categoría';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (!validateForm()) {
      return;
    }

    setLoading(true);

    try {
      if (isEditMode && id) {
        const response = await productService.update(parseInt(id), formData);
        if (response.isSuccess) {
          navigate('/products');
        } else {
          setError(response.message);
        }
      } else {
        const response = await productService.create(formData);
        if (response.isSuccess) {
          navigate('/products');
        } else {
          setError(response.message);
        }
      }
    } catch (err: any) {
      setError(err.message || 'Error al guardar producto');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: name === 'price' || name === 'stock' || name === 'categoryId' ? parseFloat(value) || 0 : value,
    });
  };

  return (
    <div className="p-8 max-w-4xl mx-auto">
      <div className="bg-white p-8 rounded-xl shadow-md">
        <h1 className="text-gray-900 text-3xl font-bold mb-6">
          {isEditMode ? 'Editar Producto' : 'Nuevo Producto'}
        </h1>

        {error && (
          <div className="bg-red-50 text-red-700 px-4 py-3 rounded-md mb-4 border border-red-200">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit}>
          <div className="mb-6">
            <label htmlFor="name" className="block mb-2 text-gray-700 font-medium">
              Nombre *
            </label>
            <input
              type="text"
              id="name"
              name="name"
              value={formData.name}
              onChange={handleChange}
              disabled={loading}
              className={`w-full px-3 py-3 border rounded-md text-base transition-colors focus:outline-none ${
                errors.name ? 'border-red-500' : 'border-gray-300 focus:border-[#667eea]'
              } disabled:bg-gray-100 disabled:cursor-not-allowed`}
            />
            {errors.name && (
              <span className="block text-red-500 text-sm mt-1">{errors.name}</span>
            )}
          </div>

          <div className="mb-6">
            <label htmlFor="description" className="block mb-2 text-gray-700 font-medium">
              Descripción *
            </label>
            <textarea
              id="description"
              name="description"
              value={formData.description}
              onChange={handleChange}
              disabled={loading}
              rows={4}
              className={`w-full px-3 py-3 border rounded-md text-base transition-colors focus:outline-none font-inherit ${
                errors.description ? 'border-red-500' : 'border-gray-300 focus:border-[#667eea]'
              } disabled:bg-gray-100 disabled:cursor-not-allowed`}
            />
            {errors.description && (
              <span className="block text-red-500 text-sm mt-1">{errors.description}</span>
            )}
          </div>

          <div className="grid grid-cols-2 gap-4 mb-6">
            <div>
              <label htmlFor="price" className="block mb-2 text-gray-700 font-medium">
                Precio *
              </label>
              <input
                type="number"
                id="price"
                name="price"
                value={formData.price}
                onChange={handleChange}
                disabled={loading}
                step="0.01"
                min="0"
                className={`w-full px-3 py-3 border rounded-md text-base transition-colors focus:outline-none ${
                  errors.price ? 'border-red-500' : 'border-gray-300 focus:border-[#667eea]'
                } disabled:bg-gray-100 disabled:cursor-not-allowed`}
              />
              {errors.price && (
                <span className="block text-red-500 text-sm mt-1">{errors.price}</span>
              )}
            </div>

            <div>
              <label htmlFor="stock" className="block mb-2 text-gray-700 font-medium">
                Stock *
              </label>
              <input
                type="number"
                id="stock"
                name="stock"
                value={formData.stock}
                onChange={handleChange}
                disabled={loading}
                min="0"
                className={`w-full px-3 py-3 border rounded-md text-base transition-colors focus:outline-none ${
                  errors.stock ? 'border-red-500' : 'border-gray-300 focus:border-[#667eea]'
                } disabled:bg-gray-100 disabled:cursor-not-allowed`}
              />
              {errors.stock && (
                <span className="block text-red-500 text-sm mt-1">{errors.stock}</span>
              )}
            </div>
          </div>

          <div className="mb-6">
            <label htmlFor="categoryId" className="block mb-2 text-gray-700 font-medium">
              Categoría *
            </label>
            <select
              id="categoryId"
              name="categoryId"
              value={formData.categoryId}
              onChange={handleChange}
              disabled={loading}
              className={`w-full px-3 py-3 border rounded-md text-base transition-colors focus:outline-none ${
                errors.categoryId ? 'border-red-500' : 'border-gray-300 focus:border-[#667eea]'
              } disabled:bg-gray-100 disabled:cursor-not-allowed`}
            >
              <option value="0">Seleccionar categoría...</option>
              {categories.map((cat) => (
                <option key={cat.id} value={cat.id}>
                  {cat.name}
                </option>
              ))}
            </select>
            {errors.categoryId && (
              <span className="block text-red-500 text-sm mt-1">{errors.categoryId}</span>
            )}
          </div>

          <div className="flex gap-4 justify-end mt-8">
            <button 
              type="button" 
              className="px-6 py-3 bg-gray-600 text-white border-none rounded-md text-base font-semibold cursor-pointer transition-colors hover:bg-gray-700 disabled:opacity-60 disabled:cursor-not-allowed"
              onClick={() => navigate('/products')} 
              disabled={loading}
            >
              Cancelar
            </button>
            <button 
              type="submit" 
              className="px-6 py-3 bg-gradient-to-r from-[#667eea] to-[#764ba2] text-white border-none rounded-md text-base font-semibold cursor-pointer transition-all hover:-translate-y-0.5 hover:shadow-lg disabled:opacity-60 disabled:cursor-not-allowed"
              disabled={loading}
            >
              {loading ? 'Guardando...' : isEditMode ? 'Actualizar' : 'Crear'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
