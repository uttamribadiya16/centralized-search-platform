import React, { useState, useEffect } from 'react';
import apiService from '../services/apiService';

const PurchaseListPage = ({ currentUser }) => {
  const [purchases, setPurchases] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [searchParams, setSearchParams] = useState({
    page: 1,
    pageSize: 10,
    status: '',
    make: '',
    model: '',
    year: '',
    fromDate: '',
    toDate: ''
  });
  const [totalPages, setTotalPages] = useState(0);
  const [success, setSuccess] = useState('');

  useEffect(() => {
    if (currentUser?.id) {
      loadPurchases();
    }
  }, [searchParams, currentUser]);

  const loadPurchases = async () => {
    if (!currentUser?.id) return;
    
    setLoading(true);
    setError('');
    
    try {
      const response = await apiService.getPurchasesByBuyer(currentUser.id, searchParams);
      setPurchases(response.items || []);
      setTotalPages(response.totalPages || 0);
    } catch (error) {
      console.error('Error loading purchases:', error);
      setError('Failed to load purchases. Please try again.');
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

  const handleUpdateStatus = async (purchaseId, newStatus) => {
    try {
      await apiService.updatePurchase(purchaseId, { status: parseInt(newStatus) });
      setSuccess('Purchase status updated successfully!');
      setTimeout(() => setSuccess(''), 3000);
      loadPurchases(); // Refresh list
    } catch (error) {
      console.error('Error updating purchase:', error);
      setError('Failed to update purchase status.');
    }
  };

  const handleDeletePurchase = async (purchaseId) => {
    if (!window.confirm('Are you sure you want to delete this purchase?')) {
      return;
    }
    
    try {
      await apiService.deletePurchase(purchaseId);
      setSuccess('Purchase deleted successfully!');
      setTimeout(() => setSuccess(''), 3000);
      loadPurchases(); // Refresh list
    } catch (error) {
      console.error('Error deleting purchase:', error);
      setError('Failed to delete purchase.');
    }
  };

  const formatPrice = (price) => {
    return price ? `$${price.toLocaleString()}` : 'N/A';
  };

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString();
  };

  const getStatusColor = (status) => {
    const statusMap = {
      1: 'pending',
      2: 'confirmed', 
      3: 'in-progress',
      4: 'completed',
      5: 'cancelled',
      6: 'refunded'
    };
    return statusMap[status] || 'pending';
  };

  const getStatusName = (status) => {
    const statusMap = {
      1: 'Pending',
      2: 'Confirmed',
      3: 'In Progress',
      4: 'Completed',
      5: 'Cancelled',
      6: 'Refunded'
    };
    return statusMap[status] || 'Unknown';
  };

  return (
    <div className="purchase-list-page">
      <h2>My Purchases</h2>
      
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
        <h3>Search My Purchases</h3>
        <div className="form-row">
          <div className="form-group">
            <label>Status:</label>
            <select
              value={searchParams.status}
              onChange={(e) => handleSearchChange('status', e.target.value)}
            >
              <option value="">All Statuses</option>
              <option value="1">Pending</option>
              <option value="2">Confirmed</option>
              <option value="3">In Progress</option>
              <option value="4">Completed</option>
              <option value="5">Cancelled</option>
              <option value="6">Refunded</option>
            </select>
          </div>
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
            <label>From Date:</label>
            <input
              type="date"
              value={searchParams.fromDate}
              onChange={(e) => handleSearchChange('fromDate', e.target.value)}
            />
          </div>
          <div className="form-group">
            <label>To Date:</label>
            <input
              type="date"
              value={searchParams.toDate}
              onChange={(e) => handleSearchChange('toDate', e.target.value)}
            />
          </div>
        </div>
      </div>

      {/* Results */}
      {loading ? (
        <div className="loading">Loading purchases...</div>
      ) : (
        <>
          <div className="results-info">
            <p>Showing page {searchParams.page} of {totalPages} ({purchases.length} purchases)</p>
          </div>

          {purchases.length === 0 ? (
            <div className="no-results">
              <p>No purchases found. <a href="#" onClick={() => window.location.reload()}>Browse available offers</a> to make your first purchase!</p>
            </div>
          ) : (
            <div className="purchase-grid">
              {purchases.map(purchase => (
                <div key={purchase.id} className="card">
                  <h3>{purchase.year} {purchase.make} {purchase.model}</h3>
                  
                  <div className="card-details">
                    <div><strong>Purchase Price:</strong> {formatPrice(purchase.purchaseAmount)}</div>
                    <div><strong>Status:</strong> 
                      <span className={`status-badge status-${getStatusColor(purchase.status)}`}>
                        {getStatusName(purchase.status)}
                      </span>
                    </div>
                    <div><strong>Purchase Date:</strong> {formatDate(purchase.purchasedAt)}</div>
                    <div><strong>Condition:</strong> {purchase.condition || 'N/A'}</div>
                    <div><strong>VIN:</strong> {purchase.vin || 'N/A'}</div>
                    <div><strong>Location:</strong> {purchase.address || 'N/A'}</div>
                    {purchase.notes && (
                      <div><strong>Notes:</strong> {purchase.notes}</div>
                    )}
                  </div>

                  <div className="card-actions">
                    {purchase.status === 1 && ( // Pending
                      <>
                        <button
                          className="btn btn-primary"
                          onClick={() => handleUpdateStatus(purchase.id, '2')}
                        >
                          Confirm Purchase
                        </button>
                        <button
                          className="btn btn-danger"
                          onClick={() => handleUpdateStatus(purchase.id, '5')}
                        >
                          Cancel
                        </button>
                      </>
                    )}
                    
                    {purchase.status === 2 && ( // Confirmed
                      <button
                        className="btn btn-primary"
                        onClick={() => handleUpdateStatus(purchase.id, '3')}
                      >
                        Mark In Progress
                      </button>
                    )}
                    
                    {purchase.status === 3 && ( // In Progress
                      <button
                        className="btn btn-primary"
                        onClick={() => handleUpdateStatus(purchase.id, '4')}
                      >
                        Mark Completed
                      </button>
                    )}
                    
                    {(purchase.status === 1 || purchase.status === 5) && ( // Pending or Cancelled
                      <button
                        className="btn btn-danger"
                        onClick={() => handleDeletePurchase(purchase.id)}
                      >
                        Delete
                      </button>
                    )}
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
    </div>
  );
};

export default PurchaseListPage;