import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/authContext';
import { vendorService } from '../Services/vendorService';
import ProductManagement from './productManagement';
import VendorProfileManagement from './VendorProfileManagement';
import OrdersShippingManagement from './OrdersShippingManagement';
import type { VendorDashboard } from '../types/vendor';

const VendorDashboard: React.FC = () => {
  const { user } = useAuth();
  const [activeTab, setActiveTab] = useState('overview');
  const [dashboardData, setDashboardData] = useState<VendorDashboard | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadAnalytics();
  }, []);

  const loadAnalytics = async () => {
    try {
      setLoading(true);
      if (user?.role === 'Vendor') {
        const dashboard = await vendorService.getMyDashboard();
        setDashboardData(dashboard);
      }
    } catch (err) {
      console.error('Failed to load dashboard data:', err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="bg-light min-vh-100">
      {/* Top Navigation */}
      <div className="bg-white border-bottom">
        <div className="container-fluid">
          <div className="row">
            <div className="col-12">
              <nav className="navbar navbar-expand-lg navbar-light">
                <div className="container-fluid px-0">
                  <span className="navbar-brand mb-0 h1 text-primary">
                    <i className="bi bi-shop me-2"></i>Vendor Center
                  </span>
                  <div className="navbar-nav ms-auto">
                    <span className="nav-link text-muted">
                      Welcome, {user?.email}
                    </span>
                  </div>
                </div>
              </nav>
            </div>
          </div>
        </div>
      </div>

      {/* Secondary Navigation */}
      <div className="bg-primary">
        <div className="container-fluid">
          <ul className="nav nav-pills nav-fill">
            <li className="nav-item">
              <button 
                className={`nav-link text-white border-0 bg-transparent ${activeTab === 'overview' ? 'active bg-white text-primary' : ''}`}
                onClick={() => setActiveTab('overview')}
              >
                <i className="bi bi-house-door me-2"></i>Dashboard
              </button>
            </li>
            <li className="nav-item">
              <button 
                className={`nav-link text-white border-0 bg-transparent ${activeTab === 'products' ? 'active bg-white text-primary' : ''}`}
                onClick={() => setActiveTab('products')}
              >
                <i className="bi bi-box-seam me-2"></i>Products
              </button>
            </li>
            <li className="nav-item">
              <button 
                className={`nav-link text-white border-0 bg-transparent ${activeTab === 'orders' ? 'active bg-white text-primary' : ''}`}
                onClick={() => setActiveTab('orders')}
              >
                <i className="bi bi-truck me-2"></i>Orders & Shipping
              </button>
            </li>
            <li className="nav-item">
              <button 
                className={`nav-link text-white border-0 bg-transparent ${activeTab === 'profile' ? 'active bg-white text-primary' : ''}`}
                onClick={() => setActiveTab('profile')}
              >
                <i className="bi bi-person-circle me-2"></i>Profile
              </button>
            </li>
          </ul>
        </div>
      </div>

      {/* Main Content */}
      <div className="container-fluid py-4">

        {activeTab === 'overview' && (
          <div>
            {/* Subscription Info */}
            {dashboardData?.subscription && (
              <div className="row mb-4">
                <div className="col-12">
                  <div className="card border-0 shadow-sm">
                    <div className="card-body">
                      <div className="row align-items-center">
                        <div className="col-md-3">
                          <h5 className="text-primary mb-1">{dashboardData.subscription.planName} Plan</h5>
                          <p className="text-muted mb-0">Current Subscription</p>
                        </div>
                        <div className="col-md-3">
                          <div className="d-flex align-items-center">
                            <div className="me-3">
                              <div className="text-success" style={{fontSize: '1.5rem'}}>
                                <i className="bi bi-box-seam"></i>
                              </div>
                            </div>
                            <div>
                              <h6 className="mb-0">{dashboardData.subscription.usedProducts} / {dashboardData.subscription.maxProducts}</h6>
                              <small className="text-muted">Products Used</small>
                            </div>
                          </div>
                        </div>
                        <div className="col-md-3">
                          <div className="d-flex align-items-center">
                            <div className="me-3">
                              <div className="text-warning" style={{fontSize: '1.5rem'}}>
                                <i className="bi bi-plus-circle"></i>
                              </div>
                            </div>
                            <div>
                              <h6 className="mb-0">{dashboardData.subscription.remainingProducts}</h6>
                              <small className="text-muted">Remaining Slots</small>
                            </div>
                          </div>
                        </div>
                        <div className="col-md-3">
                          <div className="d-flex align-items-center">
                            <div className="me-3">
                              <div className="text-info" style={{fontSize: '1.5rem'}}>
                                <i className="bi bi-calendar-check"></i>
                              </div>
                            </div>
                            <div>
                              <h6 className="mb-0">{dashboardData.subscription.daysRemaining} days</h6>
                              <small className="text-muted">Until Renewal</small>
                            </div>
                          </div>
                        </div>
                      </div>
                      {dashboardData.subscription.remainingProducts <= 2 && (
                        <div className="alert alert-warning mt-3 mb-0">
                          <i className="bi bi-exclamation-triangle me-2"></i>
                          You're running low on product slots. Consider upgrading your plan.
                        </div>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            )}

            {/* Quick Stats */}
            <div className="row mb-4">
              <div className="col-md-3 mb-3">
                <div className="card border-0 shadow-sm h-100">
                  <div className="card-body text-center">
                    <div className="text-primary mb-2">
                      <i className="bi bi-box-seam" style={{fontSize: '2rem'}}></i>
                    </div>
                    <h5 className="card-title text-muted">Products</h5>
                    <h3 className="text-primary mb-0">{loading ? '--' : dashboardData?.analytics?.totalProducts || 0}</h3>
                  </div>
                </div>
              </div>
              <div className="col-md-3 mb-3">
                <div className="card border-0 shadow-sm h-100">
                  <div className="card-body text-center">
                    <div className="text-success mb-2">
                      <i className="bi bi-cart-check" style={{fontSize: '2rem'}}></i>
                    </div>
                    <h5 className="card-title text-muted">Orders</h5>
                    <h3 className="text-success mb-0">{loading ? '--' : dashboardData?.analytics?.totalOrders || 0}</h3>
                  </div>
                </div>
              </div>
              <div className="col-md-3 mb-3">
                <div className="card border-0 shadow-sm h-100">
                  <div className="card-body text-center">
                    <div className="text-warning mb-2">
                      <i className="bi bi-truck" style={{fontSize: '2rem'}}></i>
                    </div>
                    <h5 className="card-title text-muted">Shipping</h5>
                    <h3 className="text-warning mb-0">{loading ? '--' : dashboardData?.analytics?.totalShipments || 0}</h3>
                  </div>
                </div>
              </div>
              <div className="col-md-3 mb-3">
                <div className="card border-0 shadow-sm h-100">
                  <div className="card-body text-center">
                    <div className="text-info mb-2">
                      <i className="bi bi-currency-rupee" style={{fontSize: '2rem'}}></i>
                    </div>
                    <h5 className="card-title text-muted">Revenue</h5>
                    <h3 className="text-info mb-0">₹{loading ? '--' : dashboardData?.analytics?.totalRevenue?.toLocaleString() || 0}</h3>
                  </div>
                </div>
              </div>
            </div>

            {/* Detailed Analytics */}
            <div className="row mb-4">
              <div className="col-md-6 mb-3">
                <div className="card border-0 shadow-sm h-100">
                  <div className="card-header bg-transparent border-0">
                    <h6 className="mb-0 text-primary">Product Analytics</h6>
                  </div>
                  <div className="card-body">
                    <div className="row text-center">
                      <div className="col-4">
                        <div className="text-success mb-1" style={{fontSize: '1.2rem'}}>
                          <i className="bi bi-check-circle"></i>
                        </div>
                        <h6 className="mb-0">{dashboardData?.analytics?.activeProducts || 0}</h6>
                        <small className="text-muted">Active</small>
                      </div>
                      <div className="col-4">
                        <div className="text-warning mb-1" style={{fontSize: '1.2rem'}}>
                          <i className="bi bi-exclamation-triangle"></i>
                        </div>
                        <h6 className="mb-0">{dashboardData?.analytics?.lowStockProducts || 0}</h6>
                        <small className="text-muted">Low Stock</small>
                      </div>
                      <div className="col-4">
                        <div className="text-info mb-1" style={{fontSize: '1.2rem'}}>
                          <i className="bi bi-box"></i>
                        </div>
                        <h6 className="mb-0">{dashboardData?.analytics?.totalProducts || 0}</h6>
                        <small className="text-muted">Total</small>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
              <div className="col-md-6 mb-3">
                <div className="card border-0 shadow-sm h-100">
                  <div className="card-header bg-transparent border-0">
                    <h6 className="mb-0 text-success">Order Status</h6>
                  </div>
                  <div className="card-body">
                    <div className="row text-center">
                      <div className="col-3">
                        <div className="text-warning mb-1" style={{fontSize: '1.2rem'}}>
                          <i className="bi bi-clock"></i>
                        </div>
                        <h6 className="mb-0">{dashboardData?.analytics?.pendingOrders || 0}</h6>
                        <small className="text-muted">Pending</small>
                      </div>
                      <div className="col-3">
                        <div className="text-info mb-1" style={{fontSize: '1.2rem'}}>
                          <i className="bi bi-gear"></i>
                        </div>
                        <h6 className="mb-0">{dashboardData?.analytics?.processingOrders || 0}</h6>
                        <small className="text-muted">Processing</small>
                      </div>
                      <div className="col-3">
                        <div className="text-primary mb-1" style={{fontSize: '1.2rem'}}>
                          <i className="bi bi-truck"></i>
                        </div>
                        <h6 className="mb-0">{dashboardData?.analytics?.shippedOrders || 0}</h6>
                        <small className="text-muted">Shipped</small>
                      </div>
                      <div className="col-3">
                        <div className="text-success mb-1" style={{fontSize: '1.2rem'}}>
                          <i className="bi bi-check-circle"></i>
                        </div>
                        <h6 className="mb-0">{dashboardData?.analytics?.deliveredOrders || 0}</h6>
                        <small className="text-muted">Delivered</small>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            {/* Revenue Analytics */}
            <div className="row mb-4">
              <div className="col-md-4 mb-3">
                <div className="card border-0 shadow-sm text-center">
                  <div className="card-body">
                    <div className="text-success mb-2" style={{fontSize: '2rem'}}>
                      <i className="bi bi-currency-rupee"></i>
                    </div>
                    <h4 className="text-success mb-1">₹{dashboardData?.analytics?.monthlyRevenue?.toLocaleString() || 0}</h4>
                    <p className="text-muted mb-0">This Month</p>
                  </div>
                </div>
              </div>
              <div className="col-md-4 mb-3">
                <div className="card border-0 shadow-sm text-center">
                  <div className="card-body">
                    <div className="text-info mb-2" style={{fontSize: '2rem'}}>
                      <i className="bi bi-graph-up"></i>
                    </div>
                    <h4 className="text-info mb-1">₹{dashboardData?.analytics?.averageOrderValue?.toFixed(0) || 0}</h4>
                    <p className="text-muted mb-0">Avg Order Value</p>
                  </div>
                </div>
              </div>
              <div className="col-md-4 mb-3">
                <div className="card border-0 shadow-sm text-center">
                  <div className="card-body">
                    <div className="text-primary mb-2" style={{fontSize: '2rem'}}>
                      <i className="bi bi-trophy"></i>
                    </div>
                    <h4 className="text-primary mb-1">₹{dashboardData?.analytics?.totalRevenue?.toLocaleString() || 0}</h4>
                    <p className="text-muted mb-0">Total Revenue</p>
                  </div>
                </div>
              </div>
            </div>

            {/* Quick Actions */}
            <div className="row mb-4">
              <div className="col-md-4 mb-3">
                <div className="card border-0 shadow-sm h-100 hover-card">
                  <div className="card-body">
                    <div className="d-flex align-items-center mb-3">
                      <div className="bg-primary bg-opacity-10 rounded-circle p-3 me-3">
                        <i className="bi bi-plus-circle text-primary" style={{fontSize: '1.5rem'}}></i>
                      </div>
                      <div>
                        <h5 className="card-title mb-1">Add Products</h5>
                        <p className="card-text text-muted mb-0">List new products for sale</p>
                      </div>
                    </div>
                    <button 
                      className="btn btn-primary w-100"
                      onClick={() => setActiveTab('products')}
                    >
                      Manage Products
                    </button>
                  </div>
                </div>
              </div>
              
              <div className="col-md-4 mb-3">
                <div className="card border-0 shadow-sm h-100 hover-card">
                  <div className="card-body">
                    <div className="d-flex align-items-center mb-3">
                      <div className="bg-success bg-opacity-10 rounded-circle p-3 me-3">
                        <i className="bi bi-clipboard-check text-success" style={{fontSize: '1.5rem'}}></i>
                      </div>
                      <div>
                        <h5 className="card-title mb-1">Process Orders</h5>
                        <p className="card-text text-muted mb-0">Handle customer orders</p>
                      </div>
                    </div>
                    <button 
                      className="btn btn-success w-100"
                      onClick={() => setActiveTab('orders')}
                    >
                      View Orders
                    </button>
                  </div>
                </div>
              </div>
              
              <div className="col-md-4 mb-3">
                <div className="card border-0 shadow-sm h-100 hover-card">
                  <div className="card-body">
                    <div className="d-flex align-items-center mb-3">
                      <div className="bg-info bg-opacity-10 rounded-circle p-3 me-3">
                        <i className="bi bi-person-gear text-info" style={{fontSize: '1.5rem'}}></i>
                      </div>
                      <div>
                        <h5 className="card-title mb-1">Business Profile</h5>
                        <p className="card-text text-muted mb-0">Update your information</p>
                      </div>
                    </div>
                    <button 
                      className="btn btn-info w-100"
                      onClick={() => setActiveTab('profile')}
                    >
                      Edit Profile
                    </button>
                  </div>
                </div>
              </div>
            </div>

            {/* Welcome Card */}
            <div className="card border-0 shadow-sm">
              <div className="card-body">
                <div className="row align-items-center">
                  <div className="col-md-8">
                    <h4 className="text-primary mb-2">Welcome back, {user?.email}!</h4>
                    <p className="text-muted mb-0">Manage your business efficiently with our vendor tools</p>
                  </div>
                  <div className="col-md-4 text-md-end">
                    <span className="badge bg-primary fs-6 px-3 py-2">{user?.role}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        )}

        {activeTab === 'products' && <ProductManagement />}

        {activeTab === 'orders' && <OrdersShippingManagement />}

        {activeTab === 'profile' && <VendorProfileManagement />}
      </div>
      
      <style>{`
        .hover-card {
          transition: transform 0.2s, box-shadow 0.2s;
        }
        .hover-card:hover {
          transform: translateY(-2px);
          box-shadow: 0 8px 25px rgba(0,0,0,0.1) !important;
        }
        .nav-pills .nav-link {
          border-radius: 0;
          margin: 0;
        }
        .nav-pills .nav-link.active {
          border-radius: 0;
        }
      `}</style>
    </div>
  );
};

export default VendorDashboard;
