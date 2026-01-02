import React, { useState } from 'react';
import offerService from '../services/offerService';
import './OfferList.css';

const OfferList = ({ offers, onOfferUpdated, onOfferDeleted, onRefresh }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [editingOffer, setEditingOffer] = useState(null);
  const [editFormData, setEditFormData] = useState({});

  const handleDelete = async (offerId) => {
    if (!window.confirm('Are you sure you want to delete this offer?')) {
      return;
    }

    setIsLoading(true);
    try {
      await offerService.deleteOffer(offerId);
      onOfferDeleted(offerId);
      alert('Offer deleted successfully!');
    } catch (error) {
      console.error('Error deleting offer:', error);
      alert('Failed to delete offer. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleEdit = (offer) => {
    setEditingOffer(offer.id);
    setEditFormData({
      offerAmount: offer.offerAmount || '',
      condition: offer.condition || '',
      address: offer.address || '',
      status: offer.status
    });
  };

  const handleCancelEdit = () => {
    setEditingOffer(null);
    setEditFormData({});
  };

  const handleSaveEdit = async () => {
    setIsLoading(true);
    try {
      const updatedOffer = await offerService.updateOffer(editingOffer, editFormData);
      onOfferUpdated(updatedOffer);
      setEditingOffer(null);
      setEditFormData({});
      alert('Offer updated successfully!');
    } catch (error) {
      console.error('Error updating offer:', error);
      alert('Failed to update offer. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleEditFormChange = (e) => {
    const { name, value } = e.target;
    setEditFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  };

  if (offers.length === 0) {
    return (
      <div className="offer-list">
        <div className="no-offers">
          <h3>No Offers Found</h3>
          <p>You haven't created any vehicle offers yet.</p>
          <p>Create your first offer to get started!</p>
        </div>
      </div>
    );
  }

  return (
    <div className="offer-list">
      <div className="list-header">
        <h2>My Vehicle Offers ({offers.length})</h2>
        <button onClick={onRefresh} className="refresh-button" disabled={isLoading}>
          Refresh
        </button>
      </div>

      <div className="offers-grid">
        {offers.map(offer => (
          <div key={offer.id} className="offer-card">
            <div className="offer-header">
              <h3>{offer.year} {offer.make} {offer.model}</h3>
              <span className={`status-badge ${offer.status ? offer.status.toLowerCase() : 'unknown'}`}>
                {offer.status || 'Unknown'}
              </span>
            </div>

            <div className="offer-content">
              <div className="offer-details">
                <div className="detail-row">
                  <span className="label">VIN:</span>
                  <span className="value">{offer.vin || 'Not specified'}</span>
                </div>
                
                {editingOffer === offer.id ? (
                  <div className="edit-form">
                    <div className="detail-row">
                      <span className="label">Price:</span>
                      <input
                        type="number"
                        name="offerAmount"
                        value={editFormData.offerAmount}
                        onChange={handleEditFormChange}
                        min="0"
                        step="0.01"
                      />
                    </div>
                    
                    <div className="detail-row">
                      <span className="label">Condition:</span>
                      <select
                        name="condition"
                        value={editFormData.condition}
                        onChange={handleEditFormChange}
                      >
                        <option value="Excellent">Excellent</option>
                        <option value="Good">Good</option>
                        <option value="Fair">Fair</option>
                        <option value="Poor">Poor</option>
                      </select>
                    </div>
                    
                    <div className="detail-row">
                      <span className="label">Address:</span>
                      <input
                        type="text"
                        name="address"
                        value={editFormData.address}
                        onChange={handleEditFormChange}
                        placeholder="Enter location"
                      />
                    </div>
                    
                    <div className="detail-row">
                      <span className="label">Status:</span>
                      <select
                        name="status"
                        value={editFormData.status}
                        onChange={handleEditFormChange}
                      >
                        <option value="Available">Available</option>
                        <option value="Pending">Pending</option>
                        <option value="Sold">Sold</option>
                        <option value="Inactive">Inactive</option>
                      </select>
                    </div>
                  </div>
                ) : (
                  <div className="view-details">
                    {offer.offerAmount && (
                      <div className="detail-row">
                        <span className="label">Price:</span>
                        <span className="value">${offer.offerAmount.toLocaleString()}</span>
                      </div>
                    )}
                    
                    {offer.condition && (
                      <div className="detail-row">
                        <span className="label">Condition:</span>
                        <span className="value">{offer.condition}</span>
                      </div>
                    )}
                    
                    {offer.address && (
                      <div className="detail-row">
                        <span className="label">Location:</span>
                        <span className="value">{offer.address}</span>
                      </div>
                    )}
                    
                    <div className="detail-row">
                      <span className="label">Created:</span>
                      <span className="value">{formatDate(offer.createdAt)}</span>
                    </div>
                  </div>
                )}
              </div>
            </div>

            <div className="offer-actions">
              {editingOffer === offer.id ? (
                <div className="edit-actions">
                  <button
                    onClick={handleSaveEdit}
                    className="save-button"
                    disabled={isLoading}
                  >
                    Save
                  </button>
                  <button
                    onClick={handleCancelEdit}
                    className="cancel-button"
                    disabled={isLoading}
                  >
                    Cancel
                  </button>
                </div>
              ) : (
                <div className="view-actions">
                  <button
                    onClick={() => handleEdit(offer)}
                    className="edit-button"
                    disabled={isLoading}
                  >
                    Edit
                  </button>
                  <button
                    onClick={() => handleDelete(offer.id)}
                    className="delete-button"
                    disabled={isLoading}
                  >
                    Delete
                  </button>
                </div>
              )}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default OfferList;