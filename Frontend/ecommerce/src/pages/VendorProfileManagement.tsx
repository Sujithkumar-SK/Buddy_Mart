import React, { useState, useEffect } from 'react';
import { vendorService } from '../Services/vendorService';
import type { VendorProfile, VendorProfileUpdate } from '../types/vendor';

const VendorProfileManagement: React.FC = () => {
  const [profile, setProfile] = useState<VendorProfile | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const [formData, setFormData] = useState<VendorProfileUpdate>({
    businessName: '',
    ownerName: '',
    businessLicenseNumber: '',
    businessAddress: '',
    city: '',
    state: '',
    pinCode: '',
    taxRegistrationNumber: ''
  });

  useEffect(() => {
    loadProfile();
  }, []);

  const loadProfile = async () => {
    try {
      setLoading(true);
      const profileData = await vendorService.getMyProfile();
      setProfile(profileData);
      setFormData({
        businessName: profileData.businessName,
        ownerName: profileData.ownerName,
        businessLicenseNumber: profileData.businessLicenseNumber,
        businessAddress: profileData.businessAddress,
        city: profileData.city || '',
        state: profileData.state || '',
        pinCode: profileData.pinCode || '',
        taxRegistrationNumber: profileData.taxRegistrationNumber || ''
      });
    } catch (err: any) {
      setError(err.response?.data?.description || 'Failed to load profile');
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setError('');
    setSuccess('');

    try {
      const updatedProfile = await vendorService.updateMyProfile(formData);
      setProfile(updatedProfile);
      setSuccess('Profile updated successfully!');
    } catch (err: any) {
      setError(err.response?.data?.description || 'Failed to update profile');
    } finally {
      setSaving(false);
    }
  };

  const handleInputChange = (field: keyof VendorProfileUpdate) => (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    setFormData(prev => ({
      ...prev,
      [field]: e.target.value
    }));
  };



  if (loading) {
    return (
      <div className="container mt-4 text-center">
        <div className="spinner-border" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
        <p className="mt-2">Loading profile...</p>
      </div>
    );
  }

  return (
    <div className="container mt-4">
      <div className="card">
        <div className="card-body">
          <h4 className="card-title mb-4">Vendor Profile Management</h4>

          {profile && (
            <div className="alert alert-info mb-3">
              <strong>Status:</strong> {profile.status} | 
              <strong> Current Plan:</strong> {profile.currentPlan}
            </div>
          )}

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
            <div className="row">
              <div className="col-md-6 mb-3">
                <label className="form-label">Business Name *</label>
                <input
                  type="text"
                  className="form-control"
                  value={formData.businessName}
                  onChange={handleInputChange('businessName')}
                  required
                />
              </div>
              <div className="col-md-6 mb-3">
                <label className="form-label">Owner Name *</label>
                <input
                  type="text"
                  className="form-control"
                  value={formData.ownerName}
                  onChange={handleInputChange('ownerName')}
                  required
                />
              </div>
            </div>

            <div className="mb-3">
              <label className="form-label">Business License Number *</label>
              <input
                type="text"
                className="form-control"
                value={formData.businessLicenseNumber}
                onChange={handleInputChange('businessLicenseNumber')}
                required
              />
            </div>

            <div className="mb-3">
              <label className="form-label">Business Address *</label>
              <textarea
                className="form-control"
                rows={3}
                value={formData.businessAddress}
                onChange={handleInputChange('businessAddress')}
                required
              />
            </div>

            <div className="row">
              <div className="col-md-4 mb-3">
                <label className="form-label">City</label>
                <input
                  type="text"
                  className="form-control"
                  value={formData.city}
                  onChange={handleInputChange('city')}
                />
              </div>
              <div className="col-md-4 mb-3">
                <label className="form-label">State</label>
                <input
                  type="text"
                  className="form-control"
                  value={formData.state}
                  onChange={handleInputChange('state')}
                />
              </div>
              <div className="col-md-4 mb-3">
                <label className="form-label">Pin Code</label>
                <input
                  type="text"
                  className="form-control"
                  value={formData.pinCode}
                  onChange={(e) => {
                    const value = e.target.value.replace(/\D/g, '').slice(0, 6);
                    setFormData(prev => ({ ...prev, pinCode: value }));
                  }}
                  maxLength={6}
                />
              </div>
            </div>

            <div className="mb-3">
              <label className="form-label">Tax Registration Number</label>
              <input
                type="text"
                className="form-control"
                value={formData.taxRegistrationNumber}
                onChange={handleInputChange('taxRegistrationNumber')}
              />
            </div>

            {profile?.documentPath && (
              <div className="mb-3">
                <label className="form-label">Business Document</label>
                <div className="alert alert-info">
                  <small className="text-success">✓ Document uploaded during registration</small>
                  <a href={`http://localhost:5108${profile.documentPath}`} target="_blank" rel="noopener noreferrer" className="ms-2 btn btn-sm btn-outline-primary">
                    View Document
                  </a>
                </div>
              </div>
            )}

            <button
              type="submit"
              className="btn btn-primary btn-lg"
              disabled={saving}
            >
              {saving ? (
                <>
                  <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                  Updating...
                </>
              ) : (
                'Update Profile'
              )}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
};

export default VendorProfileManagement;