import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/authContext';
import { productService } from '../Services/productService';
import type { Product, ProductCreateRequest, Category, ProductImage } from '../types/product';

const ProductManagement: React.FC = () => {
  const { user } = useAuth();
  const [products, setProducts] = useState<Product[]>([]);
  const [filteredProducts, setFilteredProducts] = useState<Product[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [showImageModal, setShowImageModal] = useState(false);
  const [selectedProductId, setSelectedProductId] = useState<number | null>(null);
  const [editingProduct, setEditingProduct] = useState<Product | null>(null);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [selectedFiles, setSelectedFiles] = useState<File[]>([]);
  const [uploading, setUploading] = useState(false);
  
  // Filter states
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedCategory, setSelectedCategory] = useState<number | null>(null);
  const [selectedStatus, setSelectedStatus] = useState<string>('');
  const [priceRange, setPriceRange] = useState({ min: '', max: '' });
  const [sortBy, setSortBy] = useState('newest');

  const [formData, setFormData] = useState<ProductCreateRequest>({
    name: '',
    description: '',
    sku: '',
    price: 0,
    discountPrice: 0,
    stockQuantity: 0,
    minStockLevel: 0,
    brand: '',
    weight: '',
    dimensions: '',
    vendorId: user?.userId || 0,
    categoryId: 0
  });

  useEffect(() => {
    loadData();
  }, []);
  
  useEffect(() => {
    filterProducts();
  }, [products, searchTerm, selectedCategory, selectedStatus, priceRange, sortBy]);

  const loadData = async () => {
    try {
      setLoading(true);
      const [productsData, categoriesData] = await Promise.all([
        productService.getProductsByVendor(user?.userId || 0),
        productService.getCategories()
      ]);
      setProducts(productsData);
      setCategories(categoriesData);
    } catch (err: any) {
      setError('Failed to load data');
    } finally {
      setLoading(false);
    }
  };
  
  const filterProducts = () => {
    let filtered = [...products];
    
    // Search filter
    if (searchTerm) {
      filtered = filtered.filter(product =>
        product.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        product.sku.toLowerCase().includes(searchTerm.toLowerCase()) ||
        product.description?.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }
    
    // Category filter
    if (selectedCategory) {
      filtered = filtered.filter(product => product.categoryId === selectedCategory);
    }
    
    // Status filter
    if (selectedStatus) {
      filtered = filtered.filter(product => product.status === selectedStatus);
    }
    
    // Price range filter
    if (priceRange.min) {
      filtered = filtered.filter(product => product.price >= parseFloat(priceRange.min));
    }
    if (priceRange.max) {
      filtered = filtered.filter(product => product.price <= parseFloat(priceRange.max));
    }
    
    // Sort products
    filtered.sort((a, b) => {
      switch (sortBy) {
        case 'name':
          return a.name.localeCompare(b.name);
        case 'price-low':
          return a.price - b.price;
        case 'price-high':
          return b.price - a.price;
        case 'stock':
          return b.stockQuantity - a.stockQuantity;
        case 'newest':
        default:
          return new Date(b.createdOn).getTime() - new Date(a.createdOn).getTime();
      }
    });
    
    setFilteredProducts(filtered);
  };
  
  const clearFilters = () => {
    setSearchTerm('');
    setSelectedCategory(null);
    setSelectedStatus('');
    setPriceRange({ min: '', max: '' });
    setSortBy('newest');
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (editingProduct) {
        const updated = await productService.updateProduct(editingProduct.productId, formData);
        setProducts(products.map(p => p.productId === updated.productId ? updated : p));
        setSuccess('Product updated successfully');
      } else {
        const newProduct = await productService.createProduct(formData);
        setProducts([...products, newProduct]);
        setSuccess('Product created successfully');
      }
      handleCloseModal();
    } catch (err: any) {
      setError(err.response?.data?.description || 'Operation failed');
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('Are you sure you want to delete this product?')) {
      try {
        await productService.deleteProduct(id);
        setProducts(products.filter(p => p.productId !== id));
        setSuccess('Product deleted successfully');
      } catch (err: any) {
        setError('Failed to delete product');
      }
    }
  };

  const handleStatusToggle = async (product: Product) => {
    try {
      const newStatus = product.status === 'Active' ? 'Inactive' : 'Active';
      await productService.updateProductStatus(product.productId, newStatus);
      setProducts(products.map(p => 
        p.productId === product.productId ? { ...p, status: newStatus } : p
      ));
      setSuccess(`Product ${newStatus.toLowerCase()} successfully`);
    } catch (err: any) {
      setError('Failed to update product status');
    }
  };

  const handleOpenModal = (product?: Product) => {
    if (product) {
      setEditingProduct(product);
      setFormData({
        name: product.name,
        description: product.description || '',
        sku: product.sku,
        price: product.price,
        discountPrice: product.discountPrice || 0,
        stockQuantity: product.stockQuantity,
        minStockLevel: product.minStockLevel || 0,
        brand: product.brand || '',
        weight: product.weight || '',
        dimensions: product.dimensions || '',
        vendorId: product.vendorId,
        categoryId: product.categoryId
      });
    } else {
      setEditingProduct(null);
      setFormData({
        name: '',
        description: '',
        sku: '',
        price: 0,
        discountPrice: 0,
        stockQuantity: 0,
        minStockLevel: 0,
        brand: '',
        weight: '',
        dimensions: '',
        vendorId: user?.userId || 0,
        categoryId: 0
      });
    }
    setShowModal(true);
  };

  const handleCloseModal = () => {
    setShowModal(false);
    setEditingProduct(null);
    setError('');
  };

  const handleImageUpload = async () => {
    if (!selectedProductId || selectedFiles.length === 0) return;
    
    try {
      setUploading(true);
      await productService.uploadProductImages(selectedProductId, selectedFiles);
      setSuccess('Images uploaded successfully');
      setSelectedFiles([]);
      setShowImageModal(false);
      loadData(); // Reload to get updated product with images
    } catch (err: any) {
      setError('Failed to upload images');
    } finally {
      setUploading(false);
    }
  };

  const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = Array.from(e.target.files || []);
    const validFiles = files.filter(file => {
      const isValidType = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif'].includes(file.type);
      const isValidSize = file.size <= 5 * 1024 * 1024;
      return isValidType && isValidSize;
    });
    setSelectedFiles(validFiles);
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
    <div>
      {/* Header */}
      <div className="bg-white border-bottom mb-4">
        <div className="container-fluid">
          <div className="row align-items-center py-3">
            <div className="col-md-6">
              <h3 className="mb-0">Product Management</h3>
              <small className="text-muted">{filteredProducts.length} of {products.length} products</small>
            </div>
            <div className="col-md-6 text-md-end">
              <button className="btn btn-primary" onClick={() => handleOpenModal()}>
                <i className="bi bi-plus-circle me-2"></i>Add New Product
              </button>
            </div>
          </div>
        </div>
      </div>

      {error && (
        <div className="alert alert-danger alert-dismissible fade show" role="alert">
          {error}
          <button type="button" className="btn-close" onClick={() => setError('')}></button>
        </div>
      )}

      {success && (
        <div className="alert alert-success alert-dismissible fade show" role="alert">
          {success}
          <button type="button" className="btn-close" onClick={() => setSuccess('')}></button>
        </div>
      )}

      {/* Filters */}
      <div className="row mb-4">
        <div className="col-12">
          <div className="card border-0 shadow-sm">
            <div className="card-body">
              <div className="row g-3">
                {/* Search */}
                <div className="col-md-3">
                  <div className="input-group">
                    <span className="input-group-text bg-light border-end-0">
                      <i className="bi bi-search"></i>
                    </span>
                    <input
                      type="text"
                      className="form-control border-start-0"
                      placeholder="Search products..."
                      value={searchTerm}
                      onChange={(e) => setSearchTerm(e.target.value)}
                    />
                  </div>
                </div>
                
                {/* Category Filter */}
                <div className="col-md-2">
                  <select
                    className="form-select"
                    value={selectedCategory || ''}
                    onChange={(e) => setSelectedCategory(e.target.value ? parseInt(e.target.value) : null)}
                  >
                    <option value="">All Categories</option>
                    {categories.map((category) => (
                      <option key={category.categoryId} value={category.categoryId}>
                        {category.name}
                      </option>
                    ))}
                  </select>
                </div>
                
                {/* Status Filter */}
                <div className="col-md-2">
                  <select
                    className="form-select"
                    value={selectedStatus}
                    onChange={(e) => setSelectedStatus(e.target.value)}
                  >
                    <option value="">All Status</option>
                    <option value="Active">Active</option>
                    <option value="Inactive">Inactive</option>
                    <option value="Draft">Draft</option>
                  </select>
                </div>
                
                {/* Price Range */}
                <div className="col-md-2">
                  <div className="row g-1">
                    <div className="col-6">
                      <input
                        type="number"
                        className="form-control form-control-sm"
                        placeholder="Min ₹"
                        value={priceRange.min}
                        onChange={(e) => setPriceRange(prev => ({ ...prev, min: e.target.value }))}
                      />
                    </div>
                    <div className="col-6">
                      <input
                        type="number"
                        className="form-control form-control-sm"
                        placeholder="Max ₹"
                        value={priceRange.max}
                        onChange={(e) => setPriceRange(prev => ({ ...prev, max: e.target.value }))}
                      />
                    </div>
                  </div>
                </div>
                
                {/* Sort */}
                <div className="col-md-2">
                  <select
                    className="form-select"
                    value={sortBy}
                    onChange={(e) => setSortBy(e.target.value)}
                  >
                    <option value="newest">Newest First</option>
                    <option value="name">Name A-Z</option>
                    <option value="price-low">Price: Low to High</option>
                    <option value="price-high">Price: High to Low</option>
                    <option value="stock">Stock: High to Low</option>
                  </select>
                </div>
                
                {/* Clear Filters */}
                <div className="col-md-1">
                  <button
                    className="btn btn-outline-secondary w-100"
                    onClick={clearFilters}
                    title="Clear all filters"
                  >
                    <i className="bi bi-x-circle"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Products Grid */}
      <div className="row">
        {filteredProducts.length === 0 ? (
          <div className="col-12">
            <div className="card border-0 shadow-sm">
              <div className="card-body text-center py-5">
                <i className="bi bi-box-seam text-muted" style={{fontSize: '3rem'}}></i>
                <h5 className="mt-3 text-muted">No products found</h5>
                <p className="text-muted">Try adjusting your filters or add your first product</p>
                <button className="btn btn-primary" onClick={() => handleOpenModal()}>
                  <i className="bi bi-plus-circle me-2"></i>Add Product
                </button>
              </div>
            </div>
          </div>
        ) : (
          filteredProducts.map((product) => (
            <div key={product.productId} className="col-md-6 col-lg-4 mb-4">
              <div className="card h-100 border-0 shadow-sm product-card">
              {/* Image Carousel */}
              {product.imagePaths && product.imagePaths.length > 0 ? (
                <div id={`carousel-${product.productId}`} className="carousel slide" data-bs-ride="carousel">
                  <div className="carousel-inner">
                    {product.imagePaths.map((imagePath, index) => (
                      <div key={index} className={`carousel-item ${index === 0 ? 'active' : ''}`}>
                        <img
                          src={`http://localhost:5108${imagePath}`}
                          className="d-block w-100"
                          alt={product.name}
                          style={{ height: '200px', objectFit: 'cover' }}
                          onError={(e) => {
                            const target = e.target as HTMLImageElement;
                            target.src = 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMzAwIiBoZWlnaHQ9IjIwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMTAwJSIgaGVpZ2h0PSIxMDAlIiBmaWxsPSIjZGRkIi8+PHRleHQgeD0iNTAlIiB5PSI1MCUiIGZvbnQtc2l6ZT0iMTgiIHRleHQtYW5jaG9yPSJtaWRkbGUiIGR5PSIuM2VtIj5ObyBJbWFnZTwvdGV4dD48L3N2Zz4=';
                          }}
                        />
                      </div>
                    ))}
                  </div>
                  {product.imagePaths.length > 1 && (
                    <>
                      <button className="carousel-control-prev" type="button" data-bs-target={`#carousel-${product.productId}`} data-bs-slide="prev">
                        <span className="carousel-control-prev-icon"></span>
                      </button>
                      <button className="carousel-control-next" type="button" data-bs-target={`#carousel-${product.productId}`} data-bs-slide="next">
                        <span className="carousel-control-next-icon"></span>
                      </button>
                    </>
                  )}
                </div>
              ) : (
                <div className="bg-light d-flex align-items-center justify-content-center" style={{ height: '200px' }}>
                  <i className="bi bi-image fs-1 text-muted"></i>
                </div>
              )}

              <div className="card-body">
                <h5 className="card-title">{product.name}</h5>
                <p className="card-text text-muted">SKU: {product.sku}</p>
                <p className="card-text">Category: {product.categoryName}</p>
                <div className="d-flex justify-content-between align-items-center">
                  <div>
                    <span className="h5 text-primary">₹{product.price}</span>
                    {product.discountPrice && (
                      <span className="text-muted text-decoration-line-through ms-2">₹{product.discountPrice}</span>
                    )}
                  </div>
                  <span className={`badge ${product.status === 'Active' ? 'bg-success' : 'bg-secondary'}`}>
                    {product.status}
                  </span>
                </div>
                <p className="card-text mt-2">Stock: {product.stockQuantity}</p>
              </div>

                <div className="card-footer bg-light border-0">
                  <div className="btn-group w-100" role="group">
                    <button 
                      className="btn btn-primary btn-sm" 
                      onClick={() => handleOpenModal(product)}
                      title="Edit Product"
                    >
                      <i className="bi bi-pencil me-1"></i>Edit
                    </button>
                    <button 
                      className="btn btn-info btn-sm" 
                      onClick={() => {
                        setSelectedProductId(product.productId);
                        setShowImageModal(true);
                      }}
                      title="Manage Images"
                    >
                      <i className="bi bi-image me-1"></i>Images
                    </button>
                    <button 
                      className={`btn btn-sm ${product.status === 'Active' ? 'btn-warning' : 'btn-success'}`}
                      onClick={() => handleStatusToggle(product)}
                      title={product.status === 'Active' ? 'Deactivate' : 'Activate'}
                    >
                      <i className={`bi ${product.status === 'Active' ? 'bi-eye-slash' : 'bi-eye'} me-1`}></i>
                      {product.status === 'Active' ? 'Hide' : 'Show'}
                    </button>
                    <button 
                      className="btn btn-danger btn-sm" 
                      onClick={() => handleDelete(product.productId)}
                      title="Delete Product"
                    >
                      <i className="bi bi-trash me-1"></i>Delete
                    </button>
                  </div>
                </div>
              </div>
            </div>
          ))
        )}
      </div>

      {/* Product Form Modal */}
      <div className={`modal fade ${showModal ? 'show' : ''}`} style={{ display: showModal ? 'block' : 'none' }} tabIndex={-1}>
        <div className="modal-dialog modal-lg">
          <div className="modal-content">
            <div className="modal-header">
              <h5 className="modal-title">{editingProduct ? 'Edit Product' : 'Add Product'}</h5>
              <button type="button" className="btn-close" onClick={handleCloseModal}></button>
            </div>
            <form onSubmit={handleSubmit}>
              <div className="modal-body">
                <div className="row">
                  <div className="col-md-6 mb-3">
                    <label className="form-label">Product Name *</label>
                    <input
                      type="text"
                      className="form-control"
                      value={formData.name}
                      onChange={(e) => setFormData({...formData, name: e.target.value})}
                      required
                    />
                  </div>
                  <div className="col-md-6 mb-3">
                    <label className="form-label">SKU *</label>
                    <input
                      type="text"
                      className="form-control"
                      value={formData.sku}
                      onChange={(e) => setFormData({...formData, sku: e.target.value})}
                      required
                    />
                  </div>
                  <div className="col-12 mb-3">
                    <label className="form-label">Description</label>
                    <textarea
                      className="form-control"
                      rows={3}
                      value={formData.description}
                      onChange={(e) => setFormData({...formData, description: e.target.value})}
                    />
                  </div>
                  <div className="col-md-6 mb-3">
                    <label className="form-label">Category *</label>
                    <select
                      className="form-select"
                      value={formData.categoryId}
                      onChange={(e) => setFormData({...formData, categoryId: parseInt(e.target.value)})}
                      required
                    >
                      <option value={0}>Select Category</option>
                      {categories.map((category) => (
                        <option key={category.categoryId} value={category.categoryId}>
                          {category.name}
                        </option>
                      ))}
                    </select>
                  </div>
                  <div className="col-md-6 mb-3">
                    <label className="form-label">Brand</label>
                    <input
                      type="text"
                      className="form-control"
                      value={formData.brand}
                      onChange={(e) => setFormData({...formData, brand: e.target.value})}
                    />
                  </div>
                  <div className="col-md-4 mb-3">
                    <label className="form-label">Price *</label>
                    <input
                      type="number"
                      className="form-control"
                      value={formData.price}
                      onChange={(e) => setFormData({...formData, price: parseFloat(e.target.value)})}
                      required
                    />
                  </div>
                  <div className="col-md-4 mb-3">
                    <label className="form-label">Discount Price</label>
                    <input
                      type="number"
                      className="form-control"
                      value={formData.discountPrice}
                      onChange={(e) => setFormData({...formData, discountPrice: parseFloat(e.target.value)})}
                    />
                  </div>
                  <div className="col-md-4 mb-3">
                    <label className="form-label">Stock Quantity *</label>
                    <input
                      type="number"
                      className="form-control"
                      value={formData.stockQuantity}
                      onChange={(e) => setFormData({...formData, stockQuantity: parseInt(e.target.value)})}
                      required
                    />
                  </div>
                  <div className="col-md-6 mb-3">
                    <label className="form-label">Min Stock Level</label>
                    <input
                      type="number"
                      className="form-control"
                      value={formData.minStockLevel}
                      onChange={(e) => setFormData({...formData, minStockLevel: parseInt(e.target.value)})}
                      placeholder="Alert when stock falls below this level"
                    />
                  </div>
                  <div className="col-md-6 mb-3">
                    <label className="form-label">Weight</label>
                    <input
                      type="text"
                      className="form-control"
                      value={formData.weight}
                      onChange={(e) => setFormData({...formData, weight: e.target.value})}
                      placeholder="e.g., 1.5kg"
                    />
                  </div>
                  <div className="col-12 mb-3">
                    <label className="form-label">Dimensions</label>
                    <input
                      type="text"
                      className="form-control"
                      value={formData.dimensions}
                      onChange={(e) => setFormData({...formData, dimensions: e.target.value})}
                      placeholder="e.g., 30cm x 20cm x 10cm"
                    />
                  </div>
                </div>
              </div>
              <div className="modal-footer">
                <button type="button" className="btn btn-secondary" onClick={handleCloseModal}>Cancel</button>
                <button type="submit" className="btn btn-primary">
                  {editingProduct ? 'Update' : 'Create'}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>

      {/* Image Upload Modal */}
      <div className={`modal fade ${showImageModal ? 'show' : ''}`} style={{ display: showImageModal ? 'block' : 'none' }} tabIndex={-1}>
        <div className="modal-dialog">
          <div className="modal-content">
            <div className="modal-header">
              <h5 className="modal-title">Upload Product Images</h5>
              <button type="button" className="btn-close" onClick={() => setShowImageModal(false)}></button>
            </div>
            <div className="modal-body">
              <div className="mb-3">
                <label className="form-label">Select Images (JPG, PNG, GIF - Max 5MB each)</label>
                <input
                  type="file"
                  className="form-control"
                  multiple
                  accept="image/*"
                  onChange={handleFileSelect}
                />
              </div>
              {selectedFiles.length > 0 && (
                <div className="row">
                  {selectedFiles.map((file, index) => (
                    <div key={index} className="col-4 mb-2">
                      <img
                        src={URL.createObjectURL(file)}
                        alt={`Preview ${index + 1}`}
                        className="img-thumbnail"
                        style={{ width: '100%', height: '80px', objectFit: 'cover' }}
                      />
                    </div>
                  ))}
                </div>
              )}
            </div>
            <div className="modal-footer">
              <button type="button" className="btn btn-secondary" onClick={() => setShowImageModal(false)}>Cancel</button>
              <button 
                type="button" 
                className="btn btn-primary" 
                onClick={handleImageUpload}
                disabled={uploading || selectedFiles.length === 0}
              >
                {uploading ? (
                  <>
                    <span className="spinner-border spinner-border-sm me-2"></span>
                    Uploading...
                  </>
                ) : (
                  'Upload Images'
                )}
              </button>
            </div>
          </div>
        </div>
      </div>

      {showModal && <div className="modal-backdrop fade show"></div>}
      {showImageModal && <div className="modal-backdrop fade show"></div>}
      
      <style>{`
        .product-card {
          transition: all 0.2s ease;
          border: 1px solid #e9ecef;
        }
        .product-card:hover {
          transform: translateY(-2px);
          box-shadow: 0 8px 25px rgba(0,0,0,0.1) !important;
          border-color: #007bff;
        }
        .input-group-text {
          background-color: #f8f9fa;
        }
        .form-control:focus {
          border-color: #007bff;
          box-shadow: 0 0 0 0.2rem rgba(0,123,255,.25);
        }
        .card {
          border-radius: 8px;
        }
        .badge {
          font-size: 0.75em;
        }
      `}</style>
    </div>
  );
};

export default ProductManagement;