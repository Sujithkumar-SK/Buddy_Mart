import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/authContext';
import { cartService } from '../Services/cartService';

const Navbar: React.FC = () => {
  const { isAuthenticated, user, logout } = useAuth();
  const navigate = useNavigate();
  const [cartItemCount, setCartItemCount] = useState(0);

  useEffect(() => {
    if (isAuthenticated && user?.role === 'Customer') {
      loadCartCount();
    }
  }, [isAuthenticated, user]);

  const loadCartCount = async () => {
    try {
      const cart = await cartService.getCart();
      setCartItemCount(cart.totalItems);
    } catch (error) {
      console.error('Error loading cart count:', error);
    }
  };

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-primary">
      <div className="container-fluid">
        <button 
          className="navbar-brand btn btn-link text-white text-decoration-none p-0"
          onClick={() => navigate('/')}
        >
          E-Commerce
        </button>
        
        <button 
          className="navbar-toggler" 
          type="button" 
          data-bs-toggle="collapse" 
          data-bs-target="#navbarNav"
        >
          <span className="navbar-toggler-icon"></span>
        </button>
        
        <div className="collapse navbar-collapse" id="navbarNav">
          <div className="navbar-nav ms-auto">
            {isAuthenticated ? (
              <>
                <span className="navbar-text me-3">
                  Welcome, {user?.email}
                </span>
                {user?.role === 'Customer' && (
                  <>
                    <button 
                      className="btn btn-outline-light me-2"
                      onClick={() => navigate('/customer-dashboard')}
                    >
                      Home
                    </button>
                    <button 
                      className="btn btn-outline-light me-2"
                      onClick={() => navigate('/order-history')}
                    >
                      Orders
                    </button>
                    <button 
                      className="btn btn-outline-light me-2 position-relative"
                      onClick={() => navigate('/cart')}
                    >
                      Cart
                      {cartItemCount > 0 && (
                        <span className="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger">
                          {cartItemCount}
                        </span>
                      )}
                    </button>
                  </>
                )}
                <button className="btn btn-outline-light" onClick={handleLogout}>
                  Logout
                </button>
              </>
            ) : (
              <>
                <button className="btn btn-outline-light me-2" onClick={() => navigate('/login')}>
                  Login
                </button>
                <button className="btn btn-light" onClick={() => navigate('/register')}>
                  Register
                </button>
              </>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;
