import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import TransportDashboard from './components/TransportDashboard';
import OffersList from './components/OffersList';
import PurchasesList from './components/PurchasesList';
import TransportAssignments from './components/TransportAssignments';
import './App.css';

function App() {
  return (
    <Router>
      <div className="App">
        <nav className="navbar navbar-expand-lg navbar-dark bg-primary">
          <div className="container">
            <Link className="navbar-brand" to="/">
              🚛 Transport Service
            </Link>
            <button
              className="navbar-toggler"
              type="button"
              data-bs-toggle="collapse"
              data-bs-target="#navbarNav"
            >
              <span className="navbar-toggler-icon"></span>
            </button>
            <div className="collapse navbar-collapse" id="navbarNav">
              <ul className="navbar-nav me-auto">
                <li className="nav-item">
                  <Link className="nav-link" to="/">
                    Dashboard
                  </Link>
                </li>
                <li className="nav-item">
                  <Link className="nav-link" to="/offers">
                    Available Offers
                  </Link>
                </li>
                <li className="nav-item">
                  <Link className="nav-link" to="/purchases">
                    Purchases
                  </Link>
                </li>
                <li className="nav-item">
                  <Link className="nav-link" to="/assignments">
                    Transport Assignments
                  </Link>
                </li>
              </ul>
            </div>
          </div>
        </nav>

        <div className="container mt-4">
          <Routes>
            <Route path="/" element={<TransportDashboard />} />
            <Route path="/offers" element={<OffersList />} />
            <Route path="/purchases" element={<PurchasesList />} />
            <Route path="/assignments" element={<TransportAssignments />} />
          </Routes>
        </div>
      </div>
    </Router>
  );
}

export default App;