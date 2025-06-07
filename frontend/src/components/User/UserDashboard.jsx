import React from 'react';
import { Typography, Box } from '@mui/material';

const UserDashboard = () => {
    return (
        <Box sx={{ p: 4 }}>
            <Typography variant="h4">User Dashboard</Typography>
            <Typography sx={{ mt: 2 }}>Welcome to your account</Typography>
        </Box>
    );
};

export default UserDashboard;