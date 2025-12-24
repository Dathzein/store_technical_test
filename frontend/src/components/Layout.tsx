import React from 'react';
import { Outlet, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import './Layout.css';

export const Layout: React.FC = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="layout">
      <nav className="navbar">
        <div className="navbar-container">
          <div className="navbar-brand" onClick={() => navigate('/products')}>
            <h2>ServerCloudStore</h2>
          </div>

          <div className="navbar-menu">
            <button className="nav-link" onClick={() => navigate('/products')}>
              📦 Productos
            </button>
          </div>

          <div className="navbar-user">
            <span className="user-info">
              👤 {user?.username} ({user?.roleName})
            </span>
            <button className="btn-logout" onClick={handleLogout}>
              Cerrar Sesión
            </button>
          </div>
        </div>
      </nav>

      <main className="main-content">
        <Outlet />
      </main>

      <footer className="footer">
        <p>&copy; 2024 ServerCloudStore - Gestión de Productos</p>
      </footer>
    </div>
  );
};

