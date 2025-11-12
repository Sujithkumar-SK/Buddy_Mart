import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/authContext';
import { adminService, type DashboardAnalytics, type PendingVendor } from '../Services/adminService';
import VendorApprovalModal from '../componenets/VendorApprovalModal';
import CategoryManagement from '../componenets/CategoryManagement';

const AdminDashboard: React.FC = () => {
  const { user } = useAuth();
  const [analytics, setAnalytics] = useState<DashboardAnalytics | null>(null);
  const [pendingVendors, setPendingVendors] = useState<PendingVendor[]>([]);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState('dashboard');
  const [selectedVendor, setSelectedVendor] = useState<PendingVendor | null>(null);
  const [showModal, setShowModal] = useState(false);

  useEffect(() => {
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    try {
      setLoading(true);
      const endDate = new Date().toISOString().split('T')[0];
      const startDate = new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0];
      
      const [analyticsData, vendorsData] = await Promise.all([
        adminService.getDashboardAnalytics(startDate, endDate),
        adminService.getPendingVendors()
      ]);
      
      setAnalytics(analyticsData);
      setPendingVendors(vendorsData);
    } catch (error) {
      console.error('Error loading dashboard data:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleVendorAction = async (vendorId: number, action: 'approve' | 'reject', reason: string) => {
    try {
      if (action === 'approve') {
        await adminService.approveVendor({ vendorId, approvalReason: reason });
      } else {
        await adminService.rejectVendor({ vendorId, rejectionReason: reason });
      }
      await loadDashboardData();
    } catch (error) {
      console.error(`Error ${action}ing vendor:`, error);
    }
  };

  const handleVendorClick = (vendor: PendingVendor) => {
    setSelectedVendor(vendor);
    setShowModal(true);
  };

  const handleModalClose = () => {
    setSelectedVendor(null);
    setShowModal(false);
  };

  const handleApprove = async (vendorId: number, reason: string) => {
    await handleVendorAction(vendorId, 'approve', reason);
  };

  const handleReject = async (vendorId: number, reason: string) => {
    await handleVendorAction(vendorId, 'reject', reason);
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
    <div className="container-fluid mt-4">
      <div className="row">
        <div className="col-12">
          <h2 className="mb-4">Admin Dashboard</h2>
          
          {/* Navigation Tabs */}
          <ul className="nav nav-tabs mb-4">
            <li className="nav-item">
              <button 
                className={`nav-link ${activeTab === 'dashboard' ? 'active' : ''}`}
                onClick={() => setActiveTab('dashboard')}
              >
                Dashboard
              </button>
            </li>
            <li className="nav-item">
              <button 
                className={`nav-link ${activeTab === 'vendors' ? 'active' : ''}`}
                onClick={() => setActiveTab('vendors')}
              >
                Vendor Management
                {pendingVendors.length > 0 && (
                  <span className="badge bg-danger ms-2">{pendingVendors.length}</span>
                )}
              </button>
            </li>
            <li className="nav-item">
              <button 
                className={`nav-link ${activeTab === 'categories' ? 'active' : ''}`}
                onClick={() => setActiveTab('categories')}
              >
                Category Management
              </button>
            </li>
          </ul>

          {/* Dashboard Tab */}
          {activeTab === 'dashboard' && analytics && (
            <div className="row">
              {/* Revenue Cards */}
              <div className="col-md-3 mb-4">
                <div className="card bg-primary text-white">
                  <div className="card-body">
                    <h5 className="card-title">Today's Revenue</h5>
                    <h3>₹{analytics.totalRevenueToday.toLocaleString()}</h3>
                  </div>
                </div>
              </div>
              <div className="col-md-3 mb-4">
                <div className="card bg-success text-white">
                  <div className="card-body">
                    <h5 className="card-title">Monthly Revenue</h5>
                    <h3>₹{analytics.totalRevenueThisMonth.toLocaleString()}</h3>
                  </div>
                </div>
              </div>
              <div className="col-md-3 mb-4">
                <div className="card bg-info text-white">
                  <div className="card-body">
                    <h5 className="card-title">Total Orders</h5>
                    <h3>{analytics.totalOrdersThisMonth}</h3>
                  </div>
                </div>
              </div>
              <div className="col-md-3 mb-4">
                <div className="card bg-warning text-white">
                  <div className="card-body">
                    <h5 className="card-title">Active Vendors</h5>
                    <h3>{analytics.activeVendors}</h3>
                  </div>
                </div>
              </div>

              {/* Recent Activities */}
              <div className="col-md-6 mb-4">
                <div className="card">
                  <div className="card-header">
                    <h5 className="mb-0">Recent Activities</h5>
                  </div>
                  <div className="card-body">
                    {analytics.recentActivities.length > 0 ? (
                      <div className="list-group list-group-flush">
                        {analytics.recentActivities.slice(0, 5).map((activity, index) => (
                          <div key={index} className="list-group-item border-0 px-0">
                            <div className="d-flex justify-content-between">
                              <div>
                                <strong>{activity.activityType}</strong>
                                <p className="mb-1 text-muted">{activity.description}</p>
                                <small className="text-muted">by {activity.userName}</small>
                              </div>
                              <small className="text-muted">
                                {new Date(activity.createdOn).toLocaleDateString()}
                              </small>
                            </div>
                          </div>
                        ))}
                      </div>
                    ) : (
                      <p className="text-muted">No recent activities</p>
                    )}
                  </div>
                </div>
              </div>

              {/* Top Selling Products */}
              <div className="col-md-6 mb-4">
                <div className="card">
                  <div className="card-header">
                    <h5 className="mb-0">Top Selling Products</h5>
                  </div>
                  <div className="card-body">
                    {analytics.topSellingProducts.length > 0 ? (
                      <div className="list-group list-group-flush">
                        {analytics.topSellingProducts.slice(0, 5).map((product, index) => (
                          <div key={product.productId} className="list-group-item border-0 px-0">
                            <div className="d-flex justify-content-between">
                              <div>
                                <strong>{product.productName}</strong>
                                <p className="mb-1 text-muted">Sold: {product.totalSold} units</p>
                              </div>
                              <div className="text-end">
                                <strong>₹{product.revenue.toLocaleString()}</strong>
                              </div>
                            </div>
                          </div>
                        ))}
                      </div>
                    ) : (
                      <p className="text-muted">No sales data available</p>
                    )}
                  </div>
                </div>
              </div>
            </div>
          )}

          {/* Vendor Management Tab */}
          {activeTab === 'vendors' && (
            <div className="row">
              <div className="col-12">
                <div className="card">
                  <div className="card-header">
                    <h5 className="mb-0">Pending Vendor Applications</h5>
                  </div>
                  <div className="card-body">
                    {pendingVendors.length > 0 ? (
                      <div className="table-responsive">
                        <table className="table table-striped">
                          <thead>
                            <tr>
                              <th>Business Name</th>
                              <th>Owner</th>
                              <th>Email</th>
                              <th>Phone</th>
                              <th>License</th>
                              <th>Applied On</th>
                              <th>Actions</th>
                            </tr>
                          </thead>
                          <tbody>
                            {pendingVendors.map((vendor) => (
                              <tr key={vendor.vendorId}>
                                <td>{vendor.businessName}</td>
                                <td>{vendor.ownerName}</td>
                                <td>{vendor.email}</td>
                                <td>{vendor.phone}</td>
                                <td>{vendor.businessLicenseNumber}</td>
                                <td>{new Date(vendor.createdOn).toLocaleDateString()}</td>
                                <td>
                                  <button 
                                    className="btn btn-primary btn-sm"
                                    onClick={() => handleVendorClick(vendor)}
                                  >
                                    Review
                                  </button>
                                </td>
                              </tr>
                            ))}
                          </tbody>
                        </table>
                      </div>
                    ) : (
                      <div className="text-center py-4">
                        <p className="text-muted">No pending vendor applications</p>
                      </div>
                    )}
                  </div>
                </div>
              </div>
            </div>
          )}

          {/* Category Management Tab */}
          {activeTab === 'categories' && (
            <div className="row">
              <div className="col-12">
                <CategoryManagement />
              </div>
            </div>
          )}

          {/* User Info */}
          <div className="row mt-4">
            <div className="col-12">
              <div className="card">
                <div className="card-body">
                  <h6>Welcome, {user?.email}!</h6>
                  <p className="mb-1">Role: <span className="badge bg-primary">{user?.role}</span></p>
                  <p className="mb-0">User ID: {user?.userId}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Vendor Approval Modal */}
      <VendorApprovalModal
        vendor={selectedVendor}
        isOpen={showModal}
        onClose={handleModalClose}
        onApprove={handleApprove}
        onReject={handleReject}
      />
    </div>
  );
};

export default AdminDashboard;
