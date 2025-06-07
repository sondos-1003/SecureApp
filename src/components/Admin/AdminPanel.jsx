import React from 'react';
import { Typography, Box } from '@mui/material';

const AdminPanel = () => {
    return (
        <Box sx={{ p: 4 }}>
            <Typography variant="h4">Admin Dashboard</Typography>
            <Typography sx={{ mt: 2 }}>Welcome to the admin panel</Typography>
        </Box>
    );
};

export default AdminPanel;