import React, { useState, useEffect } from 'react';
import { transportService } from '../services/api';

const OffersList = () => {
  const [offers, setOffers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [selectedOffers, setSelectedOffers] = useState(new Set());

  useEffect(() => {
    loadOffers();
  }, [currentPage, searchTerm]);

  const loadOffers = async () => {
    try {
      setLoading(true);
      const response = await transportService.getOffers({
        page: currentPage,
        pageSize: 10,
        search: searchTerm
      });
      
      const data = response.data;
      setOffers(data.items || []);
      setTotalPages(Math.ceil((data.totalCount || 0) / 10));
    } catch (error) {
      console.error('Error loading offers:', error);
      setOffers([]);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = (e) => {
    setSearchTerm(e.target.value);
    setCurrentPage(1);
  };

  const handleSelectOffer = (offerId) => {
    const newSelection = new Set(selectedOffers);
    if (newSelection.has(offerId)) {
      newSelection.delete(offerId);
    } else {
      newSelection.add(offerId);
    }
    setSelectedOffers(newSelection);
  };

  const handleSelectAll = () => {
    if (selectedOffers.size === offers.length) {
      setSelectedOffers(new Set());
    } else {
      setSelectedOffers(new Set(offers.map(offer => offer.id)));
    }
  };

  const getPriceColor = (price) => {
    if (price > 1000) return 'text-success';
    if (price > 500) return 'text-warning';
    return 'text-danger';
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
        <h1>Available Offers</h1>
        <div className="d-flex gap-2">
          {selectedOffers.size > 0 && (
            <span className="badge bg-primary fs-6">
              {selectedOffers.size} selected
            </span>
          )}
          <button className="btn btn-outline-primary" onClick={loadOffers}>
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
              placeholder="Search offers by title, category, or description..."
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

      {/* Offers Table */}
      <div className="card">
        <div className="card-body">
          {loading ? (
            <div className="loading-spinner">
              <div className="spinner-border text-primary" role="status">
                <span className="visually-hidden">Loading...</span>
              </div>
            </div>
          ) : offers.length === 0 ? (
            <div className="empty-state">
              <i className="bi bi-inbox fs-1 text-muted"></i>
              <h5 className="mt-3">No offers found</h5>
              <p>Try adjusting your search terms or check back later for new offers.</p>
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
                            checked={selectedOffers.size === offers.length && offers.length > 0}
                            onChange={handleSelectAll}
                          />
                        </div>
                      </th>
                      <th>Title</th>
                      <th>Category</th>
                      <th>Price</th>
                      <th>Status</th>
                      <th>Created</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {offers.map((offer) => (
                      <tr key={offer.id}>
                        <td>
                          <div className="form-check">
                            <input
                              className="form-check-input"
                              type="checkbox"
                              checked={selectedOffers.has(offer.id)}
                              onChange={() => handleSelectOffer(offer.id)}
                            />
                          </div>
                        </td>
                        <td>
                          <div>
                            <strong>{offer.title}</strong>
                            {offer.description && (
                              <div className="text-muted small text-truncate" style={{maxWidth: '200px'}}>
                                {offer.description}
                              </div>
                            )}
                          </div>
                        </td>
                        <td>
                          <span className="badge bg-secondary">{offer.category}</span>
                        </td>
                        <td>
                          <span className={`fw-bold ${getPriceColor(offer.price)}`}>
                            ${offer.price.toFixed(2)}
                          </span>
                        </td>
                        <td>
                          <span className={`badge ${offer.isActive ? 'bg-success' : 'bg-warning text-dark'}`}>
                            {offer.isActive ? 'Active' : 'Inactive'}
                          </span>
                        </td>
                        <td>{new Date(offer.createdAt).toLocaleDateString()}</td>
                        <td>
                          <div className="btn-group" role="group">
                            <button className="btn btn-sm btn-outline-primary">
                              <i className="bi bi-eye me-1"></i>View
                            </button>
                            <button 
                              className="btn btn-sm btn-success"
                              disabled={!offer.isActive}
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
    </div>
  );
};

export default OffersList;