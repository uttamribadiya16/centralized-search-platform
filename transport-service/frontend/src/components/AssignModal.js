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
    <div className="modal">
      <div className="modal-content">
        <h2>Assign Purchase to Transport</h2>
        
        {/* Purchase Details */}
        <div className="purchase-summary">
          <h3>{purchase.year} {purchase.make} {purchase.model}</h3>
          <div className="purchase-details">
            <p><strong>Purchase Price:</strong> {formatPrice(purchase.purchaseAmount)}</p>
            <p><strong>Condition:</strong> {purchase.condition || 'N/A'}</p>
            <p><strong>VIN:</strong> {purchase.vin || 'N/A'}</p>
            <p><strong>Current Location:</strong> {purchase.address || 'N/A'}</p>
            <p><strong>Buyer:</strong> {purchase.buyerEmail || 'N/A'}</p>
          </div>
        </div>

        {error && (
          <div className="error">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit}>
          <div className="form-row">
            <div className="form-group">
              <label htmlFor="originLocation">Origin Location:</label>
              <input
                type="text"
                id="originLocation"
                value={transportDetails.originLocation}
                onChange={(e) => setTransportDetails(prev => ({...prev, originLocation: e.target.value}))}
                required
                placeholder="Where to pick up the vehicle"
                disabled={loading}
              />
            </div>

            <div className="form-group">
              <label htmlFor="destinationLocation">Destination Location:</label>
              <input
                type="text"
                id="destinationLocation"
                value={transportDetails.destinationLocation}
                onChange={(e) => setTransportDetails(prev => ({...prev, destinationLocation: e.target.value}))}
                required
                placeholder="Where to deliver the vehicle"
                disabled={loading}
              />
            </div>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="vehicleType">Transport Vehicle Type:</label>
              <select
                id="vehicleType"
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

            <div className="form-group">
              <label htmlFor="capacity">Transport Capacity (kg):</label>
              <input
                type="number"
                id="capacity"
                value={transportDetails.capacity}
                onChange={(e) => setTransportDetails(prev => ({...prev, capacity: parseInt(e.target.value)}))}
                required
                min="100"
                step="100"
                placeholder="Transport capacity in kg"
                disabled={loading}
              />
            </div>
          </div>

          <div className="form-group">
            <label htmlFor="estimatedDeliveryDate">Estimated Delivery Date:</label>
            <input
              type="date"
              id="estimatedDeliveryDate"
              value={transportDetails.estimatedDeliveryDate}
              onChange={(e) => setTransportDetails(prev => ({...prev, estimatedDeliveryDate: e.target.value}))}
              min={new Date().toISOString().split('T')[0]}
              disabled={loading}
            />
          </div>

          <div className="modal-actions">
            <button
              type="button"
              onClick={onCancel}
              className="btn btn-secondary"
              disabled={loading}
            >
              Cancel
            </button>
            <button
              type="submit"
              className="btn btn-primary"
              disabled={loading || !transportDetails.originLocation || !transportDetails.destinationLocation}
            >
              {loading ? 'Assigning...' : 'Assign to Transport'}
            </button>
          </div>
        </form>

        <div className="assignment-info">
          <h4>Transport Assignment Process</h4>
          <ol>
            <li>Create transport route with pickup and delivery locations</li>
            <li>Assign purchase to your transport</li>
            <li>Coordinate with buyer for pickup</li>
            <li>Transport vehicle to destination</li>
            <li>Complete delivery and update status</li>
          </ol>
        </div>
      </div>

      <style jsx>{`
        .purchase-summary {
          background: #f8f9fa;
          padding: 1rem;
          border-radius: 8px;
          margin-bottom: 1.5rem;
        }

        .purchase-summary h3 {
          margin: 0 0 1rem 0;
          color: #2c3e50;
        }

        .purchase-details p {
          margin: 0.5rem 0;
        }

        .form-row {
          display: grid;
          grid-template-columns: 1fr 1fr;
          gap: 1rem;
          margin-bottom: 1rem;
        }

        .modal-actions {
          display: flex;
          gap: 1rem;
          justify-content: flex-end;
          margin-top: 2rem;
        }

        .btn-secondary {
          background: #6c757d;
          color: white;
          border: none;
          padding: 0.75rem 1.5rem;
          border-radius: 4px;
          cursor: pointer;
        }

        .btn-secondary:hover {
          background: #545b62;
        }

        .btn-primary {
          background: #007bff;
          color: white;
          border: none;
          padding: 0.75rem 1.5rem;
          border-radius: 4px;
          cursor: pointer;
        }

        .btn-primary:hover {
          background: #0056b3;
        }

        .btn-primary:disabled,
        .btn-secondary:disabled {
          opacity: 0.6;
          cursor: not-allowed;
        }

        .assignment-info {
          background: #e8f4f8;
          padding: 1.5rem;
          border-radius: 8px;
          margin-top: 1.5rem;
          border: 1px solid #bee5eb;
        }

        .assignment-info h4 {
          margin: 0 0 1rem 0;
          color: #0c5460;
        }

        .assignment-info ol {
          margin: 0;
          padding-left: 1.2rem;
          color: #155724;
        }

        .assignment-info li {
          margin-bottom: 0.5rem;
        }
      `}</style>
    </div>
  );
};

export default AssignModal;