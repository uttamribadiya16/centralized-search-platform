import React, { useState, useEffect } from 'react';
import searchService from '../services/searchService';
import authService from '../services/authService';
import './Search.css';

const Search = () => {
  const [searchText, setSearchText] = useState('');
  const [searchResults, setSearchResults] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(0);
  const [totalCount, setTotalCount] = useState(0);
  const [error, setError] = useState('');
  const [hasSearched, setHasSearched] = useState(false);

  const currentUser = authService.getCurrentUser();
  const pageSize = 10;

  useEffect(() => {
    // Load initial results (all seller's offers)
    if (currentUser?.id) {
      handleSearch('', 1, false);
    }
  }, []);

  const handleSearch = async (searchTerm = searchText, page = 1, updateSearched = true) => {
    if (!currentUser?.id) {
      setError('You must be logged in to search offers');
      return;
    }

    setIsLoading(true);
    setError('');
    
    if (updateSearched) {
      setHasSearched(true);
    }

    try {
      const response = await searchService.searchOffers(
        currentUser.id,
        searchTerm,
        page,
        pageSize
      );

      setSearchResults(response.results || []);
      setTotalPages(response.totalPages || 0);
      setTotalCount(response.totalCount || 0);
      setCurrentPage(page);
    } catch (error) {
      console.error('Search failed:', error);
      setError('Failed to search offers. Please try again.');
      setSearchResults([]);
      setTotalPages(0);
      setTotalCount(0);
    } finally {
      setIsLoading(false);
    }
  };

  const handleSearchSubmit = (e) => {
    e.preventDefault();
    handleSearch(searchText, 1);
  };

  const handlePageChange = (newPage) => {
    if (newPage >= 1 && newPage <= totalPages) {
      handleSearch(searchText, newPage, false);
    }
  };

  const formatCurrency = (amount) => {
    if (amount == null) return 'Not specified';
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount);
  };

  const formatDate = (dateString) => {
    if (!dateString) return 'Unknown';
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  };

  const getStatusBadgeClass = (status) => {
    const statusLower = status?.toLowerCase() || '';
    switch (statusLower) {
      case 'available':
        return 'status-badge status-available';
      case 'pending':
        return 'status-badge status-pending';
      case 'sold':
        return 'status-badge status-sold';
      case 'expired':
        return 'status-badge status-expired';
      default:
        return 'status-badge status-unknown';
    }
  };

  return (
    <div className="search-container">
      <div className="search-header">
        <h1>Search Your Offers</h1>
        <p>Find and manage your vehicle listings using our powerful search</p>
      </div>

      <form onSubmit={handleSearchSubmit} className="search-form">
        <div className="search-input-group">
          <input
            type="text"
            value={searchText}
            onChange={(e) => setSearchText(e.target.value)}
            placeholder="Search by make, model, VIN, condition, or address..."
            className="search-input"
            disabled={isLoading}
          />
          <button type="submit" disabled={isLoading} className="search-button">
            {isLoading ? 'Searching...' : 'Search'}
          </button>
        </div>
      </form>

      {error && (
        <div className="error-message">
          <i className="error-icon">⚠️</i>
          {error}
        </div>
      )}

      <div className="search-results-info">
        {!isLoading && hasSearched && (
          <p>
            {searchText ? (
              <>Found {totalCount} result{totalCount !== 1 ? 's' : ''} for "{searchText}"</>
            ) : (
              <>Showing all {totalCount} offer{totalCount !== 1 ? 's' : ''}</>
            )}
          </p>
        )}
      </div>

      {isLoading && (
        <div className="loading-container">
          <div className="loading-spinner"></div>
          <p>Searching your offers...</p>
        </div>
      )}

      {!isLoading && hasSearched && searchResults.length === 0 && (
        <div className="empty-state">
          <div className="empty-state-icon">🔍</div>
          <h3>No offers found</h3>
          {searchText ? (
            <p>
              No offers match your search "{searchText}". 
              <br />Try a different search term or check your spelling.
            </p>
          ) : (
            <p>
              You haven't created any offers yet.
              <br />Start by creating your first vehicle listing!
            </p>
          )}
        </div>
      )}

      {!isLoading && searchResults.length > 0 && (
        <>
          <div className="search-results">
            {searchResults.map((offer) => (
              <div key={offer.id} className="search-result-card">
                <div className="result-header">
                  <h3 className="result-title">
                    {offer.year} {offer.make} {offer.model}
                  </h3>
                  <div className={getStatusBadgeClass(offer.status)}>
                    {offer.status || 'Unknown'}
                  </div>
                </div>
                
                <div className="result-details">
                  <div className="result-row">
                    <span className="result-label">VIN:</span>
                    <span className="result-value">{offer.vin || 'Not specified'}</span>
                  </div>
                  
                  <div className="result-row">
                    <span className="result-label">Price:</span>
                    <span className="result-value price">{formatCurrency(offer.offerAmount)}</span>
                  </div>
                  
                  <div className="result-row">
                    <span className="result-label">Condition:</span>
                    <span className="result-value">{offer.condition || 'Not specified'}</span>
                  </div>
                  
                  {offer.address && (
                    <div className="result-row">
                      <span className="result-label">Location:</span>
                      <span className="result-value">{offer.address}</span>
                    </div>
                  )}
                  
                  <div className="result-row">
                    <span className="result-label">Created:</span>
                    <span className="result-value">{formatDate(offer.createdAt)}</span>
                  </div>
                  
                  <div className="result-row">
                    <span className="result-label">Updated:</span>
                    <span className="result-value">{formatDate(offer.updatedAt)}</span>
                  </div>
                </div>
              </div>
            ))}
          </div>

          {totalPages > 1 && (
            <div className="pagination">
              <button
                onClick={() => handlePageChange(currentPage - 1)}
                disabled={currentPage <= 1 || isLoading}
                className="pagination-button"
              >
                ← Previous
              </button>
              
              <span className="pagination-info">
                Page {currentPage} of {totalPages}
              </span>
              
              <button
                onClick={() => handlePageChange(currentPage + 1)}
                disabled={currentPage >= totalPages || isLoading}
                className="pagination-button"
              >
                Next →
              </button>
            </div>
          )}
        </>
      )}
    </div>
  );
};

export default Search;