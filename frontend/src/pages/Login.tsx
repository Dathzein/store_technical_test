import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useAuth } from '../context/AuthContext';
import { loginSchema, type LoginFormData } from '../validators/authValidators';

export const Login: React.FC = () => {
  const [apiError, setApiError] = useState<string>('');
  const [isLoading, setIsLoading] = useState(false);

  const { login } = useAuth();
  const navigate = useNavigate();

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
    mode: 'onBlur', // Validar cuando el usuario sale del campo
  });

  const onSubmit = async (data: LoginFormData) => {
    setApiError('');
    setIsLoading(true);

    try {
      await login(data);
      navigate('/products');
    } catch (err: any) {
      setApiError(err.message || 'Error al iniciar sesión');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="flex justify-center items-center min-h-screen bg-gradient-to-br from-[#667eea] to-[#764ba2]">
      <div className="bg-white p-8 rounded-xl shadow-2xl w-full max-w-md">
        <h1 className="text-center text-[#667eea] mb-2 text-3xl font-bold">ServerCloudStore</h1>
        <h2 className="text-center text-gray-900 mb-6 text-2xl">Iniciar Sesión</h2>
        
        {/* Error del API */}
        {apiError && (
          <div className="bg-red-50 text-red-700 px-4 py-3 rounded-md mb-4 border border-red-200 flex items-start">
            <svg className="w-5 h-5 mr-2 mt-0.5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
              <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
            </svg>
            <span>{apiError}</span>
          </div>
        )}

        <form onSubmit={handleSubmit(onSubmit)} noValidate>
          {/* Campo Usuario */}
          <div className="mb-6">
            <label htmlFor="username" className="block mb-2 text-gray-700 font-medium">
              Usuario
            </label>
            <input
              type="text"
              id="username"
              {...register('username')}
              disabled={isLoading || isSubmitting}
              placeholder="admin"
              className={`w-full px-3 py-3 border rounded-md text-base transition-colors focus:outline-none disabled:bg-gray-100 disabled:cursor-not-allowed ${
                errors.username 
                  ? 'border-red-500 focus:border-red-600' 
                  : 'border-gray-300 focus:border-[#667eea]'
              }`}
              aria-invalid={errors.username ? 'true' : 'false'}
              aria-describedby={errors.username ? 'username-error' : undefined}
            />
            {errors.username && (
              <p id="username-error" className="mt-1 text-sm text-red-600 flex items-center">
                <svg className="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                  <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
                </svg>
                {errors.username.message}
              </p>
            )}
          </div>

          {/* Campo Contraseña */}
          <div className="mb-6">
            <label htmlFor="password" className="block mb-2 text-gray-700 font-medium">
              Contraseña
            </label>
            <input
              type="password"
              id="password"
              {...register('password')}
              disabled={isLoading || isSubmitting}
              placeholder="••••••••"
              className={`w-full px-3 py-3 border rounded-md text-base transition-colors focus:outline-none disabled:bg-gray-100 disabled:cursor-not-allowed ${
                errors.password 
                  ? 'border-red-500 focus:border-red-600' 
                  : 'border-gray-300 focus:border-[#667eea]'
              }`}
              aria-invalid={errors.password ? 'true' : 'false'}
              aria-describedby={errors.password ? 'password-error' : undefined}
            />
            {errors.password && (
              <p id="password-error" className="mt-1 text-sm text-red-600 flex items-center">
                <svg className="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                  <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
                </svg>
                {errors.password.message}
              </p>
            )}
          </div>

          <button 
            type="submit" 
            className="w-full px-3 py-3 bg-gradient-to-r from-[#667eea] to-[#764ba2] text-white border-none rounded-md text-base font-semibold cursor-pointer transition-all hover:-translate-y-0.5 hover:shadow-lg active:translate-y-0 disabled:opacity-60 disabled:cursor-not-allowed disabled:hover:translate-y-0 disabled:hover:shadow-none"
            disabled={isLoading || isSubmitting}
          >
            {isLoading || isSubmitting ? (
              <span className="flex items-center justify-center">
                <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Iniciando sesión...
              </span>
            ) : (
              'Iniciar Sesión'
            )}
          </button>
        </form>

        <div className="mt-6 p-4 bg-gray-50 rounded-md text-sm">
          <p className="my-1 text-gray-600">
            <strong className="text-gray-900">Usuario de prueba:</strong>
          </p>
          <p className="my-1 text-gray-600">Usuario: admin</p>
          <p className="my-1 text-gray-600">Contraseña: admin123</p>
        </div>
      </div>
    </div>
  );
};
