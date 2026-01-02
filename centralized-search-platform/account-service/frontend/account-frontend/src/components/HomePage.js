import React from 'react';
import './HomePage.css';

const HomePage = ({ onCreateUser }) => {
  return (
    <div className="home-page">
      <div className="hero-section">
        <h1>Account Service</h1>
        <p>Manage users for the Automotive Marketplace Platform</p>
        <div className="actions">
          <button 
            className="create-user-btn"
            onClick={onCreateUser}
          >
            Create New User
          </button>
        </div>
      </div>
      
      <div className="info-section">
        <div className="feature-card">
          <h3>User Types</h3>
          <ul>
            <li><strong>Seller:</strong> Can create and manage vehicle offers</li>
            <li><strong>Buyer:</strong> Can browse and purchase vehicles</li>
            <li><strong>Carrier:</strong> Can provide transportation services</li>
            <li><strong>Agent:</strong> Can facilitate transactions</li>
          </ul>
        </div>
        
        <div className="feature-card">
          <h3>Services Available</h3>
          <ul>
            <li>Account Service - User management (Port 3000)</li>
            <li>Offer Service - Vehicle listings (Port 3001)</li>
            <li>More services coming soon...</li>
          </ul>
        </div>
      </div>
    </div>
  );
};

export default HomePage;