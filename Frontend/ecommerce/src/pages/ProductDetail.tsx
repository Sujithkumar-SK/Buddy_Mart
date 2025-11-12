import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { productService } from '../Services/productService';
import { cartService } from '../Services/cartService';
import type { Product } from '../types/product';

const ProductDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [product, setProduct] = useState<Product | null>(null);
  const [selectedImageIndex, setSelectedImageIndex] = useState(0);
  const [quantity, setQuantity] = useState(1);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    if (id) {
      loadProduct(parseInt(id));
    }
  }, [id]);

  const loadProduct = async (productId: number) => {
    try {
      setLoading(true);
      const productData = await productService.getProductById(productId);
      setProduct(productData);
    } catch (err: any) {
      setError('Failed to load product details');
      console.error('Error loading product:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleAddToCart = async () => {
    if (!product) return;
    
    try {
      setLoading(true);
      await cartService.addToCart({
        productId: product.productId,
        quantity: quantity
      });
      
      // Show success message
      const alertDiv = document.createElement('div');
      alertDiv.className = 'alert alert-success alert-dismissible fade show position-fixed';
      alertDiv.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
      alertDiv.innerHTML = `
        <strong>Success!</strong> ${quantity} ${product.name} added to cart.
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
      `;
      document.body.appendChild(alertDiv);
      
      // Auto remove after 3 seconds
      setTimeout(() => {
        if (alertDiv.parentNode) {
          alertDiv.parentNode.removeChild(alertDiv);
        }
      }, 3000);
      
    } catch (error: any) {
      console.error('Error adding to cart:', error);
      const errorMsg = error.response?.data?.description || error.response?.data?.error?.description || 'Failed to add item to cart';
      
      const alertDiv = document.createElement('div');
      alertDiv.className = 'alert alert-warning alert-dismissible fade show position-fixed';
      alertDiv.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
      alertDiv.innerHTML = `
        <strong>Info!</strong> ${errorMsg}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
      `;
      document.body.appendChild(alertDiv);
      
      setTimeout(() => {
        if (alertDiv.parentNode) {
          alertDiv.parentNode.removeChild(alertDiv);
        }
      }, 5000);
    } finally {
      setLoading(false);
    }
  };

  const handleBuyNow = async () => {
    if (!product) return;
    
    try {
      await handleAddToCart();
      // Navigate to cart page after adding to cart
      navigate('/cart');
    } catch (error) {
      console.error('Error in buy now:', error);
    }
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

  if (error || !product) {
    return (
      <div className="container mt-4">
        <div className="alert alert-danger" role="alert">
          {error || 'Product not found'}
        </div>
        <button className="btn btn-primary" onClick={() => navigate('/dashboard')}>
          Back to Products
        </button>
      </div>
    );
  }

  const images = product.imagePaths?.length > 0 
    ? product.imagePaths.map(path => `http://localhost:5108${path}`) 
    : ['data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNTAwIiBoZWlnaHQ9IjUwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMTAwJSIgaGVpZ2h0PSIxMDAlIiBmaWxsPSIjZjhmOWZhIi8+PHRleHQgeD0iNTAlIiB5PSI1MCUiIGZvbnQtc2l6ZT0iMjQiIHRleHQtYW5jaG9yPSJtaWRkbGUiIGR5PSIuM2VtIiBmaWxsPSIjNmM3NTdkIj5ObyBJbWFnZTwvdGV4dD48L3N2Zz4='];

  return (
    <div className="product-detail-page">
      <div className="container-fluid px-3 px-md-4 py-4">
        <div className="container">
          {/* Enhanced Breadcrumb */}
          <nav aria-label="breadcrumb" className="mb-4">
            <ol className="breadcrumb bg-light p-3 rounded shadow-sm">
              <li className="breadcrumb-item">
                <button className="btn btn-link p-0 text-decoration-none fw-semibold" onClick={() => navigate('/dashboard')}>
                  🏠 Products
                </button>
              </li>
              <li className="breadcrumb-item text-muted">{product.categoryName}</li>
              <li className="breadcrumb-item active fw-bold" aria-current="page">{product.name}</li>
            </ol>
          </nav>

          <div className="row g-5">
            {/* Enhanced Product Images */}
            <div className="col-12 col-lg-6">
              <div className="product-gallery">
                <div className="main-image-container mb-3">
                  <img
                    src={images[selectedImageIndex]}
                    className="main-product-image"
                    alt={product.name}
                    onError={(e) => {
                      (e.target as HTMLImageElement).src = 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNTAwIiBoZWlnaHQ9IjUwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMTAwJSIgaGVpZ2h0PSIxMDAlIiBmaWxsPSIjZjhmOWZhIi8+PHRleHQgeD0iNTAlIiB5PSI1MCUiIGZvbnQtc2l6ZT0iMjQiIHRleHQtYW5jaG9yPSJtaWRkbGUiIGR5PSIuM2VtIiBmaWxsPSIjNmM3NTdkIj5ObyBJbWFnZTwvdGV4dD48L3N2Zz4=';
                    }}
                  />
                </div>
                
                {/* Thumbnail Images */}
                {images.length > 1 && (
                  <div className="thumbnail-container">
                    <div className="d-flex gap-2 flex-wrap">
                      {images.map((image, index) => (
                        <img
                          key={index}
                          src={image}
                          className={`thumbnail-image ${selectedImageIndex === index ? 'active' : ''}`}
                          alt={`${product.name} ${index + 1}`}
                          onClick={() => setSelectedImageIndex(index)}
                          onError={(e) => {
                            (e.target as HTMLImageElement).src = 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMTAwIiBoZWlnaHQ9IjEwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMTAwJSIgaGVpZ2h0PSIxMDAlIiBmaWxsPSIjZjhmOWZhIi8+PC9zdmc+';
                          }}
                        />
                      ))}
                    </div>
                  </div>
                )}
              </div>
            </div>

            {/* Enhanced Product Details */}
            <div className="col-12 col-lg-6">
              <div className="product-info">
                <h1 className="product-title mb-3">{product.name}</h1>
                
                <div className="product-badges mb-4">
                  <span className="badge bg-primary me-2 px-3 py-2">
                  {product.categoryName}
                  </span>
                  <span className="badge bg-secondary px-3 py-2">
                  {product.brand || 'No Brand'}
                  </span>
                </div>

                <div className="price-card mb-4">
                  <div className="d-flex align-items-center gap-3 mb-2">
                    <span className="current-price">₹{product.price.toLocaleString()}</span>
                    {product.discountPrice && (
                      <>
                        <span className="original-price">₹{product.discountPrice.toLocaleString()}</span>
                        <span className="discount-badge">
                          {Math.round(((product.discountPrice - product.price) / product.discountPrice) * 100)}% OFF
                        </span>
                      </>
                    )}
                  </div>
                  {product.discountPrice && (
                    <div className="savings-text">
                      You save ₹{(product.discountPrice - product.price).toLocaleString()}
                    </div>
                  )}
                </div>

                <div className="stock-status mb-4">
                  {product.stockQuantity > 0 ? (
                    <div className="in-stock">
                      <span className="fw-bold">In Stock</span>
                      <span className="text-muted ms-2">({product.stockQuantity} available)</span>
                    </div>
                  ) : (
                    <div className="out-of-stock">
                      <span className="fw-bold">Out of Stock</span>
                    </div>
                  )}
                </div>

                {product.description && (
                  <div className="description-card mb-4">
                    <h5 className="section-title">Description</h5>
                    <p className="description-text">{product.description}</p>
                  </div>
                )}

                <div className="specifications-card mb-4">
                  <h5 className="section-title"></h5>
                  <div className="spec-grid">
                    <div className="spec-item">
                      <span className="spec-label">SKU:</span>
                      <span className="spec-value">{product.sku}</span>
                    </div>
                    <div className="spec-item">
                      <span className="spec-label">Brand:</span>
                      <span className="spec-value">{product.brand || 'N/A'}</span>
                    </div>
                    {product.weight && (
                      <div className="spec-item">
                        <span className="spec-label">Weight:</span>
                        <span className="spec-value">{product.weight}</span>
                      </div>
                    )}
                    {product.dimensions && (
                      <div className="spec-item">
                        <span className="spec-label">Dimensions:</span>
                        <span className="spec-value">{product.dimensions}</span>
                      </div>
                    )}
                    <div className="spec-item">
                      <span className="spec-label">Vendor:</span>
                      <span className="spec-value">{product.vendorName}</span>
                    </div>
                  </div>
                </div>

                {product.stockQuantity > 0 && (
                  <div className="purchase-card">
                    <div className="quantity-selector mb-4">
                      <label className="form-label fw-bold"> Quantity</label>
                      <div className="quantity-controls">
                        <button 
                          className="btn btn-outline-secondary"
                          onClick={() => setQuantity(Math.max(1, quantity - 1))}
                          disabled={quantity <= 1}
                        >
                          ➖
                        </button>
                        <span className="quantity-display">{quantity}</span>
                        <button 
                          className="btn btn-outline-secondary"
                          onClick={() => setQuantity(Math.min(product.stockQuantity, quantity + 1))}
                          disabled={quantity >= product.stockQuantity}
                        >
                          ➕
                        </button>
                      </div>
                    </div>

                    <div className="action-buttons">
                      <button 
                        className="btn btn-cart btn-lg"
                        onClick={handleAddToCart}
                      >
                         Add to Cart
                      </button>
                      <button 
                        className="btn btn-buy btn-lg"
                        onClick={handleBuyNow}
                      >
                        Buy Now
                      </button>
                    </div>
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <style>{`
        .product-detail-page {
          background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
          min-height: 100vh;
        }
        
        .main-image-container {
          border-radius: 15px;
          overflow: hidden;
          box-shadow: 0 10px 30px rgba(0,0,0,0.1);
          background: white;
          padding: 20px;
        }
        
        .main-product-image {
          width: 100%;
          height: 500px;
          object-fit: contain;
          border-radius: 10px;
        }
        
        .thumbnail-image {
          width: 80px;
          height: 80px;
          object-fit: cover;
          border-radius: 8px;
          cursor: pointer;
          border: 2px solid transparent;
          transition: all 0.3s ease;
        }
        
        .thumbnail-image:hover {
          border-color: #0d6efd;
          transform: scale(1.05);
        }
        
        .thumbnail-image.active {
          border-color: #0d6efd;
          box-shadow: 0 0 10px rgba(13,110,253,0.3);
        }
        
        .product-info {
          background: white;
          border-radius: 15px;
          padding: 30px;
          box-shadow: 0 10px 30px rgba(0,0,0,0.1);
        }
        
        .product-title {
          font-size: 2.2rem;
          font-weight: 700;
          color: #2c3e50;
          line-height: 1.3;
        }
        
        .product-badges .badge {
          font-size: 0.9rem;
          border-radius: 20px;
        }
        
        .price-card {
          background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
          color: white;
          padding: 20px;
          border-radius: 12px;
          box-shadow: 0 5px 15px rgba(102,126,234,0.3);
        }
        
        .current-price {
          font-size: 2.5rem;
          font-weight: 800;
          color: white;
        }
        
        .original-price {
          font-size: 1.3rem;
          text-decoration: line-through;
          color: rgba(255,255,255,0.7);
        }
        
        .discount-badge {
          background: #28a745;
          color: white;
          padding: 4px 12px;
          border-radius: 15px;
          font-size: 0.8rem;
          font-weight: 600;
        }
        
        .savings-text {
          color: rgba(255,255,255,0.9);
          font-weight: 500;
        }
        
        .stock-status .in-stock {
          color: #28a745;
          font-size: 1.1rem;
        }
        
        .stock-status .out-of-stock {
          color: #dc3545;
          font-size: 1.1rem;
        }
        
        .description-card, .specifications-card, .purchase-card {
          background: #f8f9fa;
          border-radius: 12px;
          padding: 20px;
          border-left: 4px solid #0d6efd;
        }
        
        .section-title {
          color: #2c3e50;
          font-weight: 600;
          margin-bottom: 15px;
        }
        
        .description-text {
          color: #6c757d;
          line-height: 1.6;
          margin: 0;
        }
        
        .spec-grid {
          display: grid;
          gap: 10px;
        }
        
        .spec-item {
          display: flex;
          justify-content: space-between;
          padding: 8px 0;
          border-bottom: 1px solid #e9ecef;
        }
        
        .spec-item:last-child {
          border-bottom: none;
        }
        
        .spec-label {
          font-weight: 600;
          color: #495057;
        }
        
        .spec-value {
          color: #6c757d;
        }
        
        .quantity-controls {
          display: flex;
          align-items: center;
          gap: 15px;
          margin-top: 10px;
        }
        
        .quantity-controls button {
          width: 40px;
          height: 40px;
          border-radius: 50%;
          display: flex;
          align-items: center;
          justify-content: center;
        }
        
        .quantity-display {
          font-size: 1.2rem;
          font-weight: 600;
          min-width: 30px;
          text-align: center;
        }
        
        .action-buttons {
          display: grid;
          grid-template-columns: 1fr 1fr;
          gap: 15px;
        }
        
        .btn-cart {
          background: linear-gradient(135deg, #ffc107 0%, #ff8f00 100%);
          border: none;
          color: white;
          font-weight: 600;
          border-radius: 10px;
          transition: all 0.3s ease;
        }
        
        .btn-cart:hover {
          transform: translateY(-2px);
          box-shadow: 0 5px 15px rgba(255,193,7,0.4);
          color: white;
        }
        
        .btn-buy {
          background: linear-gradient(135deg, #dc3545 0%, #c82333 100%);
          border: none;
          color: white;
          font-weight: 600;
          border-radius: 10px;
          transition: all 0.3s ease;
        }
        
        .btn-buy:hover {
          transform: translateY(-2px);
          box-shadow: 0 5px 15px rgba(220,53,69,0.4);
          color: white;
        }
        
        @media (max-width: 991px) {
          .action-buttons {
            grid-template-columns: 1fr;
          }
          
          .product-title {
            font-size: 1.8rem;
          }
          
          .current-price {
            font-size: 2rem;
          }
        }
      `}</style>
    </div>
  );
};

export default ProductDetail;