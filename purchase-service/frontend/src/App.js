import React, { useState, useEffect } from 'react';
import './App.css';
import LoginPage from './pages/LoginPage';
import OfferListPage from './pages/OfferListPage';
import PurchaseListPage from './pages/PurchaseListPage';

function App() {
  const [currentPage, setCurrentPage] = useState('login');
  const [currentUser, setCurrentUser] = useState(null);

  useEffect(() => {
    // Check if user is already logged in
    const savedUser = localStorage.getItem('currentUser');
    if (savedUser) {
      setCurrentUser(JSON.parse(savedUser));
      setCurrentPage('offers');
    }
  }, []);

  const handleLogin = (user) => {
    setCurrentUser(user);
    localStorage.setItem('currentUser', JSON.stringify(user));
    setCurrentPage('offers');
  };

  const handleLogout = () => {
    setCurrentUser(null);
    localStorage.removeItem('currentUser');
    setCurrentPage('login');
  };

  if (currentPage === 'login' || !currentUser) {
    return <LoginPage onLogin={handleLogin} />;
  }

  return (
    <div className="App">
      <header className="header">
        <div className="container">
          <h1>Purchase Service - Buyer Portal</h1>
          <div className="user-info">
            <span>Welcome, {currentUser.email}</span>
            <button onClick={handleLogout} className="btn btn-danger">
              Logout
            </button>
          </div>
          <nav className="nav">
            <button
              className={currentPage === 'offers' ? 'active' : ''}
              onClick={() => setCurrentPage('offers')}
            >
              Available Offers
            </button>
            <button
              className={currentPage === 'purchases' ? 'active' : ''}
              onClick={() => setCurrentPage('purchases')}
            >
              My Purchases
            </button>
          </nav>
        </div>
      </header>

      <main className="container">
        {currentPage === 'offers' && (
          <OfferListPage currentUser={currentUser} />
        )}
        {currentPage === 'purchases' && (
          <PurchaseListPage currentUser={currentUser} />
        )}
      </main>
    </div>
  );
}

export default App;