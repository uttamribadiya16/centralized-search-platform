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
}

export default App;