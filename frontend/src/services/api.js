import axios from 'axios';

const API = axios.create({
    baseURL: 'http://localhost:5179',
    headers: {
        'Content-Type': 'application/json',
    },
    withCredentials: true, // For secure cookies (if using HTTP-only JWT)
});

// JWT interceptor with error handling
API.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
}, (error) => {
    return Promise.reject(error);
});

// Global error interceptor
API.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.status === 401) {
            localStorage.removeItem('token');
            window.location.href = '/login'; // Force logout on token expiry
        }
        return Promise.reject(error);
    }
);

export const register = (userData) =>
    API.post('/api/auth/register', {  // Match backend endpoint exactly
        username: userData.username,
        email: userData.email,
        password: userData.password,
        role: userData.role
    });
export const login = (credentials) => API.post('/login', credentials);
export const getAdminData = () => API.get('/admin');
export const getUserData = () => API.get('/user');
export const requestPasswordReset = (email) => API.post('/forgot-password', { email });

export default API;