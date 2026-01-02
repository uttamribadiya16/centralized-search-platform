import React from 'react';
import { UserTypeNames } from '../services/userService';
import '../styles/HomePage.css';

const SellerHomePage = ({ user, onLogout }) => {
  return (
    <div className="home-page">
      <div className="header">
        <h1>Seller Dashboard</h1>
        <div className="user-info">
          <span>Welcome, {user.fullName}</span>
          <span className="user-type">{UserTypeNames[user.userType]}</span>
          <button className="logout-button" onClick={onLogout}>Logout</button>
        </div>
      </div>

      <div className="dashboard-content">
        <div className="stats-grid">
          <div className="stat-card">
            <h3>Active Offers</h3>
            <div className="stat-number">12</div>
            <p>Vehicles currently listed</p>
          </div>
          <div className="stat-card">
            <h3>Total Sales</h3>
            <div className="stat-number">45</div>
            <p>Vehicles sold this year</p>
          </div>
          <div className="stat-card">
            <h3>Revenue</h3>
            <div className="stat-number">$342,500</div>
            <p>Total earnings</p>
          </div>
        </div>

        <div className="action-cards">
          <div className="action-card">
            <h4>📝 Create New Offer</h4>
            <p>List a new vehicle for sale</p>
            <button className="action-button">Add Vehicle</button>
          </div>
          <div className="action-card">
            <h4>📊 View My Offers</h4>
            <p>Manage your existing listings</p>
            <button className="action-button">Manage Offers</button>
          </div>
          <div className="action-card">
            <h4>📈 Sales Analytics</h4>
            <p>View your performance metrics</p>
            <button className="action-button">View Analytics</button>
          </div>
        </div>

        <div className="recent-activity">
          <h3>Recent Activity</h3>
          <div className="activity-list">
            <div className="activity-item">
              <span>🚗 New inquiry on 2023 Honda Civic</span>
              <span className="time">2 hours ago</span>
            </div>
            <div className="activity-item">
              <span>💰 Offer accepted for 2022 Toyota Camry</span>
              <span className="time">1 day ago</span>
            </div>
            <div className="activity-item">
              <span>📝 Listed new 2024 Ford F-150</span>
              <span className="time">3 days ago</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default SellerHomePage;