import { createContext, useContext, useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { login as apiLogin, register as apiRegister } from '../services/api';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    // Initialize auth state
    useEffect(() => {
        const token = localStorage.getItem('token');
        const userData = localStorage.getItem('user');

        if (token && userData) {
            try {
                setUser(JSON.parse(userData));
            } catch (error) {
                console.error("Failed to parse user data:", error);
                logout();
            }
        }
        setLoading(false);
    }, []);

    const login = async (credentials) => {
        try {
            const response = await apiLogin({
                // Match what your backend expects
                identifier: credentials.username, // or email
                password: credentials.password
            });

            localStorage.setItem('token', response.data.token);
            localStorage.setItem('user', JSON.stringify(response.data.user));
            setUser(response.data.user);

            // Ensure this matches your backend response
            navigate(response.data.user.role === 'Admin' ? '/admin' : '/user'); // case-sensitive
        } catch (error) {
            console.error('Login error:', {
                error: error.response?.data,
                status: error.response?.status
            });
            throw error;
        }
    };

    const register = async (userData) => {
        try {
            setLoading(true);
            await apiRegister(userData);
            navigate('/register');
        } catch (error) {
            console.error("Registration failed:", error);
            throw error;
        } finally {
            setLoading(false);
        }
    };

    const logout = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        setUser(null);
        navigate('/login');
    };

    return (
        <AuthContext.Provider value={{
            user,
            loading,
            login,
            register,
            logout
        }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};