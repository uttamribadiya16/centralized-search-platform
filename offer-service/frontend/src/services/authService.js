const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5002/api';

const authService = {
  async login(email, password) {
    try {
      console.log('Attempting login for:', email);
      
      const response = await fetch(`${API_BASE_URL}/auth/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email, password }),
      });

      console.log('Response status:', response.status);

      if (!response.ok) {
        const errorData = await response.json();
        console.error('Login failed:', errorData);
        throw new Error(errorData.message || 'Login failed');
      }

      const data = await response.json();
      console.log('Login successful:', data);

      // Store user data in localStorage
      localStorage.setItem('offerService_user', JSON.stringify(data.user));
      localStorage.setItem('offerService_isLoggedIn', 'true');

      return data;
    } catch (error) {
      console.error('Login error:', error);
      throw error;
    }
  },

  logout() {
    localStorage.removeItem('offerService_user');
    localStorage.removeItem('offerService_isLoggedIn');
  },

  getCurrentUser() {
    const userData = localStorage.getItem('offerService_user');
    return userData ? JSON.parse(userData) : null;
  },

  isLoggedIn() {
    return localStorage.getItem('offerService_isLoggedIn') === 'true';
  }
};

export default authService;