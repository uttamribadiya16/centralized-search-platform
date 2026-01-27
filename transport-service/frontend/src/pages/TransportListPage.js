import React, { useState, useEffect } from 'react';
import apiService from '../services/api';

const TransportListPage = ({ currentUser }) => {
  const [transports, setTransports] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [searchParams, setSearchParams] = useState({
    page: 1,
    pageSize: 10,
    status: '',
    originLocation: '',
    destinationLocation: '',
    vehicleType: '',
    fromDate: '',
    toDate: ''
  });
  const [totalPages, setTotalPages] = useState(0);
  const [success, setSuccess] = useState('');

  useEffect(() => {
    if (currentUser?.id) {
      loadTransports();
    }
  }, [searchParams, currentUser]);

  const loadTransports = async () => {
    if (!currentUser?.id) return;
    
    console.log('Loading transports for carrier:', currentUser.id);
    setLoading(true);
    setError('');
    
    try {
      const response = await apiService.getTransportsByCarrier(currentUser.id, searchParams);
      console.log('Transports response:', response);
      setTransports(response.items || []);
      setTotalPages(response.totalPages || 0);
    } catch (error) {
      console.error('Error loading transports:', error);
      setError('Failed to load transports. Please try again.');
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

  const handleUpdateStatus = async (transportId, newStatus) => {
    try {
      await apiService.updateTransport(transportId, { status: newStatus });
      setSuccess('Transport status updated successfully!');
      setTimeout(() => setSuccess(''), 3000);
      loadTransports(); // Refresh list
    } catch (error) {
      console.error('Error updating transport:', error);
      setError('Failed to update transport status.');
    }
  };

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString();
  };

  const getStatusColor = (status) => {
    const statusMap = {
      1: 'pending',     // Assigned
      2: 'confirmed',   // PickupScheduled
      3: 'in-progress', // InTransit
      4: 'completed',   // Delivered
      5: 'cancelled'    // Cancelled
    };
    return statusMap[status] || 'pending';
  };

  const getStatusName = (status) => {
    const statusMap = {
      1: 'Assigned',
      2: 'Pickup Scheduled',
      3: 'In Transit',
      4: 'Delivered',
      5: 'Cancelled'
    };
    return statusMap[status] || 'Unknown';
  };

  const getVehicleIcon = (vehicleType) => {
    // Since API doesn't return vehicleType, use generic truck icon
    return '🚛';
  };

  return (
    <div className="transport-list-page">
      <h2>My Transports</h2>
      <p>Manage your transport assignments and delivery status</p>
      
      {/* Debug info */}
      <div style={{background: '#f0f0f0', padding: '10px', margin: '10px 0', fontSize: '12px'}}>
        <strong>Debug Info:</strong><br/>
        Current User ID: {currentUser?.id}<br/>
        Transports Count: {transports.length}<br/>
        Loading: {loading ? 'Yes' : 'No'}<br/>
        Error: {error || 'None'}
      </div>
      
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
        <h3>Search My Transports</h3>
        <div className="form-row">
          <div className="form-group">
            <label>Status:</label>
            <select
              value={searchParams.status}
              onChange={(e) => handleSearchChange('status', e.target.value)}
            >
              <option value="">All Statuses</option>
              <option value="pending">Pending</option>
              <option value="active">Active</option>
              <option value="in-transit">In Transit</option>
              <option value="completed">Completed</option>
              <option value="cancelled">Cancelled</option>
            </select>
          </div>
          <div className="form-group">
            <label>Vehicle Type:</label>
            <select
              value={searchParams.vehicleType}
              onChange={(e) => handleSearchChange('vehicleType', e.target.value)}
            >
              <option value="">All Vehicles</option>
              <option value="Truck">Truck</option>
              <option value="Van">Van</option>
              <option value="Motorcycle">Motorcycle</option>
              <option value="Ship">Ship</option>
              <option value="Plane">Plane</option>
            </select>
          </div>
          <div className="form-group">
            <label>Origin:</label>
            <input
              type="text"
              value={searchParams.originLocation}
              onChange={(e) => handleSearchChange('originLocation', e.target.value)}
              placeholder="Origin city/state"
            />
          </div>
          <div className="form-group">
            <label>Destination:</label>
            <input
              type="text"
              value={searchParams.destinationLocation}
              onChange={(e) => handleSearchChange('destinationLocation', e.target.value)}
              placeholder="Destination city/state"
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
        <div className="loading">Loading transports...</div>
      ) : (
        <>
          <div className="results-info">
            <p>Showing page {searchParams.page} of {totalPages} ({transports.length} transports)</p>
          </div>

          {transports.length === 0 ? (
            <div className="no-results">
              <p>No transport assignments found. Start by assigning purchases to create transports!</p>
            </div>
          ) : (
            <div className="transport-grid">
              {transports.map(transport => (
                <div key={transport.id} className="card">
                  <h3>
                    {getVehicleIcon(transport.vehicleType)} Transport #{transport.id?.substring(0, 8)}
                  </h3>
                  
                  <div className="card-details">
                    <div><strong>Route:</strong> {transport.pickupAddress} → {transport.deliveryAddress}</div>
                    <div><strong>Vehicle:</strong> {transport.offerMake} {transport.offerModel} ({transport.offerYear})</div>
                    <div><strong>VIN:</strong> {transport.offerVin}</div>
                    <div><strong>Purchase Amount:</strong> ${transport.purchaseAmount?.toFixed(2) || 'N/A'}</div>
                    <div><strong>Status:</strong> 
                      <span className={`status-badge status-${getStatusColor(transport.status)}`}>
                        {getStatusName(transport.status)}
                      </span>
                    </div>
                    <div><strong>Created:</strong> {formatDate(transport.createdAt)}</div>
                    {transport.estimatedDeliveryDate && (
                      <div><strong>Est. Delivery:</strong> {formatDate(transport.estimatedDeliveryDate)}</div>
                    )}
                    {transport.assignedPurchases && transport.assignedPurchases.length > 0 && (
                      <div><strong>Assigned Purchases:</strong> {transport.assignedPurchases.length}</div>
                    )}
                  </div>

                  <div className="card-actions">
                    {transport.status === 1 && (
                      <>
                        <button
                          className="btn btn-primary"
                          onClick={() => handleUpdateStatus(transport.id, 2)}
                        >
                          Schedule Pickup
                        </button>
                        <button
                          className="btn btn-danger"
                          onClick={() => handleUpdateStatus(transport.id, 5)}
                        >
                          Cancel
                        </button>
                      </>
                    )}
                    
                    {transport.status === 2 && (
                      <button
                        className="btn btn-primary"
                        onClick={() => handleUpdateStatus(transport.id, 3)}
                      >
                        Mark In Transit
                      </button>
                    )}
                    
                    {transport.status === 3 && (
                      <button
                        className="btn btn-primary"
                        onClick={() => handleUpdateStatus(transport.id, 4)}
                      >
                        Mark Delivered
                      </button>
                    )}
                    
                    <button
                      className="btn btn-info"
                      onClick={() => alert('Transport details: ' + JSON.stringify(transport, null, 2))}
                    >
                      View Details
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
    </div>
  );
};

export default TransportListPage;