import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { productService } from '../Services/productService';
import { cartService } from '../Services/cartService';
import OrderHistory from './OrderHistory';
import CustomerProfileManagement from './CustomerProfileManagement';
import type { Product, Category } from '../types/product';

const CustomerDashboard: React.FC = () => {
  const navigate = useNavigate();
  const [products, setProducts] = useState<Product[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [filteredProducts, setFilteredProducts] = useState<Product[]>([]);
  const [selectedCategory, setSelectedCategory] = useState<number | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [priceRange, setPriceRange] = useState({ min: '', max: '' });
  const [sortBy, setSortBy] = useState('featured');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [addingToCart, setAddingToCart] = useState<Set<number>>(new Set());
  const [activeTab, setActiveTab] = useState('products');

  useEffect(() => {
    loadData();
  }, []);

  useEffect(() => {
    filterProducts();
  }, [products, selectedCategory, searchTerm, priceRange, sortBy]);

  const loadData = async () => {
    try {
      setLoading(true);
      const [productsData, categoriesData] = await Promise.all([
        productService.getAllProducts(),
        productService.getCategories()
      ]);
      setProducts(productsData);
      setCategories(categoriesData);
    } catch (err: any) {
      setError('Failed to load products');
      console.error('Error loading data:', err);
    } finally {
      setLoading(false);
    }
  };

  const filterProducts = () => {
    let filtered = [...products];

    // Filter by category (include subcategories)
    if (selectedCategory) {
      const selectedCategoryObj = categories.find(c => c.categoryId === selectedCategory);
      const categoryIds = [selectedCategory];
      
      // If it's a parent category, include all its subcategories
      if (selectedCategoryObj && !selectedCategoryObj.parentCategoryId) {
        const subcategories = categories.filter(c => c.parentCategoryId === selectedCategory);
        categoryIds.push(...subcategories.map(c => c.categoryId));
      }
      
      filtered = filtered.filter(product => categoryIds.includes(product.categoryId));
    }

    // Filter by search term
    if (searchTerm) {
      filtered = filtered.filter(product =>
        product.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        product.description?.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    // Filter by price range
    if (priceRange.min) {
      filtered = filtered.filter(product => product.price >= parseFloat(priceRange.min));
    }
    if (priceRange.max) {
      filtered = filtered.filter(product => product.price <= parseFloat(priceRange.max));
    }

    // Sort products
    const sorted = [...filtered].sort((a, b) => {
      switch (sortBy) {
        case 'price-low':
          return a.price - b.price;
        case 'price-high':
          return b.price - a.price;
        case 'name':
          return a.name.localeCompare(b.name);
        case 'newest':
          return new Date(b.createdOn).getTime() - new Date(a.createdOn).getTime();
        default:
          return 0;
      }
    });

    setFilteredProducts(sorted);
  };

  const handleCategoryFilter = (categoryId: number | null) => {
    setSelectedCategory(categoryId);
  };

  const clearFilters = () => {
    setSelectedCategory(null);
    setSearchTerm('');
    setPriceRange({ min: '', max: '' });
    setSortBy('featured');
  };

  const handleAddToCart = async (productId: number) => {
    try {
      setAddingToCart(prev => new Set(prev).add(productId));
      await cartService.addToCart({ productId, quantity: 1 });
      
      // Show success message
      const alertDiv = document.createElement('div');
      alertDiv.className = 'alert alert-success alert-dismissible fade show position-fixed';
      alertDiv.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
      alertDiv.innerHTML = `
        <strong>Success!</strong> Item added to cart.
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
      `;
      document.body.appendChild(alertDiv);
      
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
      setAddingToCart(prev => {
        const newSet = new Set(prev);
        newSet.delete(productId);
        return newSet;
      });
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

  return (
    <div className="container-fluid mt-3">
      {error && (
        <div className="alert alert-danger" role="alert">
          {error}
        </div>
      )}

      {/* Navigation Tabs */}
      <div className="row mb-4">
        <div className="col-12">
          <ul className="nav nav-tabs">
            <li className="nav-item">
              <button 
                className={`nav-link ${activeTab === 'products' ? 'active' : ''}`}
                onClick={() => setActiveTab('products')}
              >
                <i className="bi bi-shop"></i> Products
              </button>
            </li>
            <li className="nav-item">
              <button 
                className={`nav-link ${activeTab === 'orders' ? 'active' : ''}`}
                onClick={() => setActiveTab('orders')}
              >
                <i className="bi bi-box-seam"></i> Order History
              </button>
            </li>
            <li className="nav-item">
              <button 
                className={`nav-link ${activeTab === 'profile' ? 'active' : ''}`}
                onClick={() => setActiveTab('profile')}
              >
                <i className="bi bi-person-circle"></i> My Profile
              </button>
            </li>
          </ul>
        </div>
      </div>

      {/* Tab Content */}
      {activeTab === 'orders' ? (
        <OrderHistory />
      ) : activeTab === 'profile' ? (
        <CustomerProfileManagement />
      ) : (
        <div className="row">
          {/* Sidebar Filters */}
          <div className="col-md-3">
            <div className="card">
              <div className="card-header">
                <h5 className="mb-0">Filters</h5>
              </div>
              <div className="card-body">
                {/* Search */}
                <div className="mb-3">
                  <label className="form-label">Search Products</label>
                  <input
                    type="text"
                    className="form-control"
                    placeholder="Search products..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                  />
                </div>

                {/* Categories */}
                <div className="mb-3">
                  <label className="form-label">Categories</label>
                  <div className="list-group">
                    <button
                      className={`list-group-item list-group-item-action ${!selectedCategory ? 'active' : ''}`}
                      onClick={() => handleCategoryFilter(null)}
                    >
                      All Categories
                    </button>
                    {categories.map(category => (
                      <button
                        key={category.categoryId}
                        className={`list-group-item list-group-item-action ${selectedCategory === category.categoryId ? 'active' : ''}`}
                        onClick={() => handleCategoryFilter(category.categoryId)}
                      >
                        {category.name}
                      </button>
                    ))}
                  </div>
                </div>

                {/* Price Range */}
                <div className="mb-3">
                  <label className="form-label">Price Range</label>
                  <div className="row">
                    <div className="col-6">
                      <input
                        type="number"
                        className="form-control"
                        placeholder="Min"
                        value={priceRange.min}
                        onChange={(e) => setPriceRange(prev => ({ ...prev, min: e.target.value }))}
                      />
                    </div>
                    <div className="col-6">
                      <input
                        type="number"
                        className="form-control"
                        placeholder="Max"
                        value={priceRange.max}
                        onChange={(e) => setPriceRange(prev => ({ ...prev, max: e.target.value }))}
                      />
                    </div>
                  </div>
                </div>

                <button className="btn btn-outline-secondary w-100" onClick={clearFilters}>
                  Clear Filters
                </button>
              </div>
            </div>
          </div>

          {/* Products Grid */}
          <div className="col-md-9">
            <div className="d-flex justify-content-between align-items-center mb-3">
              <h4>Products ({filteredProducts.length})</h4>
              <div className="d-flex align-items-center">
                <span className="me-2">Sort by:</span>
                <select 
                  className="form-select" 
                  style={{ width: 'auto' }}
                  value={sortBy}
                  onChange={(e) => setSortBy(e.target.value)}
                >
                  <option value="featured">Featured</option>
                  <option value="price-low">Price: Low to High</option>
                  <option value="price-high">Price: High to Low</option>
                  <option value="name">Name A-Z</option>
                  <option value="newest">Newest</option>
                </select>
              </div>
            </div>

            {filteredProducts.length === 0 ? (
              <div className="text-center py-5">
                <h5>No products found</h5>
                <p className="text-muted">Try adjusting your filters or search terms</p>
              </div>
            ) : (
              <div className="row">
                {filteredProducts.map(product => (
                  <div key={product.productId} className="col-lg-4 col-md-6 mb-4">
                    <div className="card h-100 product-card">
                      <div className="card-img-top-container" style={{ height: '200px', overflow: 'hidden' }}>
                        <img
                          src={product.imagePaths?.[0] ? `http://localhost:5108${product.imagePaths[0]}` : 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMzAwIiBoZWlnaHQ9IjIwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMTAwJSIgaGVpZ2h0PSIxMDAlIiBmaWxsPSIjZGRkIi8+PHRleHQgeD0iNTAlIiB5PSI1MCUiIGZvbnQtc2l6ZT0iMTgiIHRleHQtYW5jaG9yPSJtaWRkbGUiIGR5PSIuM2VtIiBmaWxsPSIjOTk5Ij5ObyBJbWFnZTwvdGV4dD48L3N2Zz4='}
                          className="card-img-top"
                          alt={product.name}
                          style={{ width: '100%', height: '100%', objectFit: 'cover' }}
                          onError={(e) => {
                            (e.target as HTMLImageElement).src = 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMzAwIiBoZWlnaHQ9IjIwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMTAwJSIgaGVpZ2h0PSIxMDAlIiBmaWxsPSIjZGRkIi8+PHRleHQgeD0iNTAlIiB5PSI1MCUiIGZvbnQtc2l6ZT0iMTgiIHRleHQtYW5jaG9yPSJtaWRkbGUiIGR5PSIuM2VtIiBmaWxsPSIjOTk5Ij5ObyBJbWFnZTwvdGV4dD48L3N2Zz4=';
                          }}
                        />
                      </div>
                      <div className="card-body d-flex flex-column">
                        <h6 className="card-title">{product.name}</h6>
                        <p className="card-text text-muted small flex-grow-1">
                          {product.description?.substring(0, 100)}...
                        </p>
                        <div className="mt-auto">
                          <div className="d-flex justify-content-between align-items-center mb-2">
                            <div>
                              <span className="h5 text-primary">₹{product.price}</span>
                              {product.discountPrice && (
                                <span className="text-muted text-decoration-line-through ms-2">
                                  ₹{product.discountPrice}
                                </span>
                              )}
                            </div>
                            <small className="text-muted">Stock: {product.stockQuantity}</small>
                          </div>
                          <div className="d-grid gap-2">
                            <button 
                              className="btn btn-warning btn-sm"
                              onClick={() => handleAddToCart(product.productId)}
                              disabled={addingToCart.has(product.productId) || product.stockQuantity === 0}
                            >
                              {addingToCart.has(product.productId) ? (
                                <div className="spinner-border spinner-border-sm me-1" role="status"></div>
                              ) : (
                                '🛒 '
                              )}
                              {product.stockQuantity === 0 ? 'Out of Stock' : 'Add to Cart'}
                            </button>
                            <button 
                              className="btn btn-outline-primary btn-sm"
                              onClick={() => navigate(`/product/${product.productId}`)}
                            >
                              View Details
                            </button>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      )}
      
      <style>{`
        .product-card {
          transition: transform 0.2s, box-shadow 0.2s;
        }
        .product-card:hover {
          transform: translateY(-2px);
          box-shadow: 0 4px 8px rgba(0,0,0,0.1);
        }
        .list-group-item {
          border: none;
          border-bottom: 1px solid #dee2e6;
        }
        .list-group-item:last-child {
          border-bottom: none;
        }
      `}</style>
    </div>
  );
};

export default CustomerDashboard;