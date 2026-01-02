import React from 'react';
import { UserTypeNames } from '../services/userService';
import '../styles/HomePage.css';

const AgentHomePage = ({ user, onLogout }) => {
  return (
    <div className="home-page">
      <div className="header">
        <h1>Agent Dashboard</h1>
        <div className="user-info">
          <span>Welcome, {user.fullName}</span>
          <span className="user-type">{UserTypeNames[user.userType]}</span>
          <button className="logout-button" onClick={onLogout}>Logout</button>
        </div>
      </div>

      <div className="dashboard-content">
        <div className="stats-grid">
          <div className="stat-card">
            <h3>Active Tickets</h3>
            <div className="stat-number">14</div>
            <p>Customer support cases</p>
          </div>
          <div className="stat-card">
            <h3>Resolved Today</h3>
            <div className="stat-number">8</div>
            <p>Cases closed successfully</p>
          </div>
          <div className="stat-card">
            <h3>Customer Rating</h3>
            <div className="stat-number">4.9</div>
            <p>Average satisfaction score</p>
          </div>
        </div>

        <div className="action-cards">
          <div className="action-card">
            <h4>🎫 Support Queue</h4>
            <p>Handle customer inquiries</p>
            <button className="action-button">View Queue</button>
          </div>
          <div className="action-card">
            <h4>🔍 Universal Search</h4>
            <p>Search across all systems</p>
            <button className="action-button">Open Search</button>
          </div>
          <div className="action-card">
            <h4>👥 Customer Lookup</h4>
            <p>Find customer information</p>
            <button className="action-button">Customer Search</button>
          </div>
        </div>

        <div className="recent-activity">
          <h3>Recent Activity</h3>
          <div className="activity-list">
            <div className="activity-item">
              <span>✅ Resolved payment issue for Order #12345</span>
              <span className="time">30 minutes ago</span>
            </div>
            <div className="activity-item">
              <span>📞 Assisted buyer with vehicle inspection questions</span>
              <span className="time">1 hour ago</span>
            </div>
            <div className="activity-item">
              <span>🚛 Helped arrange transport for seller in Phoenix</span>
              <span className="time">2 hours ago</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default AgentHomePage;