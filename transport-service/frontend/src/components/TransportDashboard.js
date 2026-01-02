import React, { useState, useEffect } from 'react';
import { transportService } from '../services/api';

const TransportDashboard = () => {
  const [stats, setStats] = useState({
    totalTransports: 0,
    activeTransports: 0,
    availableOffers: 0,
    pendingPurchases: 0
  });
  const [recentTransports, setRecentTransports] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    try {
      setLoading(true);
      
      // Load transports
      const transportsResponse = await transportService.getTransports({ pageSize: 5 });
      setRecentTransports(transportsResponse.data.items || []);
      
      // Load offers and purchases for stats
      const offersResponse = await transportService.getOffers({ pageSize: 1 });
      const purchasesResponse = await transportService.getPurchases({ pageSize: 1 });
      
      setStats({
        totalTransports: transportsResponse.data.totalCount || 0,
        activeTransports: transportsResponse.data.items?.filter(t => t.status === 'Active').length || 0,
        availableOffers: offersResponse.data.totalCount || 0,
        pendingPurchases: purchasesResponse.data.items?.filter(p => p.status === 'Pending').length || 0
      });
    } catch (error) {
      console.error('Error loading dashboard data:', error);
    } finally {
      setLoading(false);
    }
  };

  const getStatusBadgeClass = (status) => {
    switch (status?.toLowerCase()) {
      case 'active': return 'bg-success';
      case 'pending': return 'bg-warning text-dark';
      case 'completed': return 'bg-primary';
      case 'cancelled': return 'bg-danger';
      default: return 'bg-secondary';
    }
  };

  if (loading) {
    return (
      <div className="loading-spinner">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
      </div>
    );
  }

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h1>Transport Dashboard</h1>
        <button className="btn btn-primary" onClick={loadDashboardData}>
          <i className="bi bi-arrow-clockwise me-2"></i>Refresh
        </button>
      </div>

      {/* Stats Cards */}
      <div className="row mb-4">
        <div className="col-md-3 mb-3">
          <div className="card text-center">
            <div className="card-body">
              <h5 className="card-title text-primary">{stats.totalTransports}</h5>
              <p className="card-text">Total Transports</p>
            </div>
          </div>
        </div>
        <div className="col-md-3 mb-3">
          <div className="card text-center">
            <div className="card-body">
              <h5 className="card-title text-success">{stats.activeTransports}</h5>
              <p className="card-text">Active Transports</p>
            </div>
          </div>
        </div>
        <div className="col-md-3 mb-3">
          <div className="card text-center">
            <div className="card-body">
              <h5 className="card-title text-info">{stats.availableOffers}</h5>
              <p className="card-text">Available Offers</p>
            </div>
          </div>
        </div>
        <div className="col-md-3 mb-3">
          <div className="card text-center">
            <div className="card-body">
              <h5 className="card-title text-warning">{stats.pendingPurchases}</h5>
              <p className="card-text">Pending Purchases</p>
            </div>
          </div>
        </div>
      </div>

      {/* Recent Transports */}
      <div className="card">
        <div className="card-header">
          <h5 className="mb-0">Recent Transports</h5>
        </div>
        <div className="card-body">
          {recentTransports.length === 0 ? (
            <div className="empty-state">
              <p>No transports found</p>
            </div>
          ) : (
            <div className="table-responsive">
              <table className="table table-hover">
                <thead>
                  <tr>
                    <th>ID</th>
                    <th>Origin</th>
                    <th>Destination</th>
                    <th>Capacity</th>
                    <th>Status</th>
                    <th>Created</th>
                  </tr>
                </thead>
                <tbody>
                  {recentTransports.map((transport) => (
                    <tr key={transport.id}>
                      <td>
                        <code>{transport.id.substring(0, 8)}</code>
                      </td>
                      <td>{transport.originLocation}</td>
                      <td>{transport.destinationLocation}</td>
                      <td>{transport.capacity} kg</td>
                      <td>
                        <span className={`badge ${getStatusBadgeClass(transport.status)}`}>
                          {transport.status}
                        </span>
                      </td>
                      <td>{new Date(transport.createdAt).toLocaleDateString()}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default TransportDashboard;