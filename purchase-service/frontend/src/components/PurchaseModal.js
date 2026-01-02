import React, { useState } from 'react';
import apiService from '../services/apiService';

const PurchaseModal = ({ offer, currentUser, onSuccess, onCancel }) => {
  const [purchaseAmount, setPurchaseAmount] = useState(offer.offerAmount || '');
  const [notes, setNotes] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      const purchaseData = {
        offerId: offer.id,
        purchaseAmount: parseFloat(purchaseAmount),
        notes: notes.trim()
      };

      await apiService.createPurchase(currentUser.id, purchaseData);
      onSuccess();
    } catch (error) {
      console.error('Error creating purchase:', error);
      setError('Failed to create purchase. Please try again.');
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
        <h2>Purchase Vehicle</h2>
        
        {/* Offer Details */}
        <div className="offer-summary">
          <h3>{offer.year} {offer.make} {offer.model}</h3>
          <div className="offer-details">
            <p><strong>Listed Price:</strong> {formatPrice(offer.offerAmount)}</p>
            <p><strong>Condition:</strong> {offer.condition || 'N/A'}</p>
            <p><strong>VIN:</strong> {offer.vin || 'N/A'}</p>
            <p><strong>Location:</strong> {offer.address || 'N/A'}</p>
          </div>
        </div>

        {error && (
          <div className="error">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="purchaseAmount">Your Offer Amount ($):</label>
            <input
              type="number"
              id="purchaseAmount"
              value={purchaseAmount}
              onChange={(e) => setPurchaseAmount(e.target.value)}
              required
              min="0"
              step="0.01"
              placeholder="Enter your offer amount"
              disabled={loading}
            />
            <small>Suggested: {formatPrice(offer.offerAmount)}</small>
          </div>

          <div className="form-group">
            <label htmlFor="notes">Notes (Optional):</label>
            <textarea
              id="notes"
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
              placeholder="Any additional notes or comments about your purchase..."
              rows="3"
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
              disabled={loading || !purchaseAmount}
            >
              {loading ? 'Creating Purchase...' : 'Submit Purchase Request'}
            </button>
          </div>
        </form>

        <div className="purchase-info">
          <h4>Purchase Process</h4>
          <ol>
            <li>Submit your purchase request</li>
            <li>Wait for seller confirmation</li>
            <li>Complete payment and paperwork</li>
            <li>Vehicle transfer</li>
          </ol>
        </div>
      </div>

      <style jsx>{`
        .offer-summary {
          background: #f8f9fa;
          padding: 1rem;
          border-radius: 8px;
          margin-bottom: 1.5rem;
        }

        .offer-summary h3 {
          margin: 0 0 1rem 0;
          color: #2c3e50;
        }

        .offer-details p {
          margin: 0.5rem 0;
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
          background: #5a6268;
        }

        .purchase-info {
          background: #e9ecef;
          padding: 1rem;
          border-radius: 8px;
          margin-top: 2rem;
        }

        .purchase-info h4 {
          margin: 0 0 0.5rem 0;
          color: #495057;
        }

        .purchase-info ol {
          margin: 0;
          padding-left: 1.5rem;
        }

        .purchase-info li {
          margin: 0.25rem 0;
        }

        textarea {
          width: 100%;
          padding: 0.5rem;
          border: 1px solid #ddd;
          border-radius: 4px;
          font-family: inherit;
          resize: vertical;
        }

        small {
          color: #6c757d;
          font-size: 0.875rem;
        }
      `}</style>
    </div>
  );
};

export default PurchaseModal;