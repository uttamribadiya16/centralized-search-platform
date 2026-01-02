import React, { useState, useEffect } from 'react';
import authService from '../services/authService';
import offerService from '../services/offerService';
import OfferList from './OfferList';
import CreateOfferForm from './CreateOfferForm';
import Search from './Search';
import './SellerDashboard.css';

const SellerDashboard = ({ user, onLogout }) => {
  const [activeTab, setActiveTab] = useState('overview');
  const [stats, setStats] = useState({});
  const [offers, setOffers] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    loadSellerData();
  }, [user.id]);

  const loadSellerData = async () => {
    setIsLoading(true);
    setError('');
    
    try {
      // Load seller stats
      const statsData = await offerService.getSellerStats(user.id);
      setStats(statsData);

      // Load seller offers
      const offersData = await offerService.getOffersBySeller(user.id, {
        page: 1,
        pageSize: 10,
        sortBy: 'createdAt',
        sortDirection: 'desc'
      });
      setOffers(offersData.items || []);
    } catch (error) {
      console.error('Error loading seller data:', error);
      setError('Failed to load dashboard data. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleLogout = () => {
    authService.logout();
    onLogout();
  };

  const handleOfferCreated = (newOffer) => {
    setOffers(prev => [newOffer, ...prev]);
    setActiveTab('offers');
    loadSellerData(); // Refresh stats
  };

  const handleOfferUpdated = (updatedOffer) => {
    setOffers(prev => prev.map(offer => 
      offer.id === updatedOffer.id ? updatedOffer : offer
    ));
    loadSellerData(); // Refresh stats
  };

  const handleOfferDeleted = (deletedOfferId) => {
    setOffers(prev => prev.filter(offer => offer.id !== deletedOfferId));
    loadSellerData(); // Refresh stats
  };

  if (isLoading) {
    return (
      <div className="dashboard-container">
        <div className="loading">Loading dashboard...</div>
      </div>
    );
  }

  return (
    <div className="dashboard-container">
      <header className="dashboard-header">
        <div className="header-content">
          <h1>Seller Dashboard</h1>
          <div className="user-info">
            <span>Welcome, {user.firstName} {user.lastName}</span>
            <button onClick={handleLogout} className="logout-button">Logout</button>
          </div>
        </div>
      </header>

      <nav className="dashboard-nav">
        <button 
          className={activeTab === 'overview' ? 'tab-button active' : 'tab-button'}
          onClick={() => setActiveTab('overview')}
        >
          Overview
        </button>
        <button 
          className={activeTab === 'offers' ? 'tab-button active' : 'tab-button'}
          onClick={() => setActiveTab('offers')}
        >
          My Offers
        </button>
        <button 
          className={activeTab === 'search' ? 'tab-button active' : 'tab-button'}
          onClick={() => setActiveTab('search')}
        >
          Search Offers
        </button>
        <button 
          className={activeTab === 'create' ? 'tab-button active' : 'tab-button'}
          onClick={() => setActiveTab('create')}
        >
          Create Offer
        </button>
      </nav>

      <main className="dashboard-main">
        {error && <div className="error-message">{error}</div>}

        {activeTab === 'overview' && (
          <div className="overview-tab">
            <div className="stats-grid">
              <div className="stat-card">
                <h3>Total Offers</h3>
                <div className="stat-value">{stats.TotalOffers || 0}</div>
              </div>
              <div className="stat-card">
                <h3>Active Offers</h3>
                <div className="stat-value">{stats.ActiveOffers || 0}</div>
              </div>
              <div className="stat-card">
                <h3>Sold Vehicles</h3>
                <div className="stat-value">{stats.SoldOffers || 0}</div>
              </div>
              <div className="stat-card">
                <h3>Total Revenue</h3>
                <div className="stat-value">${(stats.TotalRevenue || 0).toLocaleString()}</div>
              </div>
            </div>

            <div className="recent-offers">
              <h3>Recent Offers</h3>
              {offers.length > 0 ? (
                <div className="offer-preview-list">
                  {offers.slice(0, 3).map(offer => (
                    <div key={offer.id} className="offer-preview">
                      <div className="offer-info">
                        <h4>{offer.year} {offer.make} {offer.model}</h4>
                        <p>VIN: {offer.vin || 'Not specified'}</p>
                        <p>Price: ${offer.offerAmount ? offer.offerAmount.toLocaleString() : 'Not specified'}</p>
                      </div>
                      <div className="offer-status">
                        <span className={`status-badge ${offer.status ? offer.status.toLowerCase() : 'unknown'}`}>
                          {offer.status || 'Unknown'}
                        </span>
                      </div>
                    </div>
                  ))}
                </div>
              ) : (
                <p>No offers created yet. <button 
                  className="link-button" 
                  onClick={() => setActiveTab('create')}
                >Create your first offer</button></p>
              )}
            </div>
          </div>
        )}

        {activeTab === 'offers' && (
          <div className="offers-tab">
            <OfferList 
              offers={offers}
              onOfferUpdated={handleOfferUpdated}
              onOfferDeleted={handleOfferDeleted}
              onRefresh={loadSellerData}
            />
          </div>
        )}

        {activeTab === 'search' && (
          <div className="search-tab">
            <Search />
          </div>
        )}

        {activeTab === 'create' && (
          <div className="create-tab">
            <CreateOfferForm 
              sellerId={user.id}
              onOfferCreated={handleOfferCreated}
            />
          </div>
        )}
      </main>
    </div>
  );
};

export default SellerDashboard;