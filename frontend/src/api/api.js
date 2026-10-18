import axios from 'axios';

const api_url = import.meta.env.VITE_API_URL || 'http://localhost:5207/api';
const api = axios.create({
  baseURL: api_url,    
  headers: {
    'Content-Type': 'application/json',
  },
});

export default api;