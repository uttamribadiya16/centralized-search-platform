import React, { useState, useEffect } from 'react';
import authService from './services/authService';
import LoginForm from './components/LoginForm';
import SellerDashboard from './components/SellerDashboard';
import './App.css';

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [user, setUser] = useState(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // Check if user is already logged in
    const checkLoginStatus = () => {
      try {
        const loggedIn = authService.isLoggedIn();
        const userData = authService.getCurrentUser();
        
        if (loggedIn && userData) {
          setIsLoggedIn(true);
          setUser(userData);
        }
      } catch (error) {
        console.error('Error checking login status:', error);
        // Clear any corrupted data
        authService.logout();
      } finally {
        setIsLoading(false);
      }
    };

    checkLoginStatus();
  }, []);

  const handleLoginSuccess = (userData) => {
    setUser(userData);
    setIsLoggedIn(true);
  };

  const handleLogout = () => {
    setUser(null);
    setIsLoggedIn(false);
  };

  if (isLoading) {
    return (
      <div className="app-loading">
        <h2>Loading...</h2>
      </div>
    );
  }

  return (
    <div className="App">
      {!isLoggedIn ? (
        <LoginForm onLoginSuccess={handleLoginSuccess} />
      ) : (
        <SellerDashboard user={user} onLogout={handleLogout} />
      )}
    </div>
  );
}

export default App;
