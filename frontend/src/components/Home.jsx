import React from 'react';
import { Typography, Box, Button } from '@mui/material';
import { Link } from 'react-router-dom';

const Home = () => {
    return (
        <Box sx={{ p: 4, textAlign: 'center' }}>
            <Typography variant="h3" gutterBottom>
                Welcome to SecureApp
            </Typography>
            <Box sx={{ mt: 4 }}>
                <Button
                    component={Link}
                    to="/login"
                    variant="contained"
                    sx={{ mr: 2 }}
                >
                    Login
                </Button>
                <Button
                    component={Link}
                    to="/register"
                    variant="outlined"
                >
                    Register
                </Button>
            </Box>
        </Box>
    );
};

export default Home;