import React, { useEffect, useState } from 'react';
import { Container, Card, Badge, Spinner, Alert } from 'react-bootstrap';
import { orderService } from '../Services/orderService';
import type { OrderSummary } from '../types/order';

const Orders: React.FC = () => {
  const [orders, setOrders] = useState<OrderSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchOrders();
  }, []);

  const fetchOrders = async () => {
    try {
      setLoading(true);
      const response = await orderService.getOrders();
      setOrders(response);
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
      <h2>My Orders</h2>
      {orders.length === 0 ? (
        <Alert variant="info">No orders found.</Alert>
      ) : (
        <div className="row">
          {orders.map((order) => (
            <div key={order.orderId} className="col-md-6 mb-3">
              <Card>
                <Card.Body>
                  <div className="d-flex justify-content-between align-items-start">
                    <div>
                      <h6>Order #{order.orderId}</h6>
                      <p className="text-muted mb-1">
                        {new Date(order.createdOn).toLocaleDateString()}
                      </p>
                      <p className="mb-1">Items: {order.itemCount}</p>
                      <p className="mb-1">₹{order.totalAmount.toFixed(2)}</p>
                    </div>
                    <div>
                      {getStatusBadge(order.status)}
                    </div>
                  </div>
                  <hr />
                  <small className="text-muted">{order.shippingAddress}</small>
                </Card.Body>
              </Card>
            </div>
          ))}
        </div>
      )}
    </Container>
  );
};

export default Orders;