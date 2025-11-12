import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/authContext';
import { vendorService } from '../Services/vendorService';
import { shippingService } from '../Services/shippingService';
import type { VendorOrder, ShippingDetails, UpdateTrackingDetails } from '../types/shipping';

const OrdersShippingManagement: React.FC = () => {
  const { user } = useAuth();
  const [orders, setOrders] = useState<VendorOrder[]>([]);
  const [shippings, setShippings] = useState<ShippingDetails[]>([]);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState('orders');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [showTrackingModal, setShowTrackingModal] = useState(false);
  const [selectedShipping, setSelectedShipping] = useState<ShippingDetails | null>(null);
  
  const [trackingForm, setTrackingForm] = useState<UpdateTrackingDetails>({
    shippingId: 0,
    trackingNumber: '',
    courierService: '',
    estimatedDeliveryDate: '',
    notes: ''
  });

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      let vendorId = user?.vendorId;
      
      // If vendorId is not in user context, fetch it from profile
      if (!vendorId && user?.role === 'Vendor') {
        try {
          const profile = await vendorService.getMyProfile();
          vendorId = profile.vendorId;
        } catch (profileErr) {
          setError('Failed to get vendor profile');
          return;
        }
      }
      
      if (vendorId) {
        const [ordersData, shippingsData] = await Promise.all([
          vendorService.getVendorOrders(vendorId),
          shippingService.getShippingsByVendor(vendorId)
        ]);
        setOrders(ordersData);
        setShippings(shippingsData);
      } else {
        setError('Vendor ID not found');
      }
    } catch (err: any) {
      setError('Failed to load data');
      console.error('Error loading data:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleAcceptOrder = async (orderId: number) => {
    try {
      await vendorService.acceptOrder(orderId);
      setSuccess('Order accepted successfully');
      loadData();
    } catch (err: any) {
      setError('Failed to accept order');
    }
  };

  const handleCreateShipping = async (orderId: number) => {
    try {
      await shippingService.createShipping(orderId);
      setSuccess('Shipping created successfully');
      loadData();
    } catch (err: any) {
      setError('Failed to create shipping');
    }
  };

  const handleMarkAsShipped = async (shippingId: number) => {
    try {
      await shippingService.markAsShipped(shippingId);
      setSuccess('Order marked as shipped');
      loadData();
    } catch (err: any) {
      setError('Failed to mark as shipped');
    }
  };

  const handleMarkAsDelivered = async (shippingId: number) => {
    try {
      await shippingService.markAsDelivered(shippingId);
      setSuccess('Order marked as delivered');
      loadData();
    } catch (err: any) {
      setError('Failed to mark as delivered');
    }
  };

  const handleOpenTrackingModal = (shipping: ShippingDetails) => {
    setSelectedShipping(shipping);
    setTrackingForm({
      shippingId: shipping.shippingId,
      trackingNumber: shipping.trackingNumber || '',
      courierService: shipping.courierService || '',
      estimatedDeliveryDate: shipping.estimatedDeliveryDate ? shipping.estimatedDeliveryDate.split('T')[0] : '',
      notes: shipping.deliveryNotes || ''
    });
    setShowTrackingModal(true);
  };

  const handleUpdateTracking = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await shippingService.updateTrackingDetails(trackingForm);
      setSuccess('Tracking details updated successfully');
      setShowTrackingModal(false);
      loadData();
    } catch (err: any) {
      setError('Failed to update tracking details');
    }
  };

  const getStatusBadgeClass = (status: string) => {
    switch (status.toLowerCase()) {
      case 'pending': return 'bg-warning';
      case 'confirmed': return 'bg-info';
      case 'processing': return 'bg-primary';
      case 'shipped': return 'bg-success';
      case 'delivered': return 'bg-success';
      case 'cancelled': return 'bg-danger';
      default: return 'bg-secondary';
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
      <h4>Orders & Shipping Management</h4>

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

      {/* Navigation Tabs */}
      <ul className="nav nav-tabs mb-4">
        <li className="nav-item">
          <button 
            className={`nav-link ${activeTab === 'orders' ? 'active' : ''}`}
            onClick={() => setActiveTab('orders')}
          >
            Orders ({orders.length})
          </button>
        </li>
        <li className="nav-item">
          <button 
            className={`nav-link ${activeTab === 'shipping' ? 'active' : ''}`}
            onClick={() => setActiveTab('shipping')}
          >
            Shipping ({shippings.length})
          </button>
        </li>
      </ul>

      {/* Orders Tab */}
      {activeTab === 'orders' && (
        <div className="row">
          {orders.length === 0 ? (
            <div className="col-12 text-center py-5">
              <h5>No orders found</h5>
              <p className="text-muted">Orders will appear here when customers place orders</p>
            </div>
          ) : (
            orders.map((order) => (
              <div key={order.orderId} className="col-12 mb-4">
                <div className="card">
                  <div className="card-header">
                    <div className="row align-items-center">
                      <div className="col-md-6">
                        <h6 className="mb-0">Order #{order.orderNumber}</h6>
                        <small className="text-muted">
                          {new Date(order.orderDate).toLocaleDateString()}
                        </small>
                      </div>
                      <div className="col-md-6 text-md-end">
                        <span className={`badge ${getStatusBadgeClass(order.status)} me-2`}>
                          {order.status}
                        </span>
                        <strong>₹{order.totalAmount}</strong>
                      </div>
                    </div>
                  </div>
                  <div className="card-body">
                    <div className="row">
                      <div className="col-md-6">
                        <h6>Customer Details</h6>
                        <p className="mb-1"><strong>{order.customerName}</strong></p>
                        <p className="mb-1">{order.customerPhone}</p>
                        <p className="mb-1">{order.customerEmail}</p>
                      </div>
                      <div className="col-md-6">
                        <h6>Shipping Address</h6>
                        <p className="mb-1">{order.shippingAddress}</p>
                        {order.shippingCity && (
                          <p className="mb-1">{order.shippingCity}, {order.shippingState} - {order.shippingPinCode}</p>
                        )}
                      </div>
                    </div>
                    
                    <h6 className="mt-3">Order Items</h6>
                    <div className="table-responsive">
                      <table className="table table-sm">
                        <thead>
                          <tr>
                            <th>Product</th>
                            <th>SKU</th>
                            <th>Quantity</th>
                            <th>Unit Price</th>
                            <th>Total</th>
                          </tr>
                        </thead>
                        <tbody>
                          {order.items.map((item) => (
                            <tr key={item.orderItemId}>
                              <td>{item.productName}</td>
                              <td>{item.productSKU}</td>
                              <td>{item.quantity}</td>
                              <td>₹{item.unitPrice}</td>
                              <td>₹{item.totalPrice}</td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  </div>
                  <div className="card-footer">
                    <div className="btn-group" role="group">
                      {order.status === 'Pending' && (
                        <button 
                          className="btn btn-success btn-sm"
                          onClick={() => handleAcceptOrder(order.orderId)}
                        >
                          Accept Order
                        </button>
                      )}
                      {(order.status === 'Confirmed' || order.status === 'Processing') && !order.shipping && (
                        <button 
                          className="btn btn-primary btn-sm"
                          onClick={() => handleCreateShipping(order.orderId)}
                        >
                          Create Shipping
                        </button>
                      )}
                      {order.shipping && order.shipping.status === 'Pending' && (
                        <button 
                          className="btn btn-info btn-sm"
                          onClick={() => handleOpenTrackingModal(order.shipping!)}
                        >
                          Manage Shipping
                        </button>
                      )}
                      {order.shipping && order.shipping.status === 'Shipped' && (
                        <button 
                          className="btn btn-warning btn-sm"
                          onClick={() => handleMarkAsDelivered(order.shipping!.shippingId)}
                        >
                          Mark Delivered
                        </button>
                      )}
                      {order.shipping && order.shipping.status === 'Delivered' && (
                        <span className="text-success fw-bold">✓ Delivered</span>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            ))
          )}
        </div>
      )}

      {/* Shipping Tab */}
      {activeTab === 'shipping' && (
        <div className="row">
          {shippings.length === 0 ? (
            <div className="col-12 text-center py-5">
              <h5>No shipments found</h5>
              <p className="text-muted">Shipments will appear here when you create shipping for orders</p>
            </div>
          ) : (
            shippings.map((shipping) => (
              <div key={shipping.shippingId} className="col-md-6 col-lg-4 mb-4">
                <div className="card h-100">
                  <div className="card-header">
                    <h6 className="mb-0">Order #{shipping.orderId}</h6>
                    <span className={`badge ${getStatusBadgeClass(shipping.status)}`}>
                      {shipping.status}
                    </span>
                  </div>
                  <div className="card-body">
                    {shipping.trackingNumber && (
                      <p className="mb-2">
                        <strong>Tracking:</strong> {shipping.trackingNumber}
                      </p>
                    )}
                    {shipping.courierService && (
                      <p className="mb-2">
                        <strong>Courier:</strong> {shipping.courierService}
                      </p>
                    )}
                    {shipping.estimatedDeliveryDate && (
                      <p className="mb-2">
                        <strong>Est. Delivery:</strong> {new Date(shipping.estimatedDeliveryDate).toLocaleDateString()}
                      </p>
                    )}
                    {shipping.shippedDate && (
                      <p className="mb-2">
                        <strong>Shipped:</strong> {new Date(shipping.shippedDate).toLocaleDateString()}
                      </p>
                    )}
                    {shipping.actualDeliveryDate && (
                      <p className="mb-2">
                        <strong>Delivered:</strong> {new Date(shipping.actualDeliveryDate).toLocaleDateString()}
                      </p>
                    )}
                  </div>
                  <div className="card-footer">
                    <div className="btn-group w-100" role="group">
                      {shipping.status === 'Pending' && (
                        <>
                          <button 
                            className="btn btn-outline-primary btn-sm"
                            onClick={() => handleOpenTrackingModal(shipping)}
                          >
                            Edit
                          </button>
                          <button 
                            className="btn btn-success btn-sm"
                            onClick={() => handleMarkAsShipped(shipping.shippingId)}
                          >
                            Ship
                          </button>
                        </>
                      )}
                      {shipping.status === 'Shipped' && (
                        <button 
                          className="btn btn-warning btn-sm"
                          onClick={() => handleMarkAsDelivered(shipping.shippingId)}
                        >
                          Deliver
                        </button>
                      )}
                      {shipping.status === 'Delivered' && (
                        <span className="text-success fw-bold">✓ Completed</span>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            ))
          )}
        </div>
      )}

      {/* Tracking Details Modal */}
      <div className={`modal fade ${showTrackingModal ? 'show' : ''}`} style={{ display: showTrackingModal ? 'block' : 'none' }} tabIndex={-1}>
        <div className="modal-dialog">
          <div className="modal-content">
            <div className="modal-header">
              <h5 className="modal-title">Update Tracking Details</h5>
              <button type="button" className="btn-close" onClick={() => setShowTrackingModal(false)}></button>
            </div>
            <form onSubmit={handleUpdateTracking}>
              <div className="modal-body">
                <div className="mb-3">
                  <label className="form-label">Tracking Number *</label>
                  <input
                    type="text"
                    className="form-control"
                    value={trackingForm.trackingNumber}
                    onChange={(e) => setTrackingForm({...trackingForm, trackingNumber: e.target.value})}
                    required
                  />
                </div>
                <div className="mb-3">
                  <label className="form-label">Courier Service *</label>
                  <select
                    className="form-select"
                    value={trackingForm.courierService}
                    onChange={(e) => setTrackingForm({...trackingForm, courierService: e.target.value})}
                    required
                  >
                    <option value="">Select Courier</option>
                    <option value="BlueDart">BlueDart</option>
                    <option value="DTDC">DTDC</option>
                    <option value="FedEx">FedEx</option>
                    <option value="Delhivery">Delhivery</option>
                    <option value="India Post">India Post</option>
                    <option value="Ecom Express">Ecom Express</option>
                    <option value="Aramex">Aramex</option>
                  </select>
                </div>
                <div className="mb-3">
                  <label className="form-label">Estimated Delivery Date</label>
                  <input
                    type="date"
                    className="form-control"
                    value={trackingForm.estimatedDeliveryDate}
                    onChange={(e) => setTrackingForm({...trackingForm, estimatedDeliveryDate: e.target.value})}
                  />
                </div>
                <div className="mb-3">
                  <label className="form-label">Notes</label>
                  <textarea
                    className="form-control"
                    rows={3}
                    value={trackingForm.notes}
                    onChange={(e) => setTrackingForm({...trackingForm, notes: e.target.value})}
                    placeholder="Any additional notes about the shipment..."
                  />
                </div>
              </div>
              <div className="modal-footer">
                <button type="button" className="btn btn-secondary" onClick={() => setShowTrackingModal(false)}>
                  Cancel
                </button>
                <button type="submit" className="btn btn-primary">
                  Update Tracking
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>

      {showTrackingModal && <div className="modal-backdrop fade show"></div>}
    </div>
  );
};

export default OrdersShippingManagement;