import React, { useState } from 'react';
import apiService from '../services/api';

const AssignModal = ({ purchase, currentUser, onSuccess, onCancel }) => {
  const [transportDetails, setTransportDetails] = useState({
    originLocation: purchase.address || '',
    destinationLocation: '',
    vehicleType: 'Truck',
    capacity: 1000,
    estimatedDeliveryDate: ''
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      const assignmentData = {
        purchaseId: purchase.id,
        carrierId: currentUser.id,
        ...transportDetails
      };

      await apiService.assignPurchaseToTransport(assignmentData);
      onSuccess();
    } catch (error) {
      console.error('Error assigning purchase:', error);
      setError('Failed to assign purchase to transport. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const formatPrice = (price) => {
    return price ? `$${price.toLocaleString()}` : 'N/A';
  };

  return (
    <div className="modal d-block" tabIndex="-1" style={{backgroundColor: 'rgba(0,0,0,0.5)'}}>
      <div className="modal-dialog modal-lg">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">Assign Purchase to Transport</h5>
            <button 
              type="button" 
              className="btn-close" 
              onClick={onCancel}
              disabled={loading}
            ></button>
          </div>
          
          <div className="modal-body">
            {/* Error Message */}
            {error && (
              <div className="alert alert-danger" role="alert">
                <i className="bi bi-exclamation-triangle me-2"></i>
                {error}
              </div>
            )}

            {/* Purchase Details */}
            <div className="card mb-4">
              <div className="card-header">
                <h6 className="mb-0">Purchase Details</h6>
              </div>
              <div className="card-body">
                <h5>{purchase.year} {purchase.make} {purchase.model}</h5>
                <div className="row">
                  <div className="col-md-6">
                    <p><strong>Purchase Price:</strong> {formatPrice(purchase.purchaseAmount)}</p>
                    <p><strong>VIN:</strong> {purchase.vin || 'Not specified'}</p>
                  </div>
                  <div className="col-md-6">
                    <p><strong>Condition:</strong> {purchase.condition || 'Not specified'}</p>
                    <p><strong>Address:</strong> {purchase.address || 'Not specified'}</p>
                  </div>
                </div>
              </div>
            </div>

            {/* Transport Form */}
            <form onSubmit={handleSubmit}>
              <div className="row mb-3">
                <div className="col-md-6">
                  <label htmlFor="originLocation" className="form-label">Origin Location *</label>
                  <input
                    type="text"
                    id="originLocation"
                    className="form-control"
                    value={transportDetails.originLocation}
                    onChange={(e) => setTransportDetails(prev => ({...prev, originLocation: e.target.value}))}
                    required
                    placeholder="Where to pick up the vehicle"
                    disabled={loading}
                  />
                </div>
                <div className="col-md-6">
                  <label htmlFor="destinationLocation" className="form-label">Destination Location *</label>
                  <input
                    type="text"
                    id="destinationLocation"
                    className="form-control"
                    value={transportDetails.destinationLocation}
                    onChange={(e) => setTransportDetails(prev => ({...prev, destinationLocation: e.target.value}))}
                    required
                    placeholder="Where to deliver the vehicle"
                    disabled={loading}
                  />
                </div>
              </div>

              <div className="row mb-3">
                <div className="col-md-6">
                  <label htmlFor="vehicleType" className="form-label">Transport Vehicle Type *</label>
                  <select
                    id="vehicleType"
                    className="form-select"
                    value={transportDetails.vehicleType}
                    onChange={(e) => setTransportDetails(prev => ({...prev, vehicleType: e.target.value}))}
                    required
                    disabled={loading}
                  >
                    <option value="Truck">Truck</option>
                    <option value="Van">Van</option>
                    <option value="Motorcycle">Motorcycle</option>
                    <option value="Ship">Ship</option>
                    <option value="Plane">Plane</option>
                  </select>
                </div>
                <div className="col-md-6">
                  <label htmlFor="capacity" className="form-label">Capacity (kg) *</label>
                  <input
                    type="number"
                    id="capacity"
                    className="form-control"
                    value={transportDetails.capacity}
                    onChange={(e) => setTransportDetails(prev => ({...prev, capacity: parseInt(e.target.value)}))}
                    required
                    min="1"
                    placeholder="Transport capacity in kg"
                    disabled={loading}
                  />
                </div>
              </div>

              <div className="mb-3">
                <label htmlFor="estimatedDeliveryDate" className="form-label">Estimated Delivery Date</label>
                <input
                  type="date"
                  id="estimatedDeliveryDate"
                  className="form-control"
                  value={transportDetails.estimatedDeliveryDate}
                  onChange={(e) => setTransportDetails(prev => ({...prev, estimatedDeliveryDate: e.target.value}))}
                  min={new Date().toISOString().split('T')[0]}
                  disabled={loading}
                />
              </div>

              <div className="alert alert-info">
                <i className="bi bi-info-circle me-2"></i>
                <strong>Transport Assignment Process:</strong>
                <ol className="mb-0 mt-2">
                  <li>Create transport route with pickup and delivery locations</li>
                  <li>Assign purchase to your transport</li>
                  <li>Coordinate pickup and delivery with buyer and seller</li>
                </ol>
              </div>
            </form>
          </div>

          <div className="modal-footer">
            <button
              type="button"
              onClick={onCancel}
              className="btn btn-secondary"
              disabled={loading}
            >
              Cancel
            </button>
            <button
              type="button"
              onClick={handleSubmit}
              className="btn btn-primary"
              disabled={loading || !transportDetails.originLocation || !transportDetails.destinationLocation}
            >
              {loading ? (
                <>
                  <span className="spinner-border spinner-border-sm me-2" role="status"></span>
                  Assigning...
                </>
              ) : (
                'Assign to Transport'
              )}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default AssignModal;