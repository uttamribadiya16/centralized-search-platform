import React, { useState, useEffect } from 'react';
import apiService from '../services/apiService';
import PurchaseModal from '../components/PurchaseModal';

const OfferListPage = ({ currentUser }) => {
  const [offers, setOffers] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [searchParams, setSearchParams] = useState({
    page: 1,
    pageSize: 10,
    make: '',
    model: '',
    year: '',
    minPrice: '',
    maxPrice: '',
    condition: ''
  });
  const [totalPages, setTotalPages] = useState(0);
  const [selectedOffer, setSelectedOffer] = useState(null);
  const [showPurchaseModal, setShowPurchaseModal] = useState(false);
  const [success, setSuccess] = useState('');

  useEffect(() => {
    loadOffers();
  }, [searchParams]);

  const loadOffers = async () => {
    setLoading(true);
    setError('');
    
    try {
      const response = await apiService.getAvailableOffers(searchParams);
      setOffers(response.items || []);
      setTotalPages(response.totalPages || 0);
    } catch (error) {
      console.error('Error loading offers:', error);
      setError('Failed to load offers. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const handleSearchChange = (field, value) => {
    setSearchParams(prev => ({
      ...prev,
      [field]: value,
      page: 1 // Reset to first page when searching
    }));
  };

  const handlePageChange = (page) => {
    setSearchParams(prev => ({ ...prev, page }));
  };

  const handlePurchaseOffer = (offer) => {
    setSelectedOffer(offer);
    setShowPurchaseModal(true);
  };

  const handlePurchaseSuccess = () => {
    setShowPurchaseModal(false);
    setSelectedOffer(null);
    setSuccess('Purchase request submitted successfully!');
    setTimeout(() => setSuccess(''), 5000);
    loadOffers(); // Refresh offers
  };

  const handlePurchaseCancel = () => {
    setShowPurchaseModal(false);
    setSelectedOffer(null);
  };

  const formatPrice = (price) => {
    return price ? `$${price.toLocaleString()}` : 'N/A';
  };

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString();
  };

  return (
    <div className="offer-list-page">
      <h2>Available Offers</h2>
      
      {error && (
        <div className="error">
          {error}
        </div>
      )}
      
      {success && (
        <div className="success">
          {success}
        </div>
      )}

      {/* Search Form */}
      <div className="search-form">
        <h3>Search Offers</h3>
        <div className="form-row">
          <div className="form-group">
            <label>Make:</label>
            <input
              type="text"
              value={searchParams.make}
              onChange={(e) => handleSearchChange('make', e.target.value)}
              placeholder="e.g. Toyota, Honda"
            />
          </div>
          <div className="form-group">
            <label>Model:</label>
            <input
              type="text"
              value={searchParams.model}
              onChange={(e) => handleSearchChange('model', e.target.value)}
              placeholder="e.g. Camry, Civic"
            />
          </div>
          <div className="form-group">
            <label>Year:</label>
            <input
              type="number"
              value={searchParams.year}
              onChange={(e) => handleSearchChange('year', e.target.value)}
              placeholder="e.g. 2020"
            />
          </div>
        </div>
        
        <div className="form-row">
          <div className="form-group">
            <label>Min Price:</label>
            <input
              type="number"
              value={searchParams.minPrice}
              onChange={(e) => handleSearchChange('minPrice', e.target.value)}
              placeholder="Minimum price"
            />
          </div>
          <div className="form-group">
            <label>Max Price:</label>
            <input
              type="number"
              value={searchParams.maxPrice}
              onChange={(e) => handleSearchChange('maxPrice', e.target.value)}
              placeholder="Maximum price"
            />
          </div>
          <div className="form-group">
            <label>Condition:</label>
            <select
              value={searchParams.condition}
              onChange={(e) => handleSearchChange('condition', e.target.value)}
            >
              <option value="">All Conditions</option>
              <option value="New">New</option>
              <option value="Excellent">Excellent</option>
              <option value="Good">Good</option>
              <option value="Fair">Fair</option>
              <option value="Poor">Poor</option>
            </select>
          </div>
        </div>
      </div>

      {/* Results */}
      {loading ? (
        <div className="loading">Loading offers...</div>
      ) : (
        <>
          <div className="results-info">
            <p>Showing page {searchParams.page} of {totalPages} ({offers.length} offers)</p>
          </div>

          {offers.length === 0 ? (
            <div className="no-results">
              <p>No offers found matching your criteria.</p>
            </div>
          ) : (
            <div className="offer-grid">
              {offers.map(offer => (
                <div key={offer.id} className="card">
                  <h3>{offer.year} {offer.make} {offer.model}</h3>
                  
                  <div className="card-details">
                    <div><strong>Price:</strong> {formatPrice(offer.offerAmount)}</div>
                    <div><strong>Condition:</strong> {offer.condition || 'N/A'}</div>
                    <div><strong>VIN:</strong> {offer.vin || 'N/A'}</div>
                    <div><strong>Location:</strong> {offer.address || 'N/A'}</div>
                    <div><strong>Listed:</strong> {formatDate(offer.createdAt)}</div>
                    <div><strong>Status:</strong> 
                      <span className={`status-badge status-${offer.status.toLowerCase()}`}>
                        {offer.status}
                      </span>
                    </div>
                  </div>

                  <div className="card-actions">
                    <button
                      className="btn btn-primary"
                      onClick={() => handlePurchaseOffer(offer)}
                      disabled={offer.status !== 'Available'}
                    >
                      {offer.status === 'Available' ? 'Purchase' : 'Not Available'}
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="pagination">
              <button
                onClick={() => handlePageChange(searchParams.page - 1)}
                disabled={searchParams.page <= 1}
              >
                Previous
              </button>
              
              {Array.from({ length: Math.min(5, totalPages) }, (_, i) => {
                const page = Math.max(1, searchParams.page - 2) + i;
                if (page > totalPages) return null;
                
                return (
                  <button
                    key={page}
                    onClick={() => handlePageChange(page)}
                    className={searchParams.page === page ? 'active' : ''}
                  >
                    {page}
                  </button>
                );
              })}
              
              <button
                onClick={() => handlePageChange(searchParams.page + 1)}
                disabled={searchParams.page >= totalPages}
              >
                Next
              </button>
            </div>
          )}
        </>
      )}

      {/* Purchase Modal */}
      {showPurchaseModal && selectedOffer && (
        <PurchaseModal
          offer={selectedOffer}
          currentUser={currentUser}
          onSuccess={handlePurchaseSuccess}
          onCancel={handlePurchaseCancel}
        />
      )}
    </div>
  );
};

export default OfferListPage;