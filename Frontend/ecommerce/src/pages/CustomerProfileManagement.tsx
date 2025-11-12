import React, { useState, useEffect } from 'react';
import { customerService, type CustomerProfile, type CustomerProfileUpdate } from '../Services/customerService';

const CustomerProfileManagement: React.FC = () => {
  const [profile, setProfile] = useState<CustomerProfile | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [isEditing, setIsEditing] = useState(false);

  const [formData, setFormData] = useState<CustomerProfileUpdate>({
    firstName: '',
    middleName: '',
    lastName: '',
    dateOfBirth: '',
    gender: undefined,
    address: '',
    city: '',
    state: '',
    pinCode: ''
  });

  useEffect(() => {
    loadProfile();
  }, []);

  const loadProfile = async () => {
    try {
      setLoading(true);
      const profileData = await customerService.getMyProfile();
      setProfile(profileData);
      setFormData({
        firstName: profileData.firstName,
        middleName: profileData.middleName || '',
        lastName: profileData.lastName,
        dateOfBirth: profileData.dateOfBirth ? profileData.dateOfBirth.split('T')[0] : '',
        gender: profileData.gender === 'Male' ? 1 : profileData.gender === 'Female' ? 2 : profileData.gender === 'Other' ? 3 : undefined,
        address: profileData.address || '',
        city: profileData.city || '',
        state: profileData.state || '',
        pinCode: profileData.pinCode || ''
      });
    } catch (err: any) {
      setError('Failed to load profile');
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setSaving(true);
      setError('');
      const updatedProfile = await customerService.updateMyProfile(formData);
      setProfile(updatedProfile);
      setSuccess('Profile updated successfully');
      setIsEditing(false);
    } catch (err: any) {
      setError(err.response?.data?.description || 'Failed to update profile');
    } finally {
      setSaving(false);
    }
  };

  const handleCancel = () => {
    if (profile) {
      setFormData({
        firstName: profile.firstName,
        middleName: profile.middleName || '',
        lastName: profile.lastName,
        dateOfBirth: profile.dateOfBirth ? profile.dateOfBirth.split('T')[0] : '',
        gender: profile.gender === 'Male' ? 1 : profile.gender === 'Female' ? 2 : profile.gender === 'Other' ? 3 : undefined,
        address: profile.address || '',
        city: profile.city || '',
        state: profile.state || '',
        pinCode: profile.pinCode || ''
      });
    }
    setIsEditing(false);
    setError('');
  };

  if (loading) {
    return (
      <div className="d-flex justify-content-center py-5">
        <div className="spinner-border" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
      </div>
    );
  }

  return (
    <div className="container-fluid">
      <div className="row justify-content-center">
        <div className="col-md-8">
          <div className="card border-0 shadow-sm">
            <div className="card-header bg-primary text-white">
              <div className="d-flex justify-content-between align-items-center">
                <h5 className="mb-0">
                  <i className="bi bi-person-circle me-2"></i>My Profile
                </h5>
                {!isEditing && (
                  <button 
                    className="btn btn-light btn-sm"
                    onClick={() => setIsEditing(true)}
                  >
                    <i className="bi bi-pencil me-1"></i>Edit Profile
                  </button>
                )}
              </div>
            </div>

            <div className="card-body">
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

              {isEditing ? (
                <form onSubmit={handleSubmit}>
                  <div className="row">
                    <div className="col-md-4 mb-3">
                      <label className="form-label">First Name *</label>
                      <input
                        type="text"
                        className="form-control"
                        value={formData.firstName}
                        onChange={(e) => setFormData({...formData, firstName: e.target.value})}
                        required
                      />
                    </div>
                    <div className="col-md-4 mb-3">
                      <label className="form-label">Middle Name</label>
                      <input
                        type="text"
                        className="form-control"
                        value={formData.middleName}
                        onChange={(e) => setFormData({...formData, middleName: e.target.value})}
                      />
                    </div>
                    <div className="col-md-4 mb-3">
                      <label className="form-label">Last Name *</label>
                      <input
                        type="text"
                        className="form-control"
                        value={formData.lastName}
                        onChange={(e) => setFormData({...formData, lastName: e.target.value})}
                        required
                      />
                    </div>
                    <div className="col-md-6 mb-3">
                      <label className="form-label">Date of Birth</label>
                      <input
                        type="date"
                        className="form-control"
                        value={formData.dateOfBirth}
                        onChange={(e) => setFormData({...formData, dateOfBirth: e.target.value})}
                      />
                    </div>
                    <div className="col-md-6 mb-3">
                      <label className="form-label">Gender</label>
                      <select
                        className="form-select"
                        value={formData.gender || ''}
                        onChange={(e) => setFormData({...formData, gender: e.target.value ? parseInt(e.target.value) : undefined})}
                      >
                        <option value="">Select Gender</option>
                        <option value="1">Male</option>
                        <option value="2">Female</option>
                        <option value="3">Other</option>
                      </select>
                    </div>
                    <div className="col-12 mb-3">
                      <label className="form-label">Address</label>
                      <textarea
                        className="form-control"
                        rows={3}
                        value={formData.address}
                        onChange={(e) => setFormData({...formData, address: e.target.value})}
                      />
                    </div>
                    <div className="col-md-4 mb-3">
                      <label className="form-label">City</label>
                      <input
                        type="text"
                        className="form-control"
                        value={formData.city}
                        onChange={(e) => setFormData({...formData, city: e.target.value})}
                      />
                    </div>
                    <div className="col-md-4 mb-3">
                      <label className="form-label">State</label>
                      <input
                        type="text"
                        className="form-control"
                        value={formData.state}
                        onChange={(e) => setFormData({...formData, state: e.target.value})}
                      />
                    </div>
                    <div className="col-md-4 mb-3">
                      <label className="form-label">Pin Code</label>
                      <input
                        type="text"
                        className="form-control"
                        value={formData.pinCode}
                        onChange={(e) => setFormData({...formData, pinCode: e.target.value})}
                        pattern="[0-9]{6}"
                        maxLength={6}
                      />
                    </div>
                  </div>

                  <div className="d-flex gap-2">
                    <button 
                      type="submit" 
                      className="btn btn-primary"
                      disabled={saving}
                    >
                      {saving ? (
                        <>
                          <span className="spinner-border spinner-border-sm me-2"></span>
                          Saving...
                        </>
                      ) : (
                        <>
                          <i className="bi bi-check-lg me-1"></i>Save Changes
                        </>
                      )}
                    </button>
                    <button 
                      type="button" 
                      className="btn btn-secondary"
                      onClick={handleCancel}
                      disabled={saving}
                    >
                      <i className="bi bi-x-lg me-1"></i>Cancel
                    </button>
                  </div>
                </form>
              ) : (
                <div className="row">
                  <div className="col-md-6">
                    <div className="mb-3">
                      <label className="form-label text-muted">Full Name</label>
                      <p className="fw-bold">{profile?.fullName || 'Not provided'}</p>
                    </div>
                    <div className="mb-3">
                      <label className="form-label text-muted">Email</label>
                      <p className="fw-bold">{profile?.email}</p>
                    </div>
                    <div className="mb-3">
                      <label className="form-label text-muted">Phone</label>
                      <p className="fw-bold">{profile?.phone}</p>
                    </div>
                    <div className="mb-3">
                      <label className="form-label text-muted">Date of Birth</label>
                      <p className="fw-bold">
                        {profile?.dateOfBirth ? new Date(profile.dateOfBirth).toLocaleDateString() : 'Not provided'}
                      </p>
                    </div>
                  </div>
                  <div className="col-md-6">
                    <div className="mb-3">
                      <label className="form-label text-muted">Gender</label>
                      <p className="fw-bold">{profile?.gender || 'Not provided'}</p>
                    </div>
                    <div className="mb-3">
                      <label className="form-label text-muted">Address</label>
                      <p className="fw-bold">{profile?.address || 'Not provided'}</p>
                    </div>
                    <div className="mb-3">
                      <label className="form-label text-muted">City, State</label>
                      <p className="fw-bold">
                        {profile?.city && profile?.state 
                          ? `${profile.city}, ${profile.state}` 
                          : profile?.city || profile?.state || 'Not provided'}
                      </p>
                    </div>
                    <div className="mb-3">
                      <label className="form-label text-muted">Pin Code</label>
                      <p className="fw-bold">{profile?.pinCode || 'Not provided'}</p>
                    </div>
                  </div>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default CustomerProfileManagement;