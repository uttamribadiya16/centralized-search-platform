import axios from 'axios';

const API_BASE_URL = 'http://localhost:5001/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// User Types
export const UserTypes = {
  SELLER: 1,
  BUYER: 2,
  CARRIER: 3,
  AGENT: 4
};

export const UserTypeNames = {
  1: 'Seller',
  2: 'Buyer', 
  3: 'Carrier',
  4: 'Agent'
};

// API methods
export const userService = {
  // Get all users with search and filters
  getUsers: async (searchParams = {}) => {
    const params = new URLSearchParams();
    if (searchParams.searchTerm) params.append('searchTerm', searchParams.searchTerm);
    if (searchParams.userType) params.append('userType', searchParams.userType);
    if (searchParams.status) params.append('status', searchParams.status);
    if (searchParams.page) params.append('page', searchParams.page);
    if (searchParams.pageSize) params.append('pageSize', searchParams.pageSize);

    const response = await api.get(`/users?${params.toString()}`);
    return response.data;
  },

  // Get user by ID
  getUserById: async (id) => {
    const response = await api.get(`/users/${id}`);
    return response.data;
  },

  // Get user by email
  getUserByEmail: async (email) => {
    const response = await api.get(`/users/by-email/${email}`);
    return response.data;
  },

  // Get users by type
  getUsersByType: async (userType) => {
    const response = await api.get(`/users/by-type/${userType}`);
    return response.data;
  },

  // Create new user
  createUser: async (userData) => {
    try {
      console.log('Creating user with data:', userData);
      const response = await api.post('/users', userData);
      console.log('User creation response:', response.data);
      return response.data;
    } catch (error) {
      console.error('User creation error:', error);
      console.error('Error response:', error.response?.data);
      throw error;
    }
  },

  // Update user
  updateUser: async (id, userData) => {
    const response = await api.put(`/users/${id}`, userData);
    return response.data;
  },

  // Delete user
  deleteUser: async (id) => {
    await api.delete(`/users/${id}`);
  },

  // Check if user exists
  userExists: async (id) => {
    try {
      await api.head(`/users/${id}`);
      return true;
    } catch (error) {
      return false;
    }
  },

  // Check if email exists
  emailExists: async (email) => {
    try {
      await api.head(`/users/email-exists/${email}`);
      return true;
    } catch (error) {
      return false;
    }
  }
};

export default userService;