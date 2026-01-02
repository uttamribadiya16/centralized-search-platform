import React, { useState } from 'react';
import apiService from '../services/api';

const LoginPage = ({ onLogin }) => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      const response = await apiService.loginCarrier(email, password);
      
      if (response && response.id) {
        onLogin(response);
      } else {
        setError('Login failed. Invalid response from server.');
      }
    } catch (error) {
      console.error('Login error:', error);
      setError(error.message || 'Login failed. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-page">
      <div className="login-container">
        <div className="login-card">
          <h1>🚛 Transport Service</h1>
          <h2>Carrier Portal</h2>
          <p>Sign in to manage transport assignments</p>
          
          {error && (
            <div className="error-message">
              {error}
            </div>
          )}

          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label htmlFor="email">Email Address:</label>
              <input
                type="email"
                id="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
                placeholder="Enter your email"
                disabled={loading}
              />
            </div>

            <div className="form-group">
              <label htmlFor="password">Password:</label>
              <input
                type="password"
                id="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
                placeholder="Enter your password"
                disabled={loading}
              />
            </div>

            <button
              type="submit"
              className="login-btn"
              disabled={loading || !email || !password}
            >
              {loading ? 'Signing in...' : 'Sign In'}
            </button>
          </form>

          <div className="login-info">
            <h3>For Carriers</h3>
            <ul>
              <li>View available offers and purchases</li>
              <li>Assign purchases to your transport routes</li>
              <li>Manage your transport assignments</li>
              <li>Track delivery status</li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  );
};

export default LoginPage;