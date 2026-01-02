import React from 'react';
import { UserTypeNames } from '../services/userService';
import '../styles/HomePage.css';

const CarrierHomePage = ({ user, onLogout }) => {
  return (
    <div className="home-page">
      <div className="header">
        <h1>Carrier Dashboard</h1>
        <div className="user-info">
          <span>Welcome, {user.fullName}</span>
          <span className="user-type">{UserTypeNames[user.userType]}</span>
          <button className="logout-button" onClick={onLogout}>Logout</button>
        </div>
      </div>

      <div className="dashboard-content">
        <div className="stats-grid">
          <div className="stat-card">
            <h3>Active Transports</h3>
            <div className="stat-number">6</div>
            <p>Currently in progress</p>
          </div>
          <div className="stat-card">
            <h3>Completed Jobs</h3>
            <div className="stat-number">127</div>
            <p>Successfully delivered</p>
          </div>
          <div className="stat-card">
            <h3>Monthly Revenue</h3>
            <div className="stat-number">$18,500</div>
            <p>This month's earnings</p>
          </div>
        </div>

        <div className="action-cards">
          <div className="action-card">
            <h4>🚛 Available Jobs</h4>
            <p>Find new transport assignments</p>
            <button className="action-button">Browse Jobs</button>
          </div>
          <div className="action-card">
            <h4>📋 My Assignments</h4>
            <p>Manage current transports</p>
            <button className="action-button">View Assignments</button>
          </div>
          <div className="action-card">
            <h4>📊 Performance</h4>
            <p>View delivery metrics</p>
            <button className="action-button">View Metrics</button>
          </div>
        </div>

        <div className="recent-activity">
          <h3>Recent Activity</h3>
          <div className="activity-list">
            <div className="activity-item">
              <span>✅ Successfully delivered 2023 Honda Accord to Miami, FL</span>
              <span className="time">3 hours ago</span>
            </div>
            <div className="activity-item">
              <span>🚛 Picked up 2024 Ford Mustang from Dallas, TX</span>
              <span className="time">1 day ago</span>
            </div>
            <div className="activity-item">
              <span>📋 New transport request: BMW X3 (Atlanta → Seattle)</span>
              <span className="time">2 days ago</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default CarrierHomePage;