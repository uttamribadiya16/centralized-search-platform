const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:5005/api';

class ApiService {
  async request(endpoint, options = {}) {
    const url = `${API_BASE_URL}${endpoint}`;
    const config = {
      headers: {
        'Content-Type': 'application/json',
        ...options.headers,
      },
      ...options,
    };

    try {
      const response = await fetch(url, config);
      
      if (!response.ok) {
        // Try to get error message from response body
        let errorMessage = `Request failed with status ${response.status}`;
        
        try {
          const contentType = response.headers.get('content-type');
          if (contentType && contentType.includes('application/json')) {
            const errorData = await response.json();
            if (errorData.message) {
              errorMessage = errorData.message;
            } else if (errorData.error) {
              errorMessage = errorData.error;
            } else if (typeof errorData === 'string') {
              errorMessage = errorData;
            }
          }
        } catch (parseError) {
          // If we can't parse the error response, use default message
          if (response.status === 401) {
            errorMessage = 'Invalid email or password. Please check your credentials and try again.';
          } else if (response.status === 404) {
            errorMessage = 'Service not available. Please try again later.';
          } else if (response.status >= 500) {
            errorMessage = 'Server error. Please try again later.';
          }
        }
        
        throw new Error(errorMessage);
      }

      const contentType = response.headers.get('content-type');
      if (contentType && contentType.includes('application/json')) {
        return await response.json();
      }
      
      return response;
    } catch (error) {
      console.error(`API request failed: ${endpoint}`, error);
      throw error;
    }
  }

  // Auth endpoints
  async loginCarrier(email, password) {
    return this.request('/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    });
  }

  async validateCarrier(carrierId) {
    return this.request(`/auth/validate/${carrierId}`);
  }

  // Offers endpoints - for carriers to view available offers
  async getAvailableOffers(searchParams = {}) {
    const queryString = new URLSearchParams(searchParams).toString();
    return this.request(`/offers?${queryString}`);
  }

  async getOffer(offerId) {
    return this.request(`/offers/${offerId}`);
  }

  // Purchases endpoints - for carriers to view purchases needing transport
  async getAvailablePurchases(searchParams = {}) {
    const queryString = new URLSearchParams(searchParams).toString();
    return this.request(`/purchases?${queryString}`);
  }

  async getPurchase(purchaseId) {
    return this.request(`/purchases/${purchaseId}`);
  }

  // Transport endpoints
  async getTransports(searchParams = {}) {
    const queryString = new URLSearchParams(searchParams).toString();
    return this.request(`/transports?${queryString}`);
  }

  async getTransportsByCarrier(carrierId, searchParams = {}) {
    const queryString = new URLSearchParams(searchParams).toString();
    return this.request(`/transports/carrier/${carrierId}?${queryString}`);
  }

  async getTransport(transportId) {
    return this.request(`/transports/${transportId}`);
  }

  async createTransport(data, carrierId) {
    return this.request('/transports/assign', {
      method: 'POST',
      body: JSON.stringify(data),
      headers: {
        'X-Carrier-Id': carrierId
      }
    });
  }

  async updateTransport(transportId, updateData) {
    return this.request(`/transports/${transportId}`, {
      method: 'PUT',
      body: JSON.stringify(updateData),
    });
  }

  async deleteTransport(transportId) {
    return this.request(`/transports/${transportId}`, {
      method: 'DELETE',
    });
  }

  // Assignment operations
  async assignPurchaseToTransport(assignmentData) {
    return this.request('/transports/assign', {
      method: 'POST',
      body: JSON.stringify(assignmentData),
    });
  }

  async unassignPurchaseFromTransport(transportId, purchaseId) {
    return this.request(`/transports/${transportId}/unassign/${purchaseId}`, {
      method: 'DELETE',
    });
  }
}

export default new ApiService();