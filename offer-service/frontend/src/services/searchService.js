const SEARCH_API_BASE_URL = process.env.REACT_APP_SEARCH_API_URL || 'http://localhost:5003/api';

const searchService = {
  async searchOffers(sellerId, searchText = '', page = 1, pageSize = 20) {
    try {
      console.log('Searching offers for seller:', sellerId, 'with text:', searchText);
      
      const queryParams = new URLSearchParams();
      queryParams.append('sellerId', sellerId);
      
      if (searchText?.trim()) {
        queryParams.append('searchText', searchText.trim());
      }
      
      queryParams.append('page', page.toString());
      queryParams.append('pageSize', pageSize.toString());

      const response = await fetch(`${SEARCH_API_BASE_URL}/search/offers?${queryParams}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        console.error('Search failed:', errorData);
        throw new Error(errorData.message || 'Search failed');
      }

      const data = await response.json();
      console.log('Search results:', data);
      
      return data;
    } catch (error) {
      console.error('Error searching offers:', error);
      throw error;
    }
  },

  async getSearchHealth() {
    try {
      const response = await fetch(`${SEARCH_API_BASE_URL}/search/health`);
      
      if (!response.ok) {
        throw new Error(`Health check failed: ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Error checking search service health:', error);
      throw error;
    }
  }
};

export default searchService;