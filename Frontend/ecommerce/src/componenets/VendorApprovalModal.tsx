import React, { useState } from 'react';

interface VendorApprovalModalProps {
  vendor: {
    vendorId: number;
    businessName: string;
    ownerName: string;
    email: string;
    phone: string;
    businessLicenseNumber: string;
    businessAddress: string;
    city: string;
    state: string;
    pinCode: string;
    taxRegistrationNumber?: string;
    documentPath?: string;
    createdOn: string;
  } | null;
  isOpen: boolean;
  onClose: () => void;
  onApprove: (vendorId: number, reason: string) => void;
  onReject: (vendorId: number, reason: string) => void;
}

const VendorApprovalModal: React.FC<VendorApprovalModalProps> = ({
  vendor,
  isOpen,
  onClose,
  onApprove,
  onReject
}) => {
  const [reason, setReason] = useState('');
  const [action, setAction] = useState<'approve' | 'reject' | null>(null);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!vendor || !action || !reason.trim()) return;

    if (action === 'approve') {
      onApprove(vendor.vendorId, reason);
    } else {
      onReject(vendor.vendorId, reason);
    }

    setReason('');
    setAction(null);
    onClose();
  };

  const handleClose = () => {
    setReason('');
    setAction(null);
    onClose();
  };

  if (!isOpen || !vendor) return null;

  return (
    <div className="modal show d-block" tabIndex={-1} style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
      <div className="modal-dialog modal-lg">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">Vendor Application Details</h5>
            <button type="button" className="btn-close" onClick={handleClose}></button>
          </div>
          
          <div className="modal-body">
            <div className="row">
              <div className="col-md-6">
                <h6>Business Information</h6>
                <p><strong>Business Name:</strong> {vendor.businessName}</p>
                <p><strong>Owner Name:</strong> {vendor.ownerName}</p>
                <p><strong>License Number:</strong> {vendor.businessLicenseNumber}</p>
                <p><strong>Tax Registration:</strong> {vendor.taxRegistrationNumber || 'N/A'}</p>
              </div>
              
              <div className="col-md-6">
                <h6>Contact Information</h6>
                <p><strong>Email:</strong> {vendor.email}</p>
                <p><strong>Phone:</strong> {vendor.phone}</p>
                <p><strong>Applied On:</strong> {new Date(vendor.createdOn).toLocaleDateString()}</p>
              </div>
            </div>
            
            <div className="row mt-3">
              <div className="col-12">
                <h6>Address</h6>
                <p>{vendor.businessAddress}, {vendor.city}, {vendor.state} - {vendor.pinCode}</p>
              </div>
            </div>

            <div className="row mt-3">
              <div className="col-12">
                <h6>Documents</h6>
                {vendor.documentPath && vendor.documentPath !== '/temp/pending' ? (
                  <a href={`http://localhost:5108${vendor.documentPath}`} target="_blank" rel="noopener noreferrer" className="btn btn-outline-primary btn-sm">
                    View Document
                  </a>
                ) : (
                  <span className="badge bg-warning">No document uploaded</span>
                )}
              </div>
            </div>

            <form onSubmit={handleSubmit} className="mt-4">
              <div className="mb-3">
                <label htmlFor="reason" className="form-label">
                  {action === 'approve' ? 'Approval Reason' : action === 'reject' ? 'Rejection Reason' : 'Reason'}
                </label>
                <textarea
                  className="form-control"
                  id="reason"
                  rows={3}
                  value={reason}
                  onChange={(e) => setReason(e.target.value)}
                  placeholder={action === 'approve' ? 'Enter approval reason...' : action === 'reject' ? 'Enter rejection reason...' : 'Enter reason for approval or rejection...'}
                  required
                />
              </div>
            </form>
          </div>
          
          <div className="modal-footer">
            <button type="button" className="btn btn-secondary" onClick={handleClose}>
              Cancel
            </button>
            <button 
              type="button" 
              className="btn btn-success me-2"
              onClick={() => setAction('approve')}
              disabled={action === 'approve'}
            >
              {action === 'approve' ? 'Selected: Approve' : 'Approve'}
            </button>
            <button 
              type="button" 
              className="btn btn-danger me-2"
              onClick={() => setAction('reject')}
              disabled={action === 'reject'}
            >
              {action === 'reject' ? 'Selected: Reject' : 'Reject'}
            </button>
            {action && reason.trim() && (
              <button 
                type="button" 
                className={`btn ${action === 'approve' ? 'btn-success' : 'btn-danger'}`}
                onClick={handleSubmit}
              >
                Confirm {action === 'approve' ? 'Approval' : 'Rejection'}
              </button>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default VendorApprovalModal;