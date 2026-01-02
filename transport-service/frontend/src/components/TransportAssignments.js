import React, { useState, useEffect } from 'react';
import { transportService } from '../services/api';

const TransportAssignments = () => {
  const [transports, setTransports] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [newTransport, setNewTransport] = useState({
    originLocation: '',
    destinationLocation: '',
    capacity: '',
    vehicleType: 'Truck',
    estimatedDeliveryDate: ''
  });

  useEffect(() => {
    loadTransports();
  }, [currentPage, searchTerm]);

  const loadTransports = async () => {
    try {
      setLoading(true);
      const response = await transportService.getTransports({
        page: currentPage,
        pageSize: 10,
        search: searchTerm
      });
      
      const data = response.data;
      setTransports(data.items || []);
      setTotalPages(Math.ceil((data.totalCount || 0) / 10));
    } catch (error) {
      console.error('Error loading transports:', error);
      setTransports([]);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = (e) => {
    setSearchTerm(e.target.value);
    setCurrentPage(1);
  };

  const handleCreateTransport = async (e) => {
    e.preventDefault();
    try {
      await transportService.createTransport(newTransport);
      setShowCreateModal(false);
      setNewTransport({
        originLocation: '',
        destinationLocation: '',
        capacity: '',
        vehicleType: 'Truck',
        estimatedDeliveryDate: ''
      });
      loadTransports();
    } catch (error) {
      console.error('Error creating transport:', error);
    }
  };

  const getStatusBadgeClass = (status) => {
    switch (status?.toLowerCase()) {
      case 'active': return 'bg-success';
      case 'pending': return 'bg-warning text-dark';
      case 'completed': return 'bg-primary';
      case 'cancelled': return 'bg-danger';
      case 'in-transit': return 'bg-info';
      default: return 'bg-secondary';
    }
  };

  const getVehicleTypeIcon = (vehicleType) => {
    switch (vehicleType?.toLowerCase()) {
      case 'truck': return 'bi-truck';
      case 'van': return 'bi-minecart';
      case 'motorcycle': return 'bi-bicycle';
      case 'ship': return 'bi-water';
      case 'plane': return 'bi-airplane';
      default: return 'bi-truck';
    }
  };

  const renderPagination = () => {
    if (totalPages <= 1) return null;

    return (
      <nav>
        <ul className="pagination justify-content-center">
          <li className={`page-item ${currentPage === 1 ? 'disabled' : ''}`}>
            <button 
              className="page-link" 
              onClick={() => setCurrentPage(1)}
              disabled={currentPage === 1}
            >
              First
            </button>
          </li>
          <li className={`page-item ${currentPage === 1 ? 'disabled' : ''}`}>
            <button 
              className="page-link" 
              onClick={() => setCurrentPage(currentPage - 1)}
              disabled={currentPage === 1}
            >
              Previous
            </button>
          </li>
          
          {[...Array(Math.min(5, totalPages))].map((_, i) => {
            const page = Math.max(1, Math.min(totalPages - 4, currentPage - 2)) + i;
            if (page > totalPages) return null;
            
            return (
              <li key={page} className={`page-item ${currentPage === page ? 'active' : ''}`}>
                <button 
                  className="page-link" 
                  onClick={() => setCurrentPage(page)}
                >
                  {page}
                </button>
              </li>
            );
          })}
          
          <li className={`page-item ${currentPage === totalPages ? 'disabled' : ''}`}>
            <button 
              className="page-link" 
              onClick={() => setCurrentPage(currentPage + 1)}
              disabled={currentPage === totalPages}
            >
              Next
            </button>
          </li>
          <li className={`page-item ${currentPage === totalPages ? 'disabled' : ''}`}>
            <button 
              className="page-link" 
              onClick={() => setCurrentPage(totalPages)}
              disabled={currentPage === totalPages}
            >
              Last
            </button>
          </li>
        </ul>
      </nav>
    );
  };

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h1>Transport Assignments</h1>
        <div className="d-flex gap-2">
          <button 
            className="btn btn-success"
            onClick={() => setShowCreateModal(true)}
          >
            <i className="bi bi-plus-lg me-2"></i>Create Transport
          </button>
          <button className="btn btn-outline-primary" onClick={loadTransports}>
            <i className="bi bi-arrow-clockwise me-2"></i>Refresh
          </button>
        </div>
      </div>

      {/* Search */}
      <div className="row mb-4">
        <div className="col-md-6">
          <div className="input-group search-box">
            <span className="input-group-text">
              <i className="bi bi-search"></i>
            </span>
            <input
              type="text"
              className="form-control"
              placeholder="Search transports by origin, destination, or status..."
              value={searchTerm}
              onChange={handleSearch}
            />
            {searchTerm && (
              <button 
                className="btn btn-outline-secondary" 
                type="button"
                onClick={() => setSearchTerm('')}
              >
                <i className="bi bi-x"></i>
              </button>
            )}
          </div>
        </div>
      </div>

      {/* Transports Table */}
      <div className="card">
        <div className="card-body">
          {loading ? (
            <div className="loading-spinner">
              <div className="spinner-border text-primary" role="status">
                <span className="visually-hidden">Loading...</span>
              </div>
            </div>
          ) : transports.length === 0 ? (
            <div className="empty-state">
              <i className="bi bi-truck fs-1 text-muted"></i>
              <h5 className="mt-3">No transports found</h5>
              <p>Create a new transport assignment or try adjusting your search terms.</p>
              <button 
                className="btn btn-primary"
                onClick={() => setShowCreateModal(true)}
              >
                <i className="bi bi-plus-lg me-2"></i>Create Your First Transport
              </button>
            </div>
          ) : (
            <>
              <div className="table-responsive">
                <table className="table table-hover">
                  <thead>
                    <tr>
                      <th>Transport ID</th>
                      <th>Route</th>
                      <th>Vehicle</th>
                      <th>Capacity</th>
                      <th>Status</th>
                      <th>Delivery Date</th>
                      <th>Created</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {transports.map((transport) => (
                      <tr key={transport.id}>
                        <td>
                          <code>{transport.id.substring(0, 8)}</code>
                        </td>
                        <td>
                          <div>
                            <strong>{transport.originLocation}</strong>
                            <i className="bi bi-arrow-right mx-2 text-muted"></i>
                            <strong>{transport.destinationLocation}</strong>
                          </div>
                        </td>
                        <td>
                          <span className="d-flex align-items-center">
                            <i className={`bi ${getVehicleTypeIcon(transport.vehicleType)} me-2`}></i>
                            {transport.vehicleType}
                          </span>
                        </td>
                        <td>
                          <span className="fw-bold">{transport.capacity} kg</span>
                        </td>
                        <td>
                          <span className={`badge ${getStatusBadgeClass(transport.status)}`}>
                            {transport.status}
                          </span>
                        </td>
                        <td>
                          {transport.estimatedDeliveryDate 
                            ? new Date(transport.estimatedDeliveryDate).toLocaleDateString()
                            : 'TBD'
                          }
                        </td>
                        <td>{new Date(transport.createdAt).toLocaleDateString()}</td>
                        <td>
                          <div className="btn-group" role="group">
                            <button className="btn btn-sm btn-outline-primary">
                              <i className="bi bi-eye me-1"></i>View
                            </button>
                            <button className="btn btn-sm btn-outline-secondary">
                              <i className="bi bi-pencil me-1"></i>Edit
                            </button>
                            <button 
                              className="btn btn-sm btn-outline-danger"
                              disabled={transport.status === 'active' || transport.status === 'in-transit'}
                            >
                              <i className="bi bi-trash me-1"></i>Delete
                            </button>
                          </div>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
              
              {renderPagination()}
            </>
          )}
        </div>
      </div>

      {/* Create Transport Modal */}
      {showCreateModal && (
        <div className="modal d-block" tabIndex="-1" style={{backgroundColor: 'rgba(0,0,0,0.5)'}}>
          <div className="modal-dialog modal-lg">
            <div className="modal-content">
              <form onSubmit={handleCreateTransport}>
                <div className="modal-header">
                  <h5 className="modal-title">Create New Transport</h5>
                  <button 
                    type="button" 
                    className="btn-close" 
                    onClick={() => setShowCreateModal(false)}
                  ></button>
                </div>
                <div className="modal-body">
                  <div className="row">
                    <div className="col-md-6 mb-3">
                      <label className="form-label">Origin Location *</label>
                      <input
                        type="text"
                        className="form-control"
                        required
                        value={newTransport.originLocation}
                        onChange={(e) => setNewTransport({...newTransport, originLocation: e.target.value})}
                        placeholder="Enter origin location"
                      />
                    </div>
                    <div className="col-md-6 mb-3">
                      <label className="form-label">Destination Location *</label>
                      <input
                        type="text"
                        className="form-control"
                        required
                        value={newTransport.destinationLocation}
                        onChange={(e) => setNewTransport({...newTransport, destinationLocation: e.target.value})}
                        placeholder="Enter destination location"
                      />
                    </div>
                  </div>
                  <div className="row">
                    <div className="col-md-6 mb-3">
                      <label className="form-label">Capacity (kg) *</label>
                      <input
                        type="number"
                        className="form-control"
                        required
                        min="1"
                        value={newTransport.capacity}
                        onChange={(e) => setNewTransport({...newTransport, capacity: e.target.value})}
                        placeholder="Enter capacity in kg"
                      />
                    </div>
                    <div className="col-md-6 mb-3">
                      <label className="form-label">Vehicle Type *</label>
                      <select
                        className="form-select"
                        required
                        value={newTransport.vehicleType}
                        onChange={(e) => setNewTransport({...newTransport, vehicleType: e.target.value})}
                      >
                        <option value="Truck">Truck</option>
                        <option value="Van">Van</option>
                        <option value="Motorcycle">Motorcycle</option>
                        <option value="Ship">Ship</option>
                        <option value="Plane">Plane</option>
                      </select>
                    </div>
                  </div>
                  <div className="row">
                    <div className="col-12 mb-3">
                      <label className="form-label">Estimated Delivery Date</label>
                      <input
                        type="date"
                        className="form-control"
                        value={newTransport.estimatedDeliveryDate}
                        onChange={(e) => setNewTransport({...newTransport, estimatedDeliveryDate: e.target.value})}
                        min={new Date().toISOString().split('T')[0]}
                      />
                    </div>
                  </div>
                </div>
                <div className="modal-footer">
                  <button 
                    type="button" 
                    className="btn btn-secondary" 
                    onClick={() => setShowCreateModal(false)}
                  >
                    Cancel
                  </button>
                  <button 
                    type="submit" 
                    className="btn btn-primary"
                  >
                    <i className="bi bi-plus-lg me-2"></i>Create Transport
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default TransportAssignments;