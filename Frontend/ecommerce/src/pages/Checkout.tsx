import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { orderService } from '../Services/orderService';
import type { CheckoutSummary, CreateOrderRequest } from '../types/order';

const Checkout: React.FC = () => {
  const navigate = useNavigate();
  const [checkoutData, setCheckoutData] = useState<CheckoutSummary | null>(null);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [formData, setFormData] = useState<CreateOrderRequest>({
    fullName: '',
    phone: '',
    addressLine1: '',
    addressLine2: '',
    city: '',
    state: '',
    pinCode: '',
    landmark: '',
    paymentMethod: 'Razorpay',
    orderNotes: ''
  });

  useEffect(() => {
    loadCheckoutSummary();
  }, []);

  const loadCheckoutSummary = async () => {
    try {
      setLoading(true);
      const data = await orderService.getCheckoutSummary();
      setCheckoutData(data);
    } catch (error) {
      console.error('Error loading checkout summary:', error);
      navigate('/cart');
    } finally {
      setLoading(false);
    }
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    setFormData(prev => ({
      ...prev,
      [e.target.name]: e.target.value
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!checkoutData) return;

    try {
      setSubmitting(true);
      const order = await orderService.createOrder(formData);
      
      // Redirect to payment page
      navigate(`/payment?orderId=${order.orderId}`);
    } catch (error: any) {
      console.error('Error creating order:', error);
      alert(error.response?.data?.description || 'Failed to place order');
    } finally {
      setSubmitting(false);
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

  if (!checkoutData || checkoutData.items.length === 0) {
    return (
      <div className="container mt-4">
        <div className="alert alert-warning">
          Your cart is empty. <a href="/dashboard">Continue shopping</a>
        </div>
      </div>
    );
  }

  return (
    <div className="checkout-page">
      <div className="container py-4">
        <h2 className="mb-4">🛒 Checkout</h2>
        
        <div className="row g-4">
          {/* Delivery Address & Payment */}
          <div className="col-lg-8">
            <form onSubmit={handleSubmit}>
              {/* Delivery Address */}
              <div className="card mb-4">
                <div className="card-header">
                  <h5 className="mb-0">📍 Delivery Address</h5>
                </div>
                <div className="card-body">
                  <div className="row g-3">
                    <div className="col-md-6">
                      <label className="form-label">Full Name *</label>
                      <input
                        type="text"
                        className="form-control"
                        name="fullName"
                        value={formData.fullName}
                        onChange={handleInputChange}
                        required
                      />
                    </div>
                    <div className="col-md-6">
                      <label className="form-label">Phone Number *</label>
                      <input
                        type="tel"
                        className="form-control"
                        name="phone"
                        value={formData.phone}
                        onChange={handleInputChange}
                        required
                      />
                    </div>
                    <div className="col-12">
                      <label className="form-label">Address Line 1 *</label>
                      <input
                        type="text"
                        className="form-control"
                        name="addressLine1"
                        placeholder="House/Flat No., Building Name"
                        value={formData.addressLine1}
                        onChange={handleInputChange}
                        required
                      />
                    </div>
                    <div className="col-12">
                      <label className="form-label">Address Line 2</label>
                      <input
                        type="text"
                        className="form-control"
                        name="addressLine2"
                        placeholder="Street, Area"
                        value={formData.addressLine2}
                        onChange={handleInputChange}
                      />
                    </div>
                    <div className="col-md-4">
                      <label className="form-label">City *</label>
                      <input
                        type="text"
                        className="form-control"
                        name="city"
                        value={formData.city}
                        onChange={handleInputChange}
                        required
                      />
                    </div>
                    <div className="col-md-4">
                      <label className="form-label">State *</label>
                      <input
                        type="text"
                        className="form-control"
                        name="state"
                        value={formData.state}
                        onChange={handleInputChange}
                        required
                      />
                    </div>
                    <div className="col-md-4">
                      <label className="form-label">PIN Code *</label>
                      <input
                        type="text"
                        className="form-control"
                        name="pinCode"
                        value={formData.pinCode}
                        onChange={handleInputChange}
                        required
                      />
                    </div>
                    <div className="col-12">
                      <label className="form-label">Landmark</label>
                      <input
                        type="text"
                        className="form-control"
                        name="landmark"
                        placeholder="Near by landmark (optional)"
                        value={formData.landmark}
                        onChange={handleInputChange}
                      />
                    </div>
                  </div>
                </div>
              </div>

              {/* Payment Method */}
              <div className="card mb-4">
                <div className="card-header">
                  <h5 className="mb-0">💳 Payment Method</h5>
                </div>
                <div className="card-body">
                  {checkoutData.availablePaymentMethods.map(method => (
                    <div key={method} className="form-check mb-2">
                      <input
                        className="form-check-input"
                        type="radio"
                        name="paymentMethod"
                        value={method}
                        checked={formData.paymentMethod === method}
                        onChange={handleInputChange}
                      />
                      <label className="form-check-label">
                        {method === 'Razorpay' ? '💳 Razorpay (Cards, UPI, Wallets)' : method}
                      </label>
                    </div>
                  ))}
                </div>
              </div>

              {/* Order Notes */}
              <div className="card mb-4">
                <div className="card-header">
                  <h5 className="mb-0">📝 Order Notes (Optional)</h5>
                </div>
                <div className="card-body">
                  <textarea
                    className="form-control"
                    name="orderNotes"
                    rows={3}
                    placeholder="Any special instructions for delivery..."
                    value={formData.orderNotes}
                    onChange={handleInputChange}
                  />
                </div>
              </div>
            </form>
          </div>

          {/* Order Summary */}
          <div className="col-lg-4">
            <div className="card sticky-top" style={{ top: '20px' }}>
              <div className="card-header">
                <h5 className="mb-0">📋 Order Summary</h5>
              </div>
              <div className="card-body">
                {/* Items */}
                <div className="order-items mb-3">
                  {checkoutData.items.map(item => (
                    <div key={item.cartId} className="d-flex align-items-center mb-2 pb-2 border-bottom">
                      <img
                        src={item.productImage ? `http://localhost:5108${item.productImage}` : '/placeholder.png'}
                        alt={item.productName}
                        className="me-2"
                        style={{ width: '40px', height: '40px', objectFit: 'cover', borderRadius: '4px' }}
                      />
                      <div className="flex-grow-1">
                        <div className="fw-semibold small">{item.productName}</div>
                        <div className="text-muted small">Qty: {item.quantity}</div>
                      </div>
                      <div className="text-end">
                        <div className="fw-semibold">₹{item.totalPrice.toLocaleString()}</div>
                      </div>
                    </div>
                  ))}
                </div>

                {/* Pricing */}
                <div className="pricing-summary">
                  <div className="d-flex justify-content-between mb-2">
                    <span>Subtotal ({checkoutData.totalItems} items)</span>
                    <span>₹{checkoutData.subTotal.toLocaleString()}</span>
                  </div>
                  
                  {checkoutData.totalDiscount > 0 && (
                    <div className="d-flex justify-content-between mb-2 text-success">
                      <span>Discount</span>
                      <span>-₹{checkoutData.totalDiscount.toLocaleString()}</span>
                    </div>
                  )}
                  
                  <div className="d-flex justify-content-between mb-2">
                    <span>Shipping</span>
                    <span className="text-success">FREE</span>
                  </div>
                  
                  <hr />
                  
                  <div className="d-flex justify-content-between mb-3 fw-bold fs-5">
                    <span>Total</span>
                    <span>₹{checkoutData.grandTotal.toLocaleString()}</span>
                  </div>
                </div>

                <button
                  type="submit"
                  className="btn btn-success btn-lg w-100"
                  onClick={handleSubmit}
                  disabled={submitting}
                >
                  {submitting ? (
                    <>
                      <div className="spinner-border spinner-border-sm me-2" role="status"></div>
                      Placing Order...
                    </>
                  ) : (
                    'Place Order'
                  )}
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <style>{`
        .checkout-page {
          background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
          min-height: 100vh;
        }
        
        .card {
          border: none;
          box-shadow: 0 2px 10px rgba(0,0,0,0.1);
          border-radius: 10px;
        }
        
        .card-header {
          background: linear-gradient(135deg, #007bff 0%, #0056b3 100%);
          color: white;
          border-radius: 10px 10px 0 0 !important;
        }
        
        .form-control:focus {
          border-color: #007bff;
          box-shadow: 0 0 0 0.2rem rgba(0,123,255,0.25);
        }
        
        .btn-success {
          background: linear-gradient(135deg, #28a745 0%, #20c997 100%);
          border: none;
          border-radius: 8px;
          font-weight: 600;
        }
        
        .btn-success:hover {
          transform: translateY(-1px);
          box-shadow: 0 4px 12px rgba(40,167,69,0.3);
        }
      `}</style>
    </div>
  );
};

export default Checkout;