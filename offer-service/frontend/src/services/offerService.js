const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5002/api';

const offerService = {
  async getOffers(searchParams = {}) {
    try {
      const queryParams = new URLSearchParams();
      
      Object.entries(searchParams).forEach(([key, value]) => {
        if (value !== undefined && value !== null && value !== '') {
          queryParams.append(key, value);
        }
      });

      const response = await fetch(`${API_BASE_URL}/offers?${queryParams}`);
      
      if (!response.ok) {
        throw new Error(`Failed to fetch offers: ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error fetching offers:', error);
      throw error;
    }
  },

  async getOffersBySeller(sellerId, searchParams = {}) {
    try {
      const queryParams = new URLSearchParams();
      
      Object.entries(searchParams).forEach(([key, value]) => {
        if (value !== undefined && value !== null && value !== '') {
          queryParams.append(key, value);
        }
      });

      const response = await fetch(`${API_BASE_URL}/offers/seller/${sellerId}?${queryParams}`);
      
      if (!response.ok) {
        throw new Error(`Failed to fetch seller offers: ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error fetching seller offers:', error);
      throw error;
    }
  },

  async getOffer(id) {
    try {
      const response = await fetch(`${API_BASE_URL}/offers/${id}`);
      
      if (!response.ok) {
        throw new Error(`Failed to fetch offer: ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error fetching offer:', error);
      throw error;
    }
  },

  async createOffer(offerData) {
    try {
      console.log('Creating offer:', offerData);
      
      const response = await fetch(`${API_BASE_URL}/offers`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(offerData),
      });

      if (!response.ok) {
        const errorData = await response.json();
        console.error('Failed to create offer:', errorData);
        throw new Error(errorData.message || 'Failed to create offer');
      }

      return await response.json();
    } catch (error) {
      console.error('Error creating offer:', error);
      throw error;
    }
  },

  async updateOffer(id, offerData) {
    try {
      console.log('Updating offer:', id, offerData);
      
      const response = await fetch(`${API_BASE_URL}/offers/${id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(offerData),
      });

      if (!response.ok) {
        const errorData = await response.json();
        console.error('Failed to update offer:', errorData);
        throw new Error(errorData.message || 'Failed to update offer');
      }

      return await response.json();
    } catch (error) {
      console.error('Error updating offer:', error);
      throw error;
    }
  },

  async deleteOffer(id) {
    try {
      console.log('Deleting offer:', id);
      
      const response = await fetch(`${API_BASE_URL}/offers/${id}`, {
        method: 'DELETE',
      });

      if (!response.ok) {
        const errorData = await response.json();
        console.error('Failed to delete offer:', errorData);
        throw new Error(errorData.message || 'Failed to delete offer');
      }

      return true;
    } catch (error) {
      console.error('Error deleting offer:', error);
      throw error;
    }
  },

  async getSellerStats(sellerId) {
    try {
      const response = await fetch(`${API_BASE_URL}/offers/seller/${sellerId}/stats`);
      
      if (!response.ok) {
        throw new Error(`Failed to fetch seller stats: ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error fetching seller stats:', error);
      throw error;
    }
  },

  async validateVIN(vin, excludeOfferId = null) {
    try {
      const response = await fetch(`${API_BASE_URL}/offers/validate-vin`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ vin, excludeOfferId }),
      });

      if (!response.ok) {
        throw new Error(`Failed to validate VIN: ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error validating VIN:', error);
      throw error;
    }
  }
};

export default offerService;