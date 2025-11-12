import React, { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { Container, Card, Button, Spinner, Alert } from 'react-bootstrap';
import { paymentService } from '../Services/paymentService';

declare global {
  interface Window {
    Razorpay: any;
  }
}

const Payment: React.FC = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const orderId = searchParams.get('orderId');
  const [loading, setLoading] = useState(false);
  const [verifying, setVerifying] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!orderId) {
      navigate('/orders');
    }
  }, [orderId, navigate]);

  const handlePayment = async () => {
    if (!orderId) return;

    try {
      setLoading(true);
      setError(null);

      // Initiate payment
      const paymentData = await paymentService.initiatePayment({
        orderId: parseInt(orderId),
        paymentMethod: 'Razorpay'
      });

      // Razorpay options
      const options = {
        key: paymentData.key,
        amount: paymentData.amount * 100, // Convert to paise
        currency: 'INR',
        name: 'Kanini E-commerce',
        description: paymentData.description,
        order_id: paymentData.razorpayOrderId,
        prefill: {
          name: paymentData.prefillName,
          email: paymentData.prefillEmail,
          contact: paymentData.prefillContact
        },
        theme: {
          color: '#007bff'
        },
        handler: async (response: any) => {
          try {
            setVerifying(true);
            // Verify payment
            await paymentService.verifyPayment({
              razorpayOrderId: response.razorpay_order_id,
              razorpayPaymentId: response.razorpay_payment_id,
              razorpaySignature: response.razorpay_signature
            });

            // Payment successful
            alert('Payment successful! Order confirmed.');
            navigate('/orders');
          } catch (error: any) {
            console.error('Payment verification failed:', error);
            alert('Payment verification failed. Please contact support.');
            setVerifying(false);
          }
        },
        modal: {
          ondismiss: () => {
            setLoading(false);
            setVerifying(false);
            alert('Payment cancelled');
          }
        }
      };

      // Open Razorpay
      const razorpay = new window.Razorpay(options);
      razorpay.open();

    } catch (error: any) {
      console.error('Payment initiation failed:', error);
      setError(error.response?.data?.description || 'Payment initiation failed');
    } finally {
      setLoading(false);
    }
  };

  if (!orderId) {
    return null;
  }

  return (
    <Container className="mt-4">
      <div className="row justify-content-center">
        <div className="col-md-6">
          <Card>
            <Card.Header>
              <h4>💳 Complete Payment</h4>
            </Card.Header>
            <Card.Body className="text-center">
              <p>Order ID: <strong>#{orderId}</strong></p>
              <p>Click the button below to proceed with payment</p>
              
              {error && (
                <Alert variant="danger">{error}</Alert>
              )}
              
              <Button
                variant="success"
                size="lg"
                onClick={handlePayment}
                disabled={loading || verifying}
                className="w-100"
              >
                {loading ? (
                  <>
                    <Spinner animation="border" size="sm" className="me-2" />
                    Processing...
                  </>
                ) : (
                  'Pay Now'
                )}
              </Button>
              
              <div className="mt-3">
                <small className="text-muted">
                  Secure payment powered by Razorpay
                </small>
              </div>
            </Card.Body>
          </Card>
        </div>
      </div>

      {/* Payment Verification Overlay */}
      {verifying && (
        <div className="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center" 
             style={{ backgroundColor: 'rgba(0,0,0,0.7)', zIndex: 9999 }}>
          <div className="bg-white p-4 rounded shadow text-center">
            <Spinner animation="border" variant="primary" className="mb-3" />
            <h5 className="mb-2">Verifying Payment</h5>
            <p className="text-muted mb-0">Please wait while we confirm your payment...</p>
          </div>
        </div>
      )}
    </Container>
  );
};

export default Payment;