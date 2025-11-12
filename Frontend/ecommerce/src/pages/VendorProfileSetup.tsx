import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import axios from 'axios';
import 'bootstrap/dist/css/bootstrap.min.css';

interface LocationState {
  userId: number;
  email: string;
}

interface SubscriptionPlan {
  planId: number;
  planName: string;
  maxProducts: number;
  price: number;
}

const VendorProfileSetup: React.FC = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [subscriptionPlans, setSubscriptionPlans] = useState<SubscriptionPlan[]>([]);

  const [formData, setFormData] = useState({
    businessName: '',
    ownerName: '',
    businessLicenseNumber: '',
    businessAddress: '',
    city: '',
    state: '',
    pinCode: '',
    taxRegistrationNumber: '',
    subscriptionPlanId: ''
  });
  const [document, setDocument] = useState<File | null>(null);

  const navigate = useNavigate();
  const location = useLocation();
  const { userId, email } = location.state as LocationState || {};

  if (!userId || !email) {
    navigate('/register');
    return null;
  }

  useEffect(() => {
    fetchSubscriptionPlans();
  }, []);

  const fetchSubscriptionPlans = async () => {
    try {
      const response = await axios.get('http://localhost:5108/api/vendor/subscription-plans');
      console.log('API Response:', response.data);
      const plans = Array.isArray(response.data) ? response.data : [];
      setSubscriptionPlans(plans);
    } catch (err) {
      console.error('Failed to fetch plans:', err);
      setError('Failed to load subscription plans');
    }
  };
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    if (!formData.subscriptionPlanId || formData.subscriptionPlanId === '') {
      setError('Please select a subscription plan');
      setLoading(false);
      return;
    }

    if (!formData.taxRegistrationNumber || formData.taxRegistrationNumber.trim() === '') {
      setError('Tax Registration Number is required');
      setLoading(false);
      return;
    }

    const planId = parseInt(formData.subscriptionPlanId);
    console.log('Form subscriptionPlanId:', formData.subscriptionPlanId, 'Parsed planId:', planId);
    if (isNaN(planId) || planId <= 0) {
      setError('Invalid subscription plan selected');
      setLoading(false);
      return;
    }

    try {
      const payload = {
        userId: userId,
        businessName: formData.businessName,
        ownerName: formData.ownerName,
        businessLicenseNumber: formData.businessLicenseNumber,
        businessAddress: formData.businessAddress,
        city: formData.city,
        state: formData.state,
        pinCode: formData.pinCode,
        taxRegistrationNumber: formData.taxRegistrationNumber,
        subscriptionPlanId: planId
      };
      
      console.log('Final payload:', payload);
      console.log('TaxRegistrationNumber being sent:', payload.taxRegistrationNumber);
      const response = await axios.post('http://localhost:5108/api/vendor/create-profile', payload);
      
      // Upload document if provided
      if (document) {
        console.log('Response data:', response.data);
        const vendorId = response.data.vendorProfile?.vendorId;
        console.log('VendorId from response:', vendorId);
        
        if (vendorId) {
          try {
            console.log('Uploading document for vendorId:', vendorId);
            const documentFormData = new FormData();
            documentFormData.append('document', document);
            const uploadResponse = await axios.post(`http://localhost:5108/api/vendor/${vendorId}/upload-document`, documentFormData, {
              headers: { 'Content-Type': 'multipart/form-data' }
            });
            console.log('Document upload response:', uploadResponse.data);
          } catch (uploadError) {
            console.error('Document upload failed:', uploadError);
            // Don't fail the whole process if document upload fails
          }
        } else {
          console.error('No vendorId found in response');
        }
      }

      setSuccess('Vendor profile created successfully! Your account is pending admin approval. You will be notified once approved.');
      setTimeout(() => {
        navigate('/login');
      }, 2000);
    } catch (err: any) {
      setError(err.response?.data?.description || 'Failed to create vendor profile');
    } finally {
      setLoading(false);
    }
  };


  return (
    <div className="container mt-4">
      <div className="row justify-content-center">
        <div className="col-md-8">
          <div className="card shadow">
            <div className="card-body p-4">
              <h2 className="text-center mb-4">Complete Your Vendor Profile</h2>
              
              <p className="text-center mb-4">
                Welcome {email}! Please complete your business information to start selling.
              </p>

              {error && (
                <div className="alert alert-danger mb-3" role="alert">
                  {error}
                </div>
              )}
              {success && (
                <div className="alert alert-success mb-3" role="alert">
                  {success}
                </div>
              )}

              <form onSubmit={handleSubmit}>
                <div className="mb-3">
                  <label className="form-label">Business Name *</label>
                  <input
                    type="text"
                    className="form-control"
                    value={formData.businessName}
                    onChange={(e) => setFormData({ ...formData, businessName: e.target.value })}
                    required
                  />
                </div>

                <div className="mb-3">
                  <label className="form-label">Owner Name *</label>
                  <input
                    type="text"
                    className="form-control"
                    value={formData.ownerName}
                    onChange={(e) => setFormData({ ...formData, ownerName: e.target.value })}
                    required
                  />
                </div>

                <div className="mb-3">
                  <label className="form-label">Business License Number *</label>
                  <input
                    type="text"
                    className="form-control"
                    value={formData.businessLicenseNumber}
                    onChange={(e) => setFormData({ ...formData, businessLicenseNumber: e.target.value })}
                    required
                  />
                </div>

                <div className="mb-3">
                  <label className="form-label">Business Address *</label>
                  <textarea
                    className="form-control"
                    rows={3}
                    value={formData.businessAddress}
                    onChange={(e) => setFormData({ ...formData, businessAddress: e.target.value })}
                    required
                  />
                </div>

                <div className="row">
                  <div className="col-md-4 mb-3">
                    <label className="form-label">City *</label>
                    <input
                      type="text"
                      className="form-control"
                      value={formData.city}
                      onChange={(e) => setFormData({ ...formData, city: e.target.value })}
                      required
                    />
                  </div>
                  <div className="col-md-4 mb-3">
                    <label className="form-label">State *</label>
                    <input
                      type="text"
                      className="form-control"
                      value={formData.state}
                      onChange={(e) => setFormData({ ...formData, state: e.target.value })}
                      required
                    />
                  </div>
                  <div className="col-md-4 mb-3">
                    <label className="form-label">Pin Code *</label>
                    <input
                      type="text"
                      className="form-control"
                      value={formData.pinCode}
                      onChange={(e) => setFormData({ ...formData, pinCode: e.target.value.replace(/\D/g, '').slice(0, 6) })}
                      required
                      maxLength={6}
                    />
                  </div>
                </div>

                <div className="mb-3">
                  <label className="form-label">Tax Registration Number *</label>
                  <input
                    type="text"
                    className="form-control"
                    value={formData.taxRegistrationNumber}
                    onChange={(e) => setFormData({ ...formData, taxRegistrationNumber: e.target.value })}
                    required
                    minLength={1}
                    placeholder="Enter your tax registration number"
                  />
                  <small className="form-text text-muted">This field is required for vendor registration</small>
                </div>

                <div className="mb-3">
                  <label className="form-label">Business Document (PDF)</label>
                  <input
                    type="file"
                    className="form-control"
                    accept=".pdf"
                    onChange={(e) => setDocument(e.target.files?.[0] || null)}
                  />
                  <small className="form-text text-muted">Upload business license or registration document for admin verification (PDF only)</small>
                </div>

                <div className="mb-3">
                  <label className="form-label">Subscription Plan *</label>
                  <select
                    className="form-select"
                    value={formData.subscriptionPlanId}
                    onChange={(e) => {
                      console.log('Selected value:', e.target.value);
                      setFormData({ ...formData, subscriptionPlanId: e.target.value });
                    }}
                    required
                  >
                    <option value="" disabled>
                      Select a subscription plan
                    </option>
                    {subscriptionPlans.map((plan, index) => (
                      <option key={`plan-${plan.planId}-${index}`} value={String(plan.planId)}>
                        {plan.planName} - ${plan.price} (Max {plan.maxProducts} products)
                      </option>
                    ))}
                  </select>
                </div>

                <button
                  type="submit"
                  className="btn btn-primary w-100 mt-3"
                  disabled={loading || success.includes('successfully')}
                >
                  {loading ? (
                    <>
                      <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                      Loading...
                    </>
                  ) : (
                    'Complete Profile'
                  )}
                </button>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default VendorProfileSetup;
