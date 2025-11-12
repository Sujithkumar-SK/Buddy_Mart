import React, { useEffect, useState } from 'react';
import { Container, Card, Badge, Spinner, Alert, Button, Modal, Row, Col, ProgressBar } from 'react-bootstrap';
import { orderService } from '../Services/orderService';
import type { OrderSummary, OrderDetails, OrderTracking } from '../types/order';

const OrderHistory: React.FC = () => {
  const [orders, setOrders] = useState<OrderSummary[]>([]);
  const [orderProgress, setOrderProgress] = useState<{[key: number]: number}>({});
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedOrder, setSelectedOrder] = useState<OrderDetails | null>(null);
  const [trackingInfo, setTrackingInfo] = useState<OrderTracking | null>(null);
  const [showModal, setShowModal] = useState(false);
  const [modalLoading, setModalLoading] = useState(false);

  useEffect(() => {
    fetchOrders();
  }, []);

  const fetchOrders = async () => {
    try {
      setLoading(true);
      const response = await orderService.getOrders();
      setOrders(response);
      
      // Fetch tracking progress for each order
      const progressPromises = response.map(async (order) => {
        try {
          const tracking = await orderService.getOrderTracking(order.orderId);
          const completedSteps = tracking.trackingSteps.filter((step: any) => step.status === 'Completed').length;
          const totalSteps = tracking.trackingSteps.length;
          return { orderId: order.orderId, progress: Math.round((completedSteps / totalSteps) * 100) };
        } catch {
          return { orderId: order.orderId, progress: 0 };
        }
      });
      
      const progressResults = await Promise.all(progressPromises);
      const progressMap = progressResults.reduce((acc, { orderId, progress }) => {
        acc[orderId] = progress;
        return acc;
      }, {} as {[key: number]: number});
      
      setOrderProgress(progressMap);
    } catch (err: any) {
      setError(err.message || 'Failed to fetch orders');
    } finally {
      setLoading(false);
    }
  };

  const getStatusBadge = (status: string) => {
    const statusColors: { [key: string]: string } = {
      'Pending': 'warning',
      'Confirmed': 'info',
      'Processing': 'primary',
      'Shipped': 'success',
      'Delivered': 'success',
      'Cancelled': 'danger'
    };
    return <Badge bg={statusColors[status] || 'secondary'}>{status}</Badge>;
  };

  const getStatusProgress = (orderId: number) => {
    return orderProgress[orderId] || 0;
  };

  const handleViewDetails = async (orderId: number) => {
    try {
      setModalLoading(true);
      setShowModal(true);
      
      const [orderDetails, tracking] = await Promise.all([
        orderService.getOrderById(orderId),
        orderService.getOrderTracking(orderId)
      ]);
      
      setSelectedOrder(orderDetails);
      setTrackingInfo(tracking);
    } catch (err: any) {
      setError(err.message || 'Failed to fetch order details');
    } finally {
      setModalLoading(false);
    }
  };

  const handleDownloadInvoice = async (orderId: number) => {
    try {
      await orderService.downloadInvoice(orderId);
    } catch (err: any) {
      setError(err.message || 'Failed to download invoice');
    }
  };

  const closeModal = () => {
    setShowModal(false);
    setSelectedOrder(null);
    setTrackingInfo(null);
  };

  if (loading) {
    return (
      <Container className="d-flex justify-content-center mt-5">
        <Spinner animation="border" />
      </Container>
    );
  }

  if (error) {
    return (
      <Container className="mt-4">
        <Alert variant="danger">{error}</Alert>
      </Container>
    );
  }

  return (
    <Container className="mt-4">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h3>Order History</h3>
        <small className="text-muted">{orders.length} orders found</small>
      </div>

      {orders.length === 0 ? (
        <Alert variant="info">No orders found.</Alert>
      ) : (
        <div className="row">
          {orders.map((order) => (
            <div key={order.orderId} className="col-12 mb-3">
              <Card className="order-card">
                <Card.Body>
                  <Row>
                    <Col md={8}>
                      <div className="d-flex justify-content-between align-items-start mb-2">
                        <div>
                          <h6 className="mb-1">Order #{order.orderId}</h6>
                          <small className="text-muted">
                            Placed on {new Date(order.createdOn).toLocaleDateString()}
                          </small>
                        </div>
                        <div>
                          {getStatusBadge(order.status)}
                        </div>
                      </div>
                      
                      <div className="mb-2">
                        <small className="text-muted">Progress:</small>
                        <ProgressBar 
                          now={getStatusProgress(order.orderId)} 
                          variant={order.status === 'Cancelled' ? 'danger' : 'success'}
                          className="mt-1"
                          style={{ height: '6px' }}
                        />
                        <small className="text-muted">{getStatusProgress(order.orderId)}% Complete</small>
                      </div>

                      <div className="row">
                        <div className="col-sm-6">
                          <small className="text-muted">Items: {order.itemCount}</small>
                        </div>
                        <div className="col-sm-6">
                          <small className="text-muted">Total: ₹{order.totalAmount.toFixed(2)}</small>
                        </div>
                      </div>
                      
                      <div className="mt-2">
                        <small className="text-muted d-block">
                          <i className="bi bi-geo-alt"></i> {order.shippingAddress}
                        </small>
                      </div>
                    </Col>
                    
                    <Col md={4} className="d-flex flex-column justify-content-center">
                      <div className="d-grid gap-2">
                        <Button 
                          variant="outline-primary" 
                          size="sm"
                          onClick={() => handleViewDetails(order.orderId)}
                        >
                          Track Order
                        </Button>
                        <Button 
                          variant="outline-secondary" 
                          size="sm"
                          onClick={() => handleDownloadInvoice(order.orderId)}
                        >
                          Download Invoice
                        </Button>
                      </div>
                    </Col>
                  </Row>
                </Card.Body>
              </Card>
            </div>
          ))}
        </div>
      )}

      {/* Order Details Modal */}
      <Modal show={showModal} onHide={closeModal} size="lg">
        <Modal.Header closeButton>
          <Modal.Title>
            {selectedOrder ? `Order #${selectedOrder.orderId} - Tracking` : 'Order Details'}
          </Modal.Title>
        </Modal.Header>
        <Modal.Body>
          {modalLoading ? (
            <div className="text-center py-4">
              <Spinner animation="border" />
            </div>
          ) : selectedOrder && trackingInfo ? (
            <div>
              {/* Order Summary */}
              <Card className="mb-3">
                <Card.Header>
                  <h6 className="mb-0">Order Summary</h6>
                </Card.Header>
                <Card.Body>
                  <Row>
                    <Col md={6}>
                      <p><strong>Customer:</strong> {selectedOrder.customerName}</p>
                      <p><strong>Order Date:</strong> {new Date(selectedOrder.orderDate).toLocaleDateString()}</p>
                      <p><strong>Status:</strong> {getStatusBadge(selectedOrder.status)}</p>
                    </Col>
                    <Col md={6}>
                      <p><strong>Total Amount:</strong> ₹{selectedOrder.totalAmount.toFixed(2)}</p>
                      <p><strong>Payment Method:</strong> {selectedOrder.paymentMethod}</p>
                      <p><strong>Items:</strong> {selectedOrder.items.length}</p>
                    </Col>
                  </Row>
                  <div className="mt-2">
                    <strong>Shipping Address:</strong>
                    <p className="mb-0">{selectedOrder.shippingAddress}</p>
                  </div>
                </Card.Body>
              </Card>

              {/* Tracking Timeline */}
              <Card className="mb-3">
                <Card.Header>
                  <h6 className="mb-0">Order Tracking</h6>
                </Card.Header>
                <Card.Body>
                  <div className="tracking-timeline">
                    {trackingInfo.trackingSteps.map((step, index) => (
                      <div key={index} className={`tracking-step ${step.status.toLowerCase().replace(' ', '-')}`}>
                        <div className="tracking-icon">
                          {step.status === 'Completed' ? '✓' : 
                           step.status === 'In Progress' ? '⏳' : '○'}
                        </div>
                        <div className="tracking-content">
                          <h6 className="mb-1">{step.step}</h6>
                          <small className="text-muted">
                            {step.date ? new Date(step.date).toLocaleString() : 'Pending'}
                          </small>
                        </div>
                      </div>
                    ))}
                  </div>
                </Card.Body>
              </Card>

              {/* Order Items */}
              <Card>
                <Card.Header>
                  <h6 className="mb-0">Order Items</h6>
                </Card.Header>
                <Card.Body>
                  {selectedOrder.items.map((item, index) => (
                    <div key={index} className="py-3 border-bottom">
                      <div className="d-flex justify-content-between align-items-start">
                        <div className="flex-grow-1">
                          <h6 className="mb-1">{item.productName}</h6>
                          <small className="text-muted d-block">SKU: {item.productSKU}</small>
                          <small className="text-muted d-block">Vendor: {item.vendorName}</small>
                          <small className="text-muted d-block">Qty: {item.quantity}</small>
                        </div>
                        <div className="text-end">
                          {(() => {
                            // Calculate if there's a discount at order level
                            const itemSubtotal = selectedOrder.items.reduce((sum, i) => sum + (i.unitPrice * i.quantity), 0);
                            const orderDiscount = itemSubtotal - selectedOrder.totalAmount;
                            const hasDiscount = orderDiscount > 0;
                            
                            // Calculate actual amount paid for this item (proportional discount)
                            const itemOriginalTotal = item.unitPrice * item.quantity;
                            const discountRatio = hasDiscount ? (selectedOrder.totalAmount / itemSubtotal) : 1;
                            const actualAmountPaid = itemOriginalTotal * discountRatio;
                            
                            return (
                              <>
                                {hasDiscount && (
                                  <div>
                                    <div className="text-muted text-decoration-line-through small">
                                      ₹{item.unitPrice.toFixed(2)}
                                    </div>
                                    <small className="text-muted d-block">Original Price</small>
                                  </div>
                                )}
                                <div className="fw-bold text-success mt-1">
                                  ₹{actualAmountPaid.toFixed(2)}
                                </div>
                                <small className="text-muted">Amount Paid</small>
                                {!hasDiscount && (
                                  <div className="mt-1">
                                    <small className="text-muted">No discount applied</small>
                                  </div>
                                )}
                              </>
                            );
                          })()}
                        </div>
                      </div>
                    </div>
                  ))}
                </Card.Body>
              </Card>
            </div>
          ) : null}
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={closeModal}>
            Close
          </Button>
          {selectedOrder && (
            <Button 
              variant="primary" 
              onClick={() => handleDownloadInvoice(selectedOrder.orderId)}
            >
              Download Invoice
            </Button>
          )}
        </Modal.Footer>
      </Modal>

      <style>{`
        .order-card {
          transition: transform 0.2s, box-shadow 0.2s;
          border: 1px solid #dee2e6;
        }
        .order-card:hover {
          transform: translateY(-2px);
          box-shadow: 0 4px 12px rgba(0,0,0,0.1);
        }
        .tracking-timeline {
          position: relative;
        }
        .tracking-step {
          display: flex;
          align-items: center;
          margin-bottom: 20px;
          position: relative;
        }
        .tracking-step:not(:last-child)::after {
          content: '';
          position: absolute;
          left: 15px;
          top: 30px;
          width: 2px;
          height: 20px;
          background-color: #dee2e6;
        }
        .tracking-step.completed::after {
          background-color: #28a745;
        }
        .tracking-icon {
          width: 30px;
          height: 30px;
          border-radius: 50%;
          display: flex;
          align-items: center;
          justify-content: center;
          margin-right: 15px;
          font-weight: bold;
          font-size: 14px;
        }
        .tracking-step.completed .tracking-icon {
          background-color: #28a745;
          color: white;
        }
        .tracking-step.in-progress .tracking-icon {
          background-color: #ffc107;
          color: white;
        }
        .tracking-step.pending .tracking-icon {
          background-color: #e9ecef;
          color: #6c757d;
        }
        .tracking-content h6 {
          margin-bottom: 2px;
        }
        .tracking-step.completed .tracking-content h6 {
          color: #28a745;
        }
      `}</style>
    </Container>
  );
};

export default OrderHistory;