import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { cartService } from '../Services/cartService';
import type { CartSummary, CartItem } from '../types/cart';

const Cart: React.FC = () => {
  const navigate = useNavigate();
  const [cart, setCart] = useState<CartSummary | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [updatingItems, setUpdatingItems] = useState<Set<number>>(new Set());

  useEffect(() => {
    loadCart();
  }, []);

  const loadCart = async () => {
    try {
      setLoading(true);
      const cartData = await cartService.getCart();
      setCart(cartData);
    } catch (err: any) {
      setError('Failed to load cart');
      console.error('Error loading cart:', err);
    } finally {
      setLoading(false);
    }
  };

  const updateQuantity = async (cartId: number, newQuantity: number) => {
    if (newQuantity < 1) return;
    
    try {
      setUpdatingItems(prev => new Set(prev).add(cartId));
      await cartService.updateCartItem(cartId, { quantity: newQuantity });
      await loadCart(); // Reload cart to get updated totals
    } catch (error: any) {
      console.error('Error updating quantity:', error);
      const errorMsg = error.response?.data?.description || error.response?.data?.error?.description || 'Failed to update quantity';
      showAlert(errorMsg, 'danger');
    } finally {
      setUpdatingItems(prev => {
        const newSet = new Set(prev);
        newSet.delete(cartId);
        return newSet;
      });
    }
  };

  const removeItem = async (cartId: number) => {
    try {
      setUpdatingItems(prev => new Set(prev).add(cartId));
      await cartService.removeCartItem(cartId);
      await loadCart();
      showAlert('Item removed from cart', 'success');
    } catch (error: any) {
      console.error('Error removing item:', error);
      const errorMsg = error.response?.data?.description || error.response?.data?.error?.description || 'Failed to remove item';
      showAlert(errorMsg, 'danger');
    } finally {
      setUpdatingItems(prev => {
        const newSet = new Set(prev);
        newSet.delete(cartId);
        return newSet;
      });
    }
  };

  const clearCart = async () => {
    if (!window.confirm('Are you sure you want to clear your cart?')) return;
    
    try {
      setLoading(true);
      await cartService.clearCart();
      await loadCart();
      showAlert('Cart cleared successfully', 'success');
    } catch (error: any) {
      console.error('Error clearing cart:', error);
      const errorMsg = error.response?.data?.description || error.response?.data?.error?.description || 'Failed to clear cart';
      showAlert(errorMsg, 'danger');
    } finally {
      setLoading(false);
    }
  };

  const showAlert = (message: string, type: 'success' | 'danger') => {
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
    alertDiv.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
    alertDiv.innerHTML = `
      ${message}
      <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    document.body.appendChild(alertDiv);
    
    setTimeout(() => {
      if (alertDiv.parentNode) {
        alertDiv.parentNode.removeChild(alertDiv);
      }
    }, 3000);
  };

  if (loading) {
    return (
      <div className="container mt-4">
        <div className="d-flex justify-content-center">
          <div className="spinner-border" role="status">
            <span className="visually-hidden">Loading...</span>
          </div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mt-4">
        <div className="alert alert-danger" role="alert">
          {error}
        </div>
        <button className="btn btn-primary" onClick={() => navigate('/dashboard')}>
          Continue Shopping
        </button>
      </div>
    );
  }

  if (!cart || cart.items.length === 0) {
    return (
      <div className="cart-page">
        <div className="container py-5">
          <div className="text-center">
            <div className="empty-cart-icon mb-4">
              🛒
            </div>
            <h2 className="mb-3">Your Cart is Empty</h2>
            <p className="text-muted mb-4">Looks like you haven't added any items to your cart yet.</p>
            <button 
              className="btn btn-primary btn-lg"
              onClick={() => navigate('/dashboard')}
            >
              Start Shopping
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="cart-page">
      <div className="container py-4">
        {/* Header */}
        <div className="d-flex justify-content-between align-items-center mb-4">
          <h2 className="cart-title">🛒 Shopping Cart</h2>
          <button 
            className="btn btn-outline-secondary"
            onClick={() => navigate('/dashboard')}
          >
            Continue Shopping
          </button>
        </div>

        <div className="row g-4">
          {/* Cart Items */}
          <div className="col-lg-8">
            <div className="cart-items-section">
              <div className="d-flex justify-content-between align-items-center mb-3">
                <h5 className="mb-0">Items ({cart.totalItems})</h5>
                <button 
                  className="btn btn-outline-danger btn-sm"
                  onClick={clearCart}
                  disabled={loading}
                >
                  Clear Cart
                </button>
              </div>

              {cart.items.map((item: CartItem) => (
                <div key={item.cartId} className="cart-item-card mb-3">
                  <div className="row g-3 align-items-center">
                    {/* Product Image */}
                    <div className="col-md-2">
                      <img
                        src={item.productImage ? `http://localhost:5108${item.productImage}` : 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMTAwIiBoZWlnaHQ9IjEwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMTAwJSIgaGVpZ2h0PSIxMDAlIiBmaWxsPSIjZjhmOWZhIi8+PHRleHQgeD0iNTAlIiB5PSI1MCUiIGZvbnQtc2l6ZT0iMTIiIHRleHQtYW5jaG9yPSJtaWRkbGUiIGR5PSIuM2VtIiBmaWxsPSIjNmM3NTdkIj5JbWFnZTwvdGV4dD48L3N2Zz4='}
                        alt={item.productName}
                        className="cart-item-image"
                      />
                    </div>

                    {/* Product Details */}
                    <div className="col-md-4">
                      <h6 className="product-name mb-1">{item.productName}</h6>
                      <small className="text-muted">SKU: {item.productSKU}</small><br />
                      <small className="text-muted">Vendor: {item.vendorName}</small>
                      {item.stockQuantity < 10 && (
                        <div className="mt-1">
                          <small className="text-warning">⚠️ Only {item.stockQuantity} left in stock</small>
                        </div>
                      )}
                    </div>

                    {/* Price */}
                    <div className="col-md-2">
                      <div className="price-section">
                        <div className="current-price">₹{item.price.toLocaleString()}</div>
                        {item.discountPrice && (
                          <div className="original-price">₹{item.discountPrice.toLocaleString()}</div>
                        )}
                      </div>
                    </div>

                    {/* Quantity Controls */}
                    <div className="col-md-2">
                      <div className="quantity-controls">
                        <button 
                          className="btn btn-outline-secondary btn-sm"
                          onClick={() => updateQuantity(item.cartId, item.quantity - 1)}
                          disabled={item.quantity <= 1 || updatingItems.has(item.cartId)}
                        >
                          -
                        </button>
                        <span className="quantity-display">
                          {updatingItems.has(item.cartId) ? (
                            <div className="spinner-border spinner-border-sm" role="status"></div>
                          ) : (
                            item.quantity
                          )}
                        </span>
                        <button 
                          className="btn btn-outline-secondary btn-sm"
                          onClick={() => updateQuantity(item.cartId, item.quantity + 1)}
                          disabled={item.quantity >= item.stockQuantity || updatingItems.has(item.cartId)}
                        >
                          +
                        </button>
                      </div>
                    </div>

                    {/* Total & Remove */}
                    <div className="col-md-2">
                      <div className="text-end">
                        <div className="item-total">₹{item.totalPrice.toLocaleString()}</div>
                        <button 
                          className="btn btn-outline-danger btn-sm mt-2"
                          onClick={() => removeItem(item.cartId)}
                          disabled={updatingItems.has(item.cartId)}
                        >
                          🗑️ Remove
                        </button>
                      </div>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>

          {/* Cart Summary */}
          <div className="col-lg-4">
            <div className="cart-summary-card">
              <h5 className="summary-title">Order Summary</h5>
              
              <div className="summary-row">
                <span>Subtotal ({cart.totalItems} items)</span>
                <span>₹{cart.subTotal.toLocaleString()}</span>
              </div>
              
              {cart.totalDiscount > 0 && (
                <div className="summary-row discount">
                  <span>Discount</span>
                  <span>-₹{cart.totalDiscount.toLocaleString()}</span>
                </div>
              )}
              
              <div className="summary-row shipping">
                <span>Shipping</span>
                <span className="text-success">FREE</span>
              </div>
              
              <hr />
              
              <div className="summary-row total">
                <span>Total</span>
                <span>₹{cart.grandTotal.toLocaleString()}</span>
              </div>
              
              <button 
                className="btn btn-checkout btn-lg w-100 mt-3"
                onClick={() => navigate('/checkout')}
              >
                Proceed to Checkout
              </button>
            </div>
          </div>
        </div>
      </div>

      <style>{`
        .cart-page {
          background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
          min-height: 100vh;
        }
        
        .cart-title {
          color: #2c3e50;
          font-weight: 700;
        }
        
        .empty-cart-icon {
          font-size: 4rem;
          opacity: 0.5;
        }
        
        .cart-items-section {
          background: white;
          border-radius: 15px;
          padding: 20px;
          box-shadow: 0 5px 15px rgba(0,0,0,0.1);
        }
        
        .cart-item-card {
          background: #f8f9fa;
          border-radius: 10px;
          padding: 15px;
          border: 1px solid #e9ecef;
          transition: all 0.3s ease;
        }
        
        .cart-item-card:hover {
          box-shadow: 0 3px 10px rgba(0,0,0,0.1);
        }
        
        .cart-item-image {
          width: 80px;
          height: 80px;
          object-fit: cover;
          border-radius: 8px;
        }
        
        .product-name {
          color: #2c3e50;
          font-weight: 600;
        }
        
        .price-section .current-price {
          font-weight: 600;
          color: #2c3e50;
        }
        
        .price-section .original-price {
          font-size: 0.9rem;
          text-decoration: line-through;
          color: #6c757d;
        }
        
        .quantity-controls {
          display: flex;
          align-items: center;
          gap: 10px;
        }
        
        .quantity-controls button {
          width: 30px;
          height: 30px;
          border-radius: 50%;
          display: flex;
          align-items: center;
          justify-content: center;
          padding: 0;
        }
        
        .quantity-display {
          min-width: 30px;
          text-align: center;
          font-weight: 600;
        }
        
        .item-total {
          font-weight: 700;
          color: #2c3e50;
          font-size: 1.1rem;
        }
        
        .cart-summary-card {
          background: white;
          border-radius: 15px;
          padding: 25px;
          box-shadow: 0 5px 15px rgba(0,0,0,0.1);
          position: sticky;
          top: 20px;
        }
        
        .summary-title {
          color: #2c3e50;
          font-weight: 600;
          margin-bottom: 20px;
        }
        
        .summary-row {
          display: flex;
          justify-content: space-between;
          margin-bottom: 10px;
        }
        
        .summary-row.discount {
          color: #28a745;
        }
        
        .summary-row.shipping {
          color: #6c757d;
        }
        
        .summary-row.total {
          font-size: 1.2rem;
          font-weight: 700;
          color: #2c3e50;
        }
        
        .btn-checkout {
          background: linear-gradient(135deg, #28a745 0%, #20c997 100%);
          border: none;
          color: white;
          font-weight: 600;
          border-radius: 10px;
          transition: all 0.3s ease;
        }
        
        .btn-checkout:hover {
          transform: translateY(-2px);
          box-shadow: 0 5px 15px rgba(40,167,69,0.4);
          color: white;
        }
        
        @media (max-width: 768px) {
          .cart-item-card .row > div {
            margin-bottom: 10px;
          }
          
          .quantity-controls {
            justify-content: center;
          }
        }
      `}</style>
    </div>
  );
};

export default Cart;