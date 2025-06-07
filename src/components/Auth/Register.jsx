import React, { useState } from 'react';
import { TextField, Button, Container, Typography, Box, LinearProgress } from '@mui/material';
import { useAuth } from '../../context/AuthContext';
import PasswordStrengthBar from 'react-password-strength-bar'; // Add strength meter

const Register = () => {
    const { register } = useAuth();
    const [formData, setFormData] = useState({
        username: '',
        password: '',
        email: '',
        role:'',
    });
    const [error, setError] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (formData.password.length < 8) {
            setError('Password must be at least 8 characters');
            return;
        }
        try {
            await register(formData);
        } catch (err) {
            setError(err.response?.data?.message || 'Registration failed');
        }
    };

    return (
        <Container maxWidth="sm">
            <Box sx={{ mt: 8 }}>
                <Typography variant="h4" component="h1" gutterBottom>
                    Register
                </Typography>
                {error && <Typography color="error">{error}</Typography>}
                <form onSubmit={handleSubmit}>
                    <TextField
                        label="Username"
                        fullWidth
                        margin="normal"
                        required
                        value={formData.username}
                        onChange={(e) => setFormData({ ...formData, username: e.target.value })}
                    />
                  
                    <TextField
                        label="Password"
                        type="password"
                        fullWidth
                        margin="normal"
                        required
                        value={formData.password}
                        onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                    />
                    <TextField
                        label="Email"
                        type="email"
                        fullWidth
                        margin="normal"
                        required
                        value={formData.email}
                        onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                    />
                    <TextField
                        label="Position"
                        type="role"
                        fullWidth
                        margin="normal"
                        required
                        value={formData.role}
                        onChange={(e) => setFormData({ ...formData, role: e.target.value })}
                    />
                    <Typography variant="caption" color={formData.password.length < 8 ? 'error' : 'success'}>
                        {formData.password.length < 8 ? 'Password must be at least 8 characters' : 'Strong password'}
                    </Typography>
                    <Button type="submit" variant="contained" fullWidth sx={{ mt: 2 }}>
                        Register
                    </Button>
                </form>
            </Box>
        </Container>
    );
};

export default Register;