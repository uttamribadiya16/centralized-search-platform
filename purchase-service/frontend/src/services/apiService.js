const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:5003/api';

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
  async loginBuyer(email, password) {
    return this.request('/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    });
  }

  async validateBuyer(buyerId) {
    return this.request(`/auth/validate/${buyerId}`);
  }

  // Offers endpoints
  async getAvailableOffers(searchParams = {}) {
    const queryString = new URLSearchParams(searchParams).toString();
    return this.request(`/offers?${queryString}`);
  }

  async getOffer(offerId) {
    return this.request(`/offers/${offerId}`);
  }

  // Purchases endpoints
  async getPurchases(searchParams = {}) {
    const queryString = new URLSearchParams(searchParams).toString();
    return this.request(`/purchases?${queryString}`);
  }

  async getPurchasesByBuyer(buyerId, searchParams = {}) {
    const queryString = new URLSearchParams(searchParams).toString();
    return this.request(`/purchases/buyer/${buyerId}?${queryString}`);
  }

  async getPurchase(purchaseId) {
    return this.request(`/purchases/${purchaseId}`);
  }

  async createPurchase(buyerId, purchaseData) {
    return this.request(`/purchases/buyer/${buyerId}`, {
      method: 'POST',
      body: JSON.stringify(purchaseData),
    });
  }

  async updatePurchase(purchaseId, updateData) {
    return this.request(`/purchases/${purchaseId}`, {
      method: 'PUT',
      body: JSON.stringify(updateData),
    });
  }

  async deletePurchase(purchaseId) {
    return this.request(`/purchases/${purchaseId}`, {
      method: 'DELETE',
    });
  }
}

export default new ApiService();