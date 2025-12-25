import React from 'react';
import { Outlet, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export const Layout: React.FC = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="min-h-screen flex flex-col">
      <nav className="bg-gradient-to-r from-[#667eea] to-[#764ba2] text-white shadow-lg sticky top-0 z-50">
        <div className="max-w-7xl mx-auto px-8 py-4 flex justify-between items-center gap-8 flex-wrap">
          <div 
            className="cursor-pointer transition-opacity hover:opacity-90"
            onClick={() => navigate('/products')}
          >
            <h2 className="text-2xl font-bold m-0">ServerCloudStore</h2>
          </div>

          <div className="flex gap-4 flex-1">
            <button 
              className="bg-transparent border-none text-white px-4 py-2 rounded-md cursor-pointer text-base font-medium transition-colors hover:bg-white/20"
              onClick={() => navigate('/products')}
            >
              📦 Productos
            </button>
          </div>

          <div className="flex items-center gap-4">
            <span className="font-medium">
              👤 {user?.username} ({user?.roleName})
            </span>
            <button 
              className="bg-white/20 border border-white/30 text-white px-4 py-2 rounded-md cursor-pointer font-medium transition-all hover:bg-white/30 hover:-translate-y-0.5"
              onClick={handleLogout}
            >
              Cerrar Sesión
            </button>
          </div>
        </div>
      </nav>

      <main className="flex-1 bg-gray-50">
        <Outlet />
      </main>

      <footer className="bg-gray-800 text-white text-center py-4">
        <p className="m-0">&copy; 2024 ServerCloudStore - Gestión de Productos</p>
      </footer>
    </div>
  );
};
