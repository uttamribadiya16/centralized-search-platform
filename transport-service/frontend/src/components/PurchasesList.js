import React, { useState, useEffect } from 'react';
import { transportService } from '../services/api';

const PurchasesList = () => {
  const [purchases, setPurchases] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [selectedPurchases, setSelectedPurchases] = useState(new Set());
  const [showAssignModal, setShowAssignModal] = useState(false);

  useEffect(() => {
    loadPurchases();
  }, [currentPage, searchTerm]);

  const loadPurchases = async () => {
    try {
      setLoading(true);
      const response = await transportService.getPurchases({
        page: currentPage,
        pageSize: 10,
        search: searchTerm
      });
      
      const data = response.data;
      setPurchases(data.items || []);
      setTotalPages(Math.ceil((data.totalCount || 0) / 10));
    } catch (error) {
      console.error('Error loading purchases:', error);
      setPurchases([]);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = (e) => {
    setSearchTerm(e.target.value);
    setCurrentPage(1);
  };

  const handleSelectPurchase = (purchaseId) => {
    const newSelection = new Set(selectedPurchases);
    if (newSelection.has(purchaseId)) {
      newSelection.delete(purchaseId);
    } else {
      newSelection.add(purchaseId);
    }
    setSelectedPurchases(newSelection);
  };

  const handleSelectAll = () => {
    if (selectedPurchases.size === purchases.length) {
      setSelectedPurchases(new Set());
    } else {
      setSelectedPurchases(new Set(purchases.map(purchase => purchase.id)));
    }
  };

  const handleAssignToTransport = () => {
    if (selectedPurchases.size > 0) {
      setShowAssignModal(true);
    }
  };

  const getStatusBadgeClass = (status) => {
    switch (status?.toLowerCase()) {
      case 'completed': return 'bg-success';
      case 'pending': return 'bg-warning text-dark';
      case 'processing': return 'bg-info';
      case 'cancelled': return 'bg-danger';
      case 'shipped': return 'bg-primary';
      default: return 'bg-secondary';
    }
  };

  const renderPagination = () => {
    if (totalPages <= 1) return null;

    return (
      <nav>
        <ul className="pagination justify-content-center">
          <li className={`page-item ${currentPage === 1 ? 'disabled' : ''}`}>
            <button 
              className="page-link" 
              onClick={() => setCurrentPage(1)}
              disabled={currentPage === 1}
            >
              First
            </button>
          </li>
          <li className={`page-item ${currentPage === 1 ? 'disabled' : ''}`}>
            <button 
              className="page-link" 
              onClick={() => setCurrentPage(currentPage - 1)}
              disabled={currentPage === 1}
            >
              Previous
            </button>
          </li>
          
          {[...Array(Math.min(5, totalPages))].map((_, i) => {
            const page = Math.max(1, Math.min(totalPages - 4, currentPage - 2)) + i;
            if (page > totalPages) return null;
            
            return (
              <li key={page} className={`page-item ${currentPage === page ? 'active' : ''}`}>
                <button 
                  className="page-link" 
                  onClick={() => setCurrentPage(page)}
                >
                  {page}
                </button>
              </li>
            );
          })}
          
          <li className={`page-item ${currentPage === totalPages ? 'disabled' : ''}`}>
            <button 
              className="page-link" 
              onClick={() => setCurrentPage(currentPage + 1)}
              disabled={currentPage === totalPages}
            >
              Next
            </button>
          </li>
          <li className={`page-item ${currentPage === totalPages ? 'disabled' : ''}`}>
            <button 
              className="page-link" 
              onClick={() => setCurrentPage(totalPages)}
              disabled={currentPage === totalPages}
            >
              Last
            </button>
          </li>
        </ul>
      </nav>
    );
  };

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h1>Purchases</h1>
        <div className="d-flex gap-2">
          {selectedPurchases.size > 0 && (
            <>
              <span className="badge bg-primary fs-6">
                {selectedPurchases.size} selected
              </span>
              <button 
                className="btn btn-success"
                onClick={handleAssignToTransport}
              >
                <i className="bi bi-truck me-2"></i>Assign to Transport
              </button>
            </>
          )}
          <button className="btn btn-outline-primary" onClick={loadPurchases}>
            <i className="bi bi-arrow-clockwise me-2"></i>Refresh
          </button>
        </div>
      </div>

      {/* Search */}
      <div className="row mb-4">
        <div className="col-md-6">
          <div className="input-group search-box">
            <span className="input-group-text">
              <i className="bi bi-search"></i>
            </span>
            <input
              type="text"
              className="form-control"
              placeholder="Search purchases by offer, amount, or status..."
              value={searchTerm}
              onChange={handleSearch}
            />
            {searchTerm && (
              <button 
                className="btn btn-outline-secondary" 
                type="button"
                onClick={() => setSearchTerm('')}
              >
                <i className="bi bi-x"></i>
              </button>
            )}
          </div>
        </div>
      </div>

      {/* Purchases Table */}
      <div className="card">
        <div className="card-body">
          {loading ? (
            <div className="loading-spinner">
              <div className="spinner-border text-primary" role="status">
                <span className="visually-hidden">Loading...</span>
              </div>
            </div>
          ) : purchases.length === 0 ? (
            <div className="empty-state">
              <i className="bi bi-cart-x fs-1 text-muted"></i>
              <h5 className="mt-3">No purchases found</h5>
              <p>Try adjusting your search terms or check back later for new purchases.</p>
            </div>
          ) : (
            <>
              <div className="table-responsive">
                <table className="table table-hover">
                  <thead>
                    <tr>
                      <th>
                        <div className="form-check">
                          <input
                            className="form-check-input"
                            type="checkbox"
                            checked={selectedPurchases.size === purchases.length && purchases.length > 0}
                            onChange={handleSelectAll}
                          />
                        </div>
                      </th>
                      <th>Purchase ID</th>
                      <th>Offer</th>
                      <th>Amount</th>
                      <th>Status</th>
                      <th>Purchase Date</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {purchases.map((purchase) => (
                      <tr key={purchase.id}>
                        <td>
                          <div className="form-check">
                            <input
                              className="form-check-input"
                              type="checkbox"
                              checked={selectedPurchases.has(purchase.id)}
                              onChange={() => handleSelectPurchase(purchase.id)}
                            />
                          </div>
                        </td>
                        <td>
                          <code>{purchase.id.toString().padStart(6, '0')}</code>
                        </td>
                        <td>
                          <div>
                            <strong>{purchase.offer?.title || 'Unknown Offer'}</strong>
                            {purchase.offer?.category && (
                              <div className="text-muted small">
                                {purchase.offer.category}
                              </div>
                            )}
                          </div>
                        </td>
                        <td>
                          <span className="fw-bold text-success">
                            ${purchase.amount.toFixed(2)}
                          </span>
                        </td>
                        <td>
                          <span className={`badge ${getStatusBadgeClass(purchase.status)}`}>
                            {purchase.status}
                          </span>
                        </td>
                        <td>{new Date(purchase.purchaseDate).toLocaleDateString()}</td>
                        <td>
                          <div className="btn-group" role="group">
                            <button className="btn btn-sm btn-outline-primary">
                              <i className="bi bi-eye me-1"></i>View
                            </button>
                            <button 
                              className="btn btn-sm btn-success"
                              disabled={purchase.status === 'shipped' || purchase.status === 'completed'}
                            >
                              <i className="bi bi-truck me-1"></i>Assign
                            </button>
                          </div>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
              
              {renderPagination()}
            </>
          )}
        </div>
      </div>

      {/* Assign Modal would go here - placeholder for now */}
      {showAssignModal && (
        <div className="modal d-block" tabIndex="-1" style={{backgroundColor: 'rgba(0,0,0,0.5)'}}>
          <div className="modal-dialog">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">Assign to Transport</h5>
                <button 
                  type="button" 
                  className="btn-close" 
                  onClick={() => setShowAssignModal(false)}
                ></button>
              </div>
              <div className="modal-body">
                <p>Assign {selectedPurchases.size} selected purchase(s) to a transport route.</p>
                <div className="alert alert-info">
                  <i className="bi bi-info-circle me-2"></i>
                  Transport assignment functionality will be implemented in the next phase.
                </div>
              </div>
              <div className="modal-footer">
                <button 
                  type="button" 
                  className="btn btn-secondary" 
                  onClick={() => setShowAssignModal(false)}
                >
                  Cancel
                </button>
                <button 
                  type="button" 
                  className="btn btn-primary"
                  onClick={() => setShowAssignModal(false)}
                >
                  Assign
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default PurchasesList;