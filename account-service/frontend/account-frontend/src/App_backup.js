import React, { useState } from 'react';
import HomePage from './components/HomePage';
import UserCreationForm from './components/UserCreationForm';
import './App.css';

function App() {
  const [currentView, setCurrentView] = useState('home');

  const handleCreateUser = () => {
    setCurrentView('create-user');
  };

  const handleUserCreated = (userData) => {
    console.log('User created:', userData);
    setCurrentView('home');
  };

  const handleCancel = () => {
    setCurrentView('home');
  };

  return (
    <div className="App">
      {currentView === 'home' && (
        <HomePage onCreateUser={handleCreateUser} />
      )}
      {currentView === 'create-user' && (
        <UserCreationForm 
          onUserCreated={handleUserCreated} 
          onCancel={handleCancel} 
        />
      )}
    </div>
  );
    setCurrentUser(userData);
    localStorage.setItem('currentUser', JSON.stringify(userData));
  };

  const handleLogout = () => {
    setCurrentUser(null);
    localStorage.removeItem('currentUser');
  };

  const renderHomePage = () => {
    if (!currentUser) return <Navigate to="/signup" replace />;

    const props = { user: currentUser, onLogout: handleLogout };

    switch (currentUser.userType) {
      case UserTypes.SELLER:
        return <SellerHomePage {...props} />;
      case UserTypes.BUYER:
        return <BuyerHomePage {...props} />;
      case UserTypes.CARRIER:
        return <CarrierHomePage {...props} />;
      case UserTypes.AGENT:
        return <AgentHomePage {...props} />;
      default:
        return <Navigate to="/signup" replace />;
    }
  };

  if (loading) {
    return (
      <div className="loading-container">
        <div className="loading-spinner"></div>
        <p>Loading...</p>
      </div>
    );
  }

  return (
    <Router>
      <div className="App">
        <Routes>
          <Route 
            path="/signup" 
            element={
              currentUser ? 
              <Navigate to="/dashboard" replace /> : 
              <SignupForm onSignupSuccess={handleSignupSuccess} />
            } 
          />
          <Route 
            path="/dashboard" 
            element={renderHomePage()} 
          />
          <Route 
            path="/" 
            element={
              currentUser ? 
              <Navigate to="/dashboard" replace /> : 
              <Navigate to="/signup" replace />
            } 
          />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
